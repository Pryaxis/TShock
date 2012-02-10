/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
	/// <param name="parameters">Parameters in the url</param>
	/// <param name="verbs">{x} in urltemplate</param>
	/// <returns>Response object or null to not handle request</returns>
	public delegate object RestCommandD(RestVerbs verbs, IParameterCollection parameters);

	public class Rest : IDisposable
	{
		private readonly List<RestCommand> commands = new List<RestCommand>();
		private HttpListener listener;
		public IPAddress Ip { get; set; }
		public int Port { get; set; }

		public Rest(IPAddress ip, int port)
		{
			Ip = ip;
			Port = port;
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
			e.Response.Connection.Type = ConnectionType.Close;
			e.Response.Add(new ContentTypeHeader("application/json"));
			e.Response.Body.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
			e.Response.Status = HttpStatusCode.OK;
			return;
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

					var obj = ExecuteCommand(com, verbs, e.Request.Parameters);
					if (obj != null)
						return obj;
				}
			}
			catch (Exception exception)
			{
				return new Dictionary<string, string>
				       	{
				       		{"status", "500"},
				       		{"error", "Internal server error."},
				       		{"errormsg", exception.Message},
				       		{"stacktrace", exception.StackTrace},
				       	};
			}
			return new Dictionary<string, string>
			       	{
			       		{"status", "404"},
			       		{"error", "Specified API endpoint doesn't exist. Refer to the documentation for a list of valid endpoints."}
			       	};
		}

		protected virtual object ExecuteCommand(RestCommand cmd, RestVerbs verbs, IParameterCollection parms)
		{
			return cmd.Callback(verbs, parms);
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