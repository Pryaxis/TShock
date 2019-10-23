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
using Orion.Events.Players;
using Orion.Events.Server;
using Orion.Players;
using Serilog.Core;
using TShock.Commands;
using TShock.Events.Commands;
using TShock.Modules;
using Xunit;

namespace TShock {
    public class TShockPluginTests {
        [Fact]
        public void Ctor_NullKernel_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            var mockPlayerService = new Mock<IPlayerService>();
            var mockCommandService = new Mock<ICommandService>();
            using var plugin = new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));
            Func<TShockPlugin> func = () => new TShockPlugin(null, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullPlayerService_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            var mockPlayerService = new Mock<IPlayerService>();
            var mockCommandService = new Mock<ICommandService>();
            using var plugin = new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));
            Func<TShockPlugin> func = () => new TShockPlugin(kernel, Logger.None,
                null,
                new Lazy<ICommandService>(() => mockCommandService.Object));

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullCommandService_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            var mockPlayerService = new Mock<IPlayerService>();
            var mockCommandService = new Mock<ICommandService>();
            using var plugin = new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));
            Func<TShockPlugin> func = () => new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RegisterModule_NullModule_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            var mockPlayerService = new Mock<IPlayerService>();
            var mockCommandService = new Mock<ICommandService>();
            using var plugin = new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));
            Action action = () => plugin.RegisterModule(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Dispose_DisposesModule() {
            using var kernel = new OrionKernel(Logger.None);
            var mockPlayerService = new Mock<IPlayerService>();
            var mockCommandService = new Mock<ICommandService>();
            using var plugin = new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));
            var module = new TestModule();
            plugin.RegisterModule(module);

            plugin.Dispose();

            module.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ServerInitialize_InitializesModule() {
            using var kernel = new OrionKernel(Logger.None);
            var mockPlayerService = new Mock<IPlayerService>();
            var mockCommandService = new Mock<ICommandService>();
            using var plugin = new TShockPlugin(kernel, Logger.None,
                new Lazy<IPlayerService>(() => mockPlayerService.Object),
                new Lazy<ICommandService>(() => mockCommandService.Object));
            var module = new TestModule();
            plugin.Initialize();
            plugin.RegisterModule(module);

            kernel.RaiseEvent(new ServerInitializeEvent(), Logger.None);

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
