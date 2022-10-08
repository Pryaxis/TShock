/*
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
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using TShockAPI.Net;
using Terraria;
using Microsoft.Xna.Framework;
using TShockAPI.Localization;
using static TShockAPI.GetDataHandlers;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using TShockAPI.Models.PlayerUpdate;
using System.Threading.Tasks;

namespace TShockAPI
{
	/// <summary>Bouncer is the TShock anti-hack and anti-cheat system.</summary>
	internal sealed class Bouncer
	{
		internal Handlers.SendTileRectHandler STSHandler { get; private set; }
		internal Handlers.NetModules.NetModulePacketHandler NetModuleHandler { get; private set; }
		internal Handlers.EmojiHandler EmojiHandler { get; private set; }
		internal Handlers.IllegalPerSe.EmojiPlayerMismatch EmojiPlayerMismatch { get; private set; }
		internal Handlers.DisplayDollItemSyncHandler DisplayDollItemSyncHandler { get; private set; }
		internal Handlers.RequestTileEntityInteractionHandler RequestTileEntityInteractionHandler { get; private set; }
		internal Handlers.LandGolfBallInCupHandler LandGolfBallInCupHandler { get; private set; }
		internal Handlers.SyncTilePickingHandler SyncTilePickingHandler { get; private set; }

		/// <summary>
		/// Represents a place style corrector.
		/// </summary>
		/// <param name="player">The player placing the tile.</param>
		/// <param name="requestedPlaceStyle">The requested place style to be placed.</param>
		/// <param name="actualItemPlaceStyle">The actual place style that should be placed, based of the player's held item.</param>
		/// <returns>The correct place style in the current context.</returns>
		internal delegate int PlaceStyleCorrector(Player player, int requestedPlaceStyle, int actualItemPlaceStyle);

		/// <summary>
		/// Represents a dictionary of <see cref="PlaceStyleCorrector"/>s, the key is the tile ID and the value is the corrector.
		/// </summary>
		internal Dictionary<int, PlaceStyleCorrector> PlaceStyleCorrectors = new Dictionary<int, PlaceStyleCorrector>();

		/// <summary>Constructor call initializes Bouncer and related functionality.</summary>
		/// <returns>A new Bouncer.</returns>
		internal Bouncer()
		{
			STSHandler = new Handlers.SendTileRectHandler();
			GetDataHandlers.SendTileRect += STSHandler.OnReceive;

			NetModuleHandler = new Handlers.NetModules.NetModulePacketHandler();
			GetDataHandlers.ReadNetModule += NetModuleHandler.OnReceive;

			EmojiPlayerMismatch = new Handlers.IllegalPerSe.EmojiPlayerMismatch();
			GetDataHandlers.Emoji += EmojiPlayerMismatch.OnReceive;

			EmojiHandler = new Handlers.EmojiHandler();
			GetDataHandlers.Emoji += EmojiHandler.OnReceive;

			DisplayDollItemSyncHandler = new Handlers.DisplayDollItemSyncHandler();
			GetDataHandlers.DisplayDollItemSync += DisplayDollItemSyncHandler.OnReceive;

			RequestTileEntityInteractionHandler = new Handlers.RequestTileEntityInteractionHandler();
			GetDataHandlers.RequestTileEntityInteraction += RequestTileEntityInteractionHandler.OnReceive;

			LandGolfBallInCupHandler = new Handlers.LandGolfBallInCupHandler();
			GetDataHandlers.LandGolfBallInCup += LandGolfBallInCupHandler.OnReceive;

			SyncTilePickingHandler = new Handlers.SyncTilePickingHandler();
			GetDataHandlers.SyncTilePicking += SyncTilePickingHandler.OnReceive;

			// Setup hooks
			GetDataHandlers.GetSection += OnGetSection;
			GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
			GetDataHandlers.TileEdit += OnTileEdit;
			GetDataHandlers.ItemDrop += OnItemDrop;
			GetDataHandlers.NewProjectile += OnNewProjectile;
			GetDataHandlers.NPCStrike += OnNPCStrike;
			GetDataHandlers.ProjectileKill += OnProjectileKill;
			GetDataHandlers.ChestItemChange += OnChestItemChange;
			GetDataHandlers.ChestOpen += OnChestOpen;
			GetDataHandlers.PlaceChest += OnPlaceChest;
			GetDataHandlers.PlayerZone += OnPlayerZone;
			GetDataHandlers.PlayerAnimation += OnPlayerAnimation;
			GetDataHandlers.LiquidSet += OnLiquidSet;
			GetDataHandlers.PlayerBuff += OnPlayerBuff;
			GetDataHandlers.NPCAddBuff += OnNPCAddBuff;
			GetDataHandlers.NPCHome += OnUpdateNPCHome;
			GetDataHandlers.HealOtherPlayer += OnHealOtherPlayer;
			GetDataHandlers.ReleaseNPC += OnReleaseNPC;
			GetDataHandlers.PlaceObject += OnPlaceObject;
			GetDataHandlers.PlaceTileEntity += OnPlaceTileEntity;
			GetDataHandlers.PlaceItemFrame += OnPlaceItemFrame;
			GetDataHandlers.PortalTeleport += OnPlayerPortalTeleport;
			GetDataHandlers.GemLockToggle += OnGemLockToggle;
			GetDataHandlers.MassWireOperation += OnMassWireOperation;
			GetDataHandlers.PlayerDamage += OnPlayerDamage;
			GetDataHandlers.KillMe += OnKillMe;
			GetDataHandlers.FishOutNPC += OnFishOutNPC;
			GetDataHandlers.FoodPlatterTryPlacing += OnFoodPlatterTryPlacing;


			// The following section is based off Player.PlaceThing_Tiles_PlaceIt and Player.PlaceThing_Tiles_PlaceIt_GetLegacyTileStyle.
			// Multi-block tiles are intentionally ignored because they don't pass through OnTileEdit.
			PlaceStyleCorrectors.Add(TileID.Torches,
				(player, requestedPlaceStyle, actualItemPlaceStyle) =>
				{
					// If the client is attempting to place a default torch, we need to check that the torch they are attempting to place is valid.
					// The place styles may mismatch if the player is placing a biome torch.
					// Biome torches can only be placed if the player has unlocked them (Torch God's Favor)
					// Therefore, the following conditions need to be true:
					// - The client's selected item will create a default torch(this should be true if this handler is running)
					// - The client's selected item's place style will be that of a default torch
					// - The client has unlocked biome torches
					if (actualItemPlaceStyle == TorchID.Torch && player.unlockedBiomeTorches)
					{
						// The server isn't notified when the player turns on biome torches.
						// So on the client it can be on, while on the server it's off.
						// BiomeTorchPlaceStyle returns placeStyle as-is if biome torches is off.
						// Because of the uncertainty, we:
						// 1. Ensure that UsingBiomeTorches is on, so we can get the correct
						// value from BiomeTorchPlaceStyle.
						// 2. Check if the torch is either 0 or the biome torch since we aren't
						// sure if the player has biome torches on
						var usingBiomeTorches = player.UsingBiomeTorches;
						player.UsingBiomeTorches = true;
						// BiomeTorchPlaceStyle returns the place style of the player's current biome's biome torch
						var biomeTorchPlaceStyle = player.BiomeTorchPlaceStyle(actualItemPlaceStyle);
						// Reset UsingBiomeTorches value
						player.UsingBiomeTorches = usingBiomeTorches;

						return biomeTorchPlaceStyle;
					}
					else
					{
						// If the player isn't holding the default torch, then biome torches don't apply and return item place style.
						// Or, they are holding the default torch but haven't unlocked biome torches yet, so return item place style.
						return actualItemPlaceStyle;
					}
				});
			PlaceStyleCorrectors.Add(TileID.Presents,
				(player, requestedPlaceStyle, actualItemPlaceStyle) =>
				{
					// RNG only generates placeStyles less than 7, so permit only <7
					// Note: there's an 8th present(blue, golden stripes) that's unplaceable.
					// https://terraria.fandom.com/wiki/Presents, last present of the 8 displayed
					if (requestedPlaceStyle < 7)
					{
						return requestedPlaceStyle;
					}
					else
					{
						// Return 0 for now, but ideally 0-7 should be returned.
						return 0;
					}
				});
			PlaceStyleCorrectors.Add(TileID.Explosives,
				(player, requestedPlaceStyle, actualItemPlaceStyle) =>
				{
					// RNG only generates placeStyles less than 2, so permit only <2
					if (requestedPlaceStyle < 2)
					{
						return requestedPlaceStyle;
					}
					else
					{
						// Return 0 for now, but ideally 0-1 should be returned.
						return 0;
					}
				});
			PlaceStyleCorrectors.Add(TileID.Crystals,
				(player, requestedPlaceStyle, actualItemPlaceStyle) =>
				{
					// RNG only generates placeStyles less than 18, so permit only <18.
					// Note: Gelatin Crystals(Queen Slime summon) share the same ID as Crystal Shards.
					// <18 includes all shards except Gelatin Crystals.
					if (requestedPlaceStyle < 18)
					{
						return requestedPlaceStyle;
					}
					else
					{
						// Return 0 for now, but ideally 0-17 should be returned.
						return 0;
					}
				});
			PlaceStyleCorrectors.Add(TileID.MinecartTrack,
				(player, requestedPlaceStyle, actualItemPlaceStyle) =>
				{
					// Booster tracks have 2 variations, but only 1 item.
					// The variation depends on the direction the player is facing.
					if (actualItemPlaceStyle == 2)
					{
						// Check the direction the player is facing.
						// 1 is right and -1 is left, these are the only possible values.
						if (player.direction == 1)
						{
							// Right-facing booster tracks
							return 3;
						}
						else if (player.direction == -1)
						{
							// Left-facing booster tracks
							return 2;
						}
						else
						{
							throw new InvalidOperationException("未识别的玩家方向");
						}
					}
					else
					{
						// Not a booster track, return as-is.
						return actualItemPlaceStyle;
					}
				});
		}

		internal void OnGetSection(object sender, GetDataHandlers.GetSectionEventArgs args)
		{
			if (args.Player.RequestedSection)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnGetSection拒绝GetSection数据包来自{0}", args.Player.Name);
				args.Handled = true;
				return;
			}
			args.Player.RequestedSection = true;

			if (String.IsNullOrEmpty(args.Player.Name))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnGetSection拒绝空白角色名.");
				args.Player.Kick("你的客户发送了一个空白角色名", true, true);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.ignorestackhackdetection))
			{
				args.Player.IsDisabledForStackDetection = args.Player.HasHackedItemStacks(shouldWarnPlayer: true);
			}
		}

		/// <summary>Handles disabling enforcement and minor anti-exploit stuff</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlayerUpdate(object sender, GetDataHandlers.PlayerUpdateEventArgs args)
		{
			byte plr = args.PlayerId;
			ControlSet control = args.Control;
			MiscDataSet1 miscData1 = args.MiscData1;
			byte item = args.SelectedItem ;
			var pos = args.Position;
			var vel = args.Velocity;

			if (Single.IsInfinity(vel.X) || Single.IsInfinity(vel.Y))
			{
				TShock.Log.ConsoleInfo("Bouncer / OnPlayerUpdate 强制将{0}踢出(试图将速度设置为无限)", args.Player.Name);
				args.Player.Kick("检测到试图将速度设置为无限.", true, true);
				args.Handled = true;
				return;
			}

			if (Single.IsNaN(vel.X) || Single.IsNaN(vel.Y))
			{
				TShock.Log.ConsoleInfo("Bouncer / OnPlayerUpdate 强制将{0}踢出(试图将速度设置为 NaN)", args.Player.Name);
				args.Player.Kick("检测到试图将速度设置为 NaN.", true, true);
				args.Handled = true;
				return;
			}

			if (vel.X > 50000 || vel.Y > 50000 || vel.X < -50000 || vel.Y < -50000)
			{
				TShock.Log.ConsoleInfo("Bouncer / OnPlayerUpdate 强制将{0}踢出(试图设置速度 +/-50000)", args.Player.Name);
				args.Player.Kick("试图设置速度 +/-50000.", true, true);
				args.Handled = true;
				return;
			}

			if (pos.X < 0 || pos.Y < 0 || pos.X >= Main.maxTilesX * 16 - 16 || pos.Y >= Main.maxTilesY * 16 - 16)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerUpdate 拒绝{0}(坐标检查)", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (item < 0 || item >= args.Player.TPlayer.inventory.Length)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerUpdate 拒绝{0}(库存长度)", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.LastNetPosition == Vector2.Zero)
			{
				TShock.Log.ConsoleInfo("Bouncer / OnPlayerUpdate * 将拒绝 * {0} (最后一个网络位置为0)", args.Player.Name);
				// args.Handled = true;
				// return;
			}

			if (!pos.Equals(args.Player.LastNetPosition))
			{
				float distance = Vector2.Distance(new Vector2(pos.X / 16f, pos.Y / 16f),
					new Vector2(args.Player.LastNetPosition.X / 16f, args.Player.LastNetPosition.Y / 16f));

				if (args.Player.IsBeingDisabled())
				{
					// If the player has moved outside the disabled zone...
					if (distance > TShock.Config.Settings.MaxRangeForDisabled)
					{
						// We need to tell them they were disabled and why, then revert the change.
						if (args.Player.IsDisabledForStackDetection)
						{
							args.Player.SendErrorMessage("你的物品堆叠超过栈上限.");
						}
						else if (args.Player.IsDisabledForBannedWearable)
						{
							args.Player.SendErrorMessage("你正在穿戴违禁装备.");
						}
						else if (args.Player.IsDisabledForSSC)
						{
							args.Player.SendErrorMessage("你需要 {0}login 才能加载你的数据.", TShock.Config.Settings.CommandSpecifier);
						}
						else if (TShock.Config.Settings.RequireLogin && !args.Player.IsLoggedIn)
						{
							args.Player.SendErrorMessage("需要一个账号! 请用 {0}register 或者 {0}login 注册登录你的账号!", TShock.Config.Settings.CommandSpecifier);
						}
						else if (args.Player.IsDisabledPendingTrashRemoval)
						{
							args.Player.SendErrorMessage("你需要重新加入，以确保您的垃圾桶被清除！");
						}

						// ??
						var lastTileX = args.Player.LastNetPosition.X;
						var lastTileY = args.Player.LastNetPosition.Y - 48;
						if (!args.Player.Teleport(lastTileX, lastTileY))
						{
							args.Player.Spawn(PlayerSpawnContext.RecallFromItem);
						}
						TShock.Log.ConsoleDebug("Bouncer / OnPlayerUpdate 拒绝{0} (??) ", args.Player.Name);
						args.Handled = true;
						return;
					}
					TShock.Log.ConsoleDebug("Bouncer / OnPlayerUpdate 拒绝{0} (在 ?? 下) ", args.Player.Name);
					args.Handled = true;
					return;
				}

				// Corpses don't move
				if (args.Player.Dead)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnPlayerUpdate拒绝 {0} (死亡后无法移动)", args.Player.Name);
					args.Handled = true;
					return;
				}
			}

			args.Player.LastNetPosition = pos;
			return;
		}

		/// <summary>Bouncer's TileEdit hook is used to revert malicious tile changes.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
		{
			// TODO: Add checks on the new edit actions. ReplaceTile, ReplaceWall, TryKillTile, Acutate, PokeLogicGate, SlopePoundTile
			EditAction action = args.Action;
			int tileX = args.X;
			int tileY = args.Y;
			short editData = args.EditData;
			EditType type = args.editDetail;

			// 'placeStyle' is a term used in Terraria land to determine which frame of a sprite is displayed when the sprite is placed. The placeStyle
			// determines the frameX and frameY offsets
			byte requestedPlaceStyle = args.Style;

			try
			{
				if (!TShock.Utils.TilePlacementValid(tileX, tileY))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝 {0} {1} {2} (物块放置有效) {0} {1} {2}", args.Player.Name, action, editData);
					args.Handled = true;
					return;
				}

				// I do not understand the ice tile check enough to be able to modify it, however I do know that it can be used to completely bypass region protection
				// This check ensures that build permission is always checked no matter what
				if (!args.Player.HasBuildPermission(tileX, tileY))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝玩家建造来自 {0} {1} {2}", args.Player.Name, action, editData);

					GetRollbackRectSize(tileX, tileY, out byte width, out byte length, out int offsetY);
					args.Player.SendTileRect((short)(tileX - width), (short)(tileY + offsetY), (byte)(width * 2), (byte)(length + 1));
					args.Handled = true;
					return;
				}

				if (editData < 0 ||
					((action == EditAction.PlaceTile || action == EditAction.ReplaceTile) && editData >= Main.maxTileSets) ||
					((action == EditAction.PlaceWall || action == EditAction.ReplaceWall) && editData >= Main.maxWallTypes))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝编辑数据越界{0}{1}{2}", args.Player.Name, action, editData);
					args.Player.SendTileSquareCentered(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (action == EditAction.KillTile && Main.tile[tileX, tileY].type == TileID.MagicalIceBlock)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 格外接受 {0}{1}{2}（冰块）", args.Player.Name, action, editData);
					args.Handled = false;
					return;
				}

				if (args.Player.Dead && TShock.Config.Settings.PreventDeadModification)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝{0} {1} {2} (数据管理系统) ", args.Player.Name, action, editData);
					args.Player.SendTileSquareCentered(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				Item selectedItem = args.Player.SelectedItem;
				int lastKilledProj = args.Player.LastKilledProjectile;
				ITile tile = Main.tile[tileX, tileY];

				if (action == EditAction.PlaceTile || action == EditAction.ReplaceTile)
				{
					if (TShock.TileBans.TileIsBanned(editData, args.Player))
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝{0} {1} {2} (tb)", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
						args.Player.SendErrorMessage("你没有权限发送替换这个图格.");
						args.Handled = true;
						return;
					}

					// This is the actual tile ID we expect the selected item to create. If the tile ID from the packet and the tile ID from the item do not match
					// we need to inspect further to determine if Terraria is sending funny information (which it does sometimes) or if someone is being malicious
					var actualTileToBeCreated = selectedItem.createTile;
					// This is the actual place style we expect the selected item to create. Same as above - if it differs from what the client tells us,
					// we need to do some inspection to check if its valid
					var actualItemPlaceStyle = selectedItem.placeStyle;

					// The client has requested to place a style that does not match their held item's actual place style
					if (requestedPlaceStyle != actualItemPlaceStyle)
					{
						var tplayer = args.Player.TPlayer;
						// Search for an extraneous tile corrector
						// If none found then it can't be a false positive so deny the action
						if (!PlaceStyleCorrectors.TryGetValue(actualTileToBeCreated, out PlaceStyleCorrector corrector))
						{
							TShock.Log.ConsoleError("Bouncer / OnTileEdit 拒绝 {0} {1} {2} (放置样式) 放置样式：{3} 预期样式：{4}",
								args.Player.Name, action, editData, requestedPlaceStyle, actualItemPlaceStyle);
							args.Player.SendTileSquareCentered(tileX, tileY, 1);
							args.Handled = true;
							return;
						}

						// See if the corrector's expected style matches
						var correctedPlaceStyle = corrector(tplayer, requestedPlaceStyle, actualItemPlaceStyle);
						if (requestedPlaceStyle != correctedPlaceStyle)
						{
							TShock.Log.ConsoleError("Bouncer / OnTileEdit 拒绝 {0} {1} {2} (放置样式) 放置样式：{3} 预期样式：{4}",
								args.Player.Name, action, editData, requestedPlaceStyle, correctedPlaceStyle);
							args.Player.SendTileSquareCentered(tileX, tileY, 1);
							args.Handled = true;
							return;
						}
					}
				}

				if (action == EditAction.KillTile && !Main.tileCut[tile.type] && !breakableTiles.Contains(tile.type))
				{
					// TPlayer.mount.Type 8 => Drill Containment Unit.

					// If the tile is an axe tile and they aren't selecting an axe, they're hacking.
					if (Main.tileAxe[tile.type] && ((args.Player.TPlayer.mount.Type != 8 && selectedItem.axe == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝{0} {1} {2} (斧)", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
					// If the tile is a hammer tile and they aren't selecting a hammer, they're hacking.
					else if (Main.tileHammer[tile.type] && ((args.Player.TPlayer.mount.Type != 8 && selectedItem.hammer == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit 拒绝 {0} {1} {2} (锤子)", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
					// If the tile is a pickaxe tile and they aren't selecting a pickaxe, they're hacking.
					// Item frames can be modified without pickaxe tile.
					// also add an exception for snake coils, they can be removed when the player places a new one or after x amount of time
					else if (tile.type != TileID.ItemFrame && tile.type != TileID.MysticSnakeRope
						&& !Main.tileAxe[tile.type] && !Main.tileHammer[tile.type] && tile.wall == 0 && args.Player.TPlayer.mount.Type != 8 && selectedItem.pick == 0 && selectedItem.type != ItemID.GravediggerShovel && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0)
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from (pick) {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.KillWall)
				{
					// If they aren't selecting a hammer, they could be hacking.
					if (selectedItem.hammer == 0 && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0 && selectedItem.createWall == 0)
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from (hammer2) {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.PlaceTile && (projectileCreatesTile.ContainsKey(lastKilledProj) && editData == projectileCreatesTile[lastKilledProj]))
				{
					args.Player.LastKilledProjectile = 0;
				}
				else if (CoilTileIds.Contains(editData))
				{
					// Handle placement if the user is placing rope that comes from a ropecoil,
					// but have not created the ropecoil projectile recently or the projectile was not at the correct coordinate, or the tile that the projectile places does not match the rope it is suposed to place
					// projectile should be the same X coordinate as all tile places (Note by @Olink)
					if (ropeCoilPlacements.ContainsKey(selectedItem.netID) &&
						!args.Player.RecentlyCreatedProjectiles.Any(p => GetDataHandlers.projectileCreatesTile.ContainsKey(p.Type) && GetDataHandlers.projectileCreatesTile[p.Type] == editData &&
						!p.Killed && Math.Abs((int)(Main.projectile[p.Index].position.X / 16f) - tileX) <= Math.Abs(Main.projectile[p.Index].velocity.X)))
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from (inconceivable rope coil) {0} {1} {2} selectedItem:{3} itemCreateTile:{4}", args.Player.Name, action, editData, selectedItem.netID, selectedItem.createTile);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.PlaceTile || action == EditAction.ReplaceTile || action == EditAction.PlaceWall || action == EditAction.ReplaceWall)
				{
					if ((action == EditAction.PlaceTile && TShock.Config.Settings.PreventInvalidPlaceStyle) &&
						requestedPlaceStyle > GetMaxPlaceStyle(editData))
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from (ms1) {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
						args.Handled = true;
						return;
					}

					/// Handle placement action if the player is using an Ice Rod but not placing the iceblock.
					if (selectedItem.netID == ItemID.IceRod && editData != TileID.MagicalIceBlock)
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from using ice rod but not placing ice block {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
						args.Handled = true;
					}
					/// If they aren't selecting the item which creates the tile, they're hacking.
					if ((action == EditAction.PlaceTile || action == EditAction.ReplaceTile) && editData != selectedItem.createTile)
					{
						/// These would get caught up in the below check because Terraria does not set their createTile field.
						if (selectedItem.netID != ItemID.IceRod && selectedItem.netID != ItemID.DirtBomb && selectedItem.netID != ItemID.StickyBomb)
						{
							TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from tile placement not matching selected item createTile {0} {1} {2} selectedItemID:{3} createTile:{4}", args.Player.Name, action, editData, selectedItem.netID, selectedItem.createTile);
							args.Player.SendTileSquareCentered(tileX, tileY, 4);
							args.Handled = true;
							return;
						}
					}
					/// If they aren't selecting the item which creates the wall, they're hacking.
					if ((action == EditAction.PlaceWall || action == EditAction.ReplaceWall) && editData != selectedItem.createWall)
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from wall placement not matching selected item createWall {0} {1} {2} selectedItemID:{3} createWall:{4}", args.Player.Name, action, editData, selectedItem.netID, selectedItem.createWall);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
						args.Handled = true;
						return;
					}
					if (action == EditAction.PlaceTile && (editData == TileID.Containers || editData == TileID.Containers2))
					{
						if (TShock.Utils.HasWorldReachedMaxChests())
						{
							TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from (chestcap) {0} {1} {2}", args.Player.Name, action, editData);
							args.Player.SendErrorMessage("已经达到了世界箱子的极限——无法再放更多了.");
							args.Player.SendTileSquareCentered(tileX, tileY, 3);
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
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from place wire from {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
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
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from wire cutter from {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				else if (action == EditAction.PlaceActuator)
				{
					// If they aren't selecting the actuator and don't have the Presserator equipped, they're hacking.
					if (selectedItem.type != ItemID.Actuator && !args.Player.TPlayer.autoActuator)
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from actuator/presserator from {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
				}
				if (TShock.Config.Settings.AllowCutTilesAndBreakables && Main.tileCut[tile.type])
				{
					if (action == EditAction.KillWall || action == EditAction.ReplaceWall)
					{
						TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from sts allow cut from {0} {1} {2}", args.Player.Name, action, editData);
						args.Player.SendTileSquareCentered(tileX, tileY, 1);
						args.Handled = true;
						return;
					}
					args.Handled = false;
					return;
				}

				if (args.Player.IsBeingDisabled())
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from disable from {0} {1} {2}", args.Player.Name, action, editData);
					args.Player.SendTileSquareCentered(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (!args.Player.HasModifiedIceSuccessfully(tileX, tileY, editData, action)
					&& !args.Player.HasBuildPermission(tileX, tileY))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from ice/build from {0} {1} {2}", args.Player.Name, action, editData);

					GetRollbackRectSize(tileX, tileY, out byte width, out byte length, out int offsetY);
					args.Player.SendTileRect((short)(tileX - width), (short)(tileY + offsetY), (byte)(width * 2), (byte)(length + 1));
					args.Handled = true;
					return;
				}

				//make sure it isnt a snake coil related edit so it doesnt spam debug logs with range check failures
				if (((action == EditAction.PlaceTile && editData != TileID.MysticSnakeRope) || (action == EditAction.KillTile && tile.type != TileID.MysticSnakeRope)) && !args.Player.IsInRange(tileX, tileY))
				{
					if (action == EditAction.PlaceTile && (editData == TileID.Rope || editData == TileID.SilkRope || editData == TileID.VineRope || editData == TileID.WebRope || editData == TileID.MysticSnakeRope))
					{
						args.Handled = false;
						return;
					}

					if (action == EditAction.KillTile || action == EditAction.KillWall && ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0)
					{
						args.Handled = false;
						return;
					}

					// Dirt bomb makes dirt everywhere
					if ((action == EditAction.PlaceTile || action == EditAction.SlopeTile) && editData == TileID.Dirt && args.Player.RecentFuse > 0)
					{
						args.Handled = false;
						return;
					}

					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from explosives/fuses from {0} {1} {2}", args.Player.Name, action, editData);
					args.Player.SendTileSquareCentered(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				if (args.Player.TileKillThreshold >= TShock.Config.Settings.TileKillThreshold)
				{
					if (TShock.Config.Settings.KickOnTileKillThresholdBroken)
					{
						args.Player.Kick(string.Format("物块挖掘阈值达到 {0}.", TShock.Config.Settings.TileKillThreshold));
					}
					else
					{
						args.Player.Disable("已达到物块挖掘阈值.", DisableFlags.WriteToLogAndConsole);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
					}
					
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from tile kill threshold from {0}, (value: {1})", args.Player.Name, args.Player.TileKillThreshold);
					TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用的物块挖掘阈值，这样我们就可以改进它了！");
					args.Handled = true;
					return;
				}

				if (args.Player.TilePlaceThreshold >= TShock.Config.Settings.TilePlaceThreshold)
				{
					if (TShock.Config.Settings.KickOnTilePlaceThresholdBroken)
					{
						args.Player.Kick(string.Format("物块放置阈值达到 {0}.", TShock.Config.Settings.TilePlaceThreshold));
					}
					else
					{
						args.Player.Disable("已达到物块放置阈值.", DisableFlags.WriteToLogAndConsole);
						args.Player.SendTileSquareCentered(tileX, tileY, 4);
					}
					
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from tile place threshold from {0}, (value: {1})", args.Player.Name, args.Player.TilePlaceThreshold);
					TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用这个阈值，这样我们就可以改进它了！");
					args.Handled = true;
					return;
				}

				if (args.Player.IsBouncerThrottled())
				{
					TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from throttled from {0} {1} {2}", args.Player.Name, action, editData);
					args.Player.SendTileSquareCentered(tileX, tileY, 4);
					args.Handled = true;
					return;
				}

				//snake coil can allow massive amounts of tile edits so it gets an exception
				if (!((action == EditAction.PlaceTile && editData == TileID.MysticSnakeRope) || (action == EditAction.KillTile && tile.type == TileID.MysticSnakeRope)))
				{
					if ((action == EditAction.PlaceTile || action == EditAction.ReplaceTile || action == EditAction.PlaceWall || action == EditAction.ReplaceWall) && !args.Player.HasPermission(Permissions.ignoreplacetiledetection))
					{
						args.Player.TilePlaceThreshold++;
						var coords = new Vector2(tileX, tileY);
						lock (args.Player.TilesCreated)
							if (!args.Player.TilesCreated.ContainsKey(coords))
								args.Player.TilesCreated.Add(coords, Main.tile[tileX, tileY]);
					}

					if ((action == EditAction.KillTile || action == EditAction.KillTileNoItem || action == EditAction.ReplaceTile || action == EditAction.KillWall || action == EditAction.ReplaceWall) && Main.tileSolid[Main.tile[tileX, tileY].type] &&
						!args.Player.HasPermission(Permissions.ignorekilltiledetection))
					{
						args.Player.TileKillThreshold++;
						var coords = new Vector2(tileX, tileY);
						lock (args.Player.TilesDestroyed)
							if (!args.Player.TilesDestroyed.ContainsKey(coords))
								args.Player.TilesDestroyed.Add(coords, Main.tile[tileX, tileY]);
					}
				}
				args.Handled = false;
				return;
			}
			catch
			{
				TShock.Log.ConsoleDebug("Bouncer / OnTileEdit rejected from weird confusing flow control from {0}", args.Player.Name);
				TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用这个阈值，这样我们就可以改进它了！");
				args.Player.SendTileSquareCentered(tileX, tileY, 4);
				args.Handled = true;
				return;
			}
		}

		/// <summary>
		/// Gets the size of the rectangle required to rollback all tiles impacted by a single tile.
		/// Eg, rolling back the destruction of a tile that had a Safe on top would require rolling back the safe as well as the
		/// tile that was destroyed
		/// </summary>
		/// <param name="tileX">X position of the initial tile</param>
		/// <param name="tileY">Y position of the initial tile</param>
		/// <param name="width">The calculated width of the rectangle</param>
		/// <param name="length">The calculated length of the rectangle</param>
		/// <param name="offsetY">The Y offset from the initial tile Y that the rectangle should begin at</param>
		private void GetRollbackRectSize(int tileX, int tileY, out byte width, out byte length, out int offsetY)
		{
			CheckForTileObjectsAbove(out byte topWidth, out byte topLength, out offsetY);
			CheckForTileObjectsBelow(out byte botWidth, out byte botLength);

			// If no tile object exists around the given tile, width will be 1. Else the width of the largest tile object will be used
			width = Math.Max((byte)1, Math.Max(topWidth, botWidth));
			// If no tile object exists around the given tile, length will be 1. Else the sum of all tile object lengths will be used
			length = Math.Max((byte)1, (byte)(topLength + botLength));

			// Checks for the presence of tile objects above the tile being checked
			void CheckForTileObjectsAbove(out byte objWidth, out byte objLength, out int yOffset)
			{
				objWidth = 0;
				objLength = 0;
				yOffset = 0;

				if (tileY <= 0)
				{
					return;
				}

				ITile above = Main.tile[tileX, tileY - 1];
				if (above.type < TileObjectData._data.Count && TileObjectData._data[above.type] != null)
				{
					TileObjectData data = TileObjectData._data[above.type];
					objWidth = (byte)data.Width;
					objLength = (byte)data.Height;
					yOffset = -data.Height; //y offset is the negative of the height of the tile object
				}
			}

			//Checks for the presence of tile objects below the tile being checked
			void CheckForTileObjectsBelow(out byte objWidth, out byte objLength)
			{
				objWidth = 0;
				objLength = 0;

				if (tileY == Main.maxTilesY)
				{
					return;
				}

				ITile below = Main.tile[tileX, tileY + 1];
				if (below.type < TileObjectData._data.Count && TileObjectData._data[below.type] != null)
				{
					TileObjectData data = TileObjectData._data[below.type];
					objWidth = (byte)data.Width;
					objLength = (byte)data.Height;
				}
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
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from attempt crash from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			// make sure the prefix is a legit value
			// Note: Not checking if prefix is less than 1 because if it is, this check
			// will break item pickups on the client.
			if (prefix > PrefixID.Count)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from prefix check from {0}", args.Player.Name);

				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			//Item removed, let client do this to prevent item duplication
			// client side (but only if it passed the range check) (i.e., return false)
			if (type == 0)
			{
				if (!args.Player.IsInRange((int)(Main.item[id].position.X / 16f), (int)(Main.item[id].position.Y / 16f)))
				{
					// Causes item duplications. Will be re added if necessary
					//args.Player.SendData(PacketTypes.ItemDrop, "", id);
					TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from dupe range check from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}

				args.Handled = false;
				return;
			}

			if (!args.Player.IsInRange((int)(pos.X / 16f), (int)(pos.Y / 16f)))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from range check from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			// stop the client from changing the item type of a drop but
			// only if the client isn't picking up the item
			if (Main.item[id].active && Main.item[id].netID != type)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from item drop/pickup check from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			Item item = new Item();
			item.netDefaults(type);
			if ((stacks > item.maxStack || stacks <= 0) || (TShock.ItemBans.DataModel.ItemIsBanned(EnglishLanguage.GetItemNameById(item.type), args.Player) && !args.Player.HasPermission(Permissions.allowdroppingbanneditems)))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from drop item ban check / max stack check / min stack check from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
			}

			// TODO: Remove item ban part of this check
			if ((Main.ServerSideCharacter) && (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - args.Player.LoginMS < TShock.ServerSideCharacterConfig.Settings.LogonDiscardThreshold))
			{
				//Player is probably trying to sneak items onto the server in their hands!!!
				TShock.Log.ConsoleInfo("玩家 {0} 试图将 {1} 带入服务器!", args.Player.Name, item.Name);
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from sneaky from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;

			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnItemDrop rejected from disabled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				args.Handled = true;
				return;
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
			float[] ai = args.Ai;

			if (index > Main.maxProjectiles)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from above projectile limit from {0}", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (TShock.ProjectileBans.ProjectileIsBanned(type, args.Player))
			{
				args.Player.Disable(String.Format("玩家没有权限生成射弹 {0}.", type), DisableFlags.WriteToLogAndConsole);
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from permission check from {0} {1}", args.Player.Name, type);
				args.Player.SendErrorMessage("你没有权限生成这个射弹.");
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (damage > TShock.Config.Settings.MaxProjDamage && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				args.Player.Disable(String.Format("Projectile damage is higher than {0}.", TShock.Config.Settings.MaxProjDamage), DisableFlags.WriteToLogAndConsole);
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from projectile damage limit from {0} {1}/{2}", args.Player.Name, damage, TShock.Config.Settings.MaxProjDamage);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from disabled from {0}", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			/// If the projectile is a directional projectile, check if the player is holding their respected item to validate the projectile creation.
			if (directionalProjectiles.ContainsKey(type))
			{
				if (directionalProjectiles[type] == args.Player.TPlayer.HeldItem.type)
				{
					args.Handled = false;
					return;
				}
			}

			/// If the created projectile is a golf club, check if the player is holding one of the golf club items to validate the projectile creation.
			if (type == ProjectileID.GolfClubHelper && Handlers.LandGolfBallInCupHandler.GolfClubItemIDs.Contains(args.Player.TPlayer.HeldItem.type))
			{
				args.Handled = false;
				return;
			}

			/// If the created projectile is a golf ball and the player is not holding a golf club item and neither a golf ball item and neither they have had a golf club projectile created recently.
			if (Handlers.LandGolfBallInCupHandler.GolfBallProjectileIDs.Contains(type) &&
				!Handlers.LandGolfBallInCupHandler.GolfClubItemIDs.Contains(args.Player.TPlayer.HeldItem.type) &&
				!Handlers.LandGolfBallInCupHandler.GolfBallItemIDs.Contains(args.Player.TPlayer.HeldItem.type) &&
				!args.Player.RecentlyCreatedProjectiles.Any(p => p.Type == ProjectileID.GolfClubHelper))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile please report to tshock about this! normally this is a reject from {0} {1} (golf)", args.Player.Name, type);
			}

			// Main.projHostile contains projectiles that can harm players
			// without PvP enabled and belong to enemy mobs, so they shouldn't be
			// possible for players to create. (Source: Ijwu, QuiCM)
			if (Main.projHostile[type])
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from hostile projectile from {0}", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			// Tombstones should never be permitted by players
			// This check means like, invalid or hacked tombstones (sent from hacked clients)
			// Death does not create a tombstone projectile by default
			if (type == ProjectileID.Tombstone)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from tombstones from {0}", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (!TShock.Config.Settings.IgnoreProjUpdate && !args.Player.HasPermission(Permissions.ignoreprojectiledetection))
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
					TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from weird check from {0} {1}", args.Player.Name, type);
					TShock.Log.Debug("某些射弹在作弊检测中被忽略了.");
				}
				else
				{
					TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile please report to tshock about this! normally this is a reject from {0} {1}", args.Player.Name, type);
					// args.Player.Disable(String.Format("Does not have projectile permission to update projectile. ({0})", type), DisableFlags.WriteToLogAndConsole);
					// args.Player.RemoveProjectile(ident, owner);
				}
				// args.Handled = false;
				// return;
			}

			if (args.Player.ProjectileThreshold >= TShock.Config.Settings.ProjectileThreshold)
			{
				if (TShock.Config.Settings.KickOnProjectileThresholdBroken)
				{
					args.Player.Kick(string.Format("Projectile update threshold exceeded {0}.", TShock.Config.Settings.ProjectileThreshold));
				}
				else
				{
					args.Player.Disable("Reached projectile update threshold.", DisableFlags.WriteToLogAndConsole);
					args.Player.RemoveProjectile(ident, owner);
				}
				
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from projectile update threshold from {0} {1}/{2}", args.Player.Name, args.Player.ProjectileThreshold, TShock.Config.Settings.ProjectileThreshold);
				TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用这个阈值，这样我们就可以改进它了！");
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from bouncer throttle from {0}", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (
				(Projectile_MaxValuesAI.ContainsKey(type) &&
					(Projectile_MaxValuesAI[type] < ai[0] || Projectile_MinValuesAI[type] > ai[0])) ||
				(Projectile_MaxValuesAI2.ContainsKey(type) &&
					(Projectile_MaxValuesAI2[type] < ai[1] || Projectile_MinValuesAI2[type] > ai[1]))
			)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from bouncer modified AI from {0}.", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			/*
			 * ai - Arguments that Projectile.AI uses for easier projectile control.
			 *	ai[0] - Distance from player (Doesn't affect the result very much)
			 *	ai[1] - The identifier of the object that will fly.
			 *
			 * FinalFractalHelper._fractalProfiles - A list of items that must be used in Zenith. (And also their colors)
			 *	If you add an item to this collection, it will also fly in the Zenith. (not active from server)
			*/
			if (TShock.Config.Settings.DisableModifiedZenith && type == ProjectileID.FinalFractal && (ai[0] < -100 || ai[0] > 101) && !Terraria.Graphics.FinalFractalHelper._fractalProfiles.ContainsKey((int)ai[1]))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNewProjectile rejected from bouncer modified Zenith projectile from {0}.", args.Player.Name);
				args.Player.RemoveProjectile(ident, owner);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.ignoreprojectiledetection))
			{
				if (type == ProjectileID.CrystalShard && TShock.Config.Settings.ProjIgnoreShrapnel) // Ignore crystal shards
				{
					TShock.Log.Debug("忽略每个配置中的子弹...");
				}
				else if (!Main.projectile[index].active)
				{
					args.Player.ProjectileThreshold++; // Creating new projectile
				}
			}

			if ((type == ProjectileID.Bomb
				|| type == ProjectileID.Dynamite
				|| type == ProjectileID.StickyBomb
				|| type == ProjectileID.StickyDynamite
				|| type == ProjectileID.ScarabBomb
				|| type == ProjectileID.DirtBomb))
			{
				//  Denotes that the player has recently set a fuse - used for cheat detection.
				args.Player.RecentFuse = 10;
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

			if (damage >= TShock.Config.Settings.MaxDamage && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				if (TShock.Config.Settings.KickOnDamageThresholdBroken)
				{
					args.Player.Kick(string.Format("超过对NPC伤害阈值 {0}.", TShock.Config.Settings.MaxDamage));
				}
				else
				{
					args.Player.Disable(String.Format("超过对NPC伤害阈值 {0}.", TShock.Config.Settings.MaxDamage), DisableFlags.WriteToLogAndConsole);
					args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				}
				
				TShock.Log.ConsoleDebug("Bouncer / OnNPCStrike rejected from damage threshold from {0} {1}/{2}", args.Player.Name, damage, TShock.Config.Settings.MaxDamage);
				TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用这个阈值，这样我们就可以改进它了！");
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNPCStrike rejected from disabled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (TShock.Config.Settings.RangeChecks &&
				!args.Player.IsInRange((int)(Main.npc[id].position.X / 16f), (int)(Main.npc[id].position.Y / 16f), 128))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNPCStrike rejected from range checks from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNPCStrike rejected from bouncer throttle from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
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
				TShock.Log.ConsoleDebug("Bouncer / OnProjectileKill rejected from negative projectile index from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnProjectileKill rejected from disabled from {0}", args.Player.Name);
				args.Player.RemoveProjectile(args.ProjectileIdentity, args.ProjectileOwner);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnProjectileKill rejected from bouncer throttle from {0}", args.Player.Name);
				args.Player.RemoveProjectile(args.ProjectileIdentity, args.ProjectileOwner);
				args.Handled = true;
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
				TShock.Log.ConsoleDebug("Bouncer / OnChestItemChange rejected from chest mismatch from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnChestItemChange rejected from disable from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.ChestItem, "", id, slot);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasBuildPermission(Main.chest[id].x, Main.chest[id].y) && TShock.Config.Settings.RegionProtectChests)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnChestItemChange rejected from region protection? from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(Main.chest[id].x, Main.chest[id].y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnChestItemChange rejected from range check from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
		}

		/// <summary>The Bouncer handler for when chests are opened.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs args)
		{
			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnChestOpen rejected from disabled from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnChestOpen rejected from range check from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasBuildPermission(args.X, args.Y) && TShock.Config.Settings.RegionProtectChests)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnChestOpen rejected from region check from {0}", args.Player.Name);
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
			short style = args.Style;

			if (!TShock.Utils.TilePlacementValid(tileX, tileY) || (args.Player.Dead && TShock.Config.Settings.PreventDeadModification))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceChest rejected from invalid check from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceChest rejected from disabled from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (args.Player.SelectedItem.placeStyle != style)
			{
				TShock.Log.ConsoleError(string.Format("Bouncer / OnPlaceChest / rejected from invalid place style from {0}", args.Player.Name));
				args.Player.SendTileSquareCentered(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (flag != 0 && flag != 4 // if no container or container2 placement
				&& Main.tile[tileX, tileY].type != TileID.Containers
				&& Main.tile[tileX, tileY].type != TileID.Dressers
				&& Main.tile[tileX, tileY].type != TileID.Containers2
				&& (!TShock.Utils.HasWorldReachedMaxChests() && Main.tile[tileX, tileY].type != TileID.Dirt)) //Chest
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceChest rejected from weird check from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (flag == 2) //place dresser
			{
				if ((TShock.Utils.TilePlacementValid(tileX, tileY + 1) && Main.tile[tileX, tileY + 1].type == TileID.Teleporter) ||
					(TShock.Utils.TilePlacementValid(tileX + 1, tileY + 1) && Main.tile[tileX + 1, tileY + 1].type == TileID.Teleporter))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnPlaceChest rejected from weird placement check from {0}", args.Player.Name);
					//Prevent a dresser from being placed on a teleporter, as this can cause client and server crashes.
					args.Player.SendTileSquareCentered(tileX, tileY, 3);
					args.Handled = true;
					return;
				}
			}

			if (!args.Player.HasBuildPermission(tileX, tileY))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceChest rejected from invalid permission from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 3);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(tileX, tileY))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceChest rejected from range check from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 3);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles PlayerZone events for preventing spawning NPC maliciously.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlayerZone(object sender, GetDataHandlers.PlayerZoneEventArgs args)
		{
			if (args.Zone2[1] || args.Zone2[2] || args.Zone2[3] || args.Zone2[4])
			{
				bool hasSolarTower = false;
				bool hasVortexTower = false;
				bool hasNebulaTower = false;
				bool hasStardustTower = false;

				foreach (var npc in Main.npc)
				{
					if (npc.netID == NPCID.LunarTowerSolar)
						hasSolarTower = true;
					else if (npc.netID == NPCID.LunarTowerVortex)
						hasVortexTower = true;
					else if (npc.netID == NPCID.LunarTowerNebula)
						hasNebulaTower = true;
					else if (npc.netID == NPCID.LunarTowerStardust)
						hasStardustTower = true;
				}

				if ((args.Zone2[1] && !hasSolarTower)
					|| (args.Zone2[2] && !hasVortexTower)
					|| (args.Zone2[3] && !hasNebulaTower)
					|| (args.Zone2[4] && !hasStardustTower)
					)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnPlayerZone rejected from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}
			}
		}

		/// <summary>Handles basic animation throttling for disabled players.</summary>
		/// <param name="sender">sender</param>
		/// <param name="args">args</param>
		internal void OnPlayerAnimation(object sender, GetDataHandlers.PlayerAnimationEventArgs args)
		{
			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerAnimation rejected from disabled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAnimation, "", args.Player.Index);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerAnimation rejected from throttle from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAnimation, "", args.Player.Index);
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
			LiquidType type = args.Type;

			if (!TShock.Utils.TilePlacementValid(tileX, tileY) || (args.Player.Dead && TShock.Config.Settings.PreventDeadModification))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected invalid check from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected disabled from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if (args.Player.TileLiquidThreshold >= TShock.Config.Settings.TileLiquidThreshold)
			{
				if (TShock.Config.Settings.KickOnTileLiquidThresholdBroken)
				{
					args.Player.Kick(string.Format("超过液体放置阈值 {0}.", TShock.Config.Settings.TileLiquidThreshold));
				}
				else
				{
					args.Player.Disable("超过液体放置阈值.", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
				}
				
				TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected from liquid threshold from {0} {1}/{2}", args.Player.Name, args.Player.TileLiquidThreshold, TShock.Config.Settings.TileLiquidThreshold);
				TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用这个阈值，这样我们就可以改进它了！");
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.ignoreliquidsetdetection))
			{
				args.Player.TileLiquidThreshold++;
			}

			bool wasThereABombNearby = false;
			lock (args.Player.RecentlyCreatedProjectiles)
			{
				IEnumerable<int> projectileTypesThatPerformThisOperation;
				if (amount > 0) //handle the projectiles that create fluid.
				{
					projectileTypesThatPerformThisOperation = projectileCreatesLiquid.Where(k => k.Value == type).Select(k => k.Key);
				}
				else //handle the scenario where we are removing liquid
				{
					projectileTypesThatPerformThisOperation = projectileCreatesLiquid.Where(k => k.Value == LiquidType.Removal).Select(k => k.Key);
				}

				var recentBombs = args.Player.RecentlyCreatedProjectiles.Where(p => projectileTypesThatPerformThisOperation.Contains(Main.projectile[p.Index].type));
				wasThereABombNearby = recentBombs.Any(r => Math.Abs(args.TileX - (Main.projectile[r.Index].position.X / 16.0f)) < TShock.Config.Settings.BombExplosionRadius
														&& Math.Abs(args.TileY - (Main.projectile[r.Index].position.Y / 16.0f)) < TShock.Config.Settings.BombExplosionRadius);
			}

			// Liquid anti-cheat
			// Arguably the banned buckets bit should be in the item bans system
			if (amount != 0)
			{
				int bucket = -1;
				int selectedItemType = args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].type;
				if (selectedItemType == ItemID.EmptyBucket)
				{
					bucket = 0;
				}
				else if (selectedItemType == ItemID.WaterBucket)
				{
					bucket = 1;
				}
				else if (selectedItemType == ItemID.LavaBucket)
				{
					bucket = 2;
				}
				else if (selectedItemType == ItemID.HoneyBucket)
				{
					bucket = 3;
				}
				else if (selectedItemType == ItemID.BottomlessBucket ||
					selectedItemType == ItemID.SuperAbsorbantSponge)
				{
					bucket = 4;
				}
				else if (selectedItemType == ItemID.LavaAbsorbantSponge)
				{
					bucket = 5;
				}
				else if (selectedItemType == ItemID.BottomlessLavaBucket)
				{
					bucket = 6;
				}

				if (!wasThereABombNearby && type == LiquidType.Lava && !(bucket == 2 || bucket == 0 || bucket == 5 || bucket == 6))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected bucket check 1 from {0}", args.Player.Name);
					args.Player.SendErrorMessage("你没有权限执行该操作.");
					args.Player.Disable("在没有提着熔岩桶的情况下撒出熔岩", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (!wasThereABombNearby && type == LiquidType.Lava && TShock.ItemBans.DataModel.ItemIsBanned("Lava Bucket", args.Player))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected lava bucket from {0}", args.Player.Name);
					args.Player.SendErrorMessage("你没有权限执行该操作.");
					args.Player.Disable("没有权限使用被禁止的岩浆桶", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (!wasThereABombNearby && type == LiquidType.Water && !(bucket == 1 || bucket == 0 || bucket == 4))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected bucket check 2 from {0}", args.Player.Name);
					args.Player.SendErrorMessage("你没有权限执行该操作.");
					args.Player.Disable("在没有提着水桶的情况下撒出水", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (!wasThereABombNearby && type == LiquidType.Water && TShock.ItemBans.DataModel.ItemIsBanned("Water Bucket", args.Player))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected bucket check 3 from {0}", args.Player.Name);
					args.Player.SendErrorMessage("你没有权限执行该操作.");
					args.Player.Disable("没有权限使用被禁止的水桶", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (!wasThereABombNearby && type == LiquidType.Honey && !(bucket == 3 || bucket == 0))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected bucket check 4 from {0}", args.Player.Name);
					args.Player.SendErrorMessage("你没有权限执行该操作.");
					args.Player.Disable("在没有提着蜂蜜桶的情况下撒出蜂蜜", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
					args.Handled = true;
					return;
				}

				if (!wasThereABombNearby && type == LiquidType.Honey && TShock.ItemBans.DataModel.ItemIsBanned("Honey Bucket", args.Player))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected bucket check 5 from {0}", args.Player.Name);
					args.Player.SendErrorMessage("你没有权限执行该操作.");
					args.Player.Disable("没有权限使用被禁止的蜂蜜桶", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquareCentered(tileX, tileY, 1);
					args.Handled = true;
					return;
				}
			}

			if (!args.Player.HasBuildPermission(tileX, tileY))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected build permission from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if (!wasThereABombNearby && !args.Player.IsInRange(tileX, tileY, 16))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected range checks from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 1);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnLiquidSet rejected throttle from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(tileX, tileY, 1);
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
			int type = args.Type;
			int time = args.Time;

			if (TShock.Players[id] == null)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected null check");
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected disabled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (id >= Main.maxPlayers)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected player cap from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (!TShock.Players[id].TPlayer.hostile || !Main.pvpBuff[type])
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected hostile/pvp from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(TShock.Players[id].TileX, TShock.Players[id].TileY, 50))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected range check from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected throttled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				args.Handled = true;
				return;
			}

			if (WhitelistBuffMaxTime[type] > 0 && time <= WhitelistBuffMaxTime[type])
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerBuff rejected buff time whitelists from {0}", args.Player.Name);
				args.Handled = false;
				return;
			}
		}

		/// <summary>Handles NPCAddBuff events.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnNPCAddBuff(object sender, GetDataHandlers.NPCAddBuffEventArgs args)
		{
			short id = args.ID;
			int type = args.Type;
			short time = args.Time;

			if (id >= Main.npc.Length)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNPCAddBuff rejected out of bounds NPC update from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			NPC npc = Main.npc[id];

			if (npc == null)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNPCAddBuff rejected null npc from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnNPCAddBuff rejected disabled from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			bool detectedNPCBuffTimeCheat = false;

			if (NPCAddBuffTimeMax.ContainsKey(type))
			{
				if (time > NPCAddBuffTimeMax[type])
				{
					detectedNPCBuffTimeCheat = true;
				}

				if (npc.townNPC && npc.netID != NPCID.Guide && npc.netID != NPCID.Clothier)
				{
					if (type != BuffID.Lovestruck && type != BuffID.Stinky && type != BuffID.DryadsWard &&
						type != BuffID.Wet && type != BuffID.Slimed && type != BuffID.GelBalloonBuff && type != BuffID.Frostburn2)
					{
						detectedNPCBuffTimeCheat = true;
					}
				}
			}
			else
			{
				detectedNPCBuffTimeCheat = true;
			}

			// if (detectedNPCBuffTimeCheat)
			// {
			// 	TShock.Log.ConsoleDebug("Bouncer / OnNPCAddBuff rejected abnormal buff ({0}) added to {1} ({2}) from {3}.", type, npc.TypeName, npc.netID, args.Player.Name);
			// 	args.Player.Kick($"异常对 {npc.TypeName} NPC 施加BUFF.", true);
			// 	args.Handled = true;
			// }
		}

		/// <summary>The Bouncer handler for when an NPC is rehomed.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnUpdateNPCHome(object sender, GetDataHandlers.NPCHomeChangeEventArgs args)
		{
			int id = args.ID;
			short x = args.X;
			short y = args.Y;

			if (!args.Player.HasBuildPermission(x, y))
			{
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				TShock.Log.ConsoleDebug("Bouncer / OnUpdateNPCHome rejected npc home build permission from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			// When kicking out an npc, x and y in args are 0, we shouldn't check range at this case
			if (args.HouseholdStatus != HouseholdStatus.Homeless && !args.Player.IsInRange(x, y))
			{
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				TShock.Log.ConsoleDebug("Bouncer / OnUpdateNPCHome rejected range checks from {0}", args.Player.Name);
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
				TShock.Log.ConsoleDebug("Bouncer / OnHealOtherPlayer rejected null checks");
				args.Handled = true;
				return;
			}

			// Why 0.2?
			// @bartico6: Because heal other player only happens when you are using the spectre armor with the hood,
			// and the healing you can do with that is 20% of your damage.
			if (amount >= TShock.Config.Settings.MaxDamage * 0.2 && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnHealOtherPlayer 0.2 check from {0}", args.Player.Name);
				args.Player.Disable("企图通过作弊治疗其他玩家!", DisableFlags.WriteToLogAndConsole);
				args.Handled = true;
				return;
			}

			if (args.Player.HealOtherThreshold >= TShock.Config.Settings.HealOtherThreshold)
			{
				if (TShock.Config.Settings.KickOnHealOtherThresholdBroken)
				{
					args.Player.Kick(string.Format("超过治疗其他玩家速率阈值 {0}.", TShock.Config.Settings.HealOtherThreshold));
				}
				else 
				{
					args.Player.Disable("超过治疗其他玩家速率阈值.", DisableFlags.WriteToLogAndConsole);
				}
				TShock.Log.ConsoleDebug("Bouncer / OnHealOtherPlayer rejected heal other threshold from {0} {1}/{2}", args.Player.Name, args.Player.HealOtherThreshold, TShock.Config.Settings.HealOtherThreshold);
				TShock.Log.ConsoleDebug("如果这个玩家不是挂逼，请向TShock报告他们被禁用这个阈值，这样我们就可以改进它了！");
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled() || args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnHealOtherPlayer rejected disabled/throttled from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			args.Player.HealOtherThreshold++;
			args.Handled = false;
			return;
		}

		/// <summary>
		/// A bouncer for checking NPC released by player
		/// </summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnReleaseNPC(object sender, GetDataHandlers.ReleaseNpcEventArgs args)
		{
			int x = args.X;
			int y = args.Y;
			short type = args.Type;
			byte style = args.Style;
			
			// if npc released outside allowed tile
			if (x >= Main.maxTilesX * 16 - 16 || x < 0 || y >= Main.maxTilesY * 16 - 16 || y < 0)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnReleaseNPC rejected out of bounds from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			// if player disabled
			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnReleaseNPC rejected npc release from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			// if released npc not from its item (from crafted packet)
			// e.g. using bunny item to release golden bunny 
			if (args.Player.TPlayer.lastVisualizedSelectedItem.makeNPC != type && args.Player.TPlayer.lastVisualizedSelectedItem.placeStyle != style)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnReleaseNPC released different critter from {0}", args.Player.Name);
				args.Player.Kick("释放的小动物不是来自其物品.", true);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnReleaseNPC rejected throttle from {0}", args.Player.Name);
				args.Handled = true;
				return;
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

			if (!TShock.Utils.TilePlacementValid(x, y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected valid placements from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (type < 0 || type >= Main.maxTileSets)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected out of bounds tile from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (x < 0 || x >= Main.maxTilesX)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected out of bounds tile x from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (y < 0 || y >= Main.maxTilesY)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected out of bounds tile y from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.SelectedItem.placeStyle != style)
			{
				TShock.Log.ConsoleError(string.Format("Bouncer / OnPlaceObject rejected object placement with invalid style from {0}", args.Player.Name));
				args.Handled = true;
				return;
			}

			//style 52 and 53 are used by ItemID.Fake_newchest1 and ItemID.Fake_newchest2
			//These two items cause localised lag and rendering issues
			if (type == TileID.FakeContainers && (style == 52 || style == 53))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected fake containers from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(x, y, 4);
				args.Handled = true;
				return;
			}

			// TODO: REMOVE. This does NOT look like Bouncer code.
			if (TShock.TileBans.TileIsBanned(type, args.Player))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected banned tiles from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(x, y, 1);
				args.Player.SendErrorMessage("你没有权限发送替换这个图格.");
				args.Handled = true;
				return;
			}

			if (args.Player.Dead && TShock.Config.Settings.PreventDeadModification)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected dead people don't do things from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(x, y, 4);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected disabled from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(x, y, 4);
				args.Handled = true;
				return;
			}

			// This is necessary to check in order to prevent special tiles such as
			// queen bee larva, paintings etc that use this packet from being placed
			// without selecting the right item.
			if (type != args.Player.TPlayer.inventory[args.Player.TPlayer.selectedItem].createTile)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected awkward tile creation/selection from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(x, y, 4);
				args.Handled = true;
				return;
			}

			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData == null)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected null tile data from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			x -= tileData.Origin.X;
			y -= tileData.Origin.Y;

			for (int i = x; i < x + tileData.Width; i++)
			{
				for (int j = y; j < y + tileData.Height; j++)
				{
					if (!args.Player.HasModifiedIceSuccessfully(i, j, type, EditAction.PlaceTile)
						&& !args.Player.HasBuildPermission(i, j))
					{
						TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected mad loop from {0}", args.Player.Name);
						args.Player.SendTileSquareCentered(i, j, 4);
						args.Handled = true;
						return;
					}
				}
			}

			// Ignore rope placement range
			if ((type != TileID.Rope
					|| type != TileID.SilkRope
					|| type != TileID.VineRope
					|| type != TileID.WebRope
					|| type != TileID.MysticSnakeRope)
					&& !args.Player.IsInRange(x, y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected range checks from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(x, y, 4);
				args.Handled = true;
				return;
			}

			if (args.Player.TilePlaceThreshold >= TShock.Config.Settings.TilePlaceThreshold)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceObject rejected tile place threshold from {0} {1}/{2}", args.Player.Name, args.Player.TilePlaceThreshold, TShock.Config.Settings.TilePlaceThreshold);
				args.Player.Disable("超过图格放置阈值.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendTileSquareCentered(x, y, 4);
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

		/// <summary>Fired when a PlaceTileEntity occurs for basic anti-cheat on perms and range.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlaceTileEntity(object sender, GetDataHandlers.PlaceTileEntityEventArgs args)
		{
			if (!TShock.Utils.TilePlacementValid(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceTileEntity rejected tile placement valid from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceTileEntity rejected disabled from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasBuildPermission(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceTileEntity rejected permissions from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceTileEntity rejected range checks from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Fired when an item frame is placed for anti-cheat detection.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnPlaceItemFrame(object sender, GetDataHandlers.PlaceItemFrameEventArgs args)
		{
			if (!TShock.Utils.TilePlacementValid(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceItemFrame rejected tile placement valid from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceItemFrame rejected disabled from {0}", args.Player.Name);
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, NetworkText.Empty, args.ItemFrame.ID, 0, 1);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasBuildPermission(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceItemFrame rejected permissions from {0}", args.Player.Name);
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, NetworkText.Empty, args.ItemFrame.ID, 0, 1);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(args.X, args.Y))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlaceItemFrame rejected range checks from {0}", args.Player.Name);
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, NetworkText.Empty, args.ItemFrame.ID, 0, 1);
				args.Handled = true;
				return;
			}
		}

		internal void OnPlayerPortalTeleport(object sender, GetDataHandlers.TeleportThroughPortalEventArgs args)
		{
			//Packet 96 (player teleport through portal) has no validation on whether or not the player id provided
			//belongs to the player who sent the packet.
			if (args.Player.Index != args.TargetPlayerIndex)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerPortalTeleport rejected untargetable teleport from {0}", args.Player.Name);
				//If the player who sent the packet is not the player being teleported, cancel this packet
				args.Player.Disable("恶意端口尝试.", DisableFlags.WriteToLogAndConsole); //Todo: this message is not particularly clear - suggestions wanted
				args.Handled = true;
				return;
			}

			//Generic bounds checking, though I'm not sure if anyone would willingly hack themselves outside the map?
			if (args.NewPosition.X > Main.maxTilesX || args.NewPosition.X < 0
				|| args.NewPosition.Y > Main.maxTilesY || args.NewPosition.Y < 0)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerPortalTeleport rejected teleport out of bounds from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			//May as well reject teleport attempts if the player is being throttled
			if (args.Player.IsBeingDisabled() || args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerPortalTeleport rejected disabled/throttled from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Handles the anti-cheat components of gem lock toggles.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnGemLockToggle(object sender, GetDataHandlers.GemLockToggleEventArgs args)
		{
			if (args.X < 0 || args.Y < 0 || args.X >= Main.maxTilesX || args.Y >= Main.maxTilesY)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnGemLockToggle rejected boundaries check from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (!TShock.Utils.TilePlacementValid(args.X, args.Y) || (args.Player.Dead && TShock.Config.Settings.PreventDeadModification))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnGemLockToggle invalid placement/deadmod from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnGemLockToggle rejected disabled from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if (TShock.Config.Settings.RegionProtectGemLocks)
			{
				if (!args.Player.HasBuildPermission(args.X, args.Y))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnGemLockToggle rejected permissions check from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}
			}
		}

		/// <summary>Handles validation of of basic anti-cheat on mass wire operations.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnMassWireOperation(object sender, GetDataHandlers.MassWireOperationEventArgs args)
		{
			short startX = args.StartX;
			short startY = args.StartY;
			short endX = args.EndX;
			short endY = args.EndY;

			List<Point> points = Utils.Instance.GetMassWireOperationRange(
				new Point(startX, startY),
				new Point(endX, endY),
				args.Player.TPlayer.direction == 1);

			int x;
			int y;
			foreach (Point p in points)
			{
				/* Perform similar checks to TileKill
				 * The server-side nature of this packet removes the need to use SendTileSquare
				 * Range checks are currently ignored here as the items that send this seem to have infinite range */

				x = p.X;
				y = p.Y;

				if (!TShock.Utils.TilePlacementValid(x, y) || (args.Player.Dead && TShock.Config.Settings.PreventDeadModification))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnMassWireOperation rejected valid placement from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}

				if (args.Player.IsBeingDisabled())
				{
					TShock.Log.ConsoleDebug("Bouncer / OnMassWireOperation rejected disabled from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}

				if (!args.Player.HasBuildPermission(x, y))
				{
					TShock.Log.ConsoleDebug("Bouncer / OnMassWireOperation rejected build perms from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}
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
			PlayerDeathReason reason = args.PlayerDeathReason;

			if (id >= Main.maxPlayers || TShock.Players[id] == null)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected null check");
				args.Handled = true;
				return;
			}

			if (damage > TShock.Config.Settings.MaxDamage && !args.Player.HasPermission(Permissions.ignoredamagecap) && id != args.Player.Index)
			{
				if (TShock.Config.Settings.KickOnDamageThresholdBroken)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected damage threshold from {0} {1}/{2}", args.Player.Name, damage, TShock.Config.Settings.MaxDamage);
					args.Player.Kick(string.Format("超过玩家伤害阈值 {0}.", TShock.Config.Settings.MaxDamage));
					args.Handled = true;
					return;
				}
				else
				{
					TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected damage threshold2 from {0} {1}/{2}", args.Player.Name, damage, TShock.Config.Settings.MaxDamage);
					args.Player.Disable(String.Format("超过玩家伤害阈值 {0}.", TShock.Config.Settings.MaxDamage), DisableFlags.WriteToLogAndConsole);
				}
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (!TShock.Players[id].TPlayer.hostile && pvp && id != args.Player.Index)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected hostile from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected disabled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(TShock.Players[id].TileX, TShock.Players[id].TileY, 100))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected range checks from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected throttled from {0}", args.Player.Name);
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				args.Handled = true;
				return;
			}

			/*
			 * PlayerDeathReason does not initially contain any information, so all fields have values -1 or null. 
			 * We can use this to determine the real cause of death.
			 * 
			 * If the player was not specified, that is, the player index is -1, then it is definitely a custom cause, as you can only deal damage with a projectile or another player.
			 * This is how everything else works. If an NPC is specified, its value is not -1, which is a custom cause.
			 * 
			 * Checking whether this damage came from the player is necessary, because the damage from the player can come even when it is hit by a NPC
			*/
			if (TShock.Config.Settings.DisableCustomDeathMessages && id != args.Player.Index && 
				(reason._sourcePlayerIndex == -1 || reason._sourceNPCIndex != -1 || reason._sourceOtherIndex != -1 || reason._sourceCustomReason != null))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnPlayerDamage rejected custom death message from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
		}

		/// <summary>Bouncer's KillMe hook stops crash exploits from out of bounds values.</summary>
		/// <param name="sender">The object that triggered the event.</param>
		/// <param name="args">The packet arguments that the event has.</param>
		internal void OnKillMe(object sender, GetDataHandlers.KillMeEventArgs args)
		{
			short damage = args.Damage;
			short id = args.PlayerId;
			PlayerDeathReason playerDeathReason = args.PlayerDeathReason;

			if (damage > 42000) //Abnormal values have the potential to cause infinite loops in the server.
			{
				TShock.Log.ConsoleDebug("Bouncer / OnKillMe rejected high damage from {0} {1}", args.Player.Name, damage);
				args.Player.Kick("未能对多边形法线进行阴影处理.", true, true);
				TShock.Log.ConsoleError("企图死亡攻击：伤害为 {0}", damage);
				args.Handled = true;
				return;
			}

			if (id >= Main.maxPlayers)
			{
				TShock.Log.ConsoleDebug("Bouncer / OnKillMe rejected index check from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			// This was formerly marked as a crash check; does not actually crash on this specific packet.
			if (playerDeathReason != null)
			{
				if (playerDeathReason.GetDeathText(TShock.Players[id].Name).ToString().Length > 500)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnKillMe rejected bad length death text from {0}", args.Player.Name);
					TShock.Players[id].Kick("非正常死亡.", true);
					args.Handled = true;
					return;
				}
				if (TShock.Config.Settings.DisableCustomDeathMessages && playerDeathReason._sourceCustomReason != null)
				{
					TShock.Log.ConsoleDebug("Bouncer / OnKillMe rejected custom death message from {0}", args.Player.Name);
					args.Handled = true;
					return;
				}
			}
		}

		/// <summary>
		/// Called when the player fishes out an NPC.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal void OnFishOutNPC(object sender, GetDataHandlers.FishOutNPCEventArgs args)
		{
			/// Getting recent projectiles of the player and selecting the first that is a bobber.
			var projectile = args.Player.RecentlyCreatedProjectiles.FirstOrDefault(p => Main.projectile[p.Index].bobber);

			if (!FishingRodItemIDs.Contains(args.Player.SelectedItem.type))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFishOutNPC rejected for not using a fishing rod! - From {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
			if (projectile.Type == 0 || projectile.Killed) /// The bobber projectile is never killed when the NPC spawns. Type can only be 0 if no recent projectile is found that is named Bobber.
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFishOutNPC rejected for not finding active bobber projectile! - From {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
			if (!FishableNpcIDs.Contains(args.NpcID))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFishOutNPC rejected for the NPC not being on the fishable NPCs list! - From {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
			if (args.NpcID == NPCID.DukeFishron && !args.Player.HasPermission(Permissions.summonboss))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFishOutNPC rejected summon boss permissions from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}
			if (!args.Player.IsInRange(args.TileX, args.TileY, 55))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFishOutNPC rejected range checks from {0}", args.Player.Name);
				args.Handled = true;
			}
		}

		/// <summary>
		/// Called when a player is trying to place an item into a food plate.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		internal void OnFoodPlatterTryPlacing(object sender, GetDataHandlers.FoodPlatterTryPlacingEventArgs args)
		{
			if (!TShock.Utils.TilePlacementValid(args.TileX, args.TileY))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFoodPlatterTryPlacing rejected tile placement valid from {0}", args.Player.Name);
				args.Handled = true;
				return;
			}

			if ((args.Player.SelectedItem.type != args.ItemID && args.Player.ItemInHand.type != args.ItemID))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFoodPlatterTryPlacing rejected item not placed by hand from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(args.TileX, args.TileY, 1);
				args.Handled = true;
				return;
			}
			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFoodPlatterTryPlacing rejected disabled from {0}", args.Player.Name);
				Item item = new Item();
				item.netDefaults(args.ItemID);
				args.Player.GiveItemCheck(args.ItemID, item.Name, args.Stack, args.Prefix);
				args.Player.SendTileSquareCentered(args.TileX, args.TileY, 1);
				args.Handled = true;
				return;
			}

			if (!args.Player.HasBuildPermission(args.TileX, args.TileY))
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFoodPlatterTryPlacing rejected permissions from {0}", args.Player.Name);
				Item item = new Item();
				item.netDefaults(args.ItemID);
				args.Player.GiveItemCheck(args.ItemID, item.Name, args.Stack, args.Prefix);
				args.Player.SendTileSquareCentered(args.TileX, args.TileY, 1);
				args.Handled = true;
				return;
			}

			if (!args.Player.IsInRange(args.TileX, args.TileY, range: 13)) // To my knowledge, max legit tile reach with accessories.
			{
				TShock.Log.ConsoleDebug("Bouncer / OnFoodPlatterTryPlacing rejected range checks from {0}", args.Player.Name);
				args.Player.SendTileSquareCentered(args.TileX, args.TileY, 1);
				args.Handled = true;
				return;
			}
		}

		internal void OnSecondUpdate()
		{
			Task.Run(() =>
			{
				foreach (var player in TShock.Players)
				{
					if (player != null && player.TPlayer.whoAmI >= 0)
					{
						var threshold = DateTime.Now.AddSeconds(-5);
						lock (player.RecentlyCreatedProjectiles)
						{
							player.RecentlyCreatedProjectiles = player.RecentlyCreatedProjectiles.Where(s => s.CreatedAt > threshold).ToList();
						}
					}
				}
			});
		}

		/// <summary>
		/// Returns the max <see cref="Item.placeStyle"/> associated with the given <paramref name="tileID"/>. Or -1 if there's no association
		/// </summary>
		/// <param name="tileID">Tile ID to query for</param>
		/// <returns>The max <see cref="Item.placeStyle"/>, otherwise -1 if there's no association</returns>
		internal static int GetMaxPlaceStyle(int tileID)
		{
			int result;
			if (ExtraneousPlaceStyles.TryGetValue(tileID, out result)
				|| MaxPlaceStyles.TryGetValue(tileID, out result))
			{
				return result;
			}
			else
			{
				return -1;
			}
		}

		// These time values are references from Projectile.cs, at npc.AddBuff() calls.
		// Moved to Projectile.StatusNPC(int i).
		private static Dictionary<int, short> NPCAddBuffTimeMax = new Dictionary<int, short>()
		{
			{ BuffID.Poisoned, 3600 },              // BuffID: 20
			{ BuffID.OnFire, 1200 },                // BuffID: 24
			{ BuffID.Confused, short.MaxValue },    // BuffID: 31 Brain of Confusion Internal Item ID: 3223
			{ BuffID.CursedInferno, 420 },          // BuffID: 39
			{ BuffID.Frostburn, 900 },              // BuffID: 44
			{ BuffID.Ichor, 1200 },                 // BuffID: 69
			{ BuffID.Venom, 1800 },                 // BuffID: 70
			{ BuffID.Midas, 120 },                  // BuffID: 72
			{ BuffID.Wet, 1500 },                   // BuffID: 103
			{ BuffID.Lovestruck, 1800 },            // BuffID: 119
			{ BuffID.Stinky, 1800 },                // BuffID: 120
			{ BuffID.Slimed, 1500 },                // BuffID: 137
			{ BuffID.SoulDrain, 30 },               // BuffID: 151
			{ BuffID.ShadowFlame, 660 },            // BuffID: 153
			{ BuffID.DryadsWard, 120 },             // BuffID: 165
			{ BuffID.BoneJavelin, 900 },            // BuffID: 169
			{ BuffID.StardustMinionBleed, 900 },    // BuffID: 183
			{ BuffID.DryadsWardDebuff, 120 },       // BuffID: 186
			{ BuffID.Daybreak, 300 },               // BuffID: 189 Solar Eruption Item ID: 3473, Daybreak Item ID: 3543
			{ BuffID.BetsysCurse, 600 },            // BuffID: 203
			{ BuffID.Oiled, 540 },                  // BuffID: 204
			{ BuffID.BlandWhipEnemyDebuff, 240  },  // BuffID: 307
			{ BuffID.SwordWhipNPCDebuff, 240  },    // BuffID: 309
			{ BuffID.ScytheWhipEnemyDebuff, 240  }, // BuffID: 310
			{ BuffID.FlameWhipEnemyDebuff, 240  },  // BuffID: 313
			{ BuffID.ThornWhipNPCDebuff, 240  },    // BuffID: 315
			{ BuffID.RainbowWhipNPCDebuff, 240  },  // BuffID: 316
			{ BuffID.MaceWhipNPCDebuff, 240  },     // BuffID: 319
			{ BuffID.GelBalloonBuff, 1800  },       // BuffID: 320
			{ BuffID.OnFire3, 360 },                // BuffID: 323
			{ BuffID.Frostburn2, 900 },             // BuffID: 324
			{ BuffID.BoneWhipNPCDebuff, 240 },      // BuffID: 326
			{ BuffID.TentacleSpike, 540 }           // BuffID: 337
		};

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

		/// <summary>
		/// These projectiles have been added or modified with Terraria 1.4.
		/// They come from normal items, but to have the directional functionality, they must be projectiles.
		/// </summary>
		private static Dictionary<int, int> directionalProjectiles = new Dictionary<int, int>()
		{
			///Spears
			{ ProjectileID.DarkLance, ItemID.DarkLance},
			{ ProjectileID.Trident, ItemID.Trident},
			{ ProjectileID.Spear, ItemID.Spear},
			{ ProjectileID.MythrilHalberd, ItemID.MythrilHalberd},
			{ ProjectileID.AdamantiteGlaive, ItemID.AdamantiteGlaive},
			{ ProjectileID.CobaltNaginata, ItemID.CobaltNaginata},
			{ ProjectileID.Gungnir, ItemID.Gungnir},
			{ ProjectileID.MushroomSpear, ItemID.MushroomSpear},
			{ ProjectileID.TheRottedFork, ItemID.TheRottedFork},
			{ ProjectileID.PalladiumPike, ItemID.PalladiumPike},
			{ ProjectileID.OrichalcumHalberd, ItemID.OrichalcumHalberd},
			{ ProjectileID.TitaniumTrident, ItemID.TitaniumTrident},
			{ ProjectileID.ChlorophytePartisan, ItemID.ChlorophytePartisan},
			{ ProjectileID.NorthPoleWeapon, ItemID.NorthPole},
			{ ProjectileID.ObsidianSwordfish, ItemID.ObsidianSwordfish},
			{ ProjectileID.Swordfish, ItemID.Swordfish},
			{ ProjectileID.MonkStaffT2, ItemID.MonkStaffT2},
			{ ProjectileID.ThunderSpear, ItemID.ThunderSpear},
			{ ProjectileID.GladiusStab, ItemID.Gladius},
			/// ShortSwords
			{ ProjectileID.RulerStab, ItemID.Ruler },
			{ ProjectileID.CopperShortswordStab, ItemID.CopperShortsword },
			{ ProjectileID.TinShortswordStab, ItemID.TinShortsword },
			{ ProjectileID.IronShortswordStab, ItemID.IronShortsword },
			{ ProjectileID.LeadShortswordStab, ItemID.LeadShortsword },
			{ ProjectileID.SilverShortswordStab, ItemID.SilverShortsword },
			{ ProjectileID.TungstenShortswordStab, ItemID.TungstenShortsword },
			{ ProjectileID.GoldShortswordStab, ItemID.GoldShortsword },
			{ ProjectileID.PlatinumShortswordStab, ItemID.PlatinumShortsword }
		};

		private Dictionary<short, float> Projectile_MinValuesAI = new Dictionary<short, float> {
			{ 611, -1 },

			{ 950, 0 }
		};
		private Dictionary<short, float> Projectile_MaxValuesAI = new Dictionary<short, float> {
			{ 611, 1 },

			{ 950, 0 }
		};

		private Dictionary<short, float> Projectile_MinValuesAI2 = new Dictionary<short, float> {
			{ 405, 0f },
			{ 410, 0f },

			{ 424, 0.5f },
			{ 425, 0.5f },
			{ 426, 0.5f },

			{ 612, 0.4f },
			{ 953, 0.85f },

			{ 756, 0.5f },
			{ 522, 0 }
		};
		private Dictionary<short, float> Projectile_MaxValuesAI2 = new Dictionary<short, float> {
			{ 405, 1.2f },
			{ 410, 1.2f },

			{ 424, 0.8f },
			{ 425, 0.8f },
			{ 426, 0.8f },

			{ 612, 0.7f },
			{ 953, 2 },

			{ 756, 1 },
			{ 522, 40f }
		};
	}
}
