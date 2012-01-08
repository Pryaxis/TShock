using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using LuaInterface;
using TShock;
using TShock.Hooks.Player;

namespace LuaPlugin
{
	public class LuaLoader : LuaPlugin
	{
		private readonly Lua _lua = null;
		public string LuaPath = "";
		public string LuaAutorunPath = "";
		public LuaLoader(string path)
		{
			_lua = new Lua();
			LuaPath = path;
			LuaAutorunPath = Path.Combine(LuaPath, "autorun");
			SendLuaDebugMsg("Lua 5.1 (serverside) initialized.");

			if (!string.IsNullOrEmpty(LuaPath) && !Directory.Exists(LuaPath))
			{
				Directory.CreateDirectory(LuaPath);
			}
			if (!string.IsNullOrEmpty(LuaAutorunPath) && !Directory.Exists(LuaAutorunPath))
			{
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
					RunLuaFile(s);
				}
			}
			catch (Exception e)
			{
				SendLuaDebugMsg(e.Message);
				SendLuaDebugMsg(e.StackTrace);
			}
		}

		public void RunLuaString(string s)
		{
			try
			{
				_lua.DoString(s);
			}
			catch (Exception e)
			{
				SendLuaDebugMsg(e.Message);
			}
		}

		public void RunLuaFile(string s)
		{
			try
			{
				_lua.DoFile(s);
			}
			catch (LuaException e)
			{
				SendLuaDebugMsg(e.Message);
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
			_lua["PlayerHooks"] = Hooks.PlayerHooks;
			_lua["Game"] = Game;
			_lua["Color"] = new Color();
			_lua["Players"] = Game.Players;
			//More Lua Functions
			LuaFunctions LuaFuncs = new LuaFunctions(this);
			var LuaFuncMethods = LuaFuncs.GetType().GetMethods();
			foreach (System.Reflection.MethodInfo method in LuaFuncMethods)
			{
				_lua.RegisterFunction(method.Name, LuaFuncs, method);
			}
		}
	}

	public class LuaFunctions
	{
		LuaLoader Parent;
		public LuaFunctions(LuaLoader parent)
		{
			Parent = parent;
		}

		[Description("Prints a message to the console from the Lua debugger.")]
		public void Print(string s)
		{
			ConsoleColor previousColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(s);
			Console.ForegroundColor = previousColor;
		}
	}
}
