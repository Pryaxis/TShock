using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("Required to be able to place spikes")]
        public static readonly string canspike;

        [Description("Required to be able to place/pickup water")]
        public static readonly string canwater;

        [Description("Required to be able to place/pickup lava")]
        public static readonly string canlava;

        [Description("Allows you to edit the spawn")]
        public static readonly string editspawn;

        [Description("Prevents you from being kicked")]
        public static readonly string immunetokick;

        [Description("Prevents you from being banned")]
        public static readonly string immunetoban;

        [Description("Prevents you from being kicked/banned by TShocks grief detections")]
        public static readonly string ignoregriefdetection;

        [Description("Prevents you from being kicked/banned by TShocks cheat detections")]
        public static readonly string ignorecheatdetection;

        [Description("Allows you to use explosives even when they are disabled")]
        public static readonly string useexplosives;

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

        [Description("")]
        public static readonly string heal;




        static Permissions()
        {
            foreach (var field in typeof(Permissions).GetFields())
            {
                field.SetValue(null, field.Name);
            }
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
