using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
    public delegate object RestCommandD(IParameterCollection parameters, RequestEventArgs request);
    public class Rest : IDisposable
    {
        List<RestCommand> commands = new List<RestCommand>();
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
            var coms = commands.Where(r => r.Path.ToLower().Equals(e.Request.Uri.AbsolutePath.ToLower()));
            foreach (var com in coms)
            {
                var obj = com.Callback(e.Request.Parameters, e);
                if (obj != null)
                {
                    var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                    e.Response.Connection.Type = ConnectionType.Close;
                    e.Response.Body.Write(Encoding.ASCII.GetBytes(str), 0, str.Length);
                    e.Response.Status = HttpStatusCode.OK;
                    return;
                }
            }
            string error = "Error: Invalid request";
            e.Response.Connection.Type = ConnectionType.Close;
            e.Response.Body.Write(Encoding.ASCII.GetBytes(error), 0, error.Length);
            e.Response.Status = HttpStatusCode.InternalServerError;
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
        public string Path { get; protected set; }
        public RestCommandD Callback { get; protected set; }

        public RestCommand(string path, RestCommandD callback)
        {
            Path = path;
            Callback = callback;
        }
    }
}
