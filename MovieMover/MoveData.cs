using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;

namespace MovieMover
{
    public class MoveData
    {
        public static void MoveMovies(string sourcePath, string destinationPath, TextBox txtMessage, bool copy)
        {
            MovieInfo movieInfo = new MovieInfo();
            string logPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "MoviesImportList.txt";
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            if (Uti.PathExist(destinationPath, true))
            {
                try
                {
                    string mainFolder = "";
                    string[] subdirectoryEntries = Directory.GetDirectories(sourcePath, "*", System.IO.SearchOption.TopDirectoryOnly);
                    foreach (var dir in subdirectoryEntries)
                    {
                        int cntDel = 0;
                        mainFolder = GetRootFolder(dir);
                        foreach (string fullpath in Directory.GetFiles(dir))
                        {
                            movieInfo = GetFileInfo(fullpath, sourcePath, destinationPath, logPath, copy);
                            cntDel = ExecuteMovies(movieInfo, txtMessage, mainFolder, cntDel);
                        }
                        string[] subFolders = Directory.GetDirectories(dir, "*", System.IO.SearchOption.AllDirectories);
                        foreach (var subDir in subFolders)
                        {
                            foreach (string fullpath in Directory.GetFiles(subDir))
                            {
                                movieInfo = GetFileInfo(fullpath, sourcePath, destinationPath, logPath, copy);
                                cntDel = ExecuteMovies(movieInfo, txtMessage, mainFolder, cntDel);
                            }
                        }
                        if (!movieInfo.copy)
                        {
                            if (cntDel > 0)
                            {
                                Directory.Delete(dir, true);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("Failed to copy movie" + e.Message + e.StackTrace, logPath);
                }
            }
        }

        static string GetRootFolder(string path)
        {
            string dir = path;
            string root = Path.GetPathRoot(dir);
            string pathWithoutRoot = dir.Substring(root.Length);
            return pathWithoutRoot.Split(Path.DirectorySeparatorChar).Last(); 
        }

        public static int ExecuteMovies(MovieInfo movieInfo, TextBox txtMessage, string mainFolder, int cntDel)
        {
            if (movieInfo.extension.ToLower() == "mkv" && !movieInfo.filename.Contains("sample") || movieInfo.extension == "srt" || movieInfo.extension == "sfv" || movieInfo.extension == "idx" ||
                movieInfo.extension == "nfo" ||
                movieInfo.extension.ToLower() == "avi" && !movieInfo.filename.Contains("sample") || movieInfo.extension.ToLower() == "mp4" && !movieInfo.filename.Contains("sample") ||
                movieInfo.extension.ToLower() == "mpeg" && !movieInfo.filename.Contains("sample") || movieInfo.extension.ToLower() == "mpg" && !movieInfo.filename.Contains("sample") ||
                movieInfo.extension.ToLower() == "wmv" && !movieInfo.filename.Contains("sample"))
            {

                string subDir = movieInfo.destinationPath + "\\" + mainFolder;
                if (!Directory.Exists(subDir))
                {
                    Directory.CreateDirectory(subDir);
                }
                string destinationFullPath = subDir + "\\" + movieInfo.filename + "." + movieInfo.extension;
                string msg = "";
                if (!File.Exists(destinationFullPath))
                {
                    destinationFullPath = subDir + "\\" + movieInfo.filename + "." + movieInfo.extension;
                }
                else
                {
                    destinationFullPath = subDir + "\\" + movieInfo.filename + "_" + Guid.NewGuid() + "." + movieInfo.extension;
                }
                try
                {
                    if (movieInfo.copy)
                    {
                        msg = "Copying: {0}To: {1}";
                        File.Copy(movieInfo.filePath, destinationFullPath);
                        cntDel++;
                    }
                    else
                    {
                        msg = "Moving: {0}From: {1} To: {2}";
                        File.Move(movieInfo.filePath, destinationFullPath);
                        cntDel++;
                    }
                    msg = string.Format(msg, Environment.NewLine + "File : " + movieInfo.filename + "." + movieInfo.extension + Environment.NewLine,
                    movieInfo.sourcePath + "\\" + mainFolder + Environment.NewLine,
                    subDir + Environment.NewLine + Environment.NewLine);
                    txtMessage.InvokeEx(x => x.Text += msg);
                    Log(msg, movieInfo.logPath);
                }
                catch (Exception e)
                {
                    Log("Failed: " + movieInfo.filename + "." + movieInfo.extension + Environment.NewLine, movieInfo.logPath);
                    txtMessage.InvokeEx(x => x.Text += "Failed: " + movieInfo.filename + "." + movieInfo.extension + Environment.NewLine + Environment.NewLine);
                }
            }
            return cntDel;
        }

        public static void Log(string msg, string path)
        {
            //string path = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + filename;
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(msg);
            }
        }

        public static MovieInfo GetFileInfo(string fullpath, string sourcePath, string destinationPath, string logPath, bool copy)
        {
            MovieInfo info = new MovieInfo();
            string filename = Path.GetFileName(fullpath);
            info.filePath = fullpath;
            info.extension = Path.GetExtension(fullpath).ToLower();
            info.filename = filename.Replace(info.extension, "");
            info.extension = info.extension.Replace(".", "");
            info.sourcePath = sourcePath;
            info.foldername = Path.GetDirectoryName(fullpath).Split('\\').LastOrDefault();
            info.destinationPath = destinationPath;
            info.logPath = logPath;
            info.copy = copy;
            return info;
        }

        public struct MovieInfo
        {
            public string filename { get; set; }
            public string foldername { get; set; }
            public string extension { get; set; }
            public string filePath { get; set; }
            public string sourcePath { get; set; }
            public string destinationPath { get; set; }
            public string logPath { get; set; }
            public bool copy { get; set; }
        }
    }
}
