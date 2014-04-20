/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

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
    /// Secure Rest command delegate including token data.
    /// </summary>
    /// <param name="parameters">Parameters in the url</param>
    /// <param name="verbs">{x} in urltemplate</param>
    /// <param name="tokenData">The data of stored for the provided token.</param>
    /// <returns>Response object or null to not handle request</returns>
    public delegate object SecureRestCommandD(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData);

    public class SecureRestCommand: RestCommand
    {
        public override bool RequiresToken { get { return true; } }
        public string[] Permissions { get; set; }

        private SecureRestCommandD callback;

        public SecureRestCommand(string name, string uritemplate, SecureRestCommandD callback, params string[] permissions)
            : base(name, uritemplate, null)
        {
            this.callback = callback;
            Permissions = permissions;
        }

        public SecureRestCommand(string uritemplate, SecureRestCommandD callback, params string[] permissions)
            : this(string.Empty, uritemplate, callback, permissions)
        {
        }

        public override object Execute(RestVerbs verbs, IParameterCollection parameters)
        {
            return new RestObject("401") { Error = "Not authorized. The specified API endpoint requires a token." };
        }

        public virtual object Execute(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
        {
            if (tokenData.Equals(SecureRest.TokenData.None))
                return new RestObject("401") { Error = "Not authorized. The specified API endpoint requires a token." };

            return callback(verbs, parameters, tokenData);
        }
    }
}
