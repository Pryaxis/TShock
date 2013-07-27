/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.Text;
using HttpServer;
using TShockAPI;
using TShockAPI.DB;

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

		public Dictionary<string,TokenData> Tokens { get; protected set; }

		public SecureRest(IPAddress ip, int port)
			: base(ip, port)
		{
			Tokens = new Dictionary<string, TokenData>();

			Register(new RestCommand("/token/create/{username}/{password}", NewToken) { DoLog = false });
			Register(new RestCommand("/v2/token/create/{password}", NewTokenV2) { DoLog = false });
			Register(new SecureRestCommand("/token/destroy/{token}", DestroyToken));
			Register(new SecureRestCommand("/v2/token/destroy/all", DestroyAllTokens, RestPermissions.restmanage));

			foreach (KeyValuePair<string, TokenData> t in TShockAPI.TShock.RESTStartupTokens)
			{
				Tokens.Add(t.Key, t.Value);
			}
		}

		private object DestroyToken(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var token = verbs["token"];
			try
			{
				Tokens.Remove(token);
			}
			catch (Exception)
			{
				return new Dictionary<string, string>
				       	{{"status", "400"}, {"error", "The specified token queued for destruction failed to be deleted."}};
			}
			return new Dictionary<string, string>
			       	{{"status", "200"}, {"response", "Requested token was successfully destroyed."}};
		}

		private object DestroyAllTokens(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			Tokens.Clear();

			return new Dictionary<string, string>
			       	{{"status", "200"}, {"response", "All tokens were successfully destroyed."}};
		}

		private object NewTokenV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var user = parameters["username"];
			var pass = verbs["password"];

			return this.NewTokenInternal(user, pass);
		}

		private object NewToken(RestVerbs verbs, IParameterCollection parameters)
		{
			var user = verbs["username"];
			var pass = verbs["password"];

			RestObject response = this.NewTokenInternal(user, pass);
			response["deprecated"] = "This endpoint is depracted and will be removed in the future.";
			return response;
		}

		private RestObject NewTokenInternal(string username, string password)
		{
			User userAccount = TShock.Users.GetUserByName(username);
			if (userAccount == null || !string.IsNullOrWhiteSpace(userAccount.Address))
				return new RestObject("401")
					{ Error = "Invalid username/password combination provided. Please re-submit your query with a correct pair." };
			
			if (!TShock.Utils.HashPassword(password).Equals(userAccount.Password, StringComparison.InvariantCultureIgnoreCase))
				return new RestObject("401")
					{ Error = "Invalid username/password combination provided. Please re-submit your query with a correct pair." };

			Group userGroup = TShock.Utils.GetGroup(userAccount.Group);
			if (!userGroup.HasPermission(RestPermissions.restapi) && userAccount.Group != "superadmin")
				return new RestObject("403")
					{ Error = "Although your account was successfully found and identified, your account lacks the permission required to use the API. (restapi)" };
			
			string tokenHash;
			var rand = new Random();
			var randbytes = new byte[32];
			do
			{
				rand.NextBytes(randbytes);
				tokenHash = randbytes.Aggregate("", (s, b) => s + b.ToString("X2"));
			} while (Tokens.ContainsKey(tokenHash));

			Tokens.Add(tokenHash, new TokenData { Username = userAccount.Name, UserGroupName = userGroup.Name });

			RestObject response = new RestObject("200") { Response = "Successful login" };
			response["token"] = tokenHash;
			return response;
		}

		protected override object ExecuteCommand(RestCommand cmd, RestVerbs verbs, IParameterCollection parms)
		{
			if (!cmd.RequiresToken)
				return base.ExecuteCommand(cmd, verbs, parms);
			
			var token = parms["token"];
			if (token == null)
				return new Dictionary<string, string>
					{{"status", "401"}, {"error", "Not authorized. The specified API endpoint requires a token."}};

			SecureRestCommand secureCmd = (SecureRestCommand)cmd;
			TokenData tokenData;
			if (!Tokens.TryGetValue(token, out tokenData))
				return new Dictionary<string, string>
				{
					{"status", "403"},
					{
						"error",
						"Not authorized. The specified API endpoint requires a token, but the provided token was not valid."
					}
				};

			Group userGroup = TShock.Groups.GetGroupByName(tokenData.UserGroupName);
			if (userGroup == null)
			{
				Tokens.Remove(token);

				return new Dictionary<string, string>
				{
					{"status", "403"},
					{
						"error",
						"Not authorized. The provided token became invalid due to group changes, please create a new token."
					}
				};
			}

			if (secureCmd.Permissions.Length > 0 && secureCmd.Permissions.All(perm => !userGroup.HasPermission(perm)))
			{
				return new Dictionary<string, string>
				{
					{"status", "403"},
					{
						"error",
						string.Format("Not authorized. User \"{0}\" has no access to use the specified API endpoint.", tokenData.Username)
					}
				};
			}
			
			object result = secureCmd.Execute(verbs, parms, tokenData);
			if (cmd.DoLog)
				TShock.Utils.SendLogs(string.Format(
					"\"{0}\" requested REST endpoint: {1}", tokenData.Username, this.BuildRequestUri(cmd, verbs, parms, false)), 
					Color.PaleVioletRed);

			return result;
		}
	}
}