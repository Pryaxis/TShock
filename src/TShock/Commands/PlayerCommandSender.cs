// Copyright (c) 2019 Pryaxis & TShock Contributors
// 
// This file is part of TShock.
// 
// TShock is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// TShock is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with TShock.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Orion.Players;
using Serilog;

namespace TShock.Commands {
    internal sealed class PlayerCommandSender : ICommandSender {
        public string Name => Player.Name;
        public ILogger Log => throw new NotImplementedException();
        public IPlayer Player { get; }

        public PlayerCommandSender(IPlayer player, ReadOnlySpan<char> input) {
            Debug.Assert(player != null, "player != null");

            Player = player;
        }

        public void SendMessage(ReadOnlySpan<char> message) => SendMessage(message, Color.White);

        public void SendMessage(ReadOnlySpan<char> message, Color color) =>
            Player.SendMessage(message.ToString(), color);
    }
}
