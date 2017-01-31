/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HttpServer;
using TShockAPI;
using TShockAPI.DB;
using Microsoft.Xna.Framework;
using Terraria;
using System.Security.Cryptography;

namespace Rests
{
	public class SecureRest : Rest
	{
		public struct TokenData
		{
			public static readonly TokenData None = default(TokenData);

			public string Username { get; set; }
			public string UserGroupName { get; set; }
		}

		public Dictionary<string, TokenData> Tokens { get; protected set; }
		public Dictionary<string, TokenData> AppTokens { get; protected set; }

		private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
		
		public SecureRest(IPAddress ip, int port)
			: base(ip, port)
		{
			Tokens = new Dictionary<string, TokenData>();
			AppTokens = new Dictionary<string, TokenData>();

			Register(new RestCommand("/v2/token/create", NewTokenV2) { DoLog = false });
			Register(new SecureRestCommand("/token/destroy/{token}", DestroyToken));
			Register(new SecureRestCommand("/v3/token/destroy/all", DestroyAllTokens, RestPermissions.restmanage));

			foreach (KeyValuePair<string, TokenData> t in TShock.RESTStartupTokens)
			{
				AppTokens.Add(t.Key, t.Value);
			}

			foreach (KeyValuePair<string, TokenData> t in TShock.Config.ApplicationRestTokens)
			{
				AppTokens.Add(t.Key, t.Value);
			}
		}

		private void AddTokenToBucket(string ip)
		{
			if (tokenBucket.ContainsKey(ip))
			{
				tokenBucket[ip] += 1;
			}
			else
			{
				tokenBucket.Add(ip, 1);
			}
		}

		private object DestroyToken(RestRequestArgs args)
		{
			var token = args.Verbs["token"];
			try
			{
				Tokens.Remove(token);
			}
			catch (Exception)
			{
				return new RestObject("400")
				{ Error = "The specified token queued for destruction failed to be deleted." };
			}
			return new RestObject()
			{ Response = "Requested token was successfully destroyed." };
		}

		private object DestroyAllTokens(RestRequestArgs args)
		{
			Tokens.Clear();

			return new RestObject()
			{ Response = "All tokens were successfully destroyed." };
		}

		private object NewTokenV2(RestRequestArgs args)
		{
			var user = args.Parameters["username"];
			var pass = args.Parameters["password"];
			var context = args.Context;

			return this.NewTokenInternal(user, pass, context);
		}

		private RestObject NewTokenInternal(string username, string password, IHttpContext context)
		{
			int tokens = 0;
			if (tokenBucket.TryGetValue(context.RemoteEndPoint.Address.ToString(), out tokens))
			{
				if (tokens >= TShock.Config.RESTMaximumRequestsPerInterval)
				{
					TShock.Log.ConsoleError("A REST login from {0} was blocked as it currently has {1} tokens", context.RemoteEndPoint.Address.ToString(), tokens);
					tokenBucket[context.RemoteEndPoint.Address.ToString()] += 1; // Tokens over limit, increment by one and reject request
					return new RestObject("403")
					{
						Error = "Username or password may be incorrect or this account may not have sufficient privileges."
					};
				}
				tokenBucket[context.RemoteEndPoint.Address.ToString()] += 1; // Tokens under limit, increment by one and process request
			}
			else
			{
				if (!TShock.Config.RESTLimitOnlyFailedLoginRequests)
					tokenBucket.Add(context.RemoteEndPoint.Address.ToString(), 1); // First time request, set to one and process request
			}

			User userAccount = TShock.Users.GetUserByName(username);
			if (userAccount == null)
			{
				AddTokenToBucket(context.RemoteEndPoint.Address.ToString());
				return new RestObject("403") { Error = "Username or password may be incorrect or this account may not have sufficient privileges." };
			}

			if (!userAccount.VerifyPassword(password))
			{
				AddTokenToBucket(context.RemoteEndPoint.Address.ToString());
				return new RestObject("403") { Error = "Username or password may be incorrect or this account may not have sufficient privileges." };
			}

			Group userGroup = TShock.Utils.GetGroup(userAccount.Group);
			if (!userGroup.HasPermission(RestPermissions.restapi) && userAccount.Group != "superadmin")
			{
				AddTokenToBucket(context.RemoteEndPoint.Address.ToString());
				return new RestObject("403")
				{ Error = "Username or password may be incorrect or this account may not have sufficient privileges." };
			}
			
			string tokenHash;
			var randbytes = new byte[32];
			do
			{
				_rng.GetBytes(randbytes);
				tokenHash = randbytes.Aggregate("", (s, b) => s + b.ToString("X2"));
			} while (Tokens.ContainsKey(tokenHash));

			Tokens.Add(tokenHash, new TokenData { Username = userAccount.Name, UserGroupName = userGroup.Name });

			AddTokenToBucket(context.RemoteEndPoint.Address.ToString());

			RestObject response = new RestObject() { Response = "Successful login" };
			response["token"] = tokenHash;
			return response;
		}

		protected override object ExecuteCommand(RestCommand cmd, RestVerbs verbs, IParameterCollection parms, IRequest request, IHttpContext context)
		{
			if (!cmd.RequiresToken)
				return base.ExecuteCommand(cmd, verbs, parms, request, context);

			var token = parms["token"];
			if (token == null)
				return new RestObject("401")
				{ Error = "Not authorized. The specified API endpoint requires a token." };

			SecureRestCommand secureCmd = (SecureRestCommand)cmd;
			TokenData tokenData;
			if (!Tokens.TryGetValue(token, out tokenData) && !AppTokens.TryGetValue(token, out tokenData))
				return new RestObject("403")
				{ Error = "Not authorized. The specified API endpoint requires a token, but the provided token was not valid." };

			Group userGroup = TShock.Groups.GetGroupByName(tokenData.UserGroupName);
			if (userGroup == null)
			{
				Tokens.Remove(token);

				return new RestObject("403")
				{ Error = "Not authorized. The provided token became invalid due to group changes, please create a new token." };
			}

			if (secureCmd.Permissions.Length > 0 && secureCmd.Permissions.All(perm => !userGroup.HasPermission(perm)))
			{
				return new RestObject("403")
				{ Error = string.Format("Not authorized. User \"{0}\" has no access to use the specified API endpoint.", tokenData.Username) };
			}

			//Main.rand being null can cause issues in command execution.
			//This should solve that
			if (Main.rand == null)
			{
				Main.rand = new Terraria.Utilities.UnifiedRandom();
			}

			object result = secureCmd.Execute(verbs, parms, tokenData, request, context);
			if (cmd.DoLog && TShock.Config.LogRest)
				TShock.Utils.SendLogs(string.Format(
					"\"{0}\" requested REST endpoint: {1}", tokenData.Username, this.BuildRequestUri(cmd, verbs, parms, false)),
					Color.PaleVioletRed);

			return result;
		}
	}
}