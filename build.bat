@echo off

rem Run this script from the Developer Command Prompt for Visual Studio

echo -------------------------------------------------------------
echo  Starting build...
echo -------------------------------------------------------------

rem Set this to Release or Debug values
set dotnetConfiguration=Release

rem Build Khernet
Resources\nuget.exe restore src\Khernet.sln
rem msbuild Khernet.UI\Khernet.sln -p:Platform=x86 /property:Configuration=%dotnetConfi#guration%
set ROOT=%~dp0

set outputPath=%ROOT%bin

if not exist %outputPath% mkdir %outputPath%

msbuild src\Khernet.sln -p:Platform=x86 -p:Configuration=%dotnetConfiguration% -p:OutputPath=%outputPath%

rem Build installer with Inno Setup
cd src\Installer
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" KhernetInstaller.iss

rem Copy final installer to bin directory
set mainApp_path=%ROOT%src\Installer\bin\KhernetInstaller-0.23.0.0.exe

if exist %mainApp_path% copy %mainApp_path% %outputPath%

cd %ROOT%

echo -------------------------------------------------------------
echo  Build finished.
echo -------------------------------------------------------------
