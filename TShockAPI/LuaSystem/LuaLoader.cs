using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using LuaInterface;

namespace TShockAPI.LuaSystem
{
	public class LuaLoader
	{
		private Lua Lua = null;
		public string LuaPath = "";
		public string LuaAutorunPath = "";

		public Dictionary<string, KeyValuePair<string, string>> Hooks = new Dictionary
			<string, KeyValuePair<string, string>>(); 
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
			HookTest();
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
				Lua.DoString(s);
			}
			catch (LuaException e)
			{
				SendLuaDebugMsg(e.Message);
			}
		}

		public void RunLuaFile(string s)
		{
			try
			{
				Lua.DoFile(s);
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
			LuaFunctions LuaFuncs = new LuaFunctions();
			Lua.RegisterFunction("Print", LuaFuncs, LuaFuncs.GetType().GetMethod("Print"));
			Lua.RegisterFunction("Hook", LuaFuncs, LuaFuncs.GetType().GetMethod("Hook"));

		}

		public void HookTest()
		{

			Console.WriteLine("Running hook test.");

			foreach (KeyValuePair<string, KeyValuePair<string, string>> kv in Hooks)
			{
				KeyValuePair<string, string> hook = kv.Value;
				LuaFunction lf = FindLuaFunction(hook.Value);

				if (lf != null)
				{
					lf.Call();
				}
			}
		}

		public LuaFunction FindLuaFunction(string name)
		{
			try
			{
				return Lua.GetFunction(name);
			} catch (Exception)
			{
				return null;
			}
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

		public void Hook(string hook, string key, string callback)
		{
			KeyValuePair<string, string> hhook = new KeyValuePair<string, string>(hook, callback);
			TShock.LuaLoader.Hooks.Add(key, hhook);
		}
	}
}
