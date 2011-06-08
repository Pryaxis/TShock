using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Update
{
    class Program
    {
        static void Main(string[] args)
        {
            string data;
            string procname;
            try
            {
                StreamReader sr = new StreamReader("pn");
                procname = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                sr = new StreamReader("pid");
                data = sr.ReadToEnd();
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                data = "";
            }

            try
            {
                Process TServer = Process.GetProcessById(Convert.ToInt32(data));
                while (!TServer.HasExited)
                {
                }
            }
            catch (Exception)
            {
            }

            try
            {
                File.Delete("TShockAPI.dll");
            }
            catch (FileNotFoundException)
            {
            }

            BinaryWriter bw = new BinaryWriter(new FileStream("TShockAPI.dll", FileMode.Create));
            bw.Write(Resources.TShockAPI);
            bw.Close();
        }
    }
}
