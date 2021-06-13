@echo off

rem Create directory for .Net 4.5 used for nuget package
set lib_directory=src\lib\net45
IF not exist %lib_directory% mkdir %lib_directory%

rem Get dotnet launcher
copy %~dp0..\..\bin\Khernet.Installer\_khernetlauncher.exe %lib_directory%\khernetlauncher.exe

rem Create nuget package
%~dp0..\..\tools\nuget.exe pack src\Khernet.Installer.Launcher.nuspec

rem Create release directory
set release_directory=release
IF not exist %release_directory% mkdir %release_directory%

rem Create squirrel installer (nuget package) 
%~dp0..\..\tools\squirrel.exe --releasify "Khernet.Installer.Launcher.0.14.4.nupkg" --releaseDir=%release_directory% --no-msi

rem Extract files from nuget package generated by squirrel
%~dp0..\..\tools\7z.exe e %release_directory%\Khernet.Installer.Launcher-0.14.4-full.nupkg -o%release_directory% khernetlauncher_ExecutionStub.exe -r -y

rem Copy stub executable (native launcher) to bin directory
copy %release_directory%\khernetlauncher_ExecutionStub.exe %~dp0..\..\bin\Khernet.Installer\khernetlauncher.exe

rem Extract updater file used by squirrel framework
%~dp0..\..\tools\7z.exe e %release_directory%\Setup.exe -o%release_directory% Update.exe -r -y

rem Copy updater to bin directory
copy %release_directory%\Update.exe %~dp0..\..\bin\Khernet.Installer\