using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers.NetModules
{
	/// <summary>
	/// Provides handling for the Creative Power net module. Checks permissions on all creative powers
	/// </summary>
	public class CreativePowerHandler : INetModuleHandler
	{
		/// <summary>
		/// The power type being activated
		/// </summary>
		public CreativePowerTypes PowerType { get; set; }

		/// <summary>
		/// Reads the power type from the stream
		/// </summary>
		/// <param name="data"></param>
		public void Deserialize(MemoryStream data)
		{
			PowerType = (CreativePowerTypes)data.ReadInt16();
		}

		/// <summary>
		/// Determines if the player has permission to use the power type
		/// </summary>
		/// <param name="player"></param>
		/// <param name="rejectPacket"></param>
		public void HandlePacket(TSPlayer player, out bool rejectPacket)
		{
			if (!HasPermission(PowerType, player))
			{
				rejectPacket = true;
				return;
			}

			rejectPacket = false;
		}

		/// <summary>
		/// Determines if a player has permission to use a specific creative power
		/// </summary>
		/// <param name="powerType"></param>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool HasPermission(CreativePowerTypes powerType, TSPlayer player)
		{
			if (!PowerToPermissionMap.ContainsKey(powerType))
			{
				TShock.Log.ConsoleDebug("CreativePowerHandler received permission check request for unknown creative power");
				return false;
			}

			string permission = PowerToPermissionMap[powerType];

			//prevent being told about the spawnrate permission on join until relogic fixes
			if (!player.HasReceivedNPCPermissionError && powerType == CreativePowerTypes.SetSpawnRate)
			{
				player.HasReceivedNPCPermissionError = true;
				return false;
			}

			if (!player.HasPermission(permission))
			{
				player.SendErrorMessage("你没有权限使用{0}.", PermissionToDescriptionMap[permission]);
				return false;
			}

			return true;
		}


		/// <summary>
		/// Maps creative powers to permission nodes
		/// </summary>
		public static Dictionary<CreativePowerTypes, string> PowerToPermissionMap = new Dictionary<CreativePowerTypes, string>
		{
			{ CreativePowerTypes.FreezeTime,              Permissions.journey_timefreeze		},
			{ CreativePowerTypes.SetDawn,                 Permissions.journey_timeset			},
			{ CreativePowerTypes.SetNoon,                 Permissions.journey_timeset			},
			{ CreativePowerTypes.SetDusk,                 Permissions.journey_timeset			},
			{ CreativePowerTypes.SetMidnight,             Permissions.journey_timeset			},
			{ CreativePowerTypes.Godmode,                 Permissions.journey_godmode			},
			{ CreativePowerTypes.WindStrength,            Permissions.journey_windstrength		},
			{ CreativePowerTypes.RainStrength,            Permissions.journey_rainstrength		},
			{ CreativePowerTypes.TimeSpeed,               Permissions.journey_timespeed			},
			{ CreativePowerTypes.RainFreeze,              Permissions.journey_rainfreeze		},
			{ CreativePowerTypes.WindFreeze,              Permissions.journey_windfreeze		},
			{ CreativePowerTypes.IncreasePlacementRange,  Permissions.journey_placementrange	},
			{ CreativePowerTypes.WorldDifficulty,         Permissions.journey_setdifficulty		},
			{ CreativePowerTypes.BiomeSpreadFreeze,       Permissions.journey_biomespreadfreeze },
			{ CreativePowerTypes.SetSpawnRate,            Permissions.journey_setspawnrate		},
		};

		/// <summary>
		/// Maps journey mode permission nodes to descriptions of what the permission allows
		/// </summary>
		public static Dictionary<string, string> PermissionToDescriptionMap = new Dictionary<string, string>
		{
			{ Permissions.journey_timefreeze,			"冻结服务器时间"						},
			{ Permissions.journey_timeset,				"修改服务器时间"						},
			{ Permissions.journey_godmode,				"开启上帝模式"							},
			{ Permissions.journey_windstrength,         "修改服务器的风速"						},
			{ Permissions.journey_rainstrength,         "修改服务器的降雨"						},
			{ Permissions.journey_timespeed,			"修改时间流逝倍速"						},
			{ Permissions.journey_rainfreeze,			"冻结服务器降雨"						},
			{ Permissions.journey_windfreeze,			"冻结服务器风速"						},
			{ Permissions.journey_placementrange,       "修改角色的图格放置范围"				},
			{ Permissions.journey_setdifficulty,        "修改服务器的世界难度"					},
			{ Permissions.journey_biomespreadfreeze,    "冻结服务器的生物群系扩散"              },
			{ Permissions.journey_setspawnrate,         "修改服务器的NPC生成速率"				},
		};
	}
}
