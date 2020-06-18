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
				player.SendErrorMessage("You do not have permission to {0}.", PermissionToDescriptionMap[permission]);
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
			{ Permissions.journey_timefreeze,			"freeze the time of the server"						},
			{ Permissions.journey_timeset,				"modify the time of the server"						},
			{ Permissions.journey_godmode,				"toggle godmode"									},
			{ Permissions.journey_windstrength,			"modify the wind strength of the server"			},
			{ Permissions.journey_rainstrength,			"modify the rain strength of the server"			},
			{ Permissions.journey_timespeed,			"modify the time speed of the server"				},
			{ Permissions.journey_rainfreeze,			"freeze the rain strength of the server"			},
			{ Permissions.journey_windfreeze,			"freeze the wind strength of the server"			},
			{ Permissions.journey_placementrange,		"modify the tile placement range of your character" },
			{ Permissions.journey_setdifficulty,		"modify the world difficulty of the server"			},
			{ Permissions.journey_biomespreadfreeze,	"freeze the biome spread of the server"				},
			{ Permissions.journey_setspawnrate,			"modify the NPC spawn rate of the server"			},
		};
	}
}
