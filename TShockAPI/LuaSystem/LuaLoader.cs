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
		public string LuaAutorunPath = "";
		public LuaLoader(string path)
		{
			Lua = new Lua();
			LuaPath = path;
			LuaAutorunPath = Path.Combine(LuaPath, "autorun");
			SendLuaDebugMsg("Lua 5.1 (serverside) initialized.");

			if (!Directory.Exists(LuaPath))
			{
				Directory.CreateDirectory(LuaPath);
				Directory.CreateDirectory(LuaAutorunPath);
			}

			RegisterLuaFunctions();
			LoadServerAutoruns();
		}

		public void LoadServerAutoruns()
		{
			try
			{
				foreach (string s in Directory.GetFiles(LuaAutorunPath))
				{
					SendLuaDebugMsg("Loading: " + s);
					try
					{
						Lua.DoFile(s);
					}
					catch (LuaException e)
					{
						SendLuaDebugMsg(e.Message);
					}
				}
			}
			catch (Exception e)
			{
				SendLuaDebugMsg(e.Message);
				SendLuaDebugMsg(e.StackTrace);
			}
		}

		public void SendLuaDebugMsg(string s)
		{
			ConsoleColor previousColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("Lua: " + s);
			Console.ForegroundColor = previousColor;
		}

		public void RegisterLuaFunctions()
		{
			LuaFunctions LuaFuncs = new LuaFunctions();
			Lua.RegisterFunction("Print", LuaFuncs, LuaFuncs.GetType().GetMethod("Print"));
		}
	}

	public class LuaFunctions
	{
		public void Print(string s)
		{
			ConsoleColor previousColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(s);
			Console.ForegroundColor = previousColor;
		}
	}
}
