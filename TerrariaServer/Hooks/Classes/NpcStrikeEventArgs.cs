using System.ComponentModel;
using Terraria;

namespace TerrariaServer.Hooks.Classes
{
	public class NpcStrikeEventArgs : HandledEventArgs
	{
		public NPC Npc
		{
			get;
			set;
		}
		public int Damage
		{
			get;
			set;
		}
		public float KnockBack
		{
			get;
			set;
		}
		public int HitDirection
		{
			get;
			set;
		}
		public bool Critical
		{
			get;
			set;
		}
        public bool NoEffect 
        { 
            get; 
            set; 
        }
        public double ReturnDamage
        {
            get; 
            set; 
        }
	}
}
