using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace mhw_backup_launcher {
  internal static class Launcher {
    private const string MHW_STEAM_APP_ID = "582010";

    private static void Main(string[] args) {
      try {
        if (args.Length != 4 && args.Length != 5) {
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


        string zipFilePath = BackupMhwSaveFiles(steamInstallPath, steamUserId, MHW_STEAM_APP_ID, steamUsername, backupFolderPath);

        StartMhw(MHW_STEAM_APP_ID);
        if (args.Length == 5) {
          UploadToGoogleDrive(zipFilePath, steamUsername);
        }

      } catch (Exception err) {
        Console.Write(err);
        Console.Write("\nPress Enter to continue...");
        Console.ReadLine();
      }

    }

    private static FileList GetGooleDriveFiles(DriveService service, string nextPageToken = null) {
      var fileListRequest = service.Files.List();
      if (nextPageToken != null) {
        fileListRequest.PageToken = nextPageToken;
      }
      return fileListRequest.Execute();
    }

    private static IList<Google.Apis.Drive.v3.Data.File> GetAllFilesFromGoogleDrive(DriveService service) {
      FileList files = GetGooleDriveFiles(service);
      FileList allFiles = files;

      while ((bool) files.IncompleteSearch) {
        files = GetGooleDriveFiles(service, files.NextPageToken);
        foreach (var file in files.Files) {
          allFiles.Files.Add(file);
        }
      };

      return allFiles.Files;
    }

    private static string CreateGoogleFolder(string folderName, DriveService service, string parentId = null, IList<Google.Apis.Drive.v3.Data.File> allFiles = null) {
      if (allFiles == null) {
        allFiles = GetAllFilesFromGoogleDrive(service);
      }

      foreach (var file in allFiles) {
        if (file.Name == folderName) {
          return file.Id;
        }
      }

      var folder = new Google.Apis.Drive.v3.Data.File() {
        Name = folderName,
        MimeType = "application/vnd.google-apps.folder"
      };

      if (parentId != null) {
        folder.Parents = new List<string>() { parentId };
      }

      var response = service.Files.Create(folder).Execute();
      return response.Id;
    }

    private static void UploadToGoogleDrive(string filePath, string steamUsername) {
      UserCredential creds;

      var curFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
      using (var stream = new FileStream($"{curFolder}/credentials.json", FileMode.Open, FileAccess.Read)) {
        string[] Scopes = { DriveService.Scope.DriveFile };
        creds = GoogleWebAuthorizationBroker.AuthorizeAsync(
          GoogleClientSecrets.Load(stream).Secrets,
          Scopes,
          "user",
          CancellationToken.None,
          new FileDataStore(".mhw-backup-launcher-auth", true)
        ).Result;
      }

      var service = new DriveService(new BaseClientService.Initializer() {
          HttpClientInitializer = creds,
          ApplicationName = "Monster Hunter World Backup Launcher",
      });

      var allFiles = GetAllFilesFromGoogleDrive(service);
      var mainFolderId = CreateGoogleFolder("mhw-backup", service, null, allFiles);
      var userFolderId = CreateGoogleFolder(steamUsername, service, mainFolderId, allFiles);

      var fileMetadata = new Google.Apis.Drive.v3.Data.File() {
          Name = Path.GetFileName(filePath)
      };
      fileMetadata.Parents = new List<string>() { userFolderId };

      FilesResource.CreateMediaUpload request;
      using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open)) {
          request = service.Files.Create(fileMetadata, stream, "application/zip");
          request.Fields = "id";
          request.Upload();
      }
    }

    private static string BackupMhwSaveFiles(string steamInstallPath, string steamUserId, string mhwSteamAppId, string steamUsername, string backupFolderPath) {
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
        "Monster Hunter World"
      );

      return DirectoryCompress(mhwSaveFolder, backupFolderNowPath);
    }

    private static string DirectoryCompress (string sourceDirName, string destDirName) {
      DirectoryInfo dir = new DirectoryInfo(sourceDirName);

      if (!dir.Exists) {
        throw new DirectoryNotFoundException(
          $"Source directory does not exist or could not be found: {sourceDirName}"
        );
      }

      Directory.CreateDirectory(destDirName);

      string fileName = $"{Path.Combine(destDirName, DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"))}.zip";
      ZipFile.CreateFromDirectory(sourceDirName, fileName);
      return fileName;
    }

    private static void StartMhw(string mhwSteamAppId) {
      var mhwStartInfo = new ProcessStartInfo {
        FileName = $"steam://rungameid/{mhwSteamAppId}",
      };
      Process.Start(mhwStartInfo);
    }
  }
}
