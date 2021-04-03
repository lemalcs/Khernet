@echo off

rem Create directory for .Net 4.5 used for nuget package
set lib_directory=src\lib\net45
IF not exist %lib_directory% mkdir %lib_directory%

rem Create nuget package
%~dp0..\..\tools\nuget.exe pack src\dotnetInstaller.nuspec

rem Create release directory
set release_directory=release
IF not exist %release_directory% mkdir %release_directory%

rem Create squirrel installer (nuget package) 
%~dp0..\..\tools\squirrel.exe --releasify "dotnetInstaller.1.0.0.nupkg" --releaseDir=%release_directory% --no-msi

rem Copy final installer to bin directory
copy %release_directory%\Setup.exe %~dp0..\..\bin\Khernet.Installer