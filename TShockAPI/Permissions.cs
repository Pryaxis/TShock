using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    public static class Permissions
    {
        //Permissions with blank descriptions basically means its described by the commands it gives access to.

        [Description("")]
        public static readonly string causeevents;

        [Description("Required to be able to build (modify tiles and liquid)")]
        public static readonly string canbuild;

        [Description("")]
        public static readonly string kill;

        [Description("Allows you to use banned items")]
        public static readonly string usebanneditem;

        [Description("Allows you to edit the spawn")]
        public static readonly string editspawn;

        [Description("Prevents you from being kicked")]
        public static readonly string immunetokick;

        [Description("Prevents you from being banned")]
        public static readonly string immunetoban;

        [Description("Prevents you from being kicked by TShocks cheat detections")]
        public static readonly string ignorecheatdetection;

        [Description("Prevents you from being reverted by kill tile abuse detection")]
        public static readonly string ignorekilltiledetection;

        [Description("Specific log messages are sent to users with this permission")]
        public static readonly string logs;

        [Description("User gets the admin prefix/color in chat")]
        public static readonly string adminchat;

        [Todo]
        [Description("Not currently working")]
        public static readonly string reservedslot;

        [Description("User is notified when an update is available")]
        public static readonly string maintenance;

        [Description("User can kick others")]
        public static readonly string kick;

        [Description("User can ban others")]
        public static readonly string ban;

        [Description("User can modify the whitelist")]
        public static readonly string whitelist;

        [Description("User can spawn bosses")]
        public static readonly string spawnboss;

        [Description("User can spawn npcs")]
        public static readonly string spawnmob;

        [Description("User can teleport")]
        public static readonly string tp;

        [Description("User can teleport people to them")]
        public static readonly string tphere;

        [Description("User can use warps")]
        public static readonly string warp;

        [Description("User can manage warps")]
        public static readonly string managewarp;

        [Description("User can manage item bans")]
        public static readonly string manageitem;

        [Description("User can manage groups")]
        public static readonly string managegroup;

        [Description("User can edit sevrer configurations")]
        public static readonly string cfg;

        [Description("")]
        public static readonly string time;

        [Description("")]
        public static readonly string pvpfun;

        [Description("User can edit regions")]
        public static readonly string manageregion;

        [Description("Meant for super admins only")]
        public static readonly string rootonly;

        [Description("User can whisper to others")]
        public static readonly string whisper;

        [Description("")]
        public static readonly string annoy;

        [Description("User can kill all enemy npcs")]
        public static readonly string butcher;

        [Description("User can spawn items")]
        public static readonly string item;

        [Description("User can clear item drops.")]
        public static readonly string clearitems;

        [Description("")]
        public static readonly string heal;

        [Description("User can buff self")]
        public static readonly string buff;

        [Description("User can buff other players")]
        public static readonly string buffplayer;

        [Description("")]
        public static readonly string grow;

        [Description("User can change hardmode state.")]
        public static readonly string hardmode;

        [Description("User can change the homes of NPCs.")]
        public static readonly string movenpc;

        [Description("Users can stop people from TPing to them")]
        public static readonly string tpallow;

        [Description("Users can tp to anyone")]
        public static readonly string tpall;

        [Description("User can convert hallow into corruption and vice-versa")]
        public static readonly string converthardmode;

        static Permissions()
        {
            foreach (var field in typeof(Permissions).GetFields())
            {
                field.SetValue(null, field.Name);
            }
        }

        static List<Command> GetCommands(string perm)
        {
            if (Commands.ChatCommands.Count < 1)
                Commands.InitCommands();
            return Commands.ChatCommands.Where(c => c.Permission == perm).ToList();
        }

        static void DumpDescriptions()
        {
            var sb = new StringBuilder();
            foreach (var field in typeof(Permissions).GetFields())
            {
                var name = field.Name;

                var descattr = field.GetCustomAttributes(false).FirstOrDefault(o => o is DescriptionAttribute) as DescriptionAttribute;
                var desc = descattr != null && !string.IsNullOrWhiteSpace(descattr.Description) ? descattr.Description : "None";

                var commands = GetCommands(name);
                foreach (var c in commands)
                {
                    for (var i = 0; i < c.Names.Count; i++)
                    {
                        c.Names[i] = "/" + c.Names[i];
                    }
                }
                var strs = commands.Select(c => c.Name + (c.Names.Count > 1 ? "({0})".SFormat(string.Join(" ", c.Names.ToArray(), 1, c.Names.Count - 1)) : ""));

                sb.AppendLine("## <a name=\"{0}\">{0}  ".SFormat(name));
                sb.AppendLine("**Description:** {0}  ".SFormat(desc));
                sb.AppendLine("**Commands:** {0}  ".SFormat(strs.Count() > 0 ? string.Join(" ", strs) : "None"));
                sb.AppendLine();
            }

            File.WriteAllText("PermissionsDescriptions.txt", sb.ToString());
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TodoAttribute : Attribute
    {
        public string Info { get; private set; }

        public TodoAttribute(string info)
        {
            Info = info;

        }
        public TodoAttribute()
        {
        }

    }

}
