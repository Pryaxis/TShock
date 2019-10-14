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

namespace TShock.Modules {
    /// <summary>
    /// Represents a module of TShock's functionality.
    /// </summary>
    public abstract class TShockModule : IDisposable {
        /// <summary>
        /// Disposes the module and any of its managed and unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the module. Typically, commands should be registered here.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Disposes the module and any of its unmanaged resources, optionally including its managed resources.
        /// </summary>
        /// <param name="disposeManaged">
        /// <see langword="true"/> to dispose managed resources, otherwise, <see langword="false"/>.
        /// </param>
        protected abstract void Dispose(bool disposeManaged);
    }
}
