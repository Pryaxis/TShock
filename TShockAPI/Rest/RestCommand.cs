using System.Linq;
using System.Text.RegularExpressions;

namespace Rests
{
    public class RestCommand
    {
        public string Name { get; protected set; }
        public string UriTemplate { get; protected set; }
        public string UriVerbMatch { get; protected set; }
        public string[] UriVerbs { get; protected set; }
        public RestCommandD Callback { get; protected set; }
        public bool RequiresToken { get; set; }

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
            UriVerbMatch = string.Format("^{0}$", string.Join("([^/]*)", Regex.Split(uritemplate, "\\{[^\\{\\}]*\\}")));
            var matches = Regex.Matches(uritemplate, "\\{([^\\{\\}]*)\\}");
            UriVerbs = (from Match match in matches select match.Groups[1].Value).ToArray();
            Callback = callback;
            RequiresToken = true;
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