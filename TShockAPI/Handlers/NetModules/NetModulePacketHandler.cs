using System;
using System.Collections.Generic;
using Terraria;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers.NetModules
{
	/// <summary>
	/// Handles packet 82 - Load Net Module packets
	/// </summary>
	public class NetModulePacketHandler : IPacketHandler<LoadNetModuleEventArgs>
	{
		/// <summary>
		/// Maps net module types to handlers for the net module type. Add to or edit this dictionary to customise handling
		/// </summary>
		public static Dictionary<NetModuleType, Type> NetModulesToHandlersMap = new Dictionary<NetModuleType, Type>
		{
			{ NetModuleType.CreativePowers,               typeof(CreativePowerHandler)    },
			{ NetModuleType.CreativeUnlocksPlayerReport,  typeof(CreativeUnlocksHandler)  },
			{ NetModuleType.TeleportPylon,                typeof(PylonHandler)            },
			{ NetModuleType.Liquid,                       typeof(LiquidHandler)           },
			{ NetModuleType.Bestiary,                     typeof(BestiaryHandler)         },
			{ NetModuleType.Ambience,                     typeof(AmbienceHandler)         }
		};

		/// <summary>
		/// Invoked when a load net module packet is received. This method picks a <see cref="INetModuleHandler"/> based on the
		/// net module type being loaded, then forwards the data to the chosen handler to process
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, LoadNetModuleEventArgs args)
		{
			INetModuleHandler handler;

			if (NetModulesToHandlersMap.ContainsKey(args.ModuleType))
			{
				handler = (INetModuleHandler)Activator.CreateInstance(NetModulesToHandlersMap[args.ModuleType]);
			}
			else
			{
				// As of 1.4.x.x, this is now used for more things:
				//	NetCreativePowersModule
				//	NetCreativePowerPermissionsModule
				//	NetLiquidModule
				//	NetParticlesModule
				//	NetPingModule
				//	NetTeleportPylonModule
				//	NetTextModule
				// I (particles) have disabled the original return here, which means that we need to
				// handle this more. In the interm, this unbreaks parts of vanilla. Originally
				// we just blocked this because it was a liquid exploit.
				args.Handled = false;
				return;
			}

			handler.Deserialize(args.Data);
			handler.HandlePacket(args.Player, out bool rejectPacket);

			args.Handled = rejectPacket;
		}
	}
}
