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

using System.Linq;
using System.Text.RegularExpressions;
using HttpServer;

namespace Rests
{
	public class RestCommand
	{
		public string Name { get; protected set; }
		public string UriTemplate { get; protected set; }
		public string UriVerbMatch { get; protected set; }
		public string[] UriVerbs { get; protected set; }
		public virtual bool RequiresToken { get { return false; } }
		public bool DoLog { get; set; }

		private RestCommandD callback;

		/// <summary>
		/// Creates a new <see cref="RestCommand"/> used with the REST API
		/// </summary>
		/// <param name="name">Used for identification</param>
		/// <param name="uritemplate">Url template</param>
		/// <param name="callback">Rest Command callback</param>
		public RestCommand(string name, string uritemplate, RestCommandD callback)
		{
			Name = name;
			UriTemplate = uritemplate;
			UriVerbMatch = string.Format("^{0}$", string.Join("([^/]*)", Regex.Split(uritemplate, "\\{[^\\{\\}]*\\}")));
			var matches = Regex.Matches(uritemplate, "\\{([^\\{\\}]*)\\}");
			UriVerbs = (from Match match in matches select match.Groups[1].Value).ToArray();
			this.callback = callback;
			DoLog = true;
		}

		/// <summary>
		/// Creates a new <see cref="RestCommand"/> used with the REST API
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

		public virtual object Execute(RestVerbs verbs, IParameterCollection parameters, IRequest request, IHttpContext context)
		{
			return callback(new RestRequestArgs(verbs, parameters, request, context));
		}
	}

	public class SecureRestCommand: RestCommand
	{
		public override bool RequiresToken { get { return true; } }
		public string[] Permissions { get; set; }

		private RestCommandD callback;

		public SecureRestCommand(string name, string uritemplate, RestCommandD callback, params string[] permissions)
			: base(name, uritemplate, null)
		{
			this.callback = callback;
			Permissions = permissions;
		}

		public SecureRestCommand(string uritemplate, RestCommandD callback, params string[] permissions)
			: this(string.Empty, uritemplate, callback, permissions)
		{
		}

		public override object Execute(RestVerbs verbs, IParameterCollection parameters, IRequest request, IHttpContext context)
		{
			return new RestObject("401") { Error = "Not authorized. The specified API endpoint requires a token." };
		}

		public object Execute(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData, IRequest request, IHttpContext context)
		{
			if (tokenData.Equals(SecureRest.TokenData.None))
				return new RestObject("401") { Error = "Not authorized. The specified API endpoint requires a token." };

			return callback(new RestRequestArgs(verbs, parameters, request, tokenData, context));
		}
	}
}