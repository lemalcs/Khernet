; Installer for Khernet

; Inno Download Plugin
; https://code.google.com/p/inno-download-plugin/
; Uncomment the following line of plugin is not installed on ISPPBuiltins.iss file.
; #pragma include __INCLUDE__ + ";" + "C:\Path-To-Inno-Setup-Download-Plugin"
#include <idp.iss>

#define ApplicationName "Khernet"
#define CurrentVersion "0.18.0.0"
#define AppDirectoryName "khernet-app"
#define SQLScript "SAVE_TEXT_MESSAGE.sql"
#define IzarcDirectory "izarc"
#define IzarcExec "IZARCE.exe"
#define FirebirdDirectory "firebird"
#define FirebirdCompressed "Firebird-3.0-x86.zip"
#define IsqlName "isql.exe"
#define ScriptDirectory "Scripts"

[Setup]
AppName={#ApplicationName}
AppVersion={#CurrentVersion}
VersionInfoCopyright=Copyright © 2021
VersionInfoVersion={#CurrentVersion}
VersionInfoProductVersion={#CurrentVersion}
WizardStyle=modern
DefaultDirName={#ApplicationName}
DefaultGroupName={#ApplicationName}
Compression=lzma2
SolidCompression=yes
OutputDir=bin
PrivilegesRequired=lowest
DisableWelcomePage=yes
DisableReadyMemo=yes
DisableProgramGroupPage=no
DisableDirPage=no
DisableStartupPrompt=yes
DisableFinishedPage=no
DisableReadyPage=no
OutputBaseFilename="{#ApplicationName}Installer-{#CurrentVersion}"
AlwaysShowDirOnReadyPage=yes
AllowNoIcons=yes
DirExistsWarning=no
CloseApplications=force
RestartApplications=no
Uninstallable=not WizardIsTaskSelected('portablemode')
WizardImageFile=images\large-logo.bmp
WizardSmallImageFile=images\logo.bmp

; Additional Space required for third party libraries.
ExtraDiskSpaceRequired=162806467


[Tasks]
Name: portablemode; Description: "Portable Mode"


[Icons]
Name: "{group}\{#ApplicationName}"; Filename: "{app}\{#ApplicationName}.exe"; WorkingDir: "{app}"
Name: "{group}\Uninstall {#ApplicationName}"; Filename: "{uninstallexe}"

[Dirs]
Name: {tmp}\{#FirebirdDirectory}; Flags: deleteafterinstall;
Name: {tmp}\{#ScriptDirectory}; Flags: deleteafterinstall;
Name: {tmp}\{#IzarcDirectory}; Flags: deleteafterinstall;


[Files]
; Remove old versions of main executable.
Source: "..\bin\Khernet.UI\{#ApplicationName}.exe"; Check:FileExists(ExpandConstant('{app}\{#AppDirectoryName}\app\{#ApplicationName}.exe')); DestDir: "{app}\{#AppDirectoryName}\app"; AfterInstall:RemoveFile(ExpandConstant('{app}\{#AppDirectoryName}\app\{#ApplicationName}.exe'));

; Install the new main executable.
Source: "..\bin\Khernet.UI\{#ApplicationName}.exe"; DestDir: "{app}"; AfterInstall:DeployFileSystem(ExpandConstant('{app}\{#AppDirectoryName}'));

; Install compress utility
Source: "..\Resources\izarc\cabinet.dll"; DestDir:"{tmp}\{#IzarcDirectory}"; Flags: deleteafterinstall;
Source: "..\Resources\izarc\unacev2.dll"; DestDir:"{tmp}\{#IzarcDirectory}"; Flags: deleteafterinstall;
Source: "..\Resources\izarc\unrar3.dll"; DestDir:"{tmp}\{#IzarcDirectory}"; Flags: deleteafterinstall;
Source: "..\Resources\izarc\{#IzarcExec}"; DestDir:"{tmp}\{#IzarcDirectory}"; Flags: deleteafterinstall;

; Modify database
Source: "..\Database\KH\Procedures\{#SQLScript}"; DestDir: "{tmp}\{#ScriptDirectory}"; Flags: deleteafterinstall;
Source: "..\Resources\{#FirebirdCompressed}"; DestDir:"{tmp}"; AfterInstall:ExecuteDatabaseScript(ExpandConstant('{app}\{#AppDirectoryName}\data\msgdb')); Flags: deleteafterinstall;


[Run]
; This runs the application when the installation finishes.
Filename: "{app}\{#ApplicationName}.exe"; Flags: shellexec postinstall nowait;


[Code]

var installed: Boolean; // Indicates whether .NET Framework was successfully installed or not
    rescode: Integer; // The result code of .NET Framework installation
    dotnetInstallerFile: String; // The path of .NET Framework installer on local file system

function InitializeSetup(): Boolean;
begin
  if not IsDotNetInstalled(net452, 0) then 
  begin
    if not IsAdmin() then
    begin
      MsgBox('Khernet requires .NET Framework 4.5.2, please re-run the installer with administrative privilegies.', mbInformation, MB_OK); 
      result := false; 
    end
    else
    begin
      dotnetInstallerFile := '{tmp}\NetFramework452Installer.exe';
      result := true;
    end
  end 
  else
    result := true;          
end;    


// Installs the .NET Framework.
procedure InstallFramework;
var
  StatusText: string;
  ResultCode: Integer;
begin
  StatusText := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := 'Installing .NET Framework 4.5.2. This might take a few minutes...';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  try
    if not Exec(ExpandConstant(dotnetInstallerFile), '/passive /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
    begin
      // Result code 740 = Administrative privileges required
      rescode := Resultcode;
    end
    else 
         installed := true;;
  finally
    WizardForm.StatusLabel.Caption := StatusText;
    WizardForm.ProgressGauge.Style := npbstNormal;
    DeleteFile(ExpandConstant(dotnetInstallerFile));
  end;
end;


function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  if not IsDotNetInstalled(net452, 0) then
  begin
    installed := false;
    InstallFramework();
        
    if installed then
    begin
      result := ''; // Successful installation
    end
    else // Failed installation
      result := 'Installation of .NET 4.5.2 failed with error: '+SysErrorMessage(rescode)+' code: '+IntToStr(rescode)+''#13#13
      'Make sure you have administrative privilegies and internet connection.';
  end;
end;

// Opens a link in the browser.
procedure URLText_OnClick(Sender: TObject);
var
  ErrorCode: Integer;
begin
  if Sender is TNewStaticText then
    ShellExec('open', TNewStaticText(Sender).Caption, '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
end;


// Adds additional information to finish page.
procedure BuildFinishPage;
var
  HostPage: TNewNotebookPage;
  URLText, TmpLabel: TNewStaticText;
begin
  HostPage := WizardForm.FinishedPage;
   
  TmpLabel := TNewStaticText.Create(HostPage);
  TmpLabel.Parent := HostPage;
  TmpLabel.Top := 185;
  TmpLabel.Left := 200;
  TmpLabel.AutoSize := True;
  TmpLabel.Caption := 'Home:';

  URLText := TNewStaticText.Create(HostPage);
  URLText.Parent := HostPage;
  URLText.Top := 200;
  URLText.AutoSize := True;
  URLText.Caption := 'https://khernet.app';
  URLText.Cursor := crHand;
  URLText.Font.Color := clBlue;
  URLText.OnClick := @URLText_OnClick;
end;


// Event fired just before the wizard is shown.
procedure InitializeWizard;
begin
  if not IsDotNetInstalled(net452, 0) then
  begin
    if IsAdmin() then
    begin 
      idpAddFile('https://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe', ExpandConstant(dotnetInstallerFile));
      idpDownloadAfter(wpReady);
    end
  end;
  BuildFinishPage;
end;


// Deletes a file.
procedure RemoveFile(filePath: String);
begin
  if not DeleteFile(filePath) then
  begin
    Log('Cannot delete '+ filePath);
  end;
end;


// Renames a file or directory.
procedure RenameFileDirectory(oldName: String; newName: String);
begin
  if not RenameFile(oldName, newName) then
  begin
    Log('Cannot rename: ' + oldName);
  end;
end;


// Installs the files used by the new version and modifies the ones of the old version.
procedure DeployFileSystem(homeDirectoryPath: String);
var
  configFile: String;
  messageDB: String;
  fileDB: String;
  appDirectory: String;
  dataDirectory: String;
  versionDirectory: String;
  packDirectory: String;
  updateLog: String;
  renameSuffix: String;
  appLauncher: String;        
  oldVersionDirectory: String;
begin
  configFile := 'Khernet.dat';
  messageDB := 'msgdb';
  fileDB := 'stgdb';
  appDirectory := 'app';
  dataDirectory := 'data';
  versionDirectory := 'app-0.16.0';
  packDirectory := 'packages';
  updateLog := 'KhernetUpdate.log';
  renameSuffix := '.old';
  appLauncher := 'khernetlauncher.exe'; 

  RenameFileDirectory(homeDirectoryPath + '\' + appDirectory + '\' + configFile, homeDirectoryPath + '\' + configFile);

  if not CreateDir(homeDirectoryPath + '\' + dataDirectory ) then
  begin
    Log('Cannot create directory ' + homeDirectoryPath + '\' + dataDirectory);
  end;

  RenameFileDirectory(homeDirectoryPath + '\' + appDirectory + '\' + dataDirectory + '\' + messageDB, homeDirectoryPath + '\' + dataDirectory + '\' + messageDB);
  RenameFileDirectory(homeDirectoryPath + '\' + appDirectory + '\' + dataDirectory + '\' + fileDB, homeDirectoryPath + '\' + dataDirectory + '\' + fileDB);

  RemoveFile(homeDirectoryPath + '\' + appLauncher);
  RemoveFile(homeDirectoryPath + '\' + versionDirectory + '\' + appLauncher);
  RemoveFile(homeDirectoryPath + '\Update.exe');

  // Move files of old version to a is-[random-characters].old directory
  if FileExists(homeDirectoryPath + '\' + updateLog) 
     or DirExists(homeDirectoryPath + '\' + appDirectory) 
     or DirExists(homeDirectoryPath + '\' + versionDirectory) 
     or DirExists(homeDirectoryPath + '\' + packDirectory) then
  begin  
    oldVersionDirectory := GenerateUniqueName(homeDirectoryPath,'.old');
    if not CreateDir(oldVersionDirectory) then
    begin
      Log('Cannot create directory ' + oldVersionDirectory);
    end; 

    RenameFileDirectory(homeDirectoryPath + '\' + updateLog, oldVersionDirectory + '\' + updateLog + renameSuffix);
    RenameFileDirectory(homeDirectoryPath + '\' + appDirectory, oldVersionDirectory + '\' + appDirectory + renameSuffix);
    RenameFileDirectory(homeDirectoryPath + '\' + versionDirectory, oldVersionDirectory + '\' + versionDirectory + renameSuffix);
    RenameFileDirectory(homeDirectoryPath + '\' + packDirectory, oldVersionDirectory + '\' + packDirectory + renameSuffix);
  end         
end;

// Uncompress the Firebird database engine
function UncompressDatabaseEngine(compressor: String; enginePath: String): Boolean;
var
  ResultCode: Integer;
begin
  if not Exec(compressor, ' -e -d ' + enginePath + ' -p'+ExpandConstant('{tmp}\{#FirebirdDirectory}'), '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    MsgBox('Error while installing database change ' + IntToStr(Resultcode) + ': ' + SysErrorMessage(ResultCode), mbInformation, MB_OK); 
    result:= false;
  end
  else
    result:=true;
end;

// Executes script to modify database.
procedure ExecuteDatabaseScript(databasePath: String);
var
  ResultCode: Integer;
begin
  if UncompressDatabaseEngine(ExpandConstant('{tmp}\{#IzarcDirectory}\{#IzarcExec}'), ExpandConstant('{tmp}\{#FirebirdCompressed}')) then
  begin
    if not Exec(ExpandConstant('{tmp}\{#FirebirdDirectory}\{#IsqlName}'), databasePath + ' -user SYSDBA -password blank -i ' + ExpandConstant('{tmp}\{#ScriptDirectory}\{#SQLScript}'), '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      MsgBox('Error while installing database change ' + IntToStr(Resultcode) + ': ' + SysErrorMessage(ResultCode), mbInformation, MB_OK); 
    end;
  end;
end;
