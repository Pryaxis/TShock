﻿/*
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

namespace TShockAPI
{
	/// <summary>The TShock server side character subsystem.</summary>
	internal sealed class SSC
	{
		/// <summary>A reference to the TShock plugin so we can register events.</summary>
		private TShock Plugin;

		/// <returns>A new SSC system.</returns>
		internal SSC()
		{
			// Setup GetDataHandlers
			GetDataHandlers.Connecting += Login;
			GetDataHandlers.PlayerSpawn += OnSpawn;
			GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
			GetDataHandlers.Password += Login;
			GetDataHandlers.KillMe += OnKillMe;

			// Setup ServerApi Hook Handler
			ServerApi.Hooks.GameUpdate.Register(Plugin, OnGameUpdate);
			ServerApi.Hooks.NetGreetPlayer.Register(Plugin, OnGreetPlayer);
			ServerApi.Hooks.ServerLeave.Register(Plugin, OnLeave);
		}

		internal void Login(object sender, GetDataHandledEventArgs args)
		{
			try
			{
				args.Player.IsDisabledForSSC = false;

				if (args.Player.HasPermission(Permissions.bypassssc))
				{
					args.Player.PlayerData.CopyCharacter(args.Player);
					TShock.CharacterDB.InsertPlayerData(args.Player);
				}

				args.Player.PlayerData.RestoreCharacter(args.Player);
				args.Player.LoginFailsBySsi = false;
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError(ex.ToString());

				args.Player.IsDisabledForSSC = true;
				args.Player.LoginFailsBySsi = true;
			}
		}

		internal void OnSpawn(object sender, SpawnEventArgs args)
		{
			var p = args.Player;
			if ((p.sX > 0) && (p.sY > 0) && (p.TPlayer.SpawnX > 0) &&
			    ((p.TPlayer.SpawnX != p.sX) && (p.TPlayer.SpawnY != p.sY)))
			{

				p.sX = p.TPlayer.SpawnX;
				p.sY = p.TPlayer.SpawnY;

				if (((Main.tile[p.sX, p.sY - 1].active() && Main.tile[p.sX, p.sY - 1].type == 79)) &&
				    (WorldGen.StartRoomCheck(p.sX, p.sY - 1)))
					p.Teleport(p.sX * 16, (p.sY * 16) - 48);
			}

			else if ((p.sX > 0) && (p.sY > 0))
			{
				if (((Main.tile[p.sX, p.sY - 1].active() && Main.tile[p.sX, p.sY - 1].type == 79)) &&
				    (WorldGen.StartRoomCheck(p.sX, p.sY - 1)))
					p.Teleport(p.sX * 16, (p.sY * 16) - 48);
			}
		}

		internal void OnPlayerUpdate(object sender, PlayerUpdateEventArgs args)
		{
			var tplr = args.Player.TPlayer;
			if (args.Player.Confused && args.Player.IsLoggedIn)
			{
				if (tplr.controlUp)
				{
					tplr.controlDown = true;
					tplr.controlUp = false;
				}
				else if (tplr.controlDown)
				{
					tplr.controlDown = false;
					tplr.controlUp = true;
				}

				if (tplr.controlLeft)
				{
					tplr.controlRight = true;
					tplr.controlLeft = false;
				}
				else if (tplr.controlRight)
				{
					tplr.controlRight = false;
					tplr.controlLeft = true;
				}

				tplr.Update(tplr.whoAmI);
				NetMessage.SendData((int) PacketTypes.PlayerUpdate, -1, -1, NetworkText.Empty, args.Player.Index);
				args.Handled = true;
			}
		}

		internal void OnKillMe(object sender, KillMeEventArgs args)
		{
			if (args.Player.TPlayer.difficulty == 2 && args.Player.IsLoggedIn)
			{
				if (TShock.CharacterDB.RemovePlayer(args.Player.Account.ID))
				{
					args.Player.SendErrorMessage(
						"You have fallen in hardcore mode, and your items have been lost forever.");
					TShock.CharacterDB.SeedInitialData(args.Player.Account);
				}
			}
		}

		internal void OnGameUpdate(EventArgs args)
		{
			if ((DateTime.UtcNow - Plugin.LastSave).TotalMinutes >=
			    TShock.ServerSideCharacterConfig.ServerSideCharacterSave)
			{
				foreach (TSPlayer player in TShock.Players)
				{
					// prevent null point exceptions
					if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
					{

						TShock.CharacterDB.InsertPlayerData(player);
					}
				}

				Plugin.LastSave = DateTime.UtcNow;
			}
		}

		internal void OnGreetPlayer(GreetPlayerEventArgs args)
		{
			var player = TShock.Players[args.Who];

			if (!player.IsLoggedIn)
			{
				player.IsDisabledForSSC = true;
				player.SendErrorMessage(String.Format(
					"Server side characters is enabled! Please {0}register or {0}login to play!", Commands.Specifier));
				player.LoginHarassed = true;
			}
		}

		private void OnLeave(LeaveEventArgs args)
		{
			var tsplr = TShock.Players[args.Who];
			if (tsplr == null)
			{
				return;
			}

			if (tsplr.ReceivedInfo)
			{
				if (tsplr.IsLoggedIn && !tsplr.IsDisabledPendingTrashRemoval && (!tsplr.Dead || tsplr.TPlayer.difficulty != 2))
				{
					tsplr.PlayerData.CopyCharacter(tsplr);
					TShock.CharacterDB.InsertPlayerData(tsplr);
				}
			}
		}
	}
}
