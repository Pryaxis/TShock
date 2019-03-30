/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace TShockAPI.Hooks
{
    public class ReloadEventArgs
    {
        public TSPlayer Player { get; set; }
        public ReloadEventArgs(TSPlayer ply)
        {
            Player = ply;
        }
    }

    public class GeneralHooks
    {
        public delegate void ReloadEventD(ReloadEventArgs e);
        public static event ReloadEventD ReloadEvent;

        public static void OnReloadEvent(TSPlayer ply)
        {
            if(ReloadEvent == null)
                return;

            ReloadEvent(new ReloadEventArgs(ply));
        }
    }
}
