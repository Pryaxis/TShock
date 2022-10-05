﻿/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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

namespace TShockAPI.Modules;

public class ReduceConsoleSpam : Module
{
	public override void Initialise() =>
		OTAPI.Hooks.Main.StatusTextChange += OnMainStatusTextChange;

	public override void Dispose() =>
		OTAPI.Hooks.Main.StatusTextChange -= OnMainStatusTextChange;

	/// <summary>
	/// Holds the last status text value, to determine if there is a suitable change to report.
	/// </summary>
	private string _lastStatusText = null;

	/// <summary>
	/// Aims to reduce the amount of console spam by filtering out load/save progress
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e">OTAPI event</param>
	private void OnMainStatusTextChange(object sender, OTAPI.Hooks.Main.StatusTextChangeArgs e)
	{
		void WriteIfChange(string text)
		{
			if (_lastStatusText != text)
			{
				Console.WriteLine(text); // write it manually instead of terraria which causes double writes
				_lastStatusText = text;
			}
		}
		bool replace(string text)
		{
			if (e.Value.StartsWith(text))
			{
				var segment = e.Value.Substring(0, text.Length);
				WriteIfChange(segment);
				e.Value = "";
				return true;
			}
			return false;
		}

		if (replace("Resetting game objects")
			|| replace("Settling liquids")
			|| replace("Loading world data")
			|| replace("Saving world data")
			|| replace("Validating world save"))
			return;

		// try parsing % - [text] - %
		const string FindMaster = "% - ";
		const string FindSub = " - ";
		var master = e.Value.IndexOf(FindMaster);
		if (master > -1)
		{
			var sub = e.Value.LastIndexOf(FindSub);
			if (master > -1 && sub > master)
			{
				var mprogress = e.Value.Substring(0, master + 1/*%*/);
				var sprogress = e.Value.Substring(sub + FindSub.Length);
				if (mprogress.EndsWith("%") && sprogress.EndsWith("%"))
				{
					var text = e.Value.Substring(master + FindMaster.Length, sub - master - FindMaster.Length).Trim();

					if (text.Length > 0 && !(
						// relogic has made a mess of this
						(
							_lastStatusText != "Validating world save"
							|| _lastStatusText != "Saving world data"
						)
						&& text == "Finalizing world"
					))
						WriteIfChange(text);

					e.Value = "";
				}
			}
		}
	}

}

