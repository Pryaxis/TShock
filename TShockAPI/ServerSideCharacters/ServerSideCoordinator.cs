using System;
using Terraria;
using TShockAPI.Hooks;

namespace TShockAPI.ServerSideCharacters
{
	public class ServerSideCoordinator
	{
		public ServerSideCoordinator()
		{
			PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
			PlayerHooks.PlayerLogout += OnPlayerLogout;
		}

		private void OnPlayerPostLogin(PlayerPostLoginEventArgs e)
		{
			if (!Main.ServerSideCharacter)
			{
				return;
			}

			if (e.Player.HasPermission(Permissions.bypassssc))
			{
				// Do bypass stuff?
				return;
			}

			if (TShock.CharacterDB.HasPlayerData(e.Player))
			{
				// Retrieve & apply existing data
				return;
			}

			// Create a new SSC data set
			ServerSidePlayerData player = ServerSidePlayerData.CreateDefaultFor(e.Player.TPlayer);
		}

		private void OnPlayerLogout(PlayerLogoutEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
