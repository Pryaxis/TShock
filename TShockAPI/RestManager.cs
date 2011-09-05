using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpServer;
using Terraria;

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
            Rest.Register(new RestCommand("/status", Status) {RequiesToken = false});
            //RegisterExamples();
        } 

        #region RestMethods

        object Status(RestVerbs verbs, IParameterCollection parameters)
        {
            var ReturnBlock = new Dictionary<string, string>();
            if (TShock.Config.EnableTokenEndpointAuthentication)
            {
                ReturnBlock.Add("status", "403");
                ReturnBlock.Add("error", "Server settings require a token for this API call.");
                return ReturnBlock;
            }
            string CurrentPlayers = "";
            int PlayerCount = 0;
            for (int i = 0; i < Main.player.Length; i++ )
            {
                if (Main.player[i].active)
                {
                    CurrentPlayers += Main.player[i].name + ", ";
                    PlayerCount++;
                }
            }
            ReturnBlock.Add("status", "200");
            ReturnBlock.Add("name", TShock.Config.ServerNickname);
            ReturnBlock.Add("port", Convert.ToString(TShock.Config.ServerPort));
            ReturnBlock.Add("playercount", Convert.ToString(PlayerCount));
            ReturnBlock.Add("players", CurrentPlayers);

            return ReturnBlock;
        }

        #endregion

        #region RestExampleMethods

        public void RegisterExamples()
        {
            Rest.Register(new RestCommand("/HelloWorld/name/{username}", UserTest));
            Rest.Register(new RestCommand("/wizard/{username}", Wizard));
        }

        //The Wizard example, for demonstrating the response convention:
        object Wizard(RestVerbs verbs, IParameterCollection parameters)
        {
            var returnBack = new Dictionary<string, string>();
            returnBack.Add("status", "200"); //Keep this in everything, 200 = ok, etc. Standard http status codes.
            returnBack.Add("error", "(If this failed, you would have a different status code and provide the error object.)"); //And only include this if the status isn't 200 or a failure
            returnBack.Add("Verified Wizard", "You're a wizard, " + verbs["username"]); //Outline any api calls and possible responses in some form of documentation somewhere
            return returnBack;
        }

        //http://127.0.0.1:8080/HelloWorld/name/{username}?type=status
        object UserTest(RestVerbs verbs, IParameterCollection parameters)
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
