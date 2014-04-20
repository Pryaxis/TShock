/*
TShock, a server mod for Terraria
Copyright (C) 2014 Commaster

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

using HttpServer;

namespace Rests
{
    /// <summary>
    /// Rest command delegate
    /// </summary>
    /// <param name="parameters">Parameters in the url</param>
    /// <param name="verbs">{x} in urltemplate</param>
    /// <returns>Response object or null to not handle request</returns>
    public delegate object SecureWebRestCommandD(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData);

    class SecureWebRestCommand : SecureRestCommand
    {
        public override bool WebHandler { get { return true; } }

        private SecureWebRestCommandD callback;

        public SecureWebRestCommand(string name, string uritemplate, SecureWebRestCommandD callback, params string[] permissions)
            : base(name, "/web" + uritemplate, null)
        {
            this.callback = callback;
            Permissions = permissions;
        }

        public SecureWebRestCommand(string uritemplate, SecureWebRestCommandD callback, params string[] permissions)
            : this(string.Empty, uritemplate, callback, permissions)
        {
        }

        public override object Execute(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
        {
            if (tokenData.Equals(SecureRest.TokenData.None))
                return new RestObject("401") { Error = "Not authorized. The specified API endpoint requires a token." };

            object response = callback(verbs, parameters, tokenData);
            if (response != null)
            {
                ((RestObject)response)["Web"] = true;
            }
            return response;
        }
    }
}
