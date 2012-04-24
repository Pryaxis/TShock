/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.Threading;
using Terraria;

namespace TShockAPI
{
	public class BackupManager
	{
		public string BackupPath { get; set; }
		public int Interval { get; set; }
		public int KeepFor { get; set; }

		private DateTime lastbackup = DateTime.UtcNow;

		public BackupManager(string path)
		{
			BackupPath = path;
		}

		public bool IsBackupTime
		{
			get { return (Interval > 0) && ((DateTime.UtcNow - lastbackup).TotalMinutes >= Interval); }
		}

		public void Backup()
		{
			lastbackup = DateTime.UtcNow;
			ThreadPool.QueueUserWorkItem(DoBackup);
			ThreadPool.QueueUserWorkItem(DeleteOld);
		}

		private void DoBackup(object o)
		{
			try
			{
				string worldname = Main.worldPathName;
				string name = Path.GetFileName(worldname);

				Main.worldPathName = Path.Combine(BackupPath, string.Format("{0}.{1:dd.MM.yy-HH.mm.ss}.bak", name, DateTime.UtcNow));

				string worldpath = Path.GetDirectoryName(Main.worldPathName);
				if (worldpath != null && !Directory.Exists(worldpath))
					Directory.CreateDirectory(worldpath);

				TShock.Utils.Broadcast("Server map saving, potential lag spike");
				Console.WriteLine("Backing up world...");

				SaveManager.Instance.SaveWorld();
				Console.WriteLine("World backed up");
				Console.ForegroundColor = ConsoleColor.Gray;
				Log.Info(string.Format("World backed up ({0})", Main.worldPathName));

				Main.worldPathName = worldname;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Backup failed");
				Console.ForegroundColor = ConsoleColor.Gray;
				Log.Error("Backup failed");
				Log.Error(ex.ToString());
			}
		}

		private void DeleteOld(object o)
		{
			if (KeepFor <= 0)
				return;
			foreach (var fi in new DirectoryInfo(BackupPath).GetFiles("*.bak"))
			{
				if ((DateTime.UtcNow - fi.LastWriteTimeUtc).TotalMinutes > KeepFor)
				{
					fi.Delete();
				}
			}
		}
	}
}