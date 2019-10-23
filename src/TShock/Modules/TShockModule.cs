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

namespace TShock.Modules {
    /// <summary>
    /// Represents a module of TShock's functionality.
    /// </summary>
    [SuppressMessage(
        "Design", "CA1063:Implement IDisposable Correctly",
        Justification = "IDisposable pattern makes no sense")]
    public abstract class TShockModule : IDisposable {
        /// <summary>
        /// Disposes the service, releasing any resources associated with it.
        /// </summary>
        [SuppressMessage(
            "Usage", "CA1816:Dispose methods should call SuppressFinalize",
            Justification = "IDisposable pattern makes no sense")]
        public virtual void Dispose() { }

        /// <summary>
        /// Initializes the module. Typically, commands should be registered here.
        /// </summary>
        public abstract void Initialize();
    }
}
