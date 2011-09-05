using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HttpServer;
using HttpServer.Headers;
using Newtonsoft.Json;
using HttpListener = HttpServer.HttpListener;

namespace TShockAPI
{
    /// <summary>
    /// Rest command delegate
    /// </summary>
    /// <param name="parameters">Parameters in the url</param>
    /// <param name="request">Http request</param>
    /// <returns>Response object or null to not handle request</returns>
    public delegate object RestCommandD(Dictionary<string,string> verbs, IParameterCollection parameters, RequestEventArgs request);
    public class Rest : IDisposable
    {
        readonly List<RestCommand> commands = new List<RestCommand>();
        HttpListener listener;

        public Rest(IPAddress ip, int port)
        {
            listener = HttpListener.Create(ip, port);
            listener.RequestReceived += OnRequest;
        }

        public void Start()
        {
            listener.Start(int.MaxValue);
        }
        public void Stop()
        {
            listener.Stop();
        }

        public void Register(string path, RestCommandD callback)
        {
            Register(new RestCommand(path, callback));
        }

        public void Register(RestCommand com)
        {
            commands.Add(com);
        }

        void OnRequest(object sender, RequestEventArgs e)
        {
            var obj = Process(sender, e);
            if (obj == null)
                throw new NullReferenceException("obj");
            var str = JsonConvert.SerializeObject(obj, Formatting.Indented);
            e.Response.Connection.Type = ConnectionType.Close;
            e.Response.Body.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
            e.Response.Status = HttpStatusCode.OK;
            return;
        }

        protected virtual object Process(object sender, RequestEventArgs e)
        {
            foreach (var com in commands)
            {
                var matches = Regex.Matches(e.Request.Uri.AbsolutePath, com.UriMatch);
                if (matches.Count == com.UriNames.Length)
                {
                    var verbs = new Dictionary<string, string>();
                    for (int i = 0; i < matches.Count; i++)
                        verbs.Add(com.UriNames[i], matches[i].Groups[1].Value);

                    var obj = com.Callback(verbs, e.Request.Parameters, e);
                    if (obj != null)
                        return obj;
                }
            }
            return new Dictionary<string, string> { { "Error", "Invalid request" } };
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
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

    public class RestCommand
    {
        public string UriTemplate { get; protected set; }
        public string UriMatch { get; protected set; }
        public string[] UriNames { get; protected set; }
        public RestCommandD Callback { get; protected set; }

        public RestCommand(string uritemplate, RestCommandD callback)
        {
            UriTemplate = uritemplate;
            UriMatch = string.Join("([^/]*)", Regex.Split(uritemplate, "\\{[^\\{\\}]*\\}"));
            var matches = Regex.Matches(uritemplate, "\\{([^\\{\\}]*)\\}");
            UriNames = (from Match match in matches select match.Groups[1].Value).ToArray();
            Callback = callback;
        }
    }
}
