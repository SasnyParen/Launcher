using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
namespace Launcher
{
    static class GameFounder
    {
        static private string V = @"\/SteamLibrary";

        static string FindGameInDrive(string drive)
        {
            string VV = V.Remove(0,1);
            string[] subfolders = Directory.GetDirectories(@"" + drive + "/");
            Trace.WriteLine(drive);
            bool IsReturn = true;
            foreach (string d in subfolders)
            {
                //Trace.WriteLine(drive + VV);
                //Trace.WriteLine(d);
                if (d == drive + VV)
                {
                    Trace.WriteLine(d);
                    IsReturn = false;
                }
            }

            if (IsReturn) return null;
            Trace.WriteLine("yES!");
            string[] allFoundFiles = Directory.GetFiles(@""+drive+ "/SteamLibrary/steamapps/common/GarrysMod/", "hl2.exe", SearchOption.AllDirectories);
            foreach (string file in allFoundFiles)
            {
                return file;
            }
            return null;
        }
        static public string FindPath()
        {
            string result = null;
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady)
                {
                    result = FindGameInDrive(d.Name);
                }
            }
            return result;
        }
    }
}
