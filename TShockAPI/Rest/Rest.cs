using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HttpServer;
using HttpServer.Headers;
using Newtonsoft.Json;
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

		protected virtual void OnRequest(object sender, RequestEventArgs e)
		{
			var obj = ProcessRequest(sender, e);
			if (obj == null)
				throw new NullReferenceException("obj");

			var str = JsonConvert.SerializeObject(obj, Formatting.Indented);
			e.Response.Connection.Type = ConnectionType.Close;
			e.Response.Body.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
			e.Response.Status = HttpStatusCode.OK;
			return;
		}

		protected virtual object ProcessRequest(object sender, RequestEventArgs e)
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