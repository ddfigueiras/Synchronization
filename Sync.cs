using System;
using System.IO;
using System.Threading;

class Sync
{
    private static string logFilePath = "SynclogAll.txt"; // Default log file
    static void Main(string[] args)
    {
        try
        {
            if ((args.Length != 3 && args.Length != 4) || !string.Equals(args[0], "Copy", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\tWrong arguments!\nPlease use: Copy \"source path\" \"replica path\" \"[Optional <log path>]\"");
                return;
            }

            int recursiveSyncTime = 5; // each X minutes we will sync again  
            string sourcePath = args[1];
            string replicaPath = args[2];
            if(args.Length == 4) 
                logFilePath = args[3];
            Console.WriteLine($"Synchronizing from {sourcePath} to {replicaPath}");
            Timer timer = new Timer(SynchronizeFolders, new Tuple<string, string>(sourcePath, replicaPath), TimeSpan.Zero, TimeSpan.FromMinutes(recursiveSyncTime));


            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            timer.Dispose(); // free the timer
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            LogOperation($"Erro => {ex.ToString()}", logFilePath);
        }
    }
    static void SynchronizeFolders(object? state) // I'm confident that here is not null but To avoid warnings let's do that way
    {
        if (state == null) return;
        var folders = (Tuple<string, string>)state;
        string sourceFolder = folders.Item1;
        string replicaFolder = folders.Item2;

        try
        {
            SyncFiles(sourceFolder, replicaFolder);
            Console.WriteLine("Synchronization complete");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during synchronization: {ex.Message}");
        }
    }

    static void SyncFiles(string source, string replica)
    {
        // we will use all 3 for remove extra files
        var sourceFiles = Directory.GetFiles(source); // ALl files in the source folder
        var replicaFiles = Directory.GetFiles(replica).Select(Path.GetFileName);// All files in the replica folder
        var extraFiles = replicaFiles.Except(sourceFiles.Select(Path.GetFileName)); // Extra files that are not in source
        // Sync files
        Console.WriteLine("Synchronizing...");
        // Sync files
        foreach (var sourceFile in sourceFiles)
        {
            string fileName = Path.GetFileName(sourceFile);
            string replicaFile = Path.Combine(replica, fileName);

            File.Copy(sourceFile, replicaFile, true);
            LogOperation($"Copied {fileName}", logFilePath);
        }

        // Sync subfolders
        foreach (var sourceSubfolder in Directory.GetDirectories(source))
        {
            string subfolderName = Path.GetFileName(sourceSubfolder);
            string replicaSubfolder = Path.Combine(replica, subfolderName);

            SyncFiles(sourceSubfolder, replicaSubfolder);
        }
        if (extraFiles.Any()) // That mean we need to delete something
        {
            Console.WriteLine("Removing extra files...");
            // Delete extra files
            foreach (var extraFile in extraFiles)
            {
                if(extraFile == null) continue; // I am confident that it's never null in this context, but I have warnings here...
                string filePath = Path.Combine(replica, extraFile);
                File.Delete(filePath);
                LogOperation($"Deleted {extraFile}", logFilePath);
            }
        }
    }
    static void LogOperation(string operation, string logFilePath)
    {
        string logEntry = $"{DateTime.Now} - {operation}";

        Console.WriteLine(logEntry); // we wrote already before that it was complete but here we check if its logged as well

        File.AppendAllText(logFilePath, logEntry + Environment.NewLine); // Log to the file
    }
}
//dotnet run Copy "C:\Users\ddfigueiras\Desktop\Programa UALG\Ano 2\C#\FolderSourceTest" "C:\Users\ddfigueiras\Desktop\Programa UALG\Ano 2\C#\FolderReplicaTest\finalcopy"
