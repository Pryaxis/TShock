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
            string procname = "cmd.exe";
            string procargs = "/c echo finalizing update&&ping 127.0.0.1 -n 2&&del UpdateTShock.exe";
            try
            {
                StreamReader sr = new StreamReader("pn");
                procname = sr.ReadToEnd();
                sr.Close();

                string[] datat = procname.Split(' ');
                procname = datat[0];
                procargs = "";
                for (int i = 0; i < datat.Count(); i++)
                {
                    procargs += datat[i] + " ";
                }

                File.Delete("pn");

                sr.Dispose();

                sr = new StreamReader("pid");
                data = sr.ReadToEnd();
                sr.Close();

                File.Delete("pid");
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

            Process.Start(new ProcessStartInfo(procname, procargs));
        }
    }
}
