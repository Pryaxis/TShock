using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LuaInterface;

namespace TShockAPI.LuaSystem
{
	public class LuaLoader
	{
		private Lua Lua = null;
		public string LuaPath = "";
		public LuaLoader(string path)
		{
			Lua = new Lua();
			LuaPath = path;
			SendLuaDebugMsg("Lua 5.1 (serverside) initialized.");
		}

		public void LoadServerAutoruns()
		{
			foreach (string s in Directory.GetFiles(Path.Combine(LuaPath, "autorun")))
			{
				Lua.DoFile(s);
				SendLuaDebugMsg("Loaded file: " + s);
			}
		}

		public void SendLuaDebugMsg(string s)
		{
			ConsoleColor previousColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Lua: " + s);
			Console.ForegroundColor = previousColor;
		}
	}
}
