/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using HttpServer;
using HttpServer.Headers;
using Newtonsoft.Json;
using TShockAPI;
using HttpListener = HttpServer.HttpListener;

namespace Rests
{
	/// <summary>
	/// Rest command delegate
	/// </summary>
	/// <param name="args">RestRequestArgs object containing Verbs, Parameters, Request, and TokenData</param>
	/// <returns>Response object or null to not handle request</returns>
	public delegate object RestCommandD(RestRequestArgs args);

	public class RestRequestArgs
	{
		public RestVerbs Verbs { get; private set; }
		public IParameterCollection Parameters { get; private set; }
		public IRequest Request { get; private set; }
		public SecureRest.TokenData TokenData { get; private set; }

		public RestRequestArgs(RestVerbs verbs, IParameterCollection param, IRequest request)
		{
			Verbs = verbs;
			Parameters = param;
			Request = request;
			TokenData = SecureRest.TokenData.None;
		}

		public RestRequestArgs(RestVerbs verbs, IParameterCollection param, IRequest request, SecureRest.TokenData tokenData)
		{
			Verbs = verbs;
			Parameters = param;
			Request = request;
			TokenData = tokenData;
		}
	}
	public class Rest : IDisposable
	{
		private readonly List<RestCommand> commands = new List<RestCommand>();
		private HttpListener listener;
		private StringHeader serverHeader;
		public IPAddress Ip { get; set; }
		public int Port { get; set; }

		public Rest(IPAddress ip, int port)
		{
			Ip = ip;
			Port = port;
			string appName = this.GetType().Assembly.GetName().Version.ToString();
			AssemblyName ass = this.GetType().Assembly.GetName();
			serverHeader = new StringHeader("Server", String.Format("{0}/{1}", ass.Name, ass.Version));
		}

		public virtual void Start()
		{
			if (listener == null)
			{
				listener = HttpListener.Create(Ip, Port);
				listener.RequestReceived += OnRequest;
				listener.Start(int.MaxValue);
			}
		}

		public void Start(IPAddress ip, int port)
		{
			Ip = ip;
			Port = port;
			Start();
		}

		public virtual void Stop()
		{
			listener.Stop();
		}

		public void Register(string path, RestCommandD callback)
		{
			AddCommand(new RestCommand(path, callback));
		}

		public void Register(RestCommand com)
		{
			AddCommand(com);
		}

		protected void AddCommand(RestCommand com)
		{
			commands.Add(com);
		}

		#region Event
		public class RestRequestEventArgs : HandledEventArgs
		{
			public RequestEventArgs Request { get; set; }
		}

		public static HandlerList<RestRequestEventArgs> RestRequestEvent;

		private static bool OnRestRequestCall(RequestEventArgs request)
		{
			if (RestRequestEvent == null)
				return false;

			var args = new RestRequestEventArgs
			{
				Request = request,
			};
			RestRequestEvent.Invoke(null, args);
			return args.Handled;
		}
		#endregion


		protected virtual void OnRequest(object sender, RequestEventArgs e)
		{
			var obj = ProcessRequest(sender, e);
			if (obj == null)
				throw new NullReferenceException("obj");

			if (OnRestRequestCall(e))
				return;

			var str = JsonConvert.SerializeObject(obj, Formatting.Indented);
			var jsonp = e.Request.Parameters["jsonp"];
			if (!string.IsNullOrWhiteSpace(jsonp))
			{
				str = string.Format("{0}({1});", jsonp, str);
			}
			e.Response.Connection.Type = ConnectionType.Close;
			e.Response.ContentType = new ContentTypeHeader("application/json; charset=utf-8");
			e.Response.Add(serverHeader);
			var bytes = Encoding.UTF8.GetBytes(str);
			e.Response.Body.Write(bytes, 0, bytes.Length);
			e.Response.Status = HttpStatusCode.OK;
		}

		protected virtual object ProcessRequest(object sender, RequestEventArgs e)
		{
			try
			{
				var uri = e.Request.Uri.AbsolutePath;
				uri = uri.TrimEnd('/');

				foreach (var com in commands)
				{
					var verbs = new RestVerbs();
					if (com.HasVerbs)
					{
						var match = Regex.Match(uri, com.UriVerbMatch);
						if (!match.Success)
							continue;
						if ((match.Groups.Count - 1) != com.UriVerbs.Length)
							continue;

						for (int i = 0; i < com.UriVerbs.Length; i++)
							verbs.Add(com.UriVerbs[i], match.Groups[i + 1].Value);
					}
					else if (com.UriTemplate.ToLower() != uri.ToLower())
					{
						continue;
					}

					var obj = ExecuteCommand(com, verbs, e.Request.Parameters, e.Request);
					if (obj != null)
						return obj;
				}
			}
			catch (Exception exception)
			{
				return new RestObject("500")
				       	{
				       		{"error", "Internal server error."},
				       		{"errormsg", exception.Message},
				       		{"stacktrace", exception.StackTrace},
				       	};
			}
			return new RestObject("404")
			       	{
			       		{"error", "Specified API endpoint doesn't exist. Refer to the documentation for a list of valid endpoints."}
			       	};
		}

		protected virtual object ExecuteCommand(RestCommand cmd, RestVerbs verbs, IParameterCollection parms, IRequest request)
		{
			object result = cmd.Execute(verbs, parms, request);
			if (cmd.DoLog && TShock.Config.LogRest)
			{
				TShock.Log.ConsoleInfo("Anonymous requested REST endpoint: " + BuildRequestUri(cmd, verbs, parms, false));
			}

			return result;
		}

		protected virtual string BuildRequestUri(
			RestCommand cmd, RestVerbs verbs, IParameterCollection parms, bool includeToken = true
		) {
			StringBuilder requestBuilder = new StringBuilder(cmd.UriTemplate);
			char separator = '?';
			foreach (IParameter paramImpl in parms)
			{
				Parameter param = (paramImpl as Parameter);
				if (param == null || (!includeToken && param.Name.Equals("token", StringComparison.InvariantCultureIgnoreCase)))
					continue;

				requestBuilder.Append(separator);
				requestBuilder.Append(param.Name);
				requestBuilder.Append('=');
				requestBuilder.Append(param.Value);
				separator = '&';
			}
			
			return requestBuilder.ToString();
		}

		#region Dispose

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (listener != null)
				{
					listener.Stop();
					listener = null;
				}
			}
		}

		~Rest()
		{
			Dispose(false);
		}

		#endregion
	}
}