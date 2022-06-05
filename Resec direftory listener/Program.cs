using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace Resec_direftory_listener
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = @"C:\Users\orlyd\Desktop\Resec Files\InputFiles";
            Console.Write($"start to listen in path: {inputPath}  ...");

            MonitorDirectoty(inputPath);            

            Console.WriteLine("end to listen.");

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

        }

        private static void MonitorDirectoty(string path)
        {
            FileSystemWatcher watcher = new FileSystemWatcher();

            watcher.Path = path;

            watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

            //watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            //watcher.Deleted += OnDeleted;
            //watcher.Renamed += OnRenamed;
            //watcher.Error += OnError;

            watcher.Filter = "*.html";
            //watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

        }


        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            //Show that a file has been created, changed, or deleted.
            WatcherChangeTypes wct = e.ChangeType;
            Console.WriteLine("File {0} {1}", e.FullPath, wct.ToString());

            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                CheckHtmlFile(e.FullPath, e.Name);
            }
        }

        private static void CheckHtmlFile(string fullPath, string fileName)
        {
            string filePath = $"Created: {fullPath}";
            string outPutPath = @"C:\Users\orlyd\Desktop\Resec Files\OutputFiles\";
            //Console.WriteLine(path);

            // check if html file contains hiperlink fot fishing
            // 1. read html file into file as text
            // 2. look for <a> in file
            //      3. if <a>  ref != to his title, then rplace it
            // 4. copy the file to the

            StringBuilder fileSB = new StringBuilder();

            using (StreamReader reader = new StreamReader(fullPath))
            {
                String line = String.Empty;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    fileSB.Append(line);
                }
            }

            string fileStr = fileSB.ToString();

            if (fileSB.ToString().Contains("<a"))
            {
                int startIndex = fileSB.ToString().IndexOf("<a");
                int endIndex = fileSB.ToString().IndexOf("</a>", startIndex);

                int indexHrefLinkStart = fileSB.ToString().IndexOf("\"", startIndex);
                int indexHrefLinkEnd = fileSB.ToString().IndexOf("\"", indexHrefLinkStart+1);
                string hrefLinkStr = fileSB.ToString().Substring(indexHrefLinkStart+1, indexHrefLinkEnd - indexHrefLinkStart -1);

                int indexHreTitleStart = fileSB.ToString().IndexOf(">", indexHrefLinkEnd) +1;
                int indexHreTitleEnd = endIndex;
                string hrefTitleStr = fileSB.ToString().Substring(indexHreTitleStart, indexHreTitleEnd - indexHreTitleStart);

                if (hrefTitleStr != hrefLinkStr)
                {
                    fileStr = fileSB.ToString().Substring(0, indexHrefLinkStart)
                            + "\"" +hrefTitleStr + "\""
                            + fileSB.ToString().Substring(indexHrefLinkEnd);
                }
            }

            // Create the file, or overwrite if the file exists.
            using (FileStream fs = File.Create(outPutPath + fileName))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(fileStr);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            // delete the original file from input directory
            File.Delete(fullPath);
        }


    }


}
