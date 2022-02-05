using System;
using FolderSync.CommandLine;
using FolderSync.Syncronization;

namespace FolderSync
{
    class Program
    {
        static void Main(string[] args)
        {
            ICmdParser cmdParser = new FolderSyncCmdArgsParser(Syncronize);
            cmdParser.invoke(args);
            Console.WriteLine("Press Enter key to stop at any time...");
            Console.ReadLine();
        }

        private static void Syncronize(string output, string replica,
            int interval, string log)
        {
            Syncronizer syncronizer = new Syncronizer(output, replica, interval, log);
            syncronizer.Syncronize();
        }
    }
}
