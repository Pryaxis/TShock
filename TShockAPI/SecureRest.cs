using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HttpServer;

namespace TShockAPI
{
    public delegate bool VerifyD(string username, string password);
    public class SecureRest : Rest
    {
        public Dictionary<string, object> Tokens { get; protected set; }
        public event VerifyD Verify;
        public SecureRest(IPAddress ip, int port)
            : base(ip, port)
        {
            Tokens = new Dictionary<string, object>();
            Register(new RestCommand("/token/new/{username}/{password}", newtoken) { RequiesToken = false });
        }
        object newtoken(RestVerbs verbs, IParameterCollection parameters)
        {
            var user = verbs["username"];
            var pass = verbs["password"];

            if (Verify != null && !Verify(user, pass))
                return new Dictionary<string, string> { { "Error", "Failed to verify username/password" } };

            string hash = string.Empty;
            var rand = new Random();
            var randbytes = new byte[20];
            do
            {
                rand.NextBytes(randbytes);
                hash = Tools.HashPassword(randbytes);
            } while (Tokens.ContainsKey(hash));

            Tokens.Add(hash, new Object());

            return new Dictionary<string, string> { { "Token", hash } }; ;
        }
        protected override object ExecuteCommand(RestCommand cmd, RestVerbs verbs, IParameterCollection parms)
        {
            if (cmd.RequiesToken)
            {
                var strtoken = parms["token"];
                if (strtoken == null)
                    return new Dictionary<string, string> { { "Error", "Token Missing" } };

                object token;
                if (!Tokens.TryGetValue(strtoken, out token))
                    return new Dictionary<string, string> { { "Error", "Token Invalid" } };
            }
            return base.ExecuteCommand(cmd, verbs, parms);
        }
    }
}
