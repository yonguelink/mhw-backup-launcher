using System;
using System.Diagnostics;
using System.IO;

namespace mhw_backup_launcher {
  internal static class Launcher {
    private const string MHW_STEAM_APP_ID = "582010";

    // TODO: Args

    private static void Main(string[] args) {
      try {
        BackupMhwSaveFiles(STEAM_INSTALL_PATH, STEAM_USER_ID, MHW_STEAM_APP_ID, STEAM_USERNAME, @"G:\Backup");
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
