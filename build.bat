@echo off

rem Build core
tools\nuget restore Khernet.Core\Khernet.Core.sln
msbuild Khernet.Core\Khernet.Core.sln -p:Platform=x86

rem Build user interface
tools\nuget.exe restore Khernet.UI\Khernet.UI.sln
msbuild Khernet.UI\Khernet.UI.sln -p:Platform=x86

rem Build launcher
tools\nuget.exe restore Khernet.Installer\Khernet.Installer.Launcher\Khernet.Installer.Launcher.sln
msbuild Khernet.Installer\Khernet.Installer.Launcher\Khernet.Installer.Launcher.sln -p:Platform=x86

rem Create dotnet installer
cd Khernet.Installer\dotnetInstaller
call create_dotnet_installer.bat

rem Create native launcher
cd ..\nativeLauncher
call generate_native_launcher.bat

rem Build installer
cd ..\..\Khernet.Installer\installer
cargo rustc --release --target=i686-pc-windows-msvc -- -Clink-args="/SUBSYSTEM:WINDOWS /ENTRY:mainCRTStartup resources.res"

rem Copy final installer to bin directory
set mainApp_path=target\i686-pc-windows-msvc\release\Khernet.exe
if exist %mainApp_path% copy %mainApp_path% ..\..\bin\