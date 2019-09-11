// Copyright (c) 2011-2019 Pryaxis & TShock Contributors
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
using Orion.Hooks;
using Orion.Items;
using Orion.Networking;
using Orion.Networking.Events;
using Orion.Npcs;
using Orion.Players;
using Orion.Projectiles;
using Orion.World;
using Orion.World.TileEntities;

namespace TShock {
    /// <summary>
    /// Represents the TShock plugin.
    /// </summary>
    public sealed class TShockPlugin : OrionPlugin {
        private readonly IItemService _itemService;
        private readonly INetworkService _networkService;
        private readonly INpcService _npcService;
        private readonly IPlayerService _playerService;
        private readonly IProjectileService _projectileService;
        private readonly IChestService _chestService;
        private readonly ISignService _signService;
        private readonly IWorldService _worldService;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Author => "Pryaxis";

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Name => "TShock";

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockPlugin"/> class with the specified
        /// <see cref="OrionKernel"/> instance and services.
        /// </summary>
        /// <param name="kernel">The <see cref="OrionKernel"/> instance.</param>
        /// <param name="itemService">The item service.</param>
        /// <param name="networkService">The network service.</param>
        /// <param name="npcService">The NPC service.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="projectileService">The projectile service.</param>
        /// <param name="chestService">The chest service.</param>
        /// <param name="signService">The sign service.</param>
        /// <param name="worldService">The world service.</param>
        /// <exception cref="ArgumentNullException">Any of the services are <c>null</c>.</exception>
        public TShockPlugin(OrionKernel kernel, IItemService itemService, INetworkService networkService,
                            INpcService npcService, IPlayerService playerService, IProjectileService projectileService,
                            IChestService chestService, ISignService signService,
                            IWorldService worldService) : base(kernel) {
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _networkService = networkService ?? throw new ArgumentNullException(nameof(networkService));
            _npcService = npcService ?? throw new ArgumentNullException(nameof(npcService));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _projectileService = projectileService ?? throw new ArgumentNullException(nameof(projectileService));
            _chestService = chestService ?? throw new ArgumentNullException(nameof(chestService));
            _signService = signService ?? throw new ArgumentNullException(nameof(signService));
            _worldService = worldService ?? throw new ArgumentNullException(nameof(worldService));

            _networkService.ReceivingPacket += ReceivingPacketHandler;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposeManaged) {
            if (!disposeManaged) return;

            _networkService.ReceivingPacket -= ReceivingPacketHandler;
        }

        [HookHandler(HookPriority.Lowest)]
        private void ReceivingPacketHandler(object sender, ReceivingPacketEventArgs args) { }
    }
}
