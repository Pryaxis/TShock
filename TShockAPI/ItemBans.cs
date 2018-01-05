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
  internal sealed class ItemBans
  {

    private ItemManager DataModel;

    private DateTime LastTimelyRun = DateTime.UtcNow;

    private TShock Plugin;

    internal ItemBans(TShock plugin, IDbConnection database)
    {
      DataModel = new ItemManager(database);
      Plugin = plugin;

      ServerApi.Hooks.GameUpdate.Register(plugin, OnGameUpdate);
    }

    internal void OnGameUpdate(EventArgs args)
    {
      if ((DateTime.UtcNow - LastTimelyRun).TotalSeconds >= 1)
      {
        OnSecondlyUpdate(args);
      }
    }

    internal void OnSecondlyUpdate(EventArgs args)
    {

      LastTimelyRun = DateTime.UtcNow;
    }

  }
}