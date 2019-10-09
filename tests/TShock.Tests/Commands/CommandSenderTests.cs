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
using Microsoft.Xna.Framework;
using Moq;
using Xunit;

namespace TShock.Commands {
    public class CommandSenderTests {
        [Fact]
        public void SendErrorMessage() {
            var mockSender = new Mock<ICommandSender>();

            mockSender.Object.SendErrorMessage("test");

            mockSender.Verify(s => s.SendMessage("test", It.IsAny<Color>()));
            mockSender.VerifyNoOtherCalls();
        }

        [Fact]
        public void SendErrorMessage_NullSender_ThrowsArgumentNullException() {
            Action action = () => CommandSenderExtensions.SendErrorMessage(null, "");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendErrorMessage_NullMessage_ThrowsArgumentNullException() {
            var sender = new Mock<ICommandSender>().Object;
            Action action = () => sender.SendErrorMessage(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendInfoMessage() {
            var mockSender = new Mock<ICommandSender>();

            mockSender.Object.SendInfoMessage("test");

            mockSender.Verify(s => s.SendMessage("test", It.IsAny<Color>()));
            mockSender.VerifyNoOtherCalls();
        }

        [Fact]
        public void SendInfoMessage_NullSender_ThrowsArgumentNullException() {
            Action action = () => CommandSenderExtensions.SendInfoMessage(null, "");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SendInfoMessage_NullMessage_ThrowsArgumentNullException() {
            var sender = new Mock<ICommandSender>().Object;
            Action action = () => sender.SendInfoMessage(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
