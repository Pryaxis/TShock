using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer;

namespace TShockAPI {

    public class RestManager
    {
        private Rest Rest;
        public RestManager(Rest rest)
        {
            Rest = rest;
        }

        public void RegisterRestfulCommands()
        {
            Rest.Register(new RestCommand("/HelloWorld/name/{username}", usertest));
        }

        #region RestMethods
        //http://127.0.0.1:8080/HelloWorld/name/{username}?type=status
        object usertest(RestVerbs verbs, IParameterCollection parameters, RequestEventArgs request)
        {
            var ret = new Dictionary<string, string>();
            var type = parameters["type"];
            if (type == null)
            {
                ret.Add("Error", "Invalid Type");
                return ret;
            }
            if (type == "status")
            {
                ret.Add("Users", "Info here");
                return ret;
            }
            return null;
        }
        #endregion
    }
}
