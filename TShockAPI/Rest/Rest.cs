/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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
using System.Threading;
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
	/// <param name="args"><see cref="RestRequestArgs"/> object containing Verbs, Parameters, Request, and TokenData</param>
	/// <returns>Response object or null to not handle request</returns>
	public delegate object RestCommandD(RestRequestArgs args);

	/// <summary>
	/// Describes the data contained in a REST request
	/// </summary>
	public class RestRequestArgs
	{
		/// <summary>
		/// Verbs sent in the request
		/// </summary>
		public RestVerbs Verbs { get; private set; }
		/// <summary>
		/// Parameters sent in the request
		/// </summary>
		public IParameterCollection Parameters { get; private set; }
		/// <summary>
		/// The HTTP request
		/// </summary>
		public IRequest Request { get; private set; }
		/// <summary>
		/// Token data used by the request
		/// </summary>
		public SecureRest.TokenData TokenData { get; private set; }
		/// <summary>
		/// <see cref="IHttpContext"/> used by the request
		/// </summary>
		public IHttpContext Context { get; private set; }

		/// <summary>
		/// Creates a new instance of <see cref="RestRequestArgs"/> with the given verbs, parameters, request, and context.
		/// No token data is used
		/// </summary>
		/// <param name="verbs">Verbs used in the request</param>
		/// <param name="param">Parameters used in the request</param>
		/// <param name="request">The HTTP request</param>
		/// <param name="context">The HTTP context</param>
		public RestRequestArgs(RestVerbs verbs, IParameterCollection param, IRequest request, IHttpContext context)
		{
			Verbs = verbs;
			Parameters = param;
			Request = request;
			TokenData = SecureRest.TokenData.None;
			Context = context;
		}

		/// <summary>
		/// Creates a new instance of <see cref="RestRequestArgs"/> with the given verbs, parameters, request, token data, and context.
		/// </summary>
		/// <param name="verbs">Verbs used in the request</param>
		/// <param name="param">Parameters used in the request</param>
		/// <param name="request">The HTTP request</param>
		/// <param name="tokenData">Token data used in the request</param>
		/// <param name="context">The HTTP context</param>
		public RestRequestArgs(RestVerbs verbs, IParameterCollection param, IRequest request, SecureRest.TokenData tokenData, IHttpContext context)
		{
			Verbs = verbs;
			Parameters = param;
			Request = request;
			TokenData = tokenData;
			Context = context;
		}
	}

	/// <summary>
	/// A RESTful API service
	/// </summary>
	public class Rest : IDisposable
	{
		private readonly List<RestCommand> commands = new List<RestCommand>();
		/// <summary>
		/// Contains redirect URIs. The key is the base URI. The first item of the tuple is the redirect URI.
		/// The second item of the tuple is an optional "upgrade" URI which will be added to the REST response.
		/// </summary>
		private Dictionary<string, Tuple<string, string>> redirects = new Dictionary<string, Tuple<string, string>>();
		private HttpListener listener;
		private StringHeader serverHeader;
		private Timer tokenBucketTimer;
		/// <summary>
		/// Contains tokens used to manage REST authentication
		/// </summary>
		public Dictionary<string, int> tokenBucket = new Dictionary<string, int>();
		/// <summary>
		/// <see cref="IPAddress"/> the REST service is listening on
		/// </summary>
		public IPAddress Ip { get; set; }
		/// <summary>
		/// Port the REST service is listening on
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="Rest"/> listening on the given IP and port
		/// </summary>
		/// <param name="ip"><see cref="IPAddress"/> to listen on</param>
		/// <param name="port">Port to listen on</param>
		public Rest(IPAddress ip, int port)
		{
			Ip = ip;
			Port = port;
			AssemblyName assembly = this.GetType().Assembly.GetName();
			serverHeader = new StringHeader("Server", String.Format("{0}/{1}", assembly.Name, assembly.Version));
		}

		/// <summary>
		/// Starts the RESTful API service
		/// </summary>
		public virtual void Start()
		{
			try
			{
				listener = HttpListener.Create(Ip, Port);
				listener.RequestReceived += OnRequest;
				listener.Start(int.MaxValue);
				tokenBucketTimer = new Timer((e) =>
				{
					DegradeBucket();
				}, null, TimeSpan.Zero, TimeSpan.FromMinutes(Math.Max(TShock.Config.RESTRequestBucketDecreaseIntervalMinutes, 1)));

			}
			catch (Exception ex)
			{
				TShock.Log.Error("Fatal Startup Exception");
				TShock.Log.Error(ex.ToString());
				TShock.Log.ConsoleError("Invalid REST configuration: \nYou may already have a REST service bound to port {0}. \nPlease adjust your configuration and restart the server. \nPress any key to exit.", Port);
				Console.ReadLine();
				Environment.Exit(1);
			}
		}

		/// <summary>
		/// Starts the RESTful API service using the given <see cref="IPAddress"/> and port
		/// </summary>
		/// <param name="ip"><see cref="IPAddress"/> to listen on</param>
		/// <param name="port">Port to listen on</param>
		public void Start(IPAddress ip, int port)
		{
			Ip = ip;
			Port = port;
			Start();
		}

		/// <summary>
		/// Stops the RESTful API service
		/// </summary>
		public virtual void Stop()
		{
			listener.Stop();
		}

		/// <summary>
		/// Registers a command using the given route
		/// </summary>
		/// <param name="path">URL route</param>
		/// <param name="callback">Command callback</param>
		public void Register(string path, RestCommandD callback)
		{
			AddCommand(new RestCommand(path, callback));
		}

		/// <summary>
		/// Registers a <see cref="RestCommand"/>
		/// </summary>
		/// <param name="com"><see cref="RestCommand"/> to register</param>
		public void Register(RestCommand com)
		{
			AddCommand(com);
		}

		/// <summary>
		/// Registers a redirection from a given REST route to a target REST route, with an optional upgrade URI
		/// </summary>
		/// <param name="baseRoute">The base URI that will be requested</param>
		/// <param name="targetRoute">The target URI to redirect to from the base URI</param>
		/// <param name="upgradeRoute">The upgrade route that will be added as an object to the <see cref="RestObject"/> response of the target route</param>
		/// <param name="parameterized">Whether the route uses parameterized querying or not.</param>
		public void RegisterRedirect(string baseRoute, string targetRoute, string upgradeRoute = null, bool parameterized = true)
		{
			if (redirects.ContainsKey(baseRoute))
			{
				redirects.Add(baseRoute, Tuple.Create(targetRoute, upgradeRoute));
			}
			else
			{
				redirects[baseRoute] = Tuple.Create(targetRoute, upgradeRoute);
			}
		}

		/// <summary>
		/// Adds a <see cref="RestCommand"/> to the service's command list
		/// </summary>
		/// <param name="com"><see cref="RestCommand"/> to add</param>
		protected void AddCommand(RestCommand com)
		{
			commands.Add(com);
		}

		private void DegradeBucket()
		{
			var _bucket = new List<string>(tokenBucket.Keys); // Duplicate the keys so we can modify tokenBucket whilst iterating
			foreach(string key in _bucket)
			{
				int tokens = tokenBucket[key];
				if(tokens > 0)
				{
					tokenBucket[key] -= 1;
				}
				if(tokens <= 0)
				{
					tokenBucket.Remove(key);
				}
			}
		}

		/// <summary>
		/// Called when the <see cref="HttpListener"/> receives a request
		/// </summary>
		/// <param name="sender">Sender of the request</param>
		/// <param name="e">RequestEventArgs received</param>
		protected virtual void OnRequest(object sender, RequestEventArgs e)
		{
			var obj = ProcessRequest(sender, e);
			if (obj == null)
				throw new NullReferenceException("obj");

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

		/// <summary>
		/// Attempts to process a request received by the <see cref="HttpListener"/>
		/// </summary>
		/// <param name="sender">Sender of the request</param>
		/// <param name="e">RequestEventArgs received</param>
		/// <returns>A <see cref="RestObject"/> describing the state of the request</returns>
		protected virtual object ProcessRequest(object sender, RequestEventArgs e)
		{
			try
			{
				var uri = e.Request.Uri.AbsolutePath;
				uri = uri.TrimEnd('/');
				string upgrade = null;

				if (redirects.ContainsKey(uri))
				{
					upgrade = redirects[uri].Item2;
					uri = redirects[uri].Item1;
				}

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

					var obj = ExecuteCommand(com, verbs, e.Request.Parameters, e.Request, e.Context);
					if (obj != null)
					{
						if (!string.IsNullOrWhiteSpace(upgrade) && obj is RestObject)
						{
							if (!(obj as RestObject).ContainsKey("upgrade"))
							{
								(obj as RestObject).Add("upgrade", upgrade);
							}
						}

						return obj;
					}
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

		/// <summary>
		/// Executes a <see cref="RestCommand"/> using the provided verbs, parameters, request, and context objects
		/// </summary>
		/// <param name="cmd">The REST command to execute</param>
		/// <param name="verbs">The REST verbs used in the command</param>
		/// <param name="parms">The REST parameters used in the command</param>
		/// <param name="request">The HTTP request object associated with the command</param>
		/// <param name="context">The HTTP context associated with the command</param>
		/// <returns></returns>
		protected virtual object ExecuteCommand(RestCommand cmd, RestVerbs verbs, IParameterCollection parms, IRequest request, IHttpContext context)
		{
			object result = cmd.Execute(verbs, parms, request, context);
			if (cmd.DoLog && TShock.Config.LogRest)
			{
				TShock.Log.ConsoleInfo("Anonymous requested REST endpoint: " + BuildRequestUri(cmd, verbs, parms, false));
			}

			return result;
		}

		/// <summary>
		/// Builds a request URI from the parameters, verbs, and URI template of a <see cref="RestCommand"/>
		/// </summary>
		/// <param name="cmd">The REST command to take the URI template from</param>
		/// <param name="verbs">Verbs used in building the URI string</param>
		/// <param name="parms">Parameters used in building the URI string</param>
		/// <param name="includeToken">Whether or not to include a token in the URI</param>
		/// <returns></returns>
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

		/// <summary>
		/// Disposes the RESTful API service
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the RESTful API service
		/// </summary>
		/// <param name="disposing"></param>
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

		/// <summary>
		/// Destructor
		/// </summary>
		~Rest()
		{
			Dispose(false);
		}

		#endregion
	}
}