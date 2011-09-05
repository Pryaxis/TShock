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
    /// <param name="verbs">{x} in urltemplate</param>
    /// <returns>Response object or null to not handle request</returns>
    public delegate object RestCommandD(RestVerbs verbs, IParameterCollection parameters);
    public class Rest : IDisposable
    {
        readonly List<RestCommand> commands = new List<RestCommand>();
        HttpListener listener;
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
            foreach (var com in commands)
            {
                var verbs = new RestVerbs();
                if (com.HasVerbs)
                {
                    var match = Regex.Match(e.Request.Uri.AbsolutePath, com.UriVerbMatch);
                    if (!match.Success)
                        continue;
                    if ((match.Groups.Count - 1) != com.UriVerbs.Length)
                        continue;

                    for (int i = 0; i < com.UriVerbs.Length; i++)
                        verbs.Add(com.UriVerbs[i], match.Groups[i + 1].Value);
                }
                else if (com.UriTemplate.ToLower() != e.Request.Uri.AbsolutePath.ToLower())
                {
                    continue;
                }

                var obj = ExecuteCommand(com, verbs, e.Request.Parameters);
                if (obj != null)
                    return obj;
                 
            }
            return new Dictionary<string, string> { { "status", "404" }, {"error", "Specified API endpoint doesn't exist. Refer to the documentation for a list of valid endpoints."} };
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

    public class RestVerbs : Dictionary<string, string>
    {

    }

    public class RestObject : Dictionary<string, string>
    {
        public string Status
        {
            get { return SafeGet("status"); }
            set { SafeSet("status", value); }
        }
        public string Error
        {
            get { return SafeGet("error"); }
            set { SafeSet("error", value); }
        }
        public string Response
        {
            get { return SafeGet("response"); }
            set { SafeSet("response", value); }
        }

        public RestObject(string status)
        {
            Status = status;
        }

        /// <summary>
        /// Gets value safely.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Returns null if key does not exist.</returns>
        public string SafeGet(string key)
        {
            string ret;
            if (TryGetValue(key, out ret))
                return ret;
            return null;
        }
        /// <summary>
        /// Sets/Adds value safely. If null it will remove.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SafeSet(string key, string value)
        {
            if (!ContainsKey(key))
            {
                if (value == null)
                    return;
                Add(key, value);
            }
            else
            {
                if (value != null)
                    this[key] = value;
                else
                Remove(key);
            }
        }
    }

    public class RestCommand
    {
        public string Name { get; protected set; }
        public string UriTemplate { get; protected set; }
        public string UriVerbMatch { get; protected set; }
        public string[] UriVerbs { get; protected set; }
        public RestCommandD Callback { get; protected set; }
        public bool RequiesToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Used for identification</param>
        /// <param name="uritemplate">Url template</param>
        /// <param name="callback">Rest Command callback</param>
        public RestCommand(string name, string uritemplate, RestCommandD callback)
        {
            Name = name;
            UriTemplate = uritemplate;
            UriVerbMatch = string.Join("([^/]*)", Regex.Split(uritemplate, "\\{[^\\{\\}]*\\}"));
            var matches = Regex.Matches(uritemplate, "\\{([^\\{\\}]*)\\}");
            UriVerbs = (from Match match in matches select match.Groups[1].Value).ToArray();
            Callback = callback;
            RequiesToken = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uritemplate">Url template</param>
        /// <param name="callback">Rest Command callback</param>
        public RestCommand(string uritemplate, RestCommandD callback)
            : this(string.Empty, uritemplate, callback)
        {

        }

        public bool HasVerbs
        {
            get { return UriVerbs.Length > 0; }
        }
    }
}
