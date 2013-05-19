using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
