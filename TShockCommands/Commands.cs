//		public static bool HandleCommand(TSPlayer player, string text)
//		{
//			string cmdText = text.Remove(0, 1);
//			string cmdPrefix = text[0].ToString();
//			bool silent = false;

//			if (cmdPrefix == SilentSpecifier)
//				silent = true;

//			int index = -1;
//			for (int i = 0; i < cmdText.Length; i++)
//			{
//				if (IsWhiteSpace(cmdText[i]))
//				{
//					index = i;
//					break;
//				}
//			}
//			string cmdName;
//			if (index == 0) // Space after the command specifier should not be supported
//			{
//				player.SendErrorMessage("Invalid command entered. Type {0}help for a list of valid commands.", Specifier);
//				return true;
//			}
//			else if (index < 0)
//				cmdName = cmdText.ToLower();
//			else
//				cmdName = cmdText.Substring(0, index).ToLower();

//			List<string> args;
//			if (index < 0)
//				args = new List<string>();
//			else
//				args = ParseParameters(cmdText.Substring(index));

//			IEnumerable<Command> cmds = ChatCommands.FindAll(c => c.HasAlias(cmdName));

//			if (Hooks.PlayerHooks.OnPlayerCommand(player, cmdName, cmdText, args, ref cmds, cmdPrefix))
//				return true;

//			if (cmds.Count() == 0)
//			{
//				if (player.AwaitingResponse.ContainsKey(cmdName))
//				{
//					Action<CommandArgs> call = player.AwaitingResponse[cmdName];
//					player.AwaitingResponse.Remove(cmdName);
//					call(new CommandArgs(cmdText, player, args));
//					return true;
//				}
//				player.SendErrorMessage("Invalid command entered. Type {0}help for a list of valid commands.", Specifier);
//				return true;
//			}
//			foreach (Command cmd in cmds)
//			{
//				if (!cmd.CanRun(player))
//				{
//					TShock.Utils.SendLogs(string.Format("{0} tried to execute {1}{2}.", player.Name, Specifier, cmdText), Color.PaleVioletRed, player);
//					player.SendErrorMessage("You do not have access to this command.");
//					if (player.HasPermission(Permissions.su))
//					{
//						player.SendInfoMessage("You can use '{0}sudo {0}{1}' to override this check.", Specifier, cmdText);
//					}
//				}
//				else if (!cmd.AllowServer && !player.RealPlayer)
//				{
//					player.SendErrorMessage("You must use this command in-game.");
//				}
//				else
//				{
//					if (cmd.DoLog)
//						TShock.Utils.SendLogs(string.Format("{0} executed: {1}{2}.", player.Name, silent ? SilentSpecifier : Specifier, cmdText), Color.PaleVioletRed, player);
//					cmd.Run(cmdText, silent, player, args);
//				}
//			}
//			return true;
//		} 
