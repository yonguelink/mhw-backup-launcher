using System;
using System.Diagnostics;
using System.IO;

namespace mhw_backup_launcher {
  internal static class Launcher {
    private const string MHW_STEAM_APP_ID = "582010";

    // TODO: Args

    private static void Main(string[] args) {
      try {
        if (args.Length != 4) {
          throw new ArgumentException(
            $"Expected 3 arguments, got {args.Length}.\n" +
            "Run like so: mhw-backup-launcher.exe <steamInstallPath> <steamUserId> <steamUsername> <backupFolderPath>\n" +
            "Where: \n" +
            "\t<steamInstallPath> is the Path to Steam installation Folder (this folder needs to contain the `userdata` folder\n" +
            "\t<steamUserId> is your Steam User ID (You can find the ID in the `userdata` folder, there should be only one folder there with a bunch of numbers)\n" +
            "\t<steamUsername> is your Steam username so you can easily identify the backups\n" +
            "\t<backupFolderPath> is the path where you want your backups to be saved to\n" +
            "NOTE: All Paths can be written with forward slash instead of double-backslash for simplicity"
          );
        }

        string steamInstallPath = args[0].Replace('/', '\\');
        string steamUserId = args[1];
        string steamUsername = args[2];
        string backupFolderPath = args[3].Replace('/', '\\');

        BackupMhwSaveFiles(steamInstallPath, steamUserId, MHW_STEAM_APP_ID, steamUsername, backupFolderPath);
      } catch (Exception err) {
        Console.Write(err);
        Console.Write("\nPress any key to continue...");
        Console.ReadLine();
      }

      //StartMhw(MHW_STEAM_APP_ID);
    }

    private static void BackupMhwSaveFiles(string steamInstallPath, string steamUserId, string mhwSteamAppId, string steamUsername, string backupFolderPath) {
      string mhwSaveFolder = Path.Combine(
        steamInstallPath,
        "userdata",
        steamUserId,
        mhwSteamAppId,
        "remote"
      );

      string backupFolderNowPath = Path.Combine(
        backupFolderPath,
        steamUsername,
        "Monster Hunter World",
        DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss")
      );

      DirectoryCopy(mhwSaveFolder, backupFolderNowPath);
    }

    private static void DirectoryCopy (string sourceDirName, string destDirName) {
      DirectoryInfo dir = new DirectoryInfo(sourceDirName);

      if (!dir.Exists) {
        throw new DirectoryNotFoundException(
          $"Source directory does not exist or could not be found: {sourceDirName}"
        );
      }

      Directory.CreateDirectory(destDirName);

      FileInfo[] files = dir.GetFiles();
      foreach (FileInfo file in files) {
        file.CopyTo(Path.Combine(destDirName, file.Name), false);
      }
    }

    private static void StartMhw(int mhwSteamAppId) {
      var mhwStartInfo = new ProcessStartInfo {
        FileName = $"steam://rungameid/{mhwSteamAppId}",
      };
      Process.Start(mhwStartInfo);
    }
  }
}
