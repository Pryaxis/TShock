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
using System.Diagnostics.CodeAnalysis;
using Orion;
using Orion.Events;
using Orion.Events.Packets;
using Orion.Players;
using TShock.Commands;

namespace TShock {
    /// <summary>
    /// Represents the TShock plugin.
    /// </summary>
    public sealed class TShockPlugin : OrionPlugin {
        private readonly Lazy<IPlayerService> _playerService;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Author => "Pryaxis";

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Name => "TShock";

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockPlugin"/> class with the specified Orion kernel and
        /// services.
        /// </summary>
        /// <param name="kernel">The Orion kernel.</param>
        /// <param name="playerService">The player service.</param>
        /// <exception cref="ArgumentNullException">Any of the services are <c>null</c>.</exception>
        public TShockPlugin(OrionKernel kernel, Lazy<IPlayerService> playerService) : base(kernel) {
            kernel.Bind<ICommandService>().To<TShockCommandService>();

            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        /// <inheritdoc />
        protected override void Initialize() {
            _playerService.Value.PacketReceive += PacketReceiveHandler;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposeManaged) {
            if (!disposeManaged) return;

            _playerService.Value.PacketReceive -= PacketReceiveHandler;
        }

        [EventHandler(EventPriority.Lowest)]
        private void PacketReceiveHandler(object sender, PacketReceiveEventArgs args) { }
    }
}
