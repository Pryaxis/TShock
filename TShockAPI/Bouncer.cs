/*
TShock, a server mod for Terraria
Copyright (C) 2011-2017 Nyx Studios (fka. The TShock Team)

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

namespace TShockAPI
{
	/// <summary>Bouncer is the TShock anti-hack and build guardian system</summary>
	internal sealed class Bouncer
	{
		/// <summary>Constructor call initializes Bouncer and related functionality.</summary>
		/// <returns>A new Bouncer.</returns>
		internal Bouncer(TerrariaPlugin pluginInstance)
		{
			// Setup hooks

			GetDataHandlers.PlayerAnimation.Register(OnPlayerAnimation);
			GetDataHandlers.NPCStrike.Register(OnNPCStrike);
			GetDataHandlers.ItemDrop.Register(OnItemDrop);
			GetDataHandlers.PlayerBuff.Register(OnPlayerBuff);
			GetDataHandlers.ChestItemChange.Register(OnChestItemChange);
			GetDataHandlers.NPCHome.Register(OnUpdateNPCHome);
			GetDataHandlers.ChestOpen.Register(OnChestOpen);
			GetDataHandlers.PlaceChest.Register(OnPlaceChest);
			GetDataHandlers.LiquidSet.Register(OnLiquidSet);
			GetDataHandlers.ProjectileKill.Register(OnProjectileKill);
			GetDataHandlers.PlayerUpdate.Register(OnPlayerUpdate);
			GetDataHandlers.KillMe.Register(OnKillMe);
			GetDataHandlers.NewProjectile.Register(OnNewProjectile);
			GetDataHandlers.PlaceObject.Register(OnPlaceObject);
			GetDataHandlers.SendTileSquare.Register(OnSendTileSquare);
			GetDataHandlers.HealOtherPlayer.Register(OnHealOtherPlayer);
			GetDataHandlers.TileEdit.Register(OnTileEdit);
		}

		/// <summary>Handles basic animation throttling for disabled players.</summary>
		/// <param name="sender">sender</param>
		/// <param name="args">args</param>
		internal void OnPlayerAnimation(object sender, GetDataHandlers.PlayerAnimationEventArgs args)
		{
			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerAnimation, "", args.Player.Index);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerAnimation, "", args.Player.Index);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles the NPC Strike event for Bouncer.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnNPCStrike(object sender, GetDataHandlers.NPCStrikeEventArgs args)
		{
			short id = args.ID;
			byte direction = args.Direction;
			short damage = args.Damage;
			float knockback = args.Knockback;
			byte crit = args.Critical;

			if (Main.npc[id] == null)
			{
				args.Handled = true;
				return;
			}

			if (damage > TShock.Config.MaxDamage && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				if (TShock.Config.KickOnDamageThresholdBroken)
				{
					TShock.Utils.Kick(args.Player, string.Format("NPC damage exceeded {0}.", TShock.Config.MaxDamage));
					args.Handled = true;
					return;
				}
				else
				{
					args.Player.Disable(String.Format("NPC damage exceeded {0}.", TShock.Config.MaxDamage), DisableFlags.WriteToLogAndConsole);
				}
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (TShock.Config.RangeChecks &&
				TShock.CheckRangePermission(args.Player, (int)(Main.npc[id].position.X / 16f), (int)(Main.npc[id].position.Y / 16f), 128))
			{
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Called when a player is damaged.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlayerDamage(object sender, GetDataHandlers.PlayerDamageEventArgs args)
		{
			byte id = args.ID;
			short damage = args.Damage;
			bool pvp = args.PVP;
			bool crit = args.Critical;
			byte direction = args.Direction;

			if (id >= Main.maxPlayers || TShock.Players[id] == null)
			{
				args.Handled = true;
				return;
			}

			if (damage > TShock.Config.MaxDamage && !args.Player.HasPermission(Permissions.ignoredamagecap) && id != args.Player.Index)
			{
				if (TShock.Config.KickOnDamageThresholdBroken)
				{
					TShock.Utils.Kick(args.Player, string.Format("Player damage exceeded {0}.", TShock.Config.MaxDamage));
					args.Handled = true;
					return;
				}
				else
				{
					args.Player.Disable(String.Format("Player damage exceeded {0}.", TShock.Config.MaxDamage), DisableFlags.WriteToLogAndConsole);
				}
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (!TShock.Players[id].TPlayer.hostile && pvp && id != args.Player.Index)
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, TShock.Players[id].TileX, TShock.Players[id].TileY, 100))
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

		}

		/// <summary>Registered when items fall to the ground to prevent cheating.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnItemDrop(object sender, GetDataHandlers.ItemDropEventArgs args)
		{
			short id = args.ID;
			Vector2 pos = args.Position;
			Vector2 vel = args.Velocity;
			short stacks = args.Stacks;
			short prefix = args.Prefix;
			bool noDelay = args.NoDelay;
			short type = args.Type;

			// player is attempting to crash clients
			if (type < -48 || type >= Main.maxItemTypes)
			{
				// Causes item duplications. Will be re added later if necessary
				//args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			//make sure the prefix is a legit value
			if (prefix > PrefixID.Count) 
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			//Item removed, let client do this to prevent item duplication
			// client side (but only if it passed the range check)
			if (type == 0)
			{
				if (TShock.CheckRangePermission(args.Player, (int)(Main.item[id].position.X / 16f), (int)(Main.item[id].position.Y / 16f)))
				{
					// Causes item duplications. Will be re added if necessary
					//args.Player.SendData(PacketTypes.ItemDrop, "", id);
					args.Handled = true;
					return;
				}

				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, (int)(pos.X / 16f), (int)(pos.Y / 16f)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			// stop the client from changing the item type of a drop but
			// only if the client isn't picking up the item
			if (Main.item[id].active && Main.item[id].netID != type)
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			Item item = new Item();
			item.netDefaults(type);
			if ((stacks > item.maxStack || stacks <= 0) || (TShock.Itembans.ItemIsBanned(EnglishLanguage.GetItemNameById(item.type), args.Player) && !args.Player.HasPermission(Permissions.allowdroppingbanneditems)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			// TODO: Remove item ban part of this check
			if ((Main.ServerSideCharacter) && (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - args.Player.LoginMS < TShock.ServerSideCharacterConfig.LogonDiscardThreshold))
			{
				//Player is probably trying to sneak items onto the server in their hands!!!
				TShock.Log.ConsoleInfo("Player {0} tried to sneak {1} onto the server!", args.Player.Name, item.Name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;

			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles Buff events.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlayerBuff(object sender, GetDataHandlers.PlayerBuffEventArgs args)
		{
			byte id = args.ID;
			byte type = args.Type;
			int time = args.Time;

			if (TShock.Players[id] == null)
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (id >= Main.maxPlayers)
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (!TShock.Players[id].TPlayer.hostile || !Main.pvpBuff[type])
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}
			
			if (TShock.CheckRangePermission(args.Player, TShock.Players[id].TileX, TShock.Players[id].TileY, 50))
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (WhitelistBuffMaxTime[type] > 0 && time <= WhitelistBuffMaxTime[type])
			{
				args.Handled = false;
				return;
			}
		}

		/// <summary>Handles when a chest item is changed.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnChestItemChange(object sender, GetDataHandlers.ChestItemEventArgs args)
		{
			short id = args.ID;
			byte slot = args.Slot;
			short stacks = args.Stacks;
			byte prefix = args.Prefix;
			short type = args.Type;

			if (args.Player.TPlayer.chest != id)
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.ChestItem, "", id, slot);
				args.Handled = true;
				return;
			}

			if (TShock.CheckTilePermission(args.Player, Main.chest[id].x, Main.chest[id].y) && TShock.Config.RegionProtectChests)
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, Main.chest[id].x, Main.chest[id].y))
			{
				args.Handled = true;
				return;
			}
		}

		/// <summary>The Bouncer handler for when an NPC is rehomed.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnUpdateNPCHome(object sender, GetDataHandlers.NPCHomeChangeEventArgs args)
		{
			int id = args.ID;
			short x = args.X;
			short y = args.Y;
			byte homeless = args.Homeless;

			// Calls to TShock.CheckTilePermission need to be broken up into different subsystems
			// In particular, this handles both regions and other things. Ouch.
			if (TShock.CheckTilePermission(args.Player, x, y))
			{
				args.Player.SendErrorMessage("You do not have access to modify this area.");
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, x, y))
			{
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				args.Handled = true;
				return;
			}
		}

		/// <summary>The Bouncer handler for when chests are opened.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs args)
		{
			if (TShock.CheckIgnores(args.Player))
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, args.X, args.Y))
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckTilePermission(args.Player, args.X, args.Y) && TShock.Config.RegionProtectChests)
			{
				args.Handled = true;
				return;
			}

			int id = Chest.FindChest(args.X, args.Y);
			args.Player.ActiveChest = id;
		}

		/// <summary>The place chest event that Bouncer hooks to prevent accidental damage.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlaceChest(object sender, GetDataHandlers.PlaceChestEventArgs args)
		{
			int tileX = args.TileX;
			int tileY = args.TileY;
			int flag = args.Flag;

			if (!TShock.Utils.TilePlacementValid(tileX, tileY) || (args.Player.Dead && TShock.Config.PreventDeadModification))
				args.Handled = true;
				return;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (flag != 0 && flag != 4 // if no container or container2 placement
				&& Main.tile[tileX, tileY].type != TileID.Containers
				&& Main.tile[tileX, tileY].type != TileID.Dressers
				&& Main.tile[tileX, tileY].type != TileID.Containers2
				&& (!TShock.Utils.MaxChests() && Main.tile[tileX, tileY].type != TileID.Dirt)) //Chest
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (flag == 2) //place dresser
			{
				if ((TShock.Utils.TilePlacementValid(tileX, tileY + 1) && Main.tile[tileX, tileY + 1].type == TileID.Teleporter) ||
					(TShock.Utils.TilePlacementValid(tileX + 1, tileY + 1) && Main.tile[tileX + 1, tileY + 1].type == TileID.Teleporter))
				{
					//Prevent a dresser from being placed on a teleporter, as this can cause client and server crashes.
					args.Player.SendTileSquare(tileX, tileY, 3);
					args.Handled = true;
					return;
				}
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles Bouncer's liquid set anti-cheat.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnLiquidSet(object sender, GetDataHandlers.LiquidSetEventArgs args)
		{
			int tileX = args.TileX;
			int tileY = args.TileY;
			byte amount = args.Amount;
			byte type = args.Type;

			if (!TShock.Utils.TilePlacementValid(tileX, tileY) || (args.Player.Dead && TShock.Config.PreventDeadModification))
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if (args.Player.TileLiquidThreshold >= TShock.Config.TileLiquidThreshold)
			{
				args.Player.Disable("Reached TileLiquid threshold.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendTileSquare(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.ignoreliquidsetdetection))
			{
				args.Player.TileLiquidThreshold++;
			}

			// Liquid anti-cheat
			// Arguably the banned buckets bit should be in the item bans system
			if (amount != 0)
			{
				int bucket = -1;
				if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == ItemID.EmptyBucket)
				{
					bucket = 0;
				}
				else if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == ItemID.WaterBucket)
				{
					bucket = 1;
				}
				else if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == ItemID.LavaBucket)
				{
					bucket = 2;
				}
				else if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == ItemID.HoneyBucket)
				{
					bucket = 3;
				}
				else if (args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == ItemID.BottomlessBucket ||
					args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type == ItemID.SuperAbsorbantSponge)
				{
					bucket = 4;
				}

				if (type == 1 && !(bucket == 2 || bucket == 0))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Spreading lava without holding a lava bucket", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (type == 1 && TShock.Itembans.ItemIsBanned("Lava Bucket", args.Player))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Using banned lava bucket without permissions", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (type == 0 && !(bucket == 1 || bucket == 0 || bucket == 4))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Spreading water without holding a water bucket", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (type == 0 && TShock.Itembans.ItemIsBanned("Water Bucket", args.Player))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Using banned water bucket without permissions", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (type == 2 && !(bucket == 3 || bucket == 0))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Spreading honey without holding a honey bucket", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (type == 2 && TShock.Itembans.ItemIsBanned("Honey Bucket", args.Player))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Using banned honey bucket without permissions", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY, 16))
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles ProjectileKill events for throttling and out of bounds projectiles.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnProjectileKill(object sender, GetDataHandlers.ProjectileKillEventArgs args)
		{
			if (args.ProjectileIndex < 0)
			{
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.RemoveProjectile(args.ProjectileIdentity, args.ProjectileOwner);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.RemoveProjectile(args.ProjectileIdentity, args.ProjectileOwner);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles disabling enforcement and minor anti-exploit stuff</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlayerUpdate(object sender, GetDataHandlers.PlayerUpdateEventArgs args)
		{
			byte plr = args.PlayerId;
			BitsByte control = args.Control;
			BitsByte pulley = args.Pulley;
			byte item = args.Item;
			var pos = args.Position;
			var vel = args.Velocity;

			if (pos.X < 0 || pos.Y < 0 || pos.X >= Main.maxTilesX * 16 - 16 || pos.Y >= Main.maxTilesY * 16 - 16)
			{
				args.Handled = true;
				return;
			}

			if (item < 0 || item >= args.Player.TPlayer.inventory.Length)
			{
				args.Handled = true;
				return;
			}

			if (args.Player.LastNetPosition == Vector2.Zero)
			{
				args.Handled = true;
				return;
			}

			if (!pos.Equals(args.Player.LastNetPosition))
			{
				float distance = Vector2.Distance(new Vector2(pos.X / 16f, pos.Y / 16f),
					new Vector2(args.Player.LastNetPosition.X / 16f, args.Player.LastNetPosition.Y / 16f));

				if (TShock.CheckIgnores(args.Player))
				{
					// If the player has moved outside the disabled zone...
					if (distance > TShock.Config.MaxRangeForDisabled)
					{
						// We need to tell them they were disabled and why, then revert the change.
						if (args.Player.IgnoreActionsForCheating != "none")
						{
							args.Player.SendErrorMessage("Disabled for cheating: " + args.Player.IgnoreActionsForCheating);
						}
						else if (args.Player.IgnoreActionsForDisabledArmor != "none")
						{
							args.Player.SendErrorMessage("Disabled for banned armor: " + args.Player.IgnoreActionsForDisabledArmor);
						}
						else if (args.Player.IgnoreActionsForInventory != "none")
						{
							args.Player.SendErrorMessage("Disabled for Server Side Inventory: " + args.Player.IgnoreActionsForInventory);
						}
						else if (TShock.Config.RequireLogin && !args.Player.IsLoggedIn)
						{
							args.Player.SendErrorMessage("Please /register or /login to play!");
						}
						else if (args.Player.IgnoreActionsForClearingTrashCan)
						{
							args.Player.SendErrorMessage("You need to rejoin to ensure your trash can is cleared!");
						}

						// ??
						var lastTileX = args.Player.LastNetPosition.X;
						var lastTileY = args.Player.LastNetPosition.Y - 48;
						if (!args.Player.Teleport(lastTileX, lastTileY))
						{
							args.Player.Spawn();
						}
						args.Handled = true;
						return;
					}
					args.Handled = true;
					return;
				}

				// Corpses don't move
				if (args.Player.Dead)
				{
					args.Handled = true;
					return;
				}

				// Noclip detection
				if (!args.Player.HasPermission(Permissions.ignorenoclipdetection) &&
					TSCheckNoclip(pos, args.Player.TPlayer.width, args.Player.TPlayer.height - (args.Player.TPlayer.mount.Active ? args.Player.TPlayer.mount.HeightBoost : 0)) && !TShock.Config.IgnoreNoClip
					&& !args.Player.TPlayer.tongued)
				{
					var lastTileX = args.Player.LastNetPosition.X;
					var lastTileY = args.Player.LastNetPosition.Y;
					if (!args.Player.Teleport(lastTileX, lastTileY))
					{
						args.Player.SendErrorMessage("You got stuck in a solid object, Sent to spawn point.");
						args.Player.Spawn();
					}
					args.Handled = true;
					return;
				}
			}

			return;
		}

		/// <summary>Bouncer's KillMe hook stops crash exploits from out of bounds values.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnKillMe(object sender, GetDataHandlers.KillMeEventArgs args)
		{
			short damage = args.Damage;
			short id = args.PlayerId;
			PlayerDeathReason playerDeathReason = args.PlayerDeathReason;

			if (damage > 20000) //Abnormal values have the potential to cause infinite loops in the server.
			{
				TShock.Utils.ForceKick(args.Player, "Crash Exploit Attempt", true);
				TShock.Log.ConsoleError("Death Exploit Attempt: Damage {0}", damage);
				args.Handled = true;
				return;
			}

			if (id >= Main.maxPlayers)
			{
				args.Handled = true;
				return;
			}

			// This was formerly marked as a crash check; does not actually crash on this specific packet.
			if (playerDeathReason != null)
			{
				if (playerDeathReason.GetDeathText(TShock.Players[id].Name).ToString().Length > 500)
				{
					TShock.Utils.Kick(TShock.Players[id], "Death reason outside of normal bounds.", true);
					args.Handled = true;
					return;
				}
			}
		}

		/// <summary>Bouncer's projectile trigger hook stops world damaging projectiles from destroying the world.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnNewProjectile(object sender, GetDataHandlers.NewProjectileEventArgs args)
		{
			short ident = args.Identity;
			Vector2 pos = args.Position;
			Vector2 vel = args.Velocity;
			float knockback = args.Knockback;
			short damage = args.Damage;
			byte owner = args.Owner;
			short type = args.Type;
			int index = args.Index;

			if (index > Main.maxProjectiles || index < 0)
			{
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (TShock.ProjectileBans.ProjectileIsBanned(type, args.Player))
			{
				args.Player.Disable("Player does not have permission to create that projectile.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendErrorMessage("You do not have permission to create that projectile.");
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (damage > TShock.Config.MaxProjDamage && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				args.Player.Disable(String.Format("Projectile damage is higher than {0}.", TShock.Config.MaxProjDamage), DisableFlags.WriteToLogAndConsole);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			bool hasPermission = args.Player.HasProjectilePermission(index, type);
			if (!TShock.Config.IgnoreProjUpdate && !hasPermission && !args.Player.HasPermission(Permissions.ignoreprojectiledetection))
			{
				if (type == ProjectileID.BlowupSmokeMoonlord
					|| type == ProjectileID.PhantasmalEye
					|| type == ProjectileID.CultistBossIceMist
					|| (type >= ProjectileID.MoonlordBullet && type <= ProjectileID.MoonlordTurretLaser)
					|| type == ProjectileID.DeathLaser || type == ProjectileID.Landmine
					|| type == ProjectileID.BulletDeadeye || type == ProjectileID.BoulderStaffOfEarth
					|| (type > ProjectileID.ConfettiMelee && type < ProjectileID.SpiritHeal)
					|| (type >= ProjectileID.FlamingWood && type <= ProjectileID.GreekFire3)
					|| (type >= ProjectileID.PineNeedleHostile && type <= ProjectileID.Spike)
					|| (type >= ProjectileID.MartianTurretBolt && type <= ProjectileID.RayGunnerLaser)
					|| type == ProjectileID.CultistBossLightningOrb)
				{
					TShock.Log.Debug("Certain projectiles have been ignored for cheat detection.");
				}
				else
				{
					args.Player.Disable(String.Format("Does not have projectile permission to update projectile. ({0})", type), DisableFlags.WriteToLogAndConsole);
					args.Player.RemoveProjectile(ident, owner);
				}
				args.Handled = true;
				return;
			}

			if (args.Player.ProjectileThreshold >= TShock.Config.ProjectileThreshold)
			{
				args.Player.Disable("Reached projectile update threshold.", DisableFlags.WriteToLogAndConsole);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.ignoreprojectiledetection))
			{
				if (type == ProjectileID.CrystalShard && TShock.Config.ProjIgnoreShrapnel) // Ignore crystal shards
				{
					TShock.Log.Debug("Ignoring shrapnel per config..");
				}
				else if (!Main.projectile[index].active)
				{
					args.Player.ProjectileThreshold++; // Creating new projectile
				}
			}

			if (hasPermission &&
				(type == ProjectileID.Bomb
				|| type == ProjectileID.Dynamite
				|| type == ProjectileID.StickyBomb
				|| type == ProjectileID.StickyDynamite))
			{
				//  Denotes that the player has recently set a fuse - used for cheat detection.
				args.Player.RecentFuse = 10;
			}
		}

		/// <summary>Bouncer's PlaceObject hook reverts malicious tile placement.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlaceObject(object sender, GetDataHandlers.PlaceObjectEventArgs args)
		{
			short x = args.X;
			short y = args.Y;
			short type = args.Type;
			short style = args.Style;
			byte alternate = args.Alternate;
			bool direction = args.Direction;

			if (type < 0 || type >= Main.maxTileSets)
			{
				args.Handled = true;
				return;
			}

			if (x < 0 || x >= Main.maxTilesX)
			{
				args.Handled = true;
				return;
			}

			if (y < 0 || y >= Main.maxTilesY)
			{
				args.Handled = true;
				return;
			}

			//style 52 and 53 are used by ItemID.Fake_newchest1 and ItemID.Fake_newchest2
			//These two items cause localised lag and rendering issues
			if (type == TileID.FakeContainers && (style == 52 || style == 53))
			{
				args.Player.SendTileSquare(x, y, 4);
				args.Handled = true;
				return;
			}

			// TODO: REMOVE. This does NOT look like Bouncer code.
			if (TShock.TileBans.TileIsBanned(type, args.Player))
			{
				args.Player.SendTileSquare(x, y, 1);
				args.Player.SendErrorMessage("You do not have permission to place this tile.");
				args.Handled = true;
				return;
			}

			if (!TShock.Utils.TilePlacementValid(x, y))
			{
				args.Player.SendTileSquare(x, y, 1);
				args.Handled = true;
				return;
			}

			if (args.Player.Dead && TShock.Config.PreventDeadModification)
			{
				args.Player.SendTileSquare(x, y, 4);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(x, y, 4);
				args.Handled = true;
				return;
			}

			// This is neccessary to check in order to prevent special tiles such as 
			// queen bee larva, paintings etc that use this packet from being placed 
			// without selecting the right item.
			if (type != args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].createTile)
			{
				args.Player.SendTileSquare(x, y, 4);
				args.Handled = true;
				return;
			}

			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData == null)
			{
				args.Handled = true;
				return;
			}

			x -= tileData.Origin.X;
			y -= tileData.Origin.Y;

			for (int i = x; i < x + tileData.Width; i++)
			{
				for (int j = y; j < y + tileData.Height; j++)
				{
					if (TShock.CheckTilePermission(args.Player, i, j, type, EditAction.PlaceTile))
					{
						args.Player.SendTileSquare(i, j, 4);
						args.Handled = true;
						return;
					}
				}
			}

			// Ignore rope placement range
			if ((type != TileID.Rope
					|| type != TileID.SilkRope
					|| type != TileID.VineRope
					|| type != TileID.WebRope)
					&& TShock.CheckRangePermission(args.Player, x, y))
			{
				args.Player.SendTileSquare(x, y, 4);
				args.Handled = true;
				return;
			}

			if (args.Player.TilePlaceThreshold >= TShock.Config.TilePlaceThreshold)
			{
				args.Player.Disable("Reached TilePlace threshold.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendTileSquare(x, y, 4);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.ignoreplacetiledetection))
			{
				args.Player.TilePlaceThreshold++;
				var coords = new Vector2(x, y);
				lock (args.Player.TilesCreated)
					if (!args.Player.TilesCreated.ContainsKey(coords))
						args.Player.TilesCreated.Add(coords, Main.tile[x, y]);
			}
		}

		/// <summary>Bouncer's TileEdit hook is used to revert malicious tile changes.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
		{
			EditAction action = args.Action;
			int tileX = args.X;
			int tileY = args.Y;
			short editData = args.EditData;
			EditType type = args.editDetail;
			byte style = args.Style;

			try
			{
				if (editData < 0)
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (!TShock.Utils.TilePlacementValid(tileX, tileY))
				{
					args.Player.SendTileSquare(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (action == EditAction.KillTile && Main.tile[tileX, tileY].type == TileID.MagicalIceBlock)
				{
					args.Handled = false;
					return;
				}
					
				if (args.Player.Dead && TShock.Config.PreventDeadModification)
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (args.Player.AwaitingName)
				{
					bool includeUnprotected = false;
					bool includeZIndexes = false;
					bool persistentMode = false;
					foreach (string parameter in args.Player.AwaitingNameParameters)
					{
						if (parameter.Equals("-u", StringComparison.InvariantCultureIgnoreCase))
							includeUnprotected = true;
						if (parameter.Equals("-z", StringComparison.InvariantCultureIgnoreCase))
							includeZIndexes = true;
						if (parameter.Equals("-p", StringComparison.InvariantCultureIgnoreCase))
							persistentMode = true;
					}


					// TODO: REMOVE. This does NOT look like Bouncer code.
					List<string> outputRegions = new List<string>();
					foreach (Region region in TShock.Regions.Regions.OrderBy(r => r.Z).Reverse())
					{
						if (!includeUnprotected && !region.DisableBuild)
							continue;
						if (tileX < region.Area.Left || tileX > region.Area.Right)
							continue;
						if (tileY < region.Area.Top || tileY > region.Area.Bottom)
							continue;

						string format = "{1}";
						if (includeZIndexes)
							format = "{1} (z:{0})";

						outputRegions.Add(string.Format(format, region.Z, region.Name));
					}

					if (outputRegions.Count == 0)
					{
						if (includeUnprotected)
							args.Player.SendInfoMessage("There are no regions at this point.");
						else
							args.Player.SendInfoMessage("There are no regions at this point or they are not protected.");
					}
					else
					{
						if (includeUnprotected)
							args.Player.SendSuccessMessage("Regions at this point:");
						else
							args.Player.SendSuccessMessage("Protected regions at this point:");

						foreach (string line in PaginationTools.BuildLinesFromTerms(outputRegions))
							args.Player.SendMessage(line, Color.White);
					}

					if (!persistentMode)
					{
						args.Player.AwaitingName = false;
						args.Player.AwaitingNameParameters = null;
					}

					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				// TODO: REMOVE. This does NOT look like Bouncer code.
				if (args.Player.AwaitingTempPoint > 0)
				{
					args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].X = tileX;
					args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].Y = tileY;
					args.Player.SendInfoMessage("Set temp point {0}.", args.Player.AwaitingTempPoint);
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Player.AwaitingTempPoint = 0;
					args.Handled = true;
					return;
				}

				Item selectedItem = args.Player.SelectedItem;
				int lastKilledProj = args.Player.LastKilledProjectile;
				ITile tile = Main.tile[tileX, tileY];

				if (action == EditAction.PlaceTile)
				{
					if (TShock.TileBans.TileIsBanned(editData, args.Player))
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Player.SendErrorMessage("You do not have permission to place this tile.");
						args.Handled = true;
						return;
					}
				}

				if (action == EditAction.KillTile && !Main.tileCut[tile.type] && !breakableTiles.Contains(tile.type))
				{
					//TPlayer.mount.Type 8 => Drill Containment Unit.

					// If the tile is an axe tile and they aren't selecting an axe, they're hacking.
					if (Main.tileAxe[tile.type] && ((args.Player.TPlayer.mount.Type != 8 && selectedItem.axe == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
					// If the tile is a hammer tile and they aren't selecting a hammer, they're hacking.
					else if (Main.tileHammer[tile.type] && ((args.Player.TPlayer.mount.Type != 8 && selectedItem.hammer == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
					// If the tile is a pickaxe tile and they aren't selecting a pickaxe, they're hacking.
					// Item frames can be modified without pickaxe tile.
					else if (tile.type != TileID.ItemFrame
						&& !Main.tileAxe[tile.type] && !Main.tileHammer[tile.type] && tile.wall == 0 && args.Player.TPlayer.mount.Type != 8 && selectedItem.pick == 0 && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0)
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.KillWall)
				{
					// If they aren't selecting a hammer, they could be hacking.
					if (selectedItem.hammer == 0 && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0 && selectedItem.createWall == 0)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.PlaceTile && (projectileCreatesTile.ContainsKey(lastKilledProj) && editData == projectileCreatesTile[lastKilledProj]))
				{
					args.Player.LastKilledProjectile = 0;
				}
				else if (action == EditAction.PlaceTile || action == EditAction.PlaceWall)
				{
					if ((action == EditAction.PlaceTile && TShock.Config.PreventInvalidPlaceStyle) &&
						(MaxPlaceStyles.ContainsKey(editData) && style > MaxPlaceStyles[editData]) &&
						(ExtraneousPlaceStyles.ContainsKey(editData) && style > ExtraneousPlaceStyles[editData]))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						args.Handled = true;
						return;
					}

					// If they aren't selecting the item which creates the tile or wall, they're hacking.
					if (!(selectedItem.netID == ItemID.IceRod && editData == TileID.MagicalIceBlock) &&
						(editData != (action == EditAction.PlaceTile ? selectedItem.createTile : selectedItem.createWall) &&
						!(ropeCoilPlacements.ContainsKey(selectedItem.netID) && editData == ropeCoilPlacements[selectedItem.netID])))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						args.Handled = true;
						return;
					}

					// Using the actuation accessory can lead to actuator hacking
					if (TShock.Itembans.ItemIsBanned("Actuator", args.Player) && args.Player.TPlayer.autoActuator)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Player.SendErrorMessage("You do not have permission to place actuators.");
						args.Handled = true;
						return;
					}
					if (TShock.Itembans.ItemIsBanned(EnglishLanguage.GetItemNameById(selectedItem.netID), args.Player) || editData >= (action == EditAction.PlaceTile ? Main.maxTileSets : Main.maxWallTypes))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
					if (action == EditAction.PlaceTile && (editData == TileID.PiggyBank || editData == TileID.Safes) && Main.ServerSideCharacter)
					{
						args.Player.SendErrorMessage("You cannot place this tile because server side characters are enabled.");
						args.Player.SendTileSquare(tileX, tileY, 3);
						args.Handled = true;
						return;
					}
					if (action == EditAction.PlaceTile && (editData == TileID.Containers || editData == TileID.Containers2))
					{
						if (TShock.Utils.MaxChests())
						{
							args.Player.SendErrorMessage("The world's chest limit has been reached - unable to place more.");
							args.Player.SendTileSquare(tileX, tileY, 3);
							args.Handled = true;
							return;
						}
						if ((TShock.Utils.TilePlacementValid(tileX, tileY + 1) && Main.tile[tileX, tileY + 1].type == TileID.Boulder) ||
							(TShock.Utils.TilePlacementValid(tileX + 1, tileY + 1) && Main.tile[tileX + 1, tileY + 1].type == TileID.Boulder))
						{
							args.Player.SendTileSquare(tileX, tileY, 3);
							args.Handled = true;
							return;
						}
					}
				}
				else if (action == EditAction.PlaceWire || action == EditAction.PlaceWire2 || action == EditAction.PlaceWire3)
				{
					// If they aren't selecting a wrench, they're hacking.
					// WireKite = The Grand Design
					if (selectedItem.type != ItemID.Wrench
						&& selectedItem.type != ItemID.BlueWrench
						&& selectedItem.type != ItemID.GreenWrench
						&& selectedItem.type != ItemID.YellowWrench
						&& selectedItem.type != ItemID.MulticolorWrench
						&& selectedItem.type != ItemID.WireKite)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.KillActuator || action == EditAction.KillWire ||
					action == EditAction.KillWire2 || action == EditAction.KillWire3)
				{
					// If they aren't selecting the wire cutter, they're hacking.
					if (selectedItem.type != ItemID.WireCutter
						&& selectedItem.type != ItemID.WireKite
						&& selectedItem.type != ItemID.MulticolorWrench)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.PlaceActuator)
				{
					// If they aren't selecting the actuator and don't have the Presserator equipped, they're hacking.
					if (selectedItem.type != ItemID.Actuator && !args.Player.TPlayer.autoActuator)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				if (TShock.Config.AllowCutTilesAndBreakables && Main.tileCut[tile.type])
				{
					if (action == EditAction.KillWall)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
					args.Handled = false;
					return;
				}

				if (TShock.CheckIgnores(args.Player))
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (TShock.CheckTilePermission(args.Player, tileX, tileY, editData, action))
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (TShock.CheckRangePermission(args.Player, tileX, tileY))
				{
					if (action == EditAction.PlaceTile && (editData == TileID.Rope || editData == TileID.SilkRope || editData == TileID.VineRope || editData == TileID.WebRope))
					{
						args.Handled = false;
						return;
					}

					if (action == EditAction.KillTile || action == EditAction.KillWall && ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0)
					{
						args.Handled = false;
						return;
					}

					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (args.Player.TileKillThreshold >= TShock.Config.TileKillThreshold)
				{
					args.Player.Disable("Reached TileKill threshold.", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (args.Player.TilePlaceThreshold >= TShock.Config.TilePlaceThreshold)
				{
					args.Player.Disable("Reached TilePlace threshold.", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if ((action == EditAction.PlaceTile || action == EditAction.PlaceWall) && !args.Player.HasPermission(Permissions.ignoreplacetiledetection))
				{
					args.Player.TilePlaceThreshold++;
					var coords = new Vector2(tileX, tileY);
					lock (args.Player.TilesCreated)
						if (!args.Player.TilesCreated.ContainsKey(coords))
							args.Player.TilesCreated.Add(coords, Main.tile[tileX, tileY]);
				}

				if ((action == EditAction.KillTile || action == EditAction.KillTileNoItem || action == EditAction.KillWall) && Main.tileSolid[Main.tile[tileX, tileY].type] &&
					!args.Player.HasPermission(Permissions.ignorekilltiledetection))
				{
					args.Player.TileKillThreshold++;
					var coords = new Vector2(tileX, tileY);
					lock (args.Player.TilesDestroyed)
						if (!args.Player.TilesDestroyed.ContainsKey(coords))
							args.Player.TilesDestroyed.Add(coords, Main.tile[tileX, tileY]);
				}
				args.Handled = false;
				return;
			}
			catch
			{
				args.Player.SendTileSquare(tileX, tileY, 4);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Bouncer's HealOther handler prevents gross misuse of HealOther packets by hackers.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnHealOtherPlayer(object sender, GetDataHandlers.HealOtherPlayerEventArgs args)
		{
			short amount = args.Amount;
			byte plr = args.TargetPlayerIndex;

			if (amount <= 0 || Main.player[plr] == null || !Main.player[plr].active)
			{
				args.Handled = true;
				return;
			}

			// Why 0.2?
			// @bartico6: Because heal other player only happens when you are using the spectre armor with the hood,
			// and the healing you can do with that is 20% of your damage.
			if (amount > TShock.Config.MaxDamage * 0.2)
			{
				args.Player.Disable("HealOtherPlayer cheat attempt!", DisableFlags.WriteToLogAndConsole);
				args.Handled = true;
				return;
			}

			if (args.Player.HealOtherThreshold > TShock.Config.HealOtherThreshold)
			{
				args.Player.Disable("Reached HealOtherPlayer threshold.", DisableFlags.WriteToLogAndConsole);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player) || (DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Handled = true;
				return;
			}

			args.Player.HealOtherThreshold++;
			args.Handled = false;
			return;
		}

		/// <summary>Bouncer's SendTileSquare hook halts large scope world destruction.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnSendTileSquare(object sender, GetDataHandlers.SendTileSquareEventArgs args)
		{
			short size = args.Size;
			int tileX = args.TileX;
			int tileY = args.TileY;

			if (args.Player.HasPermission(Permissions.allowclientsideworldedit))
			{
				args.Handled = false;
				return;
			}

			// From White:
			// IIRC it's because 5 means a 5x5 square which is normal for a tile square, and anything bigger is a non-vanilla tile modification attempt
			if (size > 5)
			{
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				args.Handled = true;
				return;
			}

			try
			{
				var tiles = new NetTile[size, size];
				for (int x = 0; x < size; x++)
				{
					for (int y = 0; y < size; y++)
					{
						tiles[x, y] = new NetTile(args.Data);
					}
				}

				bool changed = false;
				for (int x = 0; x < size; x++)
				{
					int realx = tileX + x;
					if (realx < 0 || realx >= Main.maxTilesX)
						continue;

					for (int y = 0; y < size; y++)
					{
						int realy = tileY + y;
						if (realy < 0 || realy >= Main.maxTilesY)
							continue;

						var tile = Main.tile[realx, realy];
						var newtile = tiles[x, y];
						if (TShock.CheckTilePermission(args.Player, realx, realy) ||
							TShock.CheckRangePermission(args.Player, realx, realy))
						{
							continue;
						}

						// Fixes the Flower Boots not creating flowers issue
						if (size == 1 && args.Player.Accessories.Any(i => i.active && i.netID == ItemID.FlowerBoots))
						{
							if (Main.tile[realx, realy + 1].type == TileID.Grass && (newtile.Type == TileID.Plants || newtile.Type == TileID.Plants2))
							{
								args.Handled = false;
								return;
							}

							if (Main.tile[realx, realy + 1].type == TileID.HallowedGrass && (newtile.Type == TileID.HallowedPlants || newtile.Type == TileID.HallowedPlants2))
							{
								args.Handled = false;
								return;
							}

							if (Main.tile[realx, realy + 1].type == TileID.JungleGrass && newtile.Type == TileID.JunglePlants2)
							{
								args.Handled = false;
								return;
							}
						}

						// Junction Box
						if (tile.type == TileID.WirePipe)
						{
							args.Handled = false;
							return;
						}

						// Orientable tiles
						if (tile.type == newtile.Type && orientableTiles.Contains(tile.type))
						{
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							changed = true;
						}
						// Landmine
						if (tile.type == TileID.LandMine && !newtile.Active)
						{
							Main.tile[realx, realy].active(false);
							changed = true;
						}
						// Sensors
						if(newtile.Type == TileID.LogicSensor && !Main.tile[realx, realy].active())
						{
							Main.tile[realx, realy].type = newtile.Type;
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							Main.tile[realx, realy].active(true);
							changed = true;
						}

						if (tile.active() && newtile.Active && tile.type != newtile.Type)
						{
							// Grass <-> Grass
							if ((TileID.Sets.Conversion.Grass[tile.type] && TileID.Sets.Conversion.Grass[newtile.Type]) ||
								// Dirt <-> Dirt
								((tile.type == 0 || tile.type == 59) &&
								(newtile.Type == 0 || newtile.Type == 59)) ||
								// Ice <-> Ice
								(TileID.Sets.Conversion.Ice[tile.type] && TileID.Sets.Conversion.Ice[newtile.Type]) ||
								// Stone <-> Stone
								((TileID.Sets.Conversion.Stone[tile.type] || Main.tileMoss[tile.type]) &&
								(TileID.Sets.Conversion.Stone[newtile.Type] || Main.tileMoss[newtile.Type])) ||
								// Sand <-> Sand
								(TileID.Sets.Conversion.Sand[tile.type] && TileID.Sets.Conversion.Sand[newtile.Type]) ||
								// Sandstone <-> Sandstone
								(TileID.Sets.Conversion.Sandstone[tile.type] && TileID.Sets.Conversion.Sandstone[newtile.Type]) ||
								// Hardened Sand <-> Hardened Sand
								(TileID.Sets.Conversion.HardenedSand[tile.type] && TileID.Sets.Conversion.HardenedSand[newtile.Type]))
							{
								Main.tile[realx, realy].type = newtile.Type;
								changed = true;
							}
						}
						// Stone wall <-> Stone wall
						if (((tile.wall == 1 || tile.wall == 3 || tile.wall == 28 || tile.wall == 83) &&
							(newtile.Wall == 1 || newtile.Wall == 3 || newtile.Wall == 28 || newtile.Wall == 83)) ||
							// Leaf wall <-> Leaf wall
							(((tile.wall >= 63 && tile.wall <= 70) || tile.wall == 81) &&
							((newtile.Wall >= 63 && newtile.Wall <= 70) || newtile.Wall == 81)))
						{
							Main.tile[realx, realy].wall = newtile.Wall;
							changed = true;
						}

						if ((tile.type == TileID.TrapdoorClosed && (newtile.Type == TileID.TrapdoorOpen || !newtile.Active)) ||
							(tile.type == TileID.TrapdoorOpen && (newtile.Type == TileID.TrapdoorClosed || !newtile.Active)) ||
							(!tile.active() && newtile.Active && (newtile.Type == TileID.TrapdoorOpen||newtile.Type == TileID.TrapdoorClosed)))
						{
							Main.tile[realx, realy].type = newtile.Type;
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							Main.tile[realx, realy].active(newtile.Active);
							changed = true;
						}
					}
				}

				if (changed)
				{
					TSPlayer.All.SendTileSquare(tileX, tileY, size + 1);
					WorldGen.RangeFrame(tileX, tileY, tileX + size, tileY + size);
				}
				else
				{
					args.Player.SendTileSquare(tileX, tileY, size);
				}
			}
			catch
			{
				args.Player.SendTileSquare(tileX, tileY, size);
			}
			args.Handled = false;
			return;
		}

		/// <summary>
		/// Tile IDs that can be oriented:
		/// Cannon,
		/// Chairs,
		/// Beds,
		/// Bathtubs,
		/// Statues,
		/// Mannequin,
		/// Traps,
		/// MusicBoxes,
		/// ChristmasTree,
		/// WaterFountain,
		/// Womannequin,
		/// MinecartTrack,
		/// WeaponsRack,
		/// LunarMonolith,
		/// TargetDummy,
		/// Campfire
		/// </summary>
		private static int[] orientableTiles = new int[]
		{
			TileID.Cannon,
			TileID.Chairs,
			TileID.Beds,
			TileID.Bathtubs,
			TileID.Statues,
			TileID.Mannequin,
			TileID.Traps,
			TileID.MusicBoxes,
			TileID.ChristmasTree,
			TileID.WaterFountain,
			TileID.Womannequin,
			TileID.MinecartTrack,
			TileID.WeaponsRack,
			TileID.ItemFrame,
			TileID.LunarMonolith,
			TileID.TargetDummy,
			TileID.Campfire
		};

	}
}