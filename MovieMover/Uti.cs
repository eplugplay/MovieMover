using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;

namespace MovieMover
{
    public static class Uti
    {
        public enum FileTypes
        {
            ini
        }

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        public static bool TryKillProcessByMainWindowHwnd(int hWnd)
        {
            uint processID;
            GetWindowThreadProcessId((IntPtr)hWnd, out processID);
            if (processID == 0) return false;
            try
            {
                Process.GetProcessById((int)processID).Kill();
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            return true;
        }

        public static string GetDesktopDir(string filename)
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + filename;
        }

        public static string GetFilePath(FileTypes type)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + GetFileName(type);
            return path;
        }

        public static string GetAppDataPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        public static string GetLocalFilePath(FileTypes type)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), GetFileName(type));
            return path;
        }

        public static string GetFileName(FileTypes type)
        {
            string returnType = "";
            switch (type)
            {
                case FileTypes.ini: returnType = "MovieMover.ini"; break;
                default: break;
            }
            return returnType;
        }

        public static void SaveIniFile(string section, string key, string keyValue)
        {
            INIFile iniFile = new INIFile(GetFilePath(FileTypes.ini));
            iniFile.Write(section, key, keyValue);
        }

        public static string ReadIniFile(string section, string key)
        {
            INIFile iniFile = new INIFile(GetFilePath(FileTypes.ini));
            return iniFile.Read(section, key);
        }

        public static string GetFilePath(OpenFileDialog openFileDialog, string filter, string load)
        {
            string path = "";
            openFileDialog.Multiselect = true;
            openFileDialog.DefaultExt = "*.java";
            openFileDialog.Filter = filter + " File (*." + filter + ")|*." + filter + "|All files (*.*)|*.*";
            openFileDialog.FileName = "";

            if (load != "")
            {
                openFileDialog.InitialDirectory = load;
            }

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }
            return path;
        }

        public static string GetFolderPath(FolderBrowserDialog folderBrowserDialog, string load)
        {
            string path = "";
            if (load != "")
            {
                folderBrowserDialog.SelectedPath = load;
            }

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;
            }
            return path;
        }

        public static string GetFileName(string path, bool extension)
        {
            string fileName = "";
            string[] split = path.Split('\\');
            fileName = split[split.Length - 1];
            if (!extension)
            {
                string[] fName = fileName.Split('.');
                fileName = fName[0];
            }
            return fileName;
        }

        public static string GetFilePath(string path)
        {
            string[] split = path.Split('\\');
            path = "";
            for (int i = 0; i < split.Length; i++)
            {
                if (i != split.Length - 1)
                    path += split[i] + "\\";
            }
            return path;
        }

        public static bool PathExist(string path, bool folder)
        {
            bool exist = false;

            if (folder)
            {
                exist = Directory.Exists(path);
            }
            else
            {
                exist = File.Exists(path);
            }
            return exist;
        }

    }

    public static class ISynchronizeInvokeExtensions
    {
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }

}
