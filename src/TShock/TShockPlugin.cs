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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Orion;
using Orion.Events;
using Orion.Events.Server;
using Orion.Players;
using Serilog;
using TShock.Commands;
using TShock.Modules;

namespace TShock {
    /// <summary>
    /// Represents the TShock plugin.
    /// </summary>
    [Service("tshock")]
    public sealed class TShockPlugin : OrionPlugin {
        private readonly Lazy<IPlayerService> _playerService;
        private readonly Lazy<ICommandService> _commandService;

        private readonly ISet<TShockModule> _modules = new HashSet<TShockModule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockPlugin"/> class with the specified Orion kernel, log,
        /// and services.
        /// </summary>
        /// <param name="kernel">The Orion kernel.</param>
        /// <param name="log">The log.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="commandService">The command service.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="kernel"/>, <paramref name="log"/>, or any of the services are <see langword="null"/>.
        /// </exception>
        public TShockPlugin(OrionKernel kernel, ILogger log, Lazy<IPlayerService> playerService,
                Lazy<ICommandService> commandService) : base(kernel, log) {
            Kernel.Bind<ICommandService>().To<TShockCommandService>();

            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        /// <summary>
        /// Registers the given <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <exception cref="ArgumentNullException"><paramref name="module"/> is <see langword="null"/>.</exception>
        public void RegisterModule(TShockModule module) {
            if (module is null) {
                throw new ArgumentNullException(nameof(module));
            }

            _modules.Add(module);
        }

        /// <inheritdoc/>
        public override void Initialize() {
            Kernel.ServerInitialize.RegisterHandler(ServerInitializeHandler);

            RegisterModule(new CommandModule(Kernel, _playerService.Value, _commandService.Value));
        }

        /// <inheritdoc/>
        public override void Dispose() {
            foreach (var module in _modules) {
                module.Dispose();
            }

            Kernel.ServerInitialize.UnregisterHandler(ServerInitializeHandler);
        }

        [EventHandler(EventPriority.Normal, Name = "tshock")]
        private void ServerInitializeHandler(object sender, ServerInitializeEventArgs args) {
            // The reason why we want to wait for ServerInitialize to initialize the modules is because we need three
            // stages of initialization:
            //
            // 1) Plugin constructors, which use lazy services.
            // 2) Plugin Initialize, which can set up service event handlers,
            // 3) ServerInitialize, which can use any service.
            //
            // This way, we can have service rebinding and service event handlers that always run.
            foreach (var module in _modules) {
                module.Initialize();
            }
        }
    }
}
