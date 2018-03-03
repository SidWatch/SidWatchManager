using System;
using System.IO;

namespace SidWatch.Library.Helpers
{
    public class DirectoryHelper
    {
        /// <summary>
        /// Create path if not exists
        /// </summary>
        /// <param name="_path">Path to create</param>
        public static void BuildFolderIfMissing(string _path)
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        /// <summary>
        /// See if given path is writeable
        /// </summary>
        /// <param name="_path">Path to test</param>
        /// <returns>True if writeable, False if not</returns>
        public static bool TestFolderPermissions(string _path)
        {
            StreamWriter sw = null;
            string file = "";
            bool rc = true;

            try
            {
                //Create path if it doesn't exist
                BuildFolderIfMissing(_path);

                //Create a file in the folder
                file = _path + "permission.txt";
                if (File.Exists(file))
                    File.Delete(file);

                sw = new StreamWriter(file) { AutoFlush = true };
                sw.WriteLine("test");

                //Close and delete the file
                sw.Close();
                File.Delete(file);
            }
            catch (Exception ex)
            {
                rc = false;
                TraceHelper.Exception(string.Format("Folder Permissions error - {0}", ex.ToString()));
            }

            return rc;
        }

        /// <summary>
        /// Copies files and folders from source to destination
        /// </summary>
        /// <param name="_srcdir"></param>
        /// <param name="_destdir"></param>
        /// <param name="_recursive"></param>
        public static void FileCopy(string _srcdir, string _destdir, bool _recursive)
        {
            FileCopy(_srcdir, _destdir, _recursive, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_srcdir"></param>
        /// <param name="_destdir"></param>
        /// <param name="_recursive"></param>
        /// <param name="_ignoreExtension"></param>
        public static void FileCopy(
            string _srcdir,
            string _destdir,
            bool _recursive,
            string[] _ignoreExtension)
        {
            DirectoryInfo dir;
            FileInfo[] files;
            DirectoryInfo[] dirs;
            string tmppath;

            //determine if the destination directory exists, if not create it
            if (!Directory.Exists(_destdir))
            {
                Directory.CreateDirectory(_destdir);
            }

            dir = new DirectoryInfo(_srcdir);

            //if the source dir doesn't exist, throw
            if (!dir.Exists)
            {
                throw new ArgumentException("source dir doesn't exist -> " + _srcdir);
            }

            //get all files in the current dir
            files = dir.GetFiles();

            //loop through each file
            foreach (FileInfo file in files)
            {
                bool match = false;
                if (_ignoreExtension != null)
                {
                    foreach (string s in _ignoreExtension)
                    {
                        if (s.Equals(file.Extension, StringComparison.OrdinalIgnoreCase))
                        {
                            match = true;
                            continue;
                        }
                    }
                }

                if (!match)
                {
                    //create the path to where this file should be in destdir
                    tmppath = Path.Combine(_destdir, file.Name);

                    //copy file to dest dir
                    file.CopyTo(tmppath, false);
                }
                else
                {
                    TraceHelper.Verbose(String.Format("Skipping ignored file ({0})", file.FullName));
                }
            }

            //cleanup
            files = null;

            //if not recursive, all work is done
            if (!_recursive)
            {
                return;
            }

            //otherwise, get dirs
            dirs = dir.GetDirectories();

            //loop through each sub directory in the current dir
            foreach (DirectoryInfo subdir in dirs)
            {
                bool match = false;
                if (_ignoreExtension != null)
                {
                    foreach (string s in _ignoreExtension)
                    {
                        if (s.Equals(subdir.Extension, StringComparison.OrdinalIgnoreCase))
                        {
                            match = true;
                            continue;
                        }
                    }
                }

                if (!match)
                {
                    //create the path to the directory in destdir
                    tmppath = Path.Combine(_destdir, subdir.Name);

                    //recursively call this function over and over again
                    //with each new dir.
                    FileCopy(subdir.FullName, tmppath, _recursive, _ignoreExtension);
                }
                else
                {
                    TraceHelper.Verbose(String.Format("Skipping directory file ({0})", dir.FullName));
                }
            }

            //cleanup
            dirs = null;

            dir = null;
        }
    }
}
