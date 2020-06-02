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
		/// Invoked when a load net module packet is received. This method picks a <see cref="INetModuleHandler"/> based on the
		/// net module type being loaded, then forwards the data to the chosen handler to process
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, LoadNetModuleEventArgs args)
		{
			INetModuleHandler handler;

			switch (args.ModuleType)
			{
				case NetModulesTypes.CreativePowers:
					{
						handler = new CreativePowerHandler();
						break;
					}

				case NetModulesTypes.CreativeUnlocksPlayerReport:
					{
						if (!Main.GameModeInfo.IsJourneyMode)
						{
							TShock.Log.ConsoleDebug(
								"NetModuleHandler received attempt to unlock sacrifice while not in journey mode from",
								args.Player.Name
							);

							args.Handled = true;
							return;
						}

						handler = new CreativeUnlocksHandler();
						break;
					}
				case NetModulesTypes.TeleportPylon:
					{
						handler = new PylonHandler();
						break;
					}
				case NetModulesTypes.Liquid:
					{
						handler = new LiquidHandler();
						break;
					}
				case NetModulesTypes.Bestiary:
					{
						handler = new BestiaryHandler();
						break;
					}
				default:
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
			}

			handler.Deserialize(args.Data);
			handler.HandlePacket(args.Player, out bool rejectPacket);

			args.Handled = rejectPacket;
		}
	}
}
