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
using FluentAssertions;
using Moq;
using Orion;
using Orion.Events;
using Orion.Events.Players;
using Orion.Events.Server;
using Orion.Players;
using TShock.Commands;
using TShock.Events.Commands;
using TShock.Modules;
using Xunit;

namespace TShock {
    public class TShockPluginTests {
        private readonly OrionKernel _kernel = new OrionKernel();
        private readonly TShockPlugin _plugin;
        private readonly Mock<IPlayerService> _mockPlayerService = new Mock<IPlayerService>();
        private readonly Mock<ICommandService> _mockCommandService = new Mock<ICommandService>();

        public TShockPluginTests() {
            _plugin = new TShockPlugin(_kernel,
                new Lazy<IPlayerService>(() => _mockPlayerService.Object),
                new Lazy<ICommandService>(() => _mockCommandService.Object));
        }

        [Fact]
        public void RegisterModule_NullModule_ThrowsArgumentNullException() {
            Action action = () => _plugin.RegisterModule(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Dispose_DisposesModule() {
            var module = new TestModule();
            _plugin.RegisterModule(module);

            _plugin.Dispose();

            module.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ServerInitialize_InitializesModule() {
            _mockPlayerService.Setup(ps => ps.PlayerChat).Returns(new EventHandlerCollection<PlayerChatEventArgs>());
            _mockCommandService
                .Setup(ps => ps.CommandRegister)
                .Returns(new EventHandlerCollection<CommandRegisterEventArgs>());
            _mockCommandService
                .Setup(ps => ps.CommandUnregister)
                .Returns(new EventHandlerCollection<CommandUnregisterEventArgs>());
            var module = new TestModule();
            _plugin.Initialize();
            _plugin.RegisterModule(module);

            _kernel.ServerInitialize.Invoke(this, new ServerInitializeEventArgs());

            module.IsInitialized.Should().BeTrue();
        }

        private class TestModule : TShockModule {
            public bool IsDisposed { get; private set; }
            public bool IsInitialized { get; private set; }

            public override void Initialize() => IsInitialized = true;
            protected override void Dispose(bool disposeManaged) => IsDisposed = true;
        }
    }
}
