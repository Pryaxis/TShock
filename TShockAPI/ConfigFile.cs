/*   
TShock, a server mod for Terraria
Copyright (C) <year>  <name of author>

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
namespace TShockAPI
{
    internal class ConfigFile
    {
        public int InvasionMultiplier = 1;
        public int DefaultMaximumSpawns = 4;
        public int DefaultSpawnRate = 700;
        public int ServerPort = 7777;
        public bool EnableWhitelist;
        public bool InfiniteInvasion;
        public bool AlwaysPvP;
        public bool KickCheaters = true;
        public bool BanCheaters = true;
        public bool KickGriefers = true;
        public bool BanGriefers = true;
        public bool BanKillTileAbusers;
        public bool KickKillTileAbusers;
        public bool BanExplosives = true;
        public bool KickExplosives = true;
        public bool SpawnProtection = true;
        public int SpawnProtectionRadius = 5;
        public string DistributationAgent = "facepunch";
        public int MaxSlots = 8;
        public bool RangeChecks = true;
        public bool SpamChecks = false;
    }
}