# Monster Hunter World Backup and Launch

Small tool that backups Steam user save data before launching the game.

## Usage

1. Compile the project, or use the compiled version found from [the latest
   release](https://github.com/yonguelink/mhw-backup-launcher/releases/latest) or directly
   from
   [here](https://github.com/yonguelink/mhw-backup-launcher/releases/latest/download/mhw-backup-launcher.exe)
1. The executable takes four arguments:

    1. Steam folder complete path (to the parent folder of the folder `userdata` (e.g.:
       `C:/Program Files/Steam`))
    1. Your Steam User ID (You can find your User ID in the `userdata` folder, there
       should be only one folder named with a bunch of numbers, that's your ID)
    1. Your Steam Username (That's not actually tied to Steam, you can use whatever you'd
       like, this is here to help you identify what savefile is in the backups)
    1. Complete path to where you want the backups to be stored to (e.g.: `C:/Backups`)

    NOTE: For simplicity, all paths can be defined with either forward slashes (`/`) OR
    double-backslashes (`\\`). The program deals with both format.

1. You can run the executable using CMD or create a shortcut

### How To Create the shortcut

1. Create a new shortcut pointing to the compiled EXE of this tool
    * Right click on the EXE and click `Create Shortcut`
1. Right click on the shortcut and click on `Properties`
1. Inside the `Properties`, in the `Target` box append the arguments explained above

    * If any of your path contains spaces you will need to quote them like so
    * An exemple

        `C:\mhw-backup-launcher.exe "C:/Program Files/Steam" 1234567890 YourUsername
        C:/Backups`

## Releasing

The release process happens with GitHub's `hub` CLI.

1. Build the project in `Release` mode
1. Write up your release title in `RELEASE.md`
1. Add an empty line after your title
1. Add whatever message you want
1. Create the release in GitHub

    ```sh
    hub release create -oa mhw-backup-launcher/bin/Release/mhw-backup-launcher.exe -F RELEASE.md v1.0.0
    ```

1. A browser page will open, confirm the release happened correctly and you're done!
