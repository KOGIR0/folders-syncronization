using System.CommandLine;
using System.Threading.Tasks;
using System.CommandLine.NamingConventionBinder;

namespace FolderSync.CommandLine
{
    class FolderSyncCmdArgsParser: ICmdParser
    {
        private RootCommand rootCommand;

        public delegate void HandlerFunc(string source, string replica,
            int interval, string log);

        public FolderSyncCmdArgsParser(HandlerFunc commandHandler)
        {
            rootCommand = new RootCommand(
                description: "Syncronizes content of two folders.");

            Option sourceFolder = new Option<string>(
              aliases: new string[] { "--output", "-o" }
              , description: "The path to output folder.");
            sourceFolder.IsRequired = true;

            Option replicaFolder = new Option<string>(
              aliases: new string[] { "--replica", "-r" }
              , description: "The path to folder which needs to be replecated.");
            replicaFolder.IsRequired = true;

            Option syncInterval = new Option<int>(
              aliases: new string[] { "--interval", "-i" }
              , description: "The preiod of time when syncronization is done.");
            syncInterval.IsRequired = true;

            Option logFilePath = new Option<string>(
              aliases: new string[] { "--log", "-l" }
              , description: "The path to log file.");
            logFilePath.IsRequired = true;


            rootCommand.Add(sourceFolder);
            rootCommand.Add(replicaFolder);
            rootCommand.Add(syncInterval);
            rootCommand.Add(logFilePath);

            rootCommand.Handler = CommandHandler.Create(commandHandler);
        }

        public async Task<int> invoke(string[] args)
        {
            return await rootCommand.InvokeAsync(args);
        }
    }
}
