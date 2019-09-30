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
using TShock.Commands.Parsers;
using Xunit;

namespace TShock.Commands.Extensions {
    public class CommandServiceExtensionsTests {
        [Fact]
        public void RegisterParser1_IsCorrect() {
            var parser = new Mock<IArgumentParser<byte>>().Object;
            var mockCommandService = new Mock<ICommandService>();
            mockCommandService.Setup(cs => cs.RegisterParser(typeof(byte), parser));

            mockCommandService.Object.RegisterParser(parser);

            mockCommandService.Verify(cs => cs.RegisterParser(typeof(byte), parser));
            mockCommandService.VerifyNoOtherCalls();
        }

        [Fact]
        public void RegisterParser1_NullCommandService_ThrowsArgumentNullException() {
            var parser = new Mock<IArgumentParser<byte>>().Object;
            Action action = () => CommandServiceExtensions.RegisterParser(null, parser);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RegisterParser1_NullParser_ThrowsArgumentNullException() {
            var commandService = new Mock<ICommandService>().Object;
            Action action = () => commandService.RegisterParser<byte>(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
