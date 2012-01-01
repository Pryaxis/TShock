/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System;
using System.IO;
using System.Net;
using System.Threading;
using Terraria;

namespace TShockAPI
{
	public class StatTracker
	{
		private Utils Utils = TShock.Utils;
		public DateTime lastcheck = DateTime.MinValue;
		private readonly int checkinFrequency = 5;

		public void CheckIn()
		{
			if ((DateTime.Now - lastcheck).TotalMinutes >= checkinFrequency)
			{
				ThreadPool.QueueUserWorkItem(CallHome);
				lastcheck = DateTime.Now;
			}
		}

		private void CallHome(object state)
		{
			string fp;
			string lolpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.tshock/";
			if (!Directory.Exists(lolpath))
			{
				Directory.CreateDirectory(lolpath);
			}
			if (!File.Exists(Path.Combine(lolpath, Netplay.serverPort + ".fingerprint")))
			{
				fp = "";
				int random = Utils.Random.Next(500000, 1000000);
				fp += random;

				fp = Utils.HashPassword(Netplay.serverIP + fp + Netplay.serverPort + Netplay.serverListenIP);
				TextWriter tw = new StreamWriter(Path.Combine(lolpath, Netplay.serverPort + ".fingerprint"));
				tw.Write(fp);
				tw.Close();
			}
			else
			{
				fp = "";
				TextReader tr = new StreamReader(Path.Combine(lolpath, Netplay.serverPort + ".fingerprint"));
				fp = tr.ReadToEnd();
				tr.Close();
			}

			using (var client = new WebClient())
			{
				client.Headers.Add("user-agent",
				                   "TShock (" + TShock.VersionNum + ")");
				try
				{
					string response;
					if (TShock.Config.DisablePlayerCountReporting)
					{
						response =
							client.DownloadString("http://tshock.co/tickto.php?do=log&fp=" + fp + "&ver=" + TShock.VersionNum + "&os=" +
							                      Environment.OSVersion + "&mono=" + Main.runningMono + "&port=" + Netplay.serverPort +
							                      "&plcount=0");
					}
					else
					{
						response =
							client.DownloadString("http://tshock.co/tickto.php?do=log&fp=" + fp + "&ver=" + TShock.VersionNum + "&os=" +
							                      Environment.OSVersion + "&mono=" + Main.runningMono + "&port=" + Netplay.serverPort +
							                      "&plcount=" + TShock.Utils.ActivePlayers());
					}
					Log.ConsoleInfo("Stat Tracker: " + response + "\n");
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
			}
		}
	}
}