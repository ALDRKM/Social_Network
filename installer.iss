[Setup]
AppName=Social Network
AppVersion=1.0
DefaultDirName={localappdata}\SocialNetwork
DefaultGroupName=Social Network
OutputDir=.
OutputBaseFilename=SocialNetworkSetup
SetupIconFile=Social_Network\Social_Network\Resources\AppIcon\mainicon.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest
UsePreviousAppDir=no
DisableDirPage=no
UninstallDisplayIcon={app}\client\socialnetwork_color.ico

[Files]
Source: "publish\api\*"; DestDir: "{app}\api"; Flags: recursesubdirs ignoreversion
Source: "publish\client\*"; DestDir: "{app}\client"; Excludes: "mainicon.ico"; Flags: recursesubdirs ignoreversion
Source: "Social_Network\Social_Network\Resources\AppIcon\mainicon.ico"; DestDir: "{app}\client"; DestName: "socialnetwork_color.ico"; Flags: ignoreversion
Source: "publish\start.cmd"; DestDir: "{app}"; Flags: ignoreversion

[InstallDelete]
Type: files; Name: "{userdesktop}\Social Network.lnk"
Type: files; Name: "{group}\Social Network.lnk"
Type: files; Name: "{app}\client\mainicon.ico"

[Icons]
Name: "{group}\Social Network"; Filename: "{app}\start.cmd"; WorkingDir: "{app}"; IconFilename: "{app}\client\socialnetwork_color.ico"; IconIndex: 0
Name: "{userdesktop}\Social Network"; Filename: "{app}\start.cmd"; WorkingDir: "{app}"; IconFilename: "{app}\client\socialnetwork_color.ico"; IconIndex: 0

[Run]
Filename: "{app}\start.cmd"; Description: "Запустить Social Network"; Flags: nowait postinstall skipifsilent
