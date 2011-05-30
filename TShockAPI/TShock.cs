using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;
using Microsoft.Xna.Framework;

namespace TShockAPI
{
    public class TShock : TerrariaPlugin
    {
        public override string Name
        {
            get { return "TShock"; }
        }

        public override string Author
        {
            get { return "nicatronTg, High, Mav, and Zach"; }
        }

        public override string Description
        {
            get { return "The administration modification of the future."; }
        }

        public TShock(Main game) : base (game)
        {
            GameHooks.OnPreInitialize += OnPreInit;
            GameHooks.OnPostInitialize += OnPostInit;
            GameHooks.OnUpdate += new Action<Microsoft.Xna.Framework.GameTime>(OnUpdate);
            GameHooks.OnLoadContent += new Action<Microsoft.Xna.Framework.Content.ContentManager>(OnLoadContent);
            ServerHooks.Chat += OnChat;
        }

        void OnLoadContent(Microsoft.Xna.Framework.Content.ContentManager obj)
        {
            
        }

        void OnPreInit()
        {

        }

        void OnPostInit()
        {

        }

        void OnUpdate(GameTime time)
        {

        }

        void OnChat(int ply, string msg)
        {

        }
    }
}