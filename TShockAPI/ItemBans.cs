/*
TShock, a server mod for Terraria
Copyright (C) 2011-2018 Pryaxis & TShock Contributors

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
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using TShockAPI.DB;
using TShockAPI.Net;
using Terraria;
using Microsoft.Xna.Framework;
using OTAPI.Tile;
using TShockAPI.Localization;
using static TShockAPI.GetDataHandlers;
using TerrariaApi.Server;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using System.Data;

namespace TShockAPI
{
  /// <summary>The TShock item ban subsystem. It handles keeping things out of people's inventories.</summary>
  internal sealed class ItemBans
  {

    /// <summary>The database connection layer to for the item ban subsystem.</summary>
    private ItemManager DataModel;

    /// <summary>The last time the second update process was run. Used to throttle task execution.</summary>
    private DateTime LastTimelyRun = DateTime.UtcNow;

    /// <summary>A reference to the TShock plugin so we can register events.</summary>
    private TShock Plugin;

    /// <summary>Creates an ItemBan system given a plugin to register events to and a database.</summary>
    /// <param name="plugin">The executing plugin.</param>
    /// <param name="database">The database the item ban information is stored in.</param>
    /// <returns>A new item ban system.</returns>
    internal ItemBans(TShock plugin, IDbConnection database)
    {
      DataModel = new ItemManager(database);
      Plugin = plugin;

      ServerApi.Hooks.GameUpdate.Register(plugin, OnGameUpdate);
    }

    /// <summary>Called on the game update loop (the XNA tickrate).</summary>
    /// <param name="args">The standard event arguments.</param>
    internal void OnGameUpdate(EventArgs args)
    {
      if ((DateTime.UtcNow - LastTimelyRun).TotalSeconds >= 1)
      {
        OnSecondlyUpdate(args);
      }
    }

    /// <summary>Called by OnGameUpdate once per second to execute tasks regularly but not too often.</summary>
    /// <param name="args">The standard event arguments.</param>
    internal void OnSecondlyUpdate(EventArgs args)
    {
      DisableFlags flags = TShock.Config.DisableSecondUpdateLogs ? DisableFlags.WriteToConsole : DisableFlags.WriteToLogAndConsole;

      foreach (TSPlayer player in TShock.Players)
      {
        // SSC inventory/held item check (logged out)
        if (Main.ServerSideCharacter && !player.IsLoggedIn)
        {
          if (player.IsBeingDisabled())
          {
            player.Disable(flags: flags);
          }
          else if (DataModel.ItemIsBanned(EnglishLanguage.GetItemNameById(player.TPlayer.inventory[player.TPlayer.selectedItem].netID), player))
          {
            player.Disable($"holding banned item: {player.TPlayer.inventory[player.TPlayer.selectedItem].Name}", flags);
            player.SendErrorMessage($"You are holding a banned item: {player.TPlayer.inventory[player.TPlayer.selectedItem].Name}");
          }
          continue;
        }

        // Normal item ban check
        if (!Main.ServerSideCharacter || (Main.ServerSideCharacter || player.IsLoggedIn))
        {
        }
      }

      // Set the update time to now, so that we know when to execute next.
      // We do this at the end so that the task can't re-execute faster than we expected.
      // (If we did this at the start of the method, the method execution would count towards the timer.)
      LastTimelyRun = DateTime.UtcNow;
    }

  }
}