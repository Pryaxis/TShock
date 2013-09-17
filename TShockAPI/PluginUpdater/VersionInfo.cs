/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

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
using System;
using System.Collections.Generic;

namespace JsonLoader
{
    class VersionInfo
    {
        public Version version;
        public string url;
    }

    public class Version
    {
        public int Major;
        public int Minor;
        public int Build;
        public int Revision;
        public int MajorRevision;
        public int MinorRevision;

        public Version()
        {
            SetVersion(0,0,0,0);
        }

        public Version(int m)
        {
            SetVersion(m, 0, 0, 0);
        }

        public Version(int ma, int mi)
        {
            SetVersion(ma, mi, 0, 0);
        }

        public Version(int ma, int mi, int b)
        {
            SetVersion(ma, mi, b, 0);
        }

        public Version(int ma, int mi, int b, int r)
        {
            SetVersion(ma, mi, b, r);
        }

        private void SetVersion(int ma, int mi, int b, int r)
        {
            Major = ma;
            Minor = mi;
            Build = b;
            Revision = r;
        }

        public string ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}", Major, Minor, Build, Revision);
        }
    }

    class DownloadPackage
    {
        public List<PluginFile> files;
    }

    class PluginFile
    {
        public string url;
        public string destination = "";
    }
}
