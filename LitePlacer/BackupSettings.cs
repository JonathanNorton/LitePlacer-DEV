using System.IO;
using System.Linq;

#pragma warning disable CA1031 // Do not catch general exception types

namespace LitePlacer
{
    /// <summary>
    ///     Creates backup copys of the application's settings files
    /// </summary>
    internal static class BackupSettings
    {
        /// <summary>
        ///     The number of backup copies to keep. Change this if required.
        ///     Must be >= 1
        /// </summary>
        private const int BackupCount = 3;

        /// <summary>
        ///     Create backup copies of all the program settings files.
        /// </summary>
        /// <returns>True on success.</returns>
        public static bool DoBackupNow()
        {
            try
            {
                // Get the current working directory
                string workingDir = Directory.GetCurrentDirectory();

                // Create the back up directorys if required
                CreateDirectories(workingDir);


                // Delete BackupN
                new DirectoryInfo(MakeBackupDirectoryName(workingDir, BackupCount))
                    .GetFiles().ToList()
                    .ForEach(s => s.Delete());

                // Shuffle backups along (N-1 -> N), ..., (2 -> 3), (1 -> 2).
                foreach (var b in Enumerable.Range(2, (BackupCount - 1)).Reverse())
                {
                    new DirectoryInfo(MakeBackupDirectoryName(workingDir, b - 1))
                        .GetFiles().ToList()
                        .ForEach(s => s.MoveTo(Path.Combine(MakeBackupDirectoryName(workingDir, b), Path.GetFileName(s.FullName))));
                }

                // Copy working copy to Backup1
                // What files are settings files? - Any files that have the name 'LitePlacer' and whos
                // file extension is longer than 7 characters seems to match all the settings files.
                // Not sure how robust this will be going forward though.
                new DirectoryInfo(workingDir)
                    .GetFiles()
                    .Where(s => Path.GetFileNameWithoutExtension(s.FullName) == "LitePlacer")
                    .Where(s => Path.GetExtension(s.FullName).Length > 7)
                    .ToList()
                    .ForEach(s => s.CopyTo(Path.Combine(MakeBackupDirectoryName(workingDir, 1), Path.GetFileName(s.FullName))));
            }

            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Create backup directories
        /// </summary>
        /// <param name="workingDir"></param>
        private static void CreateDirectories(string workingDir)
        {
            foreach (var i in Enumerable.Range(1, BackupCount))
            {
                string dir = MakeBackupDirectoryName(workingDir, i);

                if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

            }
        }

        /// <summary>
        ///     Calculate the name of a backup directory
        /// </summary>
        /// <param name="workingDir">The base working directory, in which the backup directorys will be created</param>
        /// <param name="backupNumber">The backup number</param>
        /// <returns>The name of the backup directory</returns>
        private static string MakeBackupDirectoryName(string workingDir, int backupNumber)
        {
            return Path.Combine(workingDir, $"Backup{backupNumber}");
        }

    }
}

#pragma warning restore CA1031 // Do not catch general exception types
