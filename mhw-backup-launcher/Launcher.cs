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
    private const int MHW_STEAM_APP_ID = 582010;
    private static void Main(string[] args) {
      //Starts THUD
      StartMhw(MHW_STEAM_APP_ID);
    }

    private static void StartMhw(int mhwSteamAppId) {
      var mhwStartInfo = new ProcessStartInfo
      {
          FileName = $"steam://rungameid/{mhwSteamAppId}",
      };
      Process.Start(mhwStartInfo);
    }
  }
}
