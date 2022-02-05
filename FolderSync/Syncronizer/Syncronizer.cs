using System;
using System.Collections.Generic;
using System.Timers;
using System.IO;
using FolderSync.Log;

namespace FolderSync.Syncronization
{
    class Syncronizer
    {
        private string outputDir;
        private string dirToReplicate;
        private int interval;
        private Logger logger;

        public Syncronizer(string output, string replica,
            int interval, string log)
        {
            this.outputDir = output;
            this.dirToReplicate = replica;
            this.interval = interval;
            this.logger = new Logger(log);
        }

        public void Syncronize()
        {
            try
            {
                Timer t = new Timer(interval);
                t.Elapsed += this.SyncronizeFolders;
                t.Start();
                this.logger.WriteLine("Started Syncronization");
                this.logger.WriteLine("===========================");
            }
            catch (ArgumentException e)
            {
                this.logger.WriteLine("Error in Synronize: " + e.Message);
            }
        }

        private void SyncronizeFolders(Object source, ElapsedEventArgs e)
        {
            List<string> replicaDirs = getListOfDirs(this.dirToReplicate);
            List<string> replicaFiles = getListOfFiles(this.dirToReplicate);

            this.RemoveNonexistentFiles(replicaFiles);
            this.RemoveNonexistentDirs(replicaDirs);
            this.CreateDirs(replicaDirs);
            this.CopyFiles(replicaFiles);
        }

        /// <summary>
        /// Remove files that do not exist in replicaFiles
        /// </summary>
        /// <param name="replicaFiles">list of paths to files</param>
        private void RemoveNonexistentFiles(List<string> replicaFiles)
        {
            List<string> outputFiles = getListOfFiles(this.outputDir);
            outputFiles.ForEach((file) =>
            {
                if (!replicaFiles.Contains(file) && File.Exists(this.outputDir + file))
                {
                    this.logger.WriteLine("Removed file: " + this.outputDir + file);
                    try
                    {
                        File.Delete(this.outputDir + file);
                    } catch (Exception e)
                    {
                        this.logger.WriteLine("Error deleting file: " + e.Message);
                    }
                }
            });
        }

        /// <summary>
        /// Removes directories that do not exist in replicaDirs
        /// </summary>
        /// <param name="replicaDirs">list of paths to dirs</param>
        private void RemoveNonexistentDirs(List<string> replicaDirs)
        {
            List<string> outputDirs = getListOfDirs(this.outputDir);
            outputDirs.ForEach((dir) =>
            {
                if(!replicaDirs.Contains(dir) && Directory.Exists(this.outputDir + dir))
                {
                    this.logger.WriteLine("Removed directory: " + this.outputDir + dir);
                    try
                    {
                        Directory.Delete(this.outputDir + dir, true);
                    } catch (Exception e)
                    {
                        this.logger.WriteLine("Error deleting directory: " + e.Message);
                    }
                }
            });
        }

        /// <summary>
        /// Copies files if it does not exist
        /// </summary>
        /// <param name="files">list of files</param>
        private void CopyFiles(List<string> files)
        {
            files.ForEach(file =>
            {
                if(!File.Exists(this.outputDir + file))
                {
                    this.logger.WriteLine("Copied file: " + this.outputDir + file);
                    try
                    {
                        File.Copy(this.dirToReplicate + file, this.outputDir + file, true);
                    } catch (Exception e)
                    {
                        this.logger.WriteLine("Error copy file: " + e.Message);
                    }
                }
            });
        }

        /// <summary>
        /// Creates directories if it does not exist
        /// </summary>
        /// <param name="dirs">List of paths to directories</param>
        private void CreateDirs(List<string> dirs)
        {
            dirs.ForEach(dir =>
            {
                if (!Directory.Exists(this.outputDir + dir))
                {
                    this.logger.WriteLine("Created dir: " + this.outputDir + dir);
                    try
                    {
                        Directory.CreateDirectory(this.outputDir + dir);
                    } catch (Exception e)
                    {
                        this.logger.WriteLine("Erroe creating directory: " + e.Message);
                    }
                }
            });
        }

        /// <summary>
        /// Get list of paths to directories and subdirectories without path to dir
        /// </summary>
        /// <param name="dir">Path to directory</param>
        /// <returns>List of paths to directories</returns>
        private List<string> getListOfDirs(string dir)
        {
            List<string> dirs = new List<string>();
            try
            {
                string[] directories = Directory.GetDirectories(dir,
                    "*", SearchOption.AllDirectories);

                foreach (string directory in directories)
                {
                    dirs.Add(directory.Split(dir)[1]);
                }
            }
            catch(DirectoryNotFoundException e)
            {
                this.logger.WriteLine($"Directory {dir} does not exist");
            }
            catch (Exception e)
            {
                this.logger.WriteLine("Error getting list of directories: " + e.Message);
            }

            return dirs;
        }

        /// <summary>
        /// Get list of paths to files in directories and subdirectories without path to dir
        /// </summary>
        /// <param name="dir">Path to directory</param>
        /// <returns>List of paths to files</returns>
        private List<string> getListOfFiles(string dir)
        {
            List<string> dirs = new List<string>();
            try
            {
                string[] directories = Directory.GetFiles(dir,
                    "*", SearchOption.AllDirectories);

                foreach (string directory in directories)
                {
                    dirs.Add(directory.Split(dir)[1]);
                }
            } 
            catch (DirectoryNotFoundException e)
            {
                this.logger.WriteLine($"Directory {dir} does not exist");
            }
            catch (Exception e)
            {
                this.logger.WriteLine("Error getting list of files: " + e.Message);
            }

            return dirs;
        }
    }
}
