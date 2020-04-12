using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace mhw_backup_launcher {
  internal static class Launcher {
    private const string DefaultThudPath = @"D:\THUD\THUD.exe";
    private static void Main(string[] args) {
      //Default path for my PC + allow anyone to cmd the sh!t out of these
      //Param 1 = THUD Path (including exe)
      //Param 2 = D3 Path (including exe)
      //Param 3 = User name to run Diablo
      //Param 4 = Password for the user that'll run Diablo
      var thudPath = args.Length > 0 ? args[0] : DefaultThudPath;

      //Make sure we won't try to launch something that does not exists
      if (!File.Exists(thudPath)) throw new FileNotFoundException(thudPath);
      //Starts THUD
      StartThud(thudPath);
    }

    private static void StartThud(string thudPath) {
      //Give 5 second to start D3, even if it takes more like 10
      Thread.Sleep(5000);
      var workingDirectory = new FileInfo(thudPath).DirectoryName;
      if (workingDirectory == null) throw new FileNotFoundException(thudPath);

      var thudStartInfo = new ProcessStartInfo
      {
          FileName = thudPath,
          WorkingDirectory = workingDirectory,
          UseShellExecute = false,
          LoadUserProfile = true,
          Verb = "runas"
      };
      Process.Start(thudStartInfo);
    }
  }
}
