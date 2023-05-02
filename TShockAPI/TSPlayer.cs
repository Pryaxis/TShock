﻿/*
TShock, a server mod for Terraria
Copyright (C) 2011-2022 Pryaxis & TShock Contributors

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

using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Net;
using Timer = System.Timers.Timer;
using System.Linq;
using Terraria.GameContent.Creative;

namespace TShockAPI
{
	/// <summary>
	/// Bitflags used with the <see cref="Disable(string, DisableFlags)"></see> method
	/// </summary>
	[Flags]
	public enum DisableFlags
	{
		/// <summary>
		/// Disable the player and leave no messages
		/// </summary>
		None,
		/// <summary>
		/// Write the Disable message to the console
		/// </summary>
		WriteToConsole,
		/// <summary>
		/// Write the Disable message to the log
		/// </summary>
		WriteToLog,
		/// <summary>
		/// Equivalent to WriteToConsole | WriteToLog
		/// </summary>
		WriteToLogAndConsole
	}

	public class TSPlayer
	{
		/// <summary>
		/// This represents the server as a player.
		/// </summary>
		public static readonly TSServerPlayer Server = new TSServerPlayer();

		/// <summary>
		/// This player represents all the players.
		/// </summary>
		public static readonly TSPlayer All = new TSPlayer("All");

		/// <summary>
		/// Finds a TSPlayer based on name or ID.
		/// If the string comes with tsi: or tsn:, we'll only return a list with one element,
		/// either the player with the matching ID or name, respectively.
		/// </summary>
		/// <param name="plr">Player name or ID</param>
		/// <returns>A list of matching players</returns>
		public static List<TSPlayer> FindByNameOrID(string search)
		{
			var found = new List<TSPlayer>();

			search = search.Trim();

			// tsi: and tsn: are used to disambiguate between usernames and not
			// and are also both 3 characters to remove them from the search
			// (the goal was to pick prefixes unlikely to be used by names)
			// (and not to collide with other prefixes used by other commands)
			var exactIndexOnly = search.StartsWith("tsi:");
			var exactNameOnly = search.StartsWith("tsn:");

			if (exactNameOnly || exactIndexOnly)
				search = search.Remove(0, 4);

			// Avoid errors caused by null search
			if (search == null || search == "")
				return found;

			byte searchID;
			if (byte.TryParse(search, out searchID) && searchID < Main.maxPlayers)
			{
				TSPlayer player = TShock.Players[searchID];
				if (player != null && player.Active)
				{
					if (exactIndexOnly)
						return new List<TSPlayer> { player };
					found.Add(player);
				}
			}

			string searchLower = search.ToLower();
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null)
				{
					if ((search == player.Name) && exactNameOnly)
						return new List<TSPlayer> { player };
					if (player.Name.ToLower().StartsWith(searchLower))
						found.Add(player);
				}
			}
			return found;
		}

		/// <summary>
		/// Used in preventing players from seeing the npc spawnrate permission error on join.
		/// </summary>
		internal bool HasReceivedNPCPermissionError { get; set; }

		/// <summary>
		/// The amount of tiles that the player has killed in the last second.
		/// </summary>
		public int TileKillThreshold { get; set; }

		/// <summary>
		/// The amount of tiles the player has placed in the last second.
		/// </summary>
		public int TilePlaceThreshold { get; set; }

		/// <summary>
		/// The amount of liquid (in tiles) that the player has placed in the last second.
		/// </summary>
		public int TileLiquidThreshold { get; set; }

		/// <summary>
		/// The amount of tiles that the player has painted in the last second.
		/// </summary>
		public int PaintThreshold { get; set; }

		/// <summary>
		/// The number of projectiles created by the player in the last second.
		/// </summary>
		public int ProjectileThreshold { get; set; }

		/// <summary>
		/// The number of HealOtherPlayer packets sent by the player in the last second.
		/// </summary>
		public int HealOtherThreshold { get; set; }

		/// <summary>
		/// A timer to keep track of whether or not the player has recently thrown an explosive
		/// </summary>
		public int RecentFuse = 0;

		/// <summary>
		/// Whether to ignore packets that are SSC-relevant.
		/// </summary>
		public bool IgnoreSSCPackets { get; set; }

		/// <summary>
		/// A system to delay Remembered Position Teleports a few seconds
		/// </summary>
		public int RPPending = 0;

		public int sX = -1;
		public int sY = -1;

		/// <summary>
		/// A queue of tiles destroyed by the player for reverting.
		/// </summary>
		public Dictionary<Vector2, ITile> TilesDestroyed { get; protected set; }

		/// <summary>
		/// A queue of tiles placed by the player for reverting.
		/// </summary>
		public Dictionary<Vector2, ITile> TilesCreated { get; protected set; }

		/// <summary>
		/// The player's group.
		/// </summary>
		public Group Group
		{
			get
			{
				if (tempGroup != null)
					return tempGroup;
				return group;
			}
			set { group = value; }
		}

		/// <summary>
		/// The player's temporary group.  This overrides the user's actual group.
		/// </summary>
		public Group tempGroup = null;

		public Timer tempGroupTimer;

		private Group group = null;

		public bool ReceivedInfo { get; set; }

		/// <summary>
		/// The players index in the player array( Main.players[] ).
		/// </summary>
		public int Index { get; protected set; }

		/// <summary>
		/// The last time the player changed their team or pvp status.
		/// </summary>
		public DateTime LastPvPTeamChange;

		/// <summary>
		/// Temp points for use in regions and other plugins.
		/// </summary>
		public Point[] TempPoints = new Point[2];

		/// <summary>
		/// Whether the player is waiting to place/break a tile to set as a temp point.
		/// </summary>
		public int AwaitingTempPoint { get; set; }

		/// <summary>
		/// A list of command callbacks indexed by the command they need to do.
		/// </summary>
		public Dictionary<string, Action<object>> AwaitingResponse;

		public bool AwaitingName { get; set; }

		public string[] AwaitingNameParameters { get; set; }

		/// <summary>
		/// The last time a player broke a grief check.
		/// </summary>
		public DateTime LastThreat { get; set; }

		/// <summary>
		/// Whether the player should see logs.
		/// </summary>
		public bool DisplayLogs = true;

		/// <summary>
		/// The last player that the player whispered with (to or from).
		/// </summary>
		public TSPlayer LastWhisper;

		/// <summary>
		/// The number of unsuccessful login attempts.
		/// </summary>
		public int LoginAttempts { get; set; }

		/// <summary>
		/// Unused.
		/// </summary>
		public Vector2 TeleportCoords = new Vector2(-1, -1);

		/// <summary>
		/// The player's last known position from PlayerUpdate packet.
		/// </summary>
		public Vector2 LastNetPosition = Vector2.Zero;

		/// <summary>
		/// UserAccount object associated with the player.
		/// Set when the player logs in.
		/// </summary>
		public UserAccount Account { get; set; }

		/// <summary>
		/// Whether the player performed a valid login attempt (i.e. entered valid user name and password) but is still blocked
		/// from logging in because of SSI.
		/// </summary>
		public bool LoginFailsBySsi { get; set; }

		/// <summary>
		/// Whether the player is logged in or not.
		/// </summary>
		public bool IsLoggedIn;

		/// <summary>
		/// Whether the player has sent their whole inventory to the server while connecting.
		/// </summary>
		public bool HasSentInventory { get; set; }

		/// <summary>
		/// Whether the player has been nagged about logging in.
		/// </summary>
		public bool HasBeenNaggedAboutLoggingIn;

		/// <summary>
		/// Whether other players can teleport to the player.
		/// </summary>
		public bool TPAllow = true;

		/// <summary>
		/// Whether the player is muted or not.
		/// </summary>
		public bool mute;

		private Player FakePlayer;

		public bool RequestedSection;

		/// <summary>
		/// The player's respawn timer.
		/// </summary>
		public int RespawnTimer
		{
			get => _respawnTimer;
			set => TPlayer.respawnTimer = (_respawnTimer = value) * 60;
		}
		private int _respawnTimer;

		/// <summary>
		/// Whether the player is dead or not.
		/// </summary>
		public bool Dead;

		public string Country = "??";

		/// <summary>
		/// The players difficulty( normal[softcore], mediumcore, hardcore ).
		/// </summary>
		public int Difficulty;

		private string CacheIP;

		/// <summary>Determines if the player is disabled by the SSC subsystem for not being logged in.</summary>
		public bool IsDisabledForSSC = false;

		/// <summary>Determines if the player is disabled by Bouncer for having hacked item stacks.</summary>
		public bool IsDisabledForStackDetection = false;

		/// <summary>Determines if the player is disabled by the item bans system for having banned wearables on the server.</summary>
		public bool IsDisabledForBannedWearable = false;

		/// <summary>Determines if the player is disabled for not clearing their trash. A re-login is the only way to reset this.</summary>
		public bool IsDisabledPendingTrashRemoval;

		/// <summary>Checks to see if active throttling is happening on events by Bouncer. Rejects repeated events by malicious clients in a short window.</summary>
		/// <returns>If the player is currently being throttled by Bouncer, or not.</returns>
		public bool IsBouncerThrottled()
		{
			return (DateTime.UtcNow - LastThreat).TotalMilliseconds < 5000;
		}

		/// <summary>Easy check if a player has any of IsDisabledForSSC, IsDisabledForStackDetection, IsDisabledForBannedWearable, or IsDisabledPendingTrashRemoval set. Or if they're not logged in and a login is required.</summary>
		/// <returns>If any of the checks that warrant disabling are set on this player. If true, Disable() is repeatedly called on them.</returns>
		public bool IsBeingDisabled()
		{
			return IsDisabledForSSC
			|| IsDisabledForStackDetection
			|| IsDisabledForBannedWearable
			|| IsDisabledPendingTrashRemoval
			|| !IsLoggedIn && TShock.Config.Settings.RequireLogin;
		}

		/// <summary>Checks to see if a player has hacked item stacks in their inventory, and messages them as it checks.</summary>
		/// <param name="shouldWarnPlayer">If the check should send a message to the player with the results of the check.</param>
		/// <returns>True if any stacks don't conform.</returns>
		public bool HasHackedItemStacks(bool shouldWarnPlayer = false)
		{
			// Iterates through each inventory location a player has.
			// This section is sub divided into number ranges for what each range of slots corresponds to.
			bool check = false;

			Item[] inventory = TPlayer.inventory;
			Item[] armor = TPlayer.armor;
			Item[] dye = TPlayer.dye;
			Item[] miscEquips = TPlayer.miscEquips;
			Item[] miscDyes = TPlayer.miscDyes;
			Item[] piggy = TPlayer.bank.item;
			Item[] safe = TPlayer.bank2.item;
			Item[] forge = TPlayer.bank3.item;
			Item[] voidVault = TPlayer.bank4.item;
			Item[] loadout1Armor = TPlayer.Loadouts[0].Armor;
			Item[] loadout1Dye = TPlayer.Loadouts[0].Dye;
			Item[] loadout2Armor = TPlayer.Loadouts[1].Armor;
			Item[] loadout2Dye = TPlayer.Loadouts[1].Dye;
			Item[] loadout3Armor = TPlayer.Loadouts[2].Armor;
			Item[] loadout3Dye = TPlayer.Loadouts[2].Dye;

			Item trash = TPlayer.trashItem;
			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				if (i < NetItem.InventoryIndex.Item2)
				{
					// From above: this is slots 0-58 in the inventory.
					// 0-58
					Item item = new Item();
					if (inventory[i] != null && inventory[i].netID != 0)
					{
						item.netDefaults(inventory[i].netID);
						item.Prefix(inventory[i].prefix);
						item.AffixName();
						if (inventory[i].stack > item.maxStack || inventory[i].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove item {0} ({1}) and then rejoin.", item.Name, inventory[i].stack));
							}
						}
					}
				}
				else if (i < NetItem.ArmorIndex.Item2)
				{
					// 59-78
					var index = i - NetItem.ArmorIndex.Item1;
					Item item = new Item();
					if (armor[index] != null && armor[index].netID != 0)
					{
						item.netDefaults(armor[index].netID);
						item.Prefix(armor[index].prefix);
						item.AffixName();
						if (armor[index].stack > item.maxStack || armor[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove armor {0} ({1}) and then rejoin.", item.Name, armor[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.DyeIndex.Item2)
				{
					// 79-88
					var index = i - NetItem.DyeIndex.Item1;
					Item item = new Item();
					if (dye[index] != null && dye[index].netID != 0)
					{
						item.netDefaults(dye[index].netID);
						item.Prefix(dye[index].prefix);
						item.AffixName();
						if (dye[index].stack > item.maxStack || dye[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove dye {0} ({1}) and then rejoin.", item.Name, dye[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.MiscEquipIndex.Item2)
				{
					// 89-93
					var index = i - NetItem.MiscEquipIndex.Item1;
					Item item = new Item();
					if (miscEquips[index] != null && miscEquips[index].netID != 0)
					{
						item.netDefaults(miscEquips[index].netID);
						item.Prefix(miscEquips[index].prefix);
						item.AffixName();
						if (miscEquips[index].stack > item.maxStack || miscEquips[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove item {0} ({1}) and then rejoin.", item.Name, miscEquips[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.MiscDyeIndex.Item2)
				{
					// 93-98
					var index = i - NetItem.MiscDyeIndex.Item1;
					Item item = new Item();
					if (miscDyes[index] != null && miscDyes[index].netID != 0)
					{
						item.netDefaults(miscDyes[index].netID);
						item.Prefix(miscDyes[index].prefix);
						item.AffixName();
						if (miscDyes[index].stack > item.maxStack || miscDyes[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove item dye {0} ({1}) and then rejoin.", item.Name, miscDyes[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.PiggyIndex.Item2)
				{
					// 98-138
					var index = i - NetItem.PiggyIndex.Item1;
					Item item = new Item();
					if (piggy[index] != null && piggy[index].netID != 0)
					{
						item.netDefaults(piggy[index].netID);
						item.Prefix(piggy[index].prefix);
						item.AffixName();

						if (piggy[index].stack > item.maxStack || piggy[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove piggy-bank item {0} ({1}) and then rejoin.", item.Name, piggy[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.SafeIndex.Item2)
				{
					// 138-178
					var index = i - NetItem.SafeIndex.Item1;
					Item item = new Item();
					if (safe[index] != null && safe[index].netID != 0)
					{
						item.netDefaults(safe[index].netID);
						item.Prefix(safe[index].prefix);
						item.AffixName();

						if (safe[index].stack > item.maxStack || safe[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove safe item {0} ({1}) and then rejoin.", item.Name, safe[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.TrashIndex.Item2)
				{
					// 178-179
					Item item = new Item();
					if (trash != null && trash.netID != 0)
					{
						item.netDefaults(trash.netID);
						item.Prefix(trash.prefix);
						item.AffixName();

						if (trash.stack > item.maxStack)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove trash item {0} ({1}) and then rejoin.", item.Name, trash.stack));
							}
						}
					}
				}
				else if (i < NetItem.ForgeIndex.Item2)
				{
					// 179-220
					var index = i - NetItem.ForgeIndex.Item1;
					Item item = new Item();
					if (forge[index] != null && forge[index].netID != 0)
					{
						item.netDefaults(forge[index].netID);
						item.Prefix(forge[index].prefix);
						item.AffixName();

						if (forge[index].stack > item.maxStack || forge[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Defender's Forge item {0} ({1}) and then rejoin.", item.Name, forge[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.VoidIndex.Item2)
				{
					// 220-260
					var index = i - NetItem.VoidIndex.Item1;
					Item item = new Item();
					if (voidVault[index] != null && voidVault[index].netID != 0)
					{
						item.netDefaults(voidVault[index].netID);
						item.Prefix(voidVault[index].prefix);
						item.AffixName();

						if (voidVault[index].stack > item.maxStack || voidVault[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Void Vault item {0} ({1}) and then rejoin.", item.Name, voidVault[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.Loadout1Armor.Item2)
				{
					var index = i - NetItem.Loadout1Armor.Item1;
					Item item = new Item();
					if (loadout1Armor[index] != null && loadout1Armor[index].netID != 0)
					{
						item.netDefaults(loadout1Armor[index].netID);
						item.Prefix(loadout1Armor[index].prefix);
						item.AffixName();

						if (loadout1Armor[index].stack > item.maxStack || loadout1Armor[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Loadout 1 item {0} ({1}) and then rejoin.", item.Name, loadout1Armor[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.Loadout1Dye.Item2)
				{
					var index = i - NetItem.Loadout1Dye.Item1;
					Item item = new Item();
					if (loadout1Dye[index] != null && loadout1Dye[index].netID != 0)
					{
						item.netDefaults(loadout1Dye[index].netID);
						item.Prefix(loadout1Dye[index].prefix);
						item.AffixName();

						if (loadout1Dye[index].stack > item.maxStack || loadout1Dye[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Loadout 1 item {0} ({1}) and then rejoin.", item.Name, loadout1Dye[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.Loadout2Armor.Item2)
				{
					var index = i - NetItem.Loadout2Armor.Item1;
					Item item = new Item();
					if (loadout2Armor[index] != null && loadout2Armor[index].netID != 0)
					{
						item.netDefaults(loadout2Armor[index].netID);
						item.Prefix(loadout2Armor[index].prefix);
						item.AffixName();

						if (loadout2Armor[index].stack > item.maxStack || loadout2Armor[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Loadout 2 item {0} ({1}) and then rejoin.", item.Name, loadout2Armor[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.Loadout2Dye.Item2)
				{
					var index = i - NetItem.Loadout2Dye.Item1;
					Item item = new Item();
					if (loadout2Dye[index] != null && loadout2Dye[index].netID != 0)
					{
						item.netDefaults(loadout2Dye[index].netID);
						item.Prefix(loadout2Dye[index].prefix);
						item.AffixName();

						if (loadout2Dye[index].stack > item.maxStack || loadout2Dye[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Loadout 2 item {0} ({1}) and then rejoin.", item.Name, loadout2Dye[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.Loadout3Armor.Item2)
				{
					var index = i - NetItem.Loadout3Armor.Item1;
					Item item = new Item();
					if (loadout3Armor[index] != null && loadout3Armor[index].netID != 0)
					{
						item.netDefaults(loadout3Armor[index].netID);
						item.Prefix(loadout3Armor[index].prefix);
						item.AffixName();

						if (loadout3Armor[index].stack > item.maxStack || loadout3Armor[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Loadout 3 item {0} ({1}) and then rejoin.", item.Name, loadout3Armor[index].stack));
							}
						}
					}
				}
				else if (i < NetItem.Loadout3Dye.Item2)
				{
					var index = i - NetItem.Loadout3Dye.Item1;
					Item item = new Item();
					if (loadout3Dye[index] != null && loadout3Dye[index].netID != 0)
					{
						item.netDefaults(loadout3Dye[index].netID);
						item.Prefix(loadout3Dye[index].prefix);
						item.AffixName();

						if (loadout3Dye[index].stack > item.maxStack || loadout3Dye[index].stack < 0)
						{
							check = true;
							if (shouldWarnPlayer)
							{
								SendErrorMessage(GetString("Stack cheat detected. Remove Loadout 3 item {0} ({1}) and then rejoin.", item.Name, loadout3Dye[index].stack));
							}
						}
					}
				}
			}

			return check;
		}

		/// <summary>
		/// The player's server side inventory data.
		/// </summary>
		public PlayerData PlayerData;

		/// <summary>
		/// Whether the player needs to specify a password upon connection( either server or user account ).
		/// </summary>
		public bool RequiresPassword;

		public bool SilentKickInProgress;

		public bool SilentJoinInProgress;

		/// <summary>
		/// Whether the player is accepting whispers from other users
		/// </summary>
		public bool AcceptingWhispers = true;

		/// <summary>Checks if a player is in range of a given tile if range checks are enabled.</summary>
		/// <param name="x"> The x coordinate of the tile.</param>
		/// <param name="y">The y coordinate of the tile.</param>
		/// <param name="range">The range to check for.</param>
		/// <returns>True if the player is in range of a tile or if range checks are off. False if not.</returns>
		public bool IsInRange(int x, int y, int range = 32)
		{
			int rgX = Math.Abs(TileX - x);
			int rgY = Math.Abs(TileY - y);
			if (TShock.Config.Settings.RangeChecks && ((rgX > range) || (rgY > range)))
			{
				TShock.Log.ConsoleDebug(GetString("Rangecheck failed for {0} ({1}, {2}) (rg: {3}/{5}, {4}/{5})", Name, x, y, rgX, rgY, range));
				return false;
			}
			return true;
		}

		private enum BuildPermissionFailPoint
		{
			GeneralBuild,
			SpawnProtect,
			Regions
		}

		/// <summary>Determines if the player can build on a given point.</summary>
		/// <param name="x">The x coordinate they want to build at.</param>
		/// <param name="y">The y coordinate they want to build at.</param>
		/// <param name="shouldWarnPlayer">Whether or not the player should be warned if their build attempt fails</param>
		/// <returns>True if the player can build at the given point from build, spawn, and region protection.</returns>
		public bool HasBuildPermission(int x, int y, bool shouldWarnPlayer = true)
		{
			PermissionHookResult hookResult = PlayerHooks.OnPlayerHasBuildPermission(this, x, y);
			if (hookResult != PermissionHookResult.Unhandled)
			{
				return hookResult == PermissionHookResult.Granted;
			}

			BuildPermissionFailPoint failure = BuildPermissionFailPoint.GeneralBuild;
			// The goal is to short circuit on easy stuff as much as possible.
			// Don't compute permissions unless needed, and don't compute taxing stuff unless needed.

			// If the player has bypass on build protection or building is enabled; continue
			// (General build protection takes precedence over spawn protection)
			if (!TShock.Config.Settings.DisableBuild || HasPermission(Permissions.antibuild))
			{
				failure = BuildPermissionFailPoint.SpawnProtect;
				// If they have spawn protect bypass, or it isn't spawn, or it isn't in spawn; continue
				// (If they have spawn protect bypass, we don't care if it's spawn or not)
				if (!TShock.Config.Settings.SpawnProtection || HasPermission(Permissions.editspawn) || !Utils.IsInSpawn(x, y))
				{
					failure = BuildPermissionFailPoint.Regions;
					// If they have build permission in this region, then they're allowed to continue
					if (TShock.Regions.CanBuild(x, y, this))
					{
						return true;
					}
				}
			}
			// If they lack build permission, they end up here.
			// If they have build permission but lack the ability to edit spawn and it's spawn, they end up here.
			// If they have build, it isn't spawn, or they can edit spawn, but they fail the region check, they end up here.

			// If they shouldn't be warned, exit early.
			if (!shouldWarnPlayer)
				return false;

			// Space out warnings by 2 seconds so that they don't get spammed.
			if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - lastPermissionWarning) < 2000)
			{
				return false;
			}

			// If they should be warned, warn them.
			if (!TShock.Config.Settings.SuppressPermissionFailureNotices)
			{
				switch (failure)
				{
					case BuildPermissionFailPoint.GeneralBuild:
						SendErrorMessage(GetString("You do not have permission to build on this server."));
						break;
					case BuildPermissionFailPoint.SpawnProtect:
						SendErrorMessage(GetString("You do not have permission to build in the spawn point."));
						break;
					case BuildPermissionFailPoint.Regions:
						SendErrorMessage(GetString("You do not have permission to build in this region."));
						break;
				}
			}
			// Set the last warning time to now.
			lastPermissionWarning = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

			return false;
		}

		/// <summary>
		/// Determines if the player can build a multi-block tile object on a given point.
		/// Tile objects include things like Doors, Trap Doors, Item Frames, Beds, and Dressers.
		/// </summary>
		/// <param name="x">The x coordinate they want to build at.</param>
		/// <param name="y">The y coordinate they want to build at.</param>
		/// <param name="width">The width of the tile object</param>
		/// <param name="height">The height of the tile object</param>
		/// <param name="shouldWarnPlayer">Whether or not the player should be warned if their build attempt fails</param>
		/// <returns>True if the player can build at the given point from build, spawn, and region protection.</returns>
		public bool HasBuildPermissionForTileObject(int x, int y, int width, int height, bool shouldWarnPlayer = true)
		{
			for (int realx = x; realx < x + width; realx++)
			{
				for (int realy = y; realy < y + height; realy++)
				{
					if (!HasBuildPermission(realx, realy, shouldWarnPlayer))
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>Determines if the player can paint on a given point. Checks general build permissions, then paint.</summary>
		/// <param name="x">The x coordinate they want to paint at.</param>
		/// <param name="y">The y coordinate they want to paint at.</param>
		/// <returns>True if they can paint.</returns>
		public bool HasPaintPermission(int x, int y)
		{
			return HasBuildPermission(x, y) && HasPermission(Permissions.canpaint);
		}

		/// <summary>Checks if a player can place ice, and if they can, tracks ice placements and removals.</summary>
		/// <param name="x">The x coordinate of the suspected ice block.</param>
		/// <param name="y">The y coordinate of the suspected ice block.</param>
		/// <param name="tileType">The tile type of the suspected ice block.</param>
		/// <param name="editAction">The EditAction on the suspected ice block.</param>
		/// <returns>True if a player successfully places an ice tile or removes one of their past ice tiles.</returns>
		public bool HasModifiedIceSuccessfully(int x, int y, short tileType, GetDataHandlers.EditAction editAction)
		{
			// The goal is to short circuit ASAP.
			// A subsequent call to HasBuildPermission can figure this out if not explicitly ice.
			if (!TShock.Config.Settings.AllowIce)
			{
				return false;
			}

			// They've placed some ice. Horrible!
			if (editAction == GetDataHandlers.EditAction.PlaceTile && tileType == TileID.MagicalIceBlock)
			{
				IceTiles.Add(new Point(x, y));
				return true;
			}

			// The edit wasn't an add, so we check to see if the position matches any of the known ice tiles
			if (editAction == GetDataHandlers.EditAction.KillTile)
			{
				foreach (Point p in IceTiles)
				{
					// If they're trying to kill ice or dirt, and the tile was in the list, we allow it.
					if (p.X == x && p.Y == y && (Main.tile[p.X, p.Y].type == TileID.Dirt || Main.tile[p.X, p.Y].type == TileID.MagicalIceBlock))
					{
						IceTiles.Remove(p);
						return true;
					}
				}
			}

			// Only a small number of cases let this happen.
			return false;
		}

		/// <summary>
		/// A list of points where ice tiles have been placed.
		/// </summary>
		public List<Point> IceTiles;

		/// <summary>
		/// The last time the player was warned for build permissions.
		/// In MS, defaults to 1 (so it will warn on the first attempt).
		/// </summary>
		public long lastPermissionWarning = 1;

		/// <summary>
		/// The time in ms when the player has logged in.
		/// </summary>
		public long LoginMS;

		/// <summary>
		/// Whether the player has been harrassed about logging in due to server side inventory or forced login.
		/// </summary>
		public bool LoginHarassed = false;

		/// <summary>
		/// Player cant die, unless onehit
		/// </summary>
		//public bool GodMode = false;

		public bool GodMode
		{
			get =>
				CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().IsEnabledForPlayer(Index);
			set =>
				CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(Index, value);
		}

		/// <summary>
		/// Players controls are inverted if using SSC
		/// </summary>
		public bool Confused = false;

		/// <summary>
		/// The last projectile type this player tried to kill.
		/// </summary>
		public int LastKilledProjectile = 0;

		/// <summary>
		/// Keeps track of recently created projectiles by this player. TShock.cs OnSecondUpdate() removes from this in an async task.
		/// Projectiles older than 5 seconds are purged from this collection as they are no longer "recent."
		/// </summary>
		public List<TShockAPI.GetDataHandlers.ProjectileStruct> RecentlyCreatedProjectiles = new List<TShockAPI.GetDataHandlers.ProjectileStruct>();

		/// <summary>
		/// The current region this player is in, or null if none.
		/// </summary>
		public Region CurrentRegion = null;

		/// <summary>
		/// Contains data stored by plugins
		/// </summary>
		protected ConcurrentDictionary<string, object> data = new ConcurrentDictionary<string, object>();

		/// <summary>
		/// Whether the player is a real, human, player on the server.
		/// </summary>
		public bool RealPlayer
		{
			get { return Index >= 0 && Index < Main.maxNetPlayers && Main.player[Index] != null; }
		}

		/// <summary>
		/// Checks if the player is active and not pending termination.
		/// </summary>
		public bool ConnectionAlive
		{
			get
			{
				return RealPlayer
					   && (Netplay.Clients[Index] != null && Netplay.Clients[Index].IsActive && !Netplay.Clients[Index].PendingTermination);
			}
		}

		/// <summary>
		/// Gets the item that the player is currently holding.
		/// </summary>
		public Item SelectedItem
		{
			get { return TPlayer.inventory[TPlayer.selectedItem]; }
		}

		/// <summary>
		/// Gets the player's Client State.
		/// </summary>
		public int State
		{
			get { return Client.State; }
			set { Client.State = value; }
		}

		/// <summary>
		/// Gets the player's UUID.
		/// </summary>
		public string UUID
		{
			get { return RealPlayer ? Client.ClientUUID : ""; }
		}

		/// <summary>
		/// Gets the player's IP.
		/// </summary>
		public string IP
		{
			get
			{
				if (string.IsNullOrEmpty(CacheIP))
					return
						CacheIP = RealPlayer ? (Client.Socket.IsConnected()
								? TShock.Utils.GetRealIP(Client.Socket.GetRemoteAddress().ToString())
								: "")
							: "127.0.0.1";
				else
					return CacheIP;
			}
		}

		/// <summary>
		/// Gets the player's inventory (first 5 rows)
		/// </summary>
		public IEnumerable<Item> Inventory
		{
			get
			{
				for (int i = 0; i < 50; i++)
					yield return TPlayer.inventory[i];
			}
		}

		/// <summary>
		/// Gets the player's accessories.
		/// </summary>
		public IEnumerable<Item> Accessories
		{
			get
			{
				for (int i = 3; i < 10; i++)
					yield return TPlayer.armor[i];
			}
		}

		/// <summary>
		/// Saves the player's inventory to SSC
		/// </summary>
		/// <returns>bool - True/false if it saved successfully</returns>
		public bool SaveServerCharacter()
		{
			if (!Main.ServerSideCharacter)
			{
				return false;
			}
			try
			{
				if (HasPermission(Permissions.bypassssc))
				{
					TShock.Log.ConsoleInfo(GetString($"Skipping SSC save (due to tshock.ignore.ssc) for {Account.Name}"));
					return true;
				}
				PlayerData.CopyCharacter(this);
				TShock.CharacterDB.InsertPlayerData(this);
				return true;
			}
			catch (Exception e)
			{
				TShock.Log.Error(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Sends the players server side character to client
		/// </summary>
		/// <returns>bool - True/false if it saved successfully</returns>
		public bool SendServerCharacter()
		{
			if (!Main.ServerSideCharacter)
			{
				return false;
			}
			try
			{
				PlayerData.RestoreCharacter(this);
				return true;
			}
			catch (Exception e)
			{
				TShock.Log.Error(e.Message);
				return false;
			}

		}

		/// <summary>
		/// Gets the Terraria Player object associated with the player.
		/// </summary>
		public Player TPlayer
		{
			get { return FakePlayer ?? Main.player[Index]; }
		}

		/// <summary>
		/// Player RemoteClient.
		/// </summary>
		public RemoteClient Client => Netplay.Clients[Index];

		/// <summary>
		/// Gets the player's name.
		/// </summary>
		public string Name
		{
			get { return TPlayer.name; }
		}

		/// <summary>
		/// Gets the player's active state.
		/// </summary>
		public bool Active
		{
			get { return TPlayer != null && TPlayer.active; }
		}

		/// <summary>
		/// Gets the player's team.
		/// </summary>
		public int Team
		{
			get { return TPlayer.team; }
		}

		/// <summary>
		/// A player's position in the world.
		/// </summary>
		public Vector2 Position => RealPlayer ? TPlayer.position : Vector2.Zero;

		/// <summary>
		/// Gets the player's X coordinate.
		/// </summary>
		public float X
		{
			get { return RealPlayer ? Position.X : Main.spawnTileX * 16; }
		}

		/// <summary>
		/// Gets the player's Y coordinate.
		/// </summary>
		public float Y
		{
			get { return RealPlayer ? Position.Y : Main.spawnTileY * 16; }
		}

		/// <summary>
		/// Player X coordinate divided by 16. Supposed X world coordinate.
		/// </summary>
		public int TileX
		{
			get { return (int)(X / 16); }
		}

		/// <summary>
		/// Player Y coordinate divided by 16. Supposed Y world coordinate.
		/// </summary>
		public int TileY
		{
			get { return (int)(Y / 16); }
		}

		/// <summary>
		/// Checks if the player has any inventory slots available.
		/// </summary>
		public bool InventorySlotAvailable
		{
			get
			{
				bool flag = false;
				if (RealPlayer)
				{
					for (int i = 0; i < 50; i++) //51 is trash can, 52-55 is coins, 56-59 is ammo
					{
						if (TPlayer.inventory[i] == null || !TPlayer.inventory[i].active || TPlayer.inventory[i].Name == "")
						{
							flag = true;
							break;
						}
					}
				}
				return flag;
			}
		}

		/// <summary>
		/// This contains the character data a player has when they join the server.
		/// </summary>
		public PlayerData DataWhenJoined { get; set; }

		/// <summary>
		/// Determines whether the player's storage contains the given key.
		/// </summary>
		/// <param name="key">Key to test.</param>
		/// <returns></returns>
		public bool ContainsData(string key)
		{
			return data.ContainsKey(key);
		}

		/// <summary>
		/// Returns the stored object associated with the given key.
		/// </summary>
		/// <typeparam name="T">Type of the object being retrieved.</typeparam>
		/// <param name="key">Key with which to access the object.</param>
		/// <returns>The stored object, or default(T) if not found.</returns>
		public T GetData<T>(string key)
		{
			object obj;
			if (!data.TryGetValue(key, out obj))
			{
				return default(T);
			}

			return (T)obj;
		}

		/// <summary>
		/// Stores an object on this player, accessible with the given key.
		/// </summary>
		/// <typeparam name="T">Type of the object being stored.</typeparam>
		/// <param name="key">Key with which to access the object.</param>
		/// <param name="value">Object to store.</param>
		public void SetData<T>(string key, T value)
		{
			if (!data.TryAdd(key, value))
			{
				data.TryUpdate(key, value, data[key]);
			}
		}

		/// <summary>
		/// Removes the stored object associated with the given key.
		/// </summary>
		/// <param name="key">Key with which to access the object.</param>
		/// <returns>The removed object.	</returns>
		public object RemoveData(string key)
		{
			object rem;
			if (data.TryRemove(key, out rem))
			{
				return rem;
			}
			return null;
		}

		/// <summary>
		/// Logs the player out of an account.
		/// </summary>
		public void Logout()
		{
			PlayerHooks.OnPlayerLogout(this);
			if (Main.ServerSideCharacter)
			{
				IsDisabledForSSC = true;
				if (!IsDisabledPendingTrashRemoval && (!Dead || TPlayer.difficulty != 2))
				{
					PlayerData.CopyCharacter(this);
					TShock.CharacterDB.InsertPlayerData(this);
				}
			}

			PlayerData = new PlayerData(true);
			Group = TShock.Groups.GetGroupByName(TShock.Config.Settings.DefaultGuestGroupName);
			tempGroup = null;
			if (tempGroupTimer != null)
			{
				tempGroupTimer.Stop();
			}
			Account = null;
			IsLoggedIn = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TSPlayer"/> class.
		/// </summary>
		/// <param name="index">The player's index in the.</param>
		public TSPlayer(int index)
		{
			TilesDestroyed = new Dictionary<Vector2, ITile>();
			TilesCreated = new Dictionary<Vector2, ITile>();
			Index = index;
			Group = Group.DefaultGroup;
			IceTiles = new List<Point>();
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TSPlayer"/> class.
		/// </summary>
		/// <param name="playerName">The player's name.</param>
		protected TSPlayer(String playerName)
		{
			TilesDestroyed = new Dictionary<Vector2, ITile>();
			TilesCreated = new Dictionary<Vector2, ITile>();
			Index = -1;
			FakePlayer = new Player { name = playerName, whoAmI = -1 };
			Group = Group.DefaultGroup;
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		/// <summary>
		/// Disconnects the player from the server.
		/// </summary>
		/// <param name="reason">The reason why the player was disconnected.</param>
		public virtual void Disconnect(string reason)
		{
			SendData(PacketTypes.Disconnect, reason);
		}

		/// <summary>
		/// Fired when the player's temporary group access expires.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void TempGroupTimerElapsed(object sender, ElapsedEventArgs args)
		{
			SendWarningMessage(GetString("Your temporary group access has expired."));

			tempGroup = null;
			if (sender != null)
			{
				((Timer)sender).Stop();
			}
		}

		/// <summary>
		/// Teleports the player to the given coordinates in the world.
		/// </summary>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <param name="style">The teleportation style.</param>
		/// <returns>True or false.</returns>
		public bool Teleport(float x, float y, byte style = 1)
		{
			if (x > Main.rightWorld - 992)
			{
				x = Main.rightWorld - 992;
			}
			if (x < 992)
			{
				x = 992;
			}
			if (y > Main.bottomWorld - 992)
			{
				y = Main.bottomWorld - 992;
			}
			if (y < 992)
			{
				y = 992;
			}

			SendTileSquareCentered((int)(x / 16), (int)(y / 16), 15);
			TPlayer.Teleport(new Vector2(x, y), style);
			NetMessage.SendData((int)PacketTypes.Teleport, -1, -1, NetworkText.Empty, 0, TPlayer.whoAmI, x, y, style);
			return true;
		}

		/// <summary>
		/// Heals the player.
		/// </summary>
		/// <param name="health">Heal health amount.</param>
		public void Heal(int health = 600)
		{
			NetMessage.SendData((int)PacketTypes.PlayerHealOther, -1, -1, NetworkText.Empty, this.TPlayer.whoAmI, health);
		}

		/// <summary>
		/// Spawns the player at his spawn point.
		/// </summary>
		public void Spawn(PlayerSpawnContext context, int? respawnTimer = null)
		{
			if (this.sX > 0 && this.sY > 0)
			{
				Spawn(this.sX, this.sY, context, respawnTimer);
			}
			else
			{
				Spawn(TPlayer.SpawnX, TPlayer.SpawnY, context, respawnTimer);
			}
		}

		/// <summary>
		/// Spawns the player at the given coordinates.
		/// </summary>
		/// <param name="tilex">The X coordinate.</param>
		/// <param name="tiley">The Y coordinate.</param>
		/// <param name="context">The PlayerSpawnContext.</param>
		/// <param name="respawnTimer">The respawn timer, will be Player.respawnTimer if parameter is null.</param>
		/// <param name="numberOfDeathsPVE">The number of deaths PVE, will be TPlayer.numberOfDeathsPVE if parameter is null.</param>
		/// <param name="numberOfDeathsPVP">The number of deaths PVP, will be TPlayer.numberOfDeathsPVP if parameter is null.</param>
		public void Spawn(int tilex, int tiley, PlayerSpawnContext context, int? respawnTimer = null, short? numberOfDeathsPVE = null, short? numberOfDeathsPVP = null)
		{
			using (var ms = new MemoryStream())
			{
				var msg = new SpawnMsg
				{
					PlayerIndex = (byte)Index,
					TileX = (short)tilex,
					TileY = (short)tiley,
					RespawnTimer = respawnTimer ?? TShock.Players[Index].RespawnTimer * 60,
					NumberOfDeathsPVE = numberOfDeathsPVE ?? (short)TPlayer.numberOfDeathsPVE,
					NumberOfDeathsPVP = numberOfDeathsPVP ?? (short)TPlayer.numberOfDeathsPVP,
					PlayerSpawnContext = context,
				};
				msg.PackFull(ms);
				SendRawData(ms.ToArray());
			}
		}

		/// <summary>
		/// Removes the projectile with the given index and owner.
		/// </summary>
		/// <param name="index">The projectile's index.</param>
		/// <param name="owner">The projectile's owner.</param>
		public void RemoveProjectile(int index, int owner)
		{
			using (var ms = new MemoryStream())
			{
				var msg = new ProjectileRemoveMsg
				{
					Index = (short)index,
					Owner = (byte)owner
				};
				msg.PackFull(ms);
				SendRawData(ms.ToArray());
			}
		}

		/// <summary>Sends a tile square at a location with a given size.
		/// Typically used to revert changes by Bouncer through sending the
		/// "old" version of modified data back to a client.
		/// Prevents desync issues.
		/// </summary>
		/// <param name="x">The x coordinate to send.</param>
		/// <param name="y">The y coordinate to send.</param>
		/// <param name="size">The size square set of tiles to send.</param>
		/// <returns>true if the tile square was sent successfully, else false</returns>
		[Obsolete("This method may not send tiles the way you would expect it to. The (x,y) coordinates are the top left corner of the tile square, switch to " + nameof(SendTileSquareCentered) + " if you wish for the coordindates to be the center of the square.")]
		public virtual bool SendTileSquare(int x, int y, int size = 10)
		{
			return SendTileRect((short)x, (short)y, (byte)size, (byte)size);
		}

		/// <summary>
		/// Sends a tile square at a center location with a given size.
		/// Typically used to revert changes by Bouncer through sending the
		/// "old" version of modified data back to a client.
		/// Prevents desync issues.
		/// </summary>
		/// <param name="x">The x coordinates of the center of the square.</param>
		/// <param name="y">The y coordinates of the center of the square.</param>
		/// <param name="size">The size square set of tiles to send.</param>
		/// <returns>true if the tile square was sent successfully, else false</returns>
		public virtual bool SendTileSquareCentered(int x, int y, byte size = 10)
		{
			return SendTileRect((short)(x - (size / 2)), (short)(y - (size / 2)), size, size);
		}

		/// <summary>
		/// Sends a rectangle of tiles at a location with the given length and width.
		/// </summary>
		/// <param name="x">The x coordinate the rectangle will begin at</param>
		/// <param name="y">The y coordinate the rectangle will begin at</param>
		/// <param name="width">The width of the rectangle</param>
		/// <param name="length">The length of the rectangle</param>
		/// <param name="changeType">Optional change type. Default None</param>
		/// <returns></returns>
		public virtual bool SendTileRect(short x, short y, byte width = 10, byte length = 10, TileChangeType changeType = TileChangeType.None)
		{
			try
			{
				NetMessage.SendTileSquare(Index, x, y, width, length, changeType);
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Sets the visibility of the sections.
		/// </summary>
		/// <param name="rectangle">Section to be changed for the player. The minimum size should be set to 200x150.
		/// </param>
		/// <param name="isLoaded">Is the section loaded.</param>
		// The server does not send the player the whole world, it sends it in sections. To do this, it sets up visible and invisible sections.
		// If the player was not in any section(Client.TileSections[x, y] == false) then the server will send the missing section of the world.
		// This method allows you to simulate what the player has or has not seen these sections.
		// For example, we can put some number of earths blocks in some vast area, for example, for the whole world, but the player will not see the changes, because some section is already loaded for him. At this point this method can come into effect! With it we will be able to select some zone and make it both visible and invisible to the player.
		// The server will assume that the zone is not loaded on the player, and will resend the data, but with earth blocks.
		public void UpdateSection(Rectangle? rectangle = null, bool isLoaded = false)
		{
			if (rectangle.HasValue)
			{
				for (int i = Netplay.GetSectionX(rectangle.Value.X); i < Netplay.GetSectionX(rectangle.Value.X + rectangle.Value.Width)
					&& i < Main.maxTilesX; i++)
					for (int j = Netplay.GetSectionY(rectangle.Value.Y); j < Netplay.GetSectionY(rectangle.Value.Y + rectangle.Value.Height)
						&& j < Main.maxSectionsY; j++)
							Client.TileSections[i, j] = isLoaded;
			}
			else
			{
				for (int i = 0; i < Main.maxSectionsX; i++)
					for (int j = 0; j < Main.maxSectionsY; j++)
						Client.TileSections[i, j] = isLoaded;
			}
		}

		/// <summary>
		/// Gives an item to the player. Includes banned item spawn prevention to check if the player can spawn the item.
		/// </summary>
		/// <param name="type">The item ID.</param>
		/// <param name="name">The item name.</param>
		/// <param name="stack">The item stack.</param>
		/// <param name="prefix">The item prefix.</param>
		/// <returns>True or false, depending if the item passed the check or not.</returns>
		public bool GiveItemCheck(int type, string name, int stack, int prefix = 0)
		{
			if ((TShock.ItemBans.DataModel.ItemIsBanned(name) && TShock.Config.Settings.PreventBannedItemSpawn) &&
				(TShock.ItemBans.DataModel.ItemIsBanned(name, this) || !TShock.Config.Settings.AllowAllowedGroupsToSpawnBannedItems))
				return false;

			GiveItem(type, stack, prefix);
			return true;
		}

		/// <summary>
		/// Gives an item to the player.
		/// </summary>
		/// <param name="type">The item ID.</param>
		/// <param name="stack">The item stack.</param>
		/// <param name="prefix">The item prefix.</param>
		public virtual void GiveItem(int type, int stack, int prefix = 0)
		{
			if (TShock.Config.Settings.GiveItemsDirectly)
				GiveItemDirectly(type, stack, prefix);
			else
				GiveItemByDrop(type, stack, prefix);
		}

		/// <summary>
		/// Gives an item to the player.
		/// </summary>
		/// <param name="item">The item</param>
		public void GiveItem(NetItem item)
		{
			GiveItem(item.NetId, item.Stack, item.PrefixId);
		}

		private Item EmptySentinelItem = new Item();

		private bool Depleted(Item item)
			=> item.type == 0 || item.stack == 0;

		private void GiveItemDirectly(int type, int stack, int prefix)
		{
			if (ItemID.Sets.IsAPickup[type] || !Main.ServerSideCharacter || this.IsDisabledForSSC)
			{
				GiveItemByDrop(type, stack, prefix);
				return;
			}

			var item = new Item();
			item.netDefaults(type);
			item.stack = stack;
			item.prefix = (byte)prefix;

			if (item.IsACoin)
				for (int slot = -4; slot < 50; slot++)
					if (Depleted(item = GiveItemDirectly_FillIntoOccupiedSlot(item, slot < 0 ? slot + 54 : slot)))
						return;

			if (item.FitsAmmoSlot())
				if (Depleted(item = GiveItem_FillAmmo(item)))
					return;

			for (int slot = 0; slot < 50; slot++)
				if (Depleted(item = GiveItemDirectly_FillIntoOccupiedSlot(item, slot)))
					return;

			if (!item.IsACoin && item.useStyle != 0)
				for (int slot = 0; slot < 10; slot++)
					if (Depleted(item = GiveItemDirectly_FillEmptyInventorySlot(item, slot)))
						return;

			int lastSlot = item.IsACoin ? 54 : 50;
			for (int slot = lastSlot - 1; slot >= 0; slot--)
				if (Depleted(item = GiveItemDirectly_FillEmptyInventorySlot(item, slot)))
					return;

			// oh no, i can't give the rest of the items... guess i gotta spill it on the floor
			GiveItemByDrop(item.type, item.stack, item.prefix);
		}

		private void SendItemSlotPacketFor(int slot)
		{
			int prefix = this.TPlayer.inventory[slot].prefix;
			NetMessage.SendData(5, this.Index, -1, null, this.Index, slot, prefix, 0f, 0, 0, 0);
		}

		private Item GiveItem_FillAmmo(Item item)
		{
			var inv = this.TPlayer.inventory;

			for (int i = 54; i < 58; i++)
				if (Depleted(item = GiveItemDirectly_FillIntoOccupiedSlot(item, i)))
					return EmptySentinelItem;

			if (!item.CanFillEmptyAmmoSlot())
				return item;

			for (int i = 54; i < 58; i++)
				if (GiveItemDirectly_FillEmptyInventorySlot(item, i) == EmptySentinelItem)
					return EmptySentinelItem;

			return item;
		}

		private Item GiveItemDirectly_FillIntoOccupiedSlot(Item item, int slot)
		{
			var inv = this.TPlayer.inventory;
			if (inv[slot].type <= 0 || inv[slot].stack >= inv[slot].maxStack || !item.IsTheSameAs(inv[slot]))
				return item;

			if (item.stack + inv[slot].stack <= inv[slot].maxStack)
			{
				inv[slot].stack += item.stack;
				SendItemSlotPacketFor(slot);
				return EmptySentinelItem;
			}

			var newItem = item.DeepClone();
			newItem.stack -= inv[slot].maxStack - inv[slot].stack;
			inv[slot].stack = inv[slot].maxStack;
			SendItemSlotPacketFor(slot);

			return newItem;
		}

		private Item GiveItemDirectly_FillEmptyInventorySlot(Item item, int slot)
		{
			var inv = this.TPlayer.inventory;
			if (inv[slot].type != 0)
				return item;

			inv[slot] = item;
			SendItemSlotPacketFor(slot);
			return EmptySentinelItem;
		}

		private void GiveItemByDrop(int type, int stack, int prefix)
		{
			int itemIndex = Item.NewItem(new EntitySource_DebugCommand(), (int)X, (int)Y, TPlayer.width, TPlayer.height, type, stack, true, prefix, true);
			Main.item[itemIndex].playerIndexTheItemIsReservedFor = this.Index;
			SendData(PacketTypes.ItemDrop, "", itemIndex, 1);
			SendData(PacketTypes.ItemOwner, null, itemIndex);
		}

		/// <summary>
		/// Sends an information message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendInfoMessage(string msg)
		{
			SendMessage(msg, Color.Yellow);
		}

		/// <summary>
		/// Sends an information message to the player.
		/// Replaces format items in the message with the string representation of a specified object.
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendInfoMessage(string format, params object[] args)
		{
			SendInfoMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends a success message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendSuccessMessage(string msg)
		{
			SendMessage(msg, Color.LimeGreen);
		}

		/// <summary>
		/// Sends a success message to the player.
		/// Replaces format items in the message with the string representation of a specified object.
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendSuccessMessage(string format, params object[] args)
		{
			SendSuccessMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends a warning message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendWarningMessage(string msg)
		{
			SendMessage(msg, Color.OrangeRed);
		}

		/// <summary>
		/// Sends a warning message to the player.
		/// Replaces format items in the message with the string representation of a specified object.
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendWarningMessage(string format, params object[] args)
		{
			SendWarningMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends an error message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendErrorMessage(string msg)
		{
			SendMessage(msg, Color.Red);
		}

		/// <summary>
		/// Sends an error message to the player.
		/// Replaces format items in the message with the string representation of a specified object
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendErrorMessage(string format, params object[] args)
		{
			SendErrorMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends a message with the specified color.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="color">The message color.</param>
		public virtual void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sends a message with the specified RGB color.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="red">The amount of red color to factor in. Max: 255.</param>
		/// <param name="green">The amount of green color to factor in. Max: 255</param>
		/// <param name="blue">The amount of blue color to factor in. Max: 255</param>
		public virtual void SendMessage(string msg, byte red, byte green, byte blue)
		{
			if (msg.Contains("\n"))
			{
				string[] msgs = msg.Split('\n');
				foreach (string message in msgs)
				{
					SendMessage(message, red, green, blue);
				}
				return;
			}

			if (this.Index == -1) //-1 is our broadcast index - this implies we're using TSPlayer.All.SendMessage and broadcasting to all clients
			{
				Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(msg), new Color(red, green, blue));
			}
			else
			{
				Terraria.Chat.ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(msg), new Color(red, green, blue), this.Index);
			}
		}

		/// <summary>
		/// Sends a message to the player with the specified RGB color.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="red">The amount of red color to factor in. Max: 255.</param>
		/// <param name="green">The amount of green color to factor in. Max: 255.</param>
		/// <param name="blue">The amount of blue color to factor in. Max: 255.</param>
		/// <param name="ply">The player who receives the message.</param>
		public virtual void SendMessageFromPlayer(string msg, byte red, byte green, byte blue, int ply)
		{
			if (msg.Contains("\n"))
			{
				string[] msgs = msg.Split('\n');
				foreach (string message in msgs)
				{
					SendMessageFromPlayer(message, red, green, blue, ply);
				}
				return;
			}
			Terraria.Chat.ChatHelper.BroadcastChatMessageAs((byte)ply, NetworkText.FromLiteral(msg), new Color(red, green, blue));
		}

		/// <summary>
		/// Sends the text of a given file to the player. Replacement of %map% and %players% if in the file.
		/// </summary>
		/// <param name="file">Filename relative to <see cref="TShock.SavePath"></see></param>
		public void SendFileTextAsMessage(string file)
		{
			string foo = "";
			bool containsOldFormat = false;
			using (var tr = new StreamReader(file))
			{
				Color lineColor;
				while ((foo = tr.ReadLine()) != null)
				{
					lineColor = Color.White;
					if (string.IsNullOrWhiteSpace(foo))
					{
						continue;
					}

					var players = new List<string>();

					foreach (TSPlayer ply in TShock.Players)
					{
						if (ply != null && ply.Active)
						{
							players.Add(ply.Name);
						}
					}

					foo = foo.Replace("%map%", (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName));
					foo = foo.Replace("%players%", String.Join(", ", players));
					foo = foo.Replace("%specifier%", TShock.Config.Settings.CommandSpecifier);
					foo = foo.Replace("%onlineplayers%", TShock.Utils.GetActivePlayerCount().ToString());
					foo = foo.Replace("%serverslots%", TShock.Config.Settings.MaxSlots.ToString());

					SendMessage(foo, lineColor);
				}
			}
		}

		/// <summary>
		/// Wounds the player with the given damage.
		/// </summary>
		/// <param name="damage">The amount of damage the player will take.</param>
		public virtual void DamagePlayer(int damage)
		{
			DamagePlayer(damage, PlayerDeathReason.LegacyDefault());
		}
		/// <summary>
		/// Wounds the player with the given damage.
		/// </summary>
		/// <param name="damage">The amount of damage the player will take.</param>
		/// <param name="reason">Player death reason</param>
		public virtual void DamagePlayer(int damage, PlayerDeathReason reason)
		{
			NetMessage.SendPlayerHurt(Index, reason,
				damage, (new Random()).Next(-1, 1), false, false, 0, -1, -1);
		}

		/// <summary>
		/// Kills the player.
		/// </summary>
		public virtual void KillPlayer()
		{
			KillPlayer(PlayerDeathReason.LegacyDefault());
		}
		/// <summary>
		/// Kills the player.
		/// </summary>
		/// <param name="reason">Player death reason</param>
		public virtual void KillPlayer(PlayerDeathReason reason)
		{
			NetMessage.SendPlayerDeath(Index, reason, 99999, (new Random()).Next(-1, 1), false, -1, -1);
		}

		/// <summary>
		/// Sets the player's team.
		/// </summary>
		/// <param name="team">The team color index.</param>
		public virtual void SetTeam(int team)
		{
			if (team < 0 || team >= Main.teamColor.Length)
				throw new ArgumentException("The player's team is not in the range of available.");
			Main.player[Index].team = team;
			NetMessage.SendData((int)PacketTypes.PlayerTeam, -1, -1, NetworkText.Empty, Index);
		}

		/// <summary>
		/// Sets the player's pvp.
		/// </summary>
		/// <param name="mode">The state of the pvp mode.</param>
		/// <param name="withMsg">Whether a chat message about the change should be sent.</param>
		public virtual void SetPvP(bool mode, bool withMsg = false)
		{
			Main.player[Index].hostile = mode;
			NetMessage.SendData((int)PacketTypes.TogglePvp, -1, -1, NetworkText.Empty, Index);
			if (withMsg)
				TSPlayer.All.SendMessage(Language.GetTextValue(mode ? "LegacyMultiplayer.11" : "LegacyMultiplayer.12", Name), Main.teamColor[Team]);
		}

		private DateTime LastDisableNotification = DateTime.UtcNow;

		/// <summary>
		/// Represents the ID of the chest that the player is viewing.
		/// </summary>
		public int ActiveChest = -1;

		/// <summary>
		/// Represents the current item the player is holding.
		/// </summary>
		public Item ItemInHand = new Item();

		/// <summary>
		/// Disables the player for the given <paramref name="reason"/>
		/// </summary>
		/// <param name="reason">The reason why the player was disabled.</param>
		/// <param name="flags">Flags to dictate where this event is logged to.</param>
		public virtual void Disable(string reason = "", DisableFlags flags = DisableFlags.WriteToLog)
		{
			LastThreat = DateTime.UtcNow;
			SetBuff(BuffID.Webbed, 330, true);

			if (ActiveChest != -1)
			{
				ActiveChest = -1;
				SendData(PacketTypes.ChestOpen, "", -1);
			}

			if (!string.IsNullOrEmpty(reason))
			{
				if ((DateTime.UtcNow - LastDisableNotification).TotalMilliseconds > 5000)
				{
					if (flags.HasFlag(DisableFlags.WriteToConsole))
					{
						if (flags.HasFlag(DisableFlags.WriteToLog))
						{
							TShock.Log.ConsoleInfo(GetString("Player {0} has been disabled for {1}.", Name, reason));
						}
						else
						{
							Server.SendInfoMessage(GetString("Player {0} has been disabled for {1}.", Name, reason));
						}
					}

					LastDisableNotification = DateTime.UtcNow;
				}
			}

			/*
			 * Calling new StackTrace() is incredibly expensive, and must be disabled
			 * in release builds.  Use a conditional call instead.
			 */
			LogStackFrame();
		}

		/// <summary>
		/// Disconnects this player from the server with a reason.
		/// </summary>
		/// <param name="reason">The reason to display to the user and to the server on kick.</param>
		/// <param name="force">If the kick should happen regardless of immunity to kick permissions.</param>
		/// <param name="silent">If no message should be broadcasted to the server.</param>
		/// <param name="adminUserName">The originator of the kick, for display purposes.</param>
		/// <param name="saveSSI">If the player's server side character should be saved on kick.</param>
		public bool Kick(string reason, bool force = false, bool silent = false, string adminUserName = null, bool saveSSI = false)
		{
			if (!ConnectionAlive)
				return true;
			if (force || !HasPermission(Permissions.immunetokick))
			{
				SilentKickInProgress = silent;
				if (IsLoggedIn && saveSSI)
					SaveServerCharacter();
				Disconnect(GetString("Kicked: {0}", reason));
				TShock.Log.ConsoleInfo(GetString("Kicked {0} for : '{1}'", Name, reason));
				if (!silent)
				{
					if (string.IsNullOrWhiteSpace(adminUserName))
						TShock.Utils.Broadcast(GetString("{0} was kicked for '{1}'", Name, reason), Color.Green);
					else
						TShock.Utils.Broadcast(GetString("{0} kicked {1} for '{2}'", adminUserName, Name, reason), Color.Green);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Bans and disconnects the player from the server.
		/// </summary>
		/// <param name="reason">The reason to be displayed to the server.</param>
		/// <param name="adminUserName">The player who initiated the ban.</param>
		public bool Ban(string reason, string adminUserName = null)
		{
			if (!ConnectionAlive)
				return true;

			TShock.Bans.InsertBan($"{Identifier.IP}{IP}", reason, adminUserName, DateTime.UtcNow, DateTime.MaxValue);
			TShock.Bans.InsertBan($"{Identifier.UUID}{UUID}", reason, adminUserName, DateTime.UtcNow, DateTime.MaxValue);
			if (Account != null)
			{
				TShock.Bans.InsertBan($"{Identifier.Account}{Account.Name}", reason, adminUserName, DateTime.UtcNow, DateTime.MaxValue);
			}

			Disconnect(GetString("Banned: {0}", reason));

			if (string.IsNullOrWhiteSpace(adminUserName))
				TSPlayer.All.SendInfoMessage(GetString("{0} was banned for '{1}'.", Name, reason));
			else
				TSPlayer.All.SendInfoMessage(GetString("{0} banned {1} for '{2}'.", adminUserName, Name, reason));
			return true;
		}

		/// <summary>
		/// Sends the player an error message stating that more than one match was found
		/// appending a csv list of the matches.
		/// </summary>
		/// <param name="matches">An enumerable list with the matches</param>
		public void SendMultipleMatchError(IEnumerable<object> matches)
		{
			SendErrorMessage(GetString("More than one match found -- unable to decide which is correct: "));

			var lines = PaginationTools.BuildLinesFromTerms(matches.ToArray());
			lines.ForEach(SendInfoMessage);

			SendErrorMessage(GetString("Use \"my query\" for items with spaces."));
			SendErrorMessage(GetString("Use tsi:[number] or tsn:[username] to distinguish between user IDs and usernames."));
		}

		[Conditional("DEBUG")]
		private void LogStackFrame()
		{
			var trace = new StackTrace();
			StackFrame frame = null;
			frame = trace.GetFrame(1);
			if (frame != null && frame.GetMethod().DeclaringType != null)
				TShock.Log.Debug(frame.GetMethod().DeclaringType.Name + " called Disable().");
		}

		/// <summary>
		/// Annoys the player for a specified amount of time.
		/// </summary>
		/// <param name="time">The</param>
		public virtual void Whoopie(object time)
		{
			var time2 = (int)time;
			var launch = DateTime.UtcNow;
			var startname = Name;
			while ((DateTime.UtcNow - launch).TotalSeconds < time2 && startname == Name)
			{
				SendData(PacketTypes.NpcSpecial, number: Index, number2: 2f);
				Thread.Sleep(50);
			}
		}

		/// <summary>
		/// Applies a buff to the player.
		/// </summary>
		/// <param name="type">The buff type.</param>
		/// <param name="time">The buff duration.</param>
		/// <param name="bypass"></param>
		public virtual void SetBuff(int type, int time = 3600, bool bypass = false)
		{
			if ((DateTime.UtcNow - LastThreat).TotalMilliseconds < 5000 && !bypass)
				return;

			SendData(PacketTypes.PlayerAddBuff, number: Index, number2: type, number3: time);
		}

		//Todo: Separate this into a few functions. SendTo, SendToAll, etc
		/// <summary>
		/// Sends data to the player.
		/// </summary>
		/// <param name="msgType">The sent packet</param>
		/// <param name="text">The packet text.</param>
		/// <param name="number"></param>
		/// <param name="number2"></param>
		/// <param name="number3"></param>
		/// <param name="number4"></param>
		/// <param name="number5"></param>
		public virtual void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0f,
			float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			if (RealPlayer && !ConnectionAlive)
				return;

			NetMessage.SendData((int)msgType, Index, -1, text == null ? null : NetworkText.FromLiteral(text), number, number2, number3, number4, number5);
		}

		/// <summary>
		/// Sends data from the given player.
		/// </summary>
		/// <param name="msgType">The sent packet.</param>
		/// <param name="ply">The packet sender.</param>
		/// <param name="text">The packet text.</param>
		/// <param name="number2"></param>
		/// <param name="number3"></param>
		/// <param name="number4"></param>
		/// <param name="number5"></param>
		public virtual void SendDataFromPlayer(PacketTypes msgType, int ply, string text = "", float number2 = 0f,
			float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			if (RealPlayer && !ConnectionAlive)
				return;

			NetMessage.SendData((int)msgType, Index, -1, NetworkText.FromFormattable(text), ply, number2, number3, number4, number5);
		}

		/// <summary>
		/// Sends raw data to the player's socket object.
		/// </summary>
		/// <param name="data">The data to send.</param>
		public virtual void SendRawData(byte[] data)
		{
			if (!RealPlayer || !ConnectionAlive)
				return;

			Netplay.Clients[Index].Socket.AsyncSend(data, 0, data.Length, Netplay.Clients[Index].ServerWriteCallBack);
		}

		/// <summary>
		/// Adds a command callback to a specified command string.
		/// </summary>
		/// <param name="name">The string representing the command i.e "yes" == /yes</param>
		/// <param name="callback">The method that will be executed on confirmation ie user accepts</param>
		public void AddResponse(string name, Action<object> callback)
		{
			if (AwaitingResponse.ContainsKey(name))
			{
				AwaitingResponse.Remove(name);
			}

			AwaitingResponse.Add(name, callback);
		}

		/// <summary>
		/// Checks to see if a player has a specific permission.
		/// Fires the <see cref="PlayerHooks.OnPlayerPermission"/> hook which may be handled to override permission checks.
		/// If the OnPlayerPermission hook is not handled and the player is assigned a temporary group, this method calls <see cref="Group.HasPermission"/> on the temporary group and returns the result.
		/// If the OnPlayerPermission hook is not handled and the player is not assigned a temporary group, this method calls <see cref="Group.HasPermission"/> on the player's current group.
		/// </summary>
		/// <param name="permission">The permission to check.</param>
		/// <returns>True if the player has that permission.</returns>
		public bool HasPermission(string permission)
		{
			PermissionHookResult hookResult = PlayerHooks.OnPlayerPermission(this, permission);

			if (hookResult != PermissionHookResult.Unhandled)
				return hookResult == PermissionHookResult.Granted;

			if (tempGroup != null)
				return tempGroup.HasPermission(permission);
			else
				return Group.HasPermission(permission);
		}

		/// <summary>
		/// Checks to see if a player has permission to use the specific banned item.
		/// Fires the <see cref="PlayerHooks.OnPlayerItembanPermission"/> hook which may be handled to override item ban permission checks.
		/// </summary>
		/// <param name="bannedItem">The <see cref="ItemBan" /> to check.</param>
		/// <returns>True if the player has permission to use the banned item.</returns>
		public bool HasPermission(ItemBan bannedItem)
		{
			return TShock.ItemBans.DataModel.ItemIsBanned(bannedItem.Name, this);
		}

		/// <summary>
		/// Checks to see if a player has permission to use the specific banned projectile.
		/// Fires the <see cref="PlayerHooks.OnPlayerProjbanPermission"/> hook which may be handled to override projectile ban permission checks.
		/// </summary>
		/// <param name="bannedProj">The <see cref="ProjectileBan" /> to check.</param>
		/// <returns>True if the player has permission to use the banned projectile.</returns>
		public bool HasPermission(ProjectileBan bannedProj)
		{
			return TShock.ProjectileBans.ProjectileIsBanned(bannedProj.ID, this);
		}
		/// <summary>
		/// Checks to see if a player has permission to use the specific banned tile.
		/// Fires the <see cref="PlayerHooks.OnPlayerTilebanPermission"/> hook which may be handled to override tile ban permission checks.
		/// </summary>
		/// <param name="bannedTile">The <see cref="TileBan" /> to check.</param>
		/// <returns>True if the player has permission to use the banned tile.</returns>
		public bool HasPermission(TileBan bannedTile)
		{
			return TShock.TileBans.TileIsBanned(bannedTile.ID, this);
		}
	}

	public class TSRestPlayer : TSPlayer
	{
		internal List<string> CommandOutput = new List<string>();

		public TSRestPlayer(string playerName, Group playerGroup) : base(playerName)
		{
			Group = playerGroup;
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		public override void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public override void SendMessage(string msg, byte red, byte green, byte blue)
		{
			this.CommandOutput.Add(msg);
		}

		public override void SendInfoMessage(string msg)
		{
			SendMessage(msg, Color.Yellow);
		}

		public override void SendSuccessMessage(string msg)
		{
			SendMessage(msg, Color.Green);
		}

		public override void SendWarningMessage(string msg)
		{
			SendMessage(msg, Color.OrangeRed);
		}

		public override void SendErrorMessage(string msg)
		{
			SendMessage(msg, Color.Red);
		}

		public List<string> GetCommandOutput()
		{
			return this.CommandOutput;
		}
	}
}
