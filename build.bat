@echo off

echo -------------------------------------------------------------
echo  Starting build...
echo -------------------------------------------------------------

rem Set this to Release or Debug values
set dotnetConfiguration=Debug

rem Build core
tools\nuget restore Khernet.Core\Khernet.Core.sln
msbuild Khernet.Core\Khernet.Core.sln -p:Platform=x86 /property:Configuration=%dotnetConfiguration%

rem Build user interface
tools\nuget.exe restore Khernet.UI\Khernet.UI.sln
msbuild Khernet.UI\Khernet.UI.sln -p:Platform=x86 /property:Configuration=%dotnetConfiguration%

rem Build installer with Inno Setup
cd Khernet.Installer
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" KhernetInstaller.iss

rem Copy final installer to bin directory
set mainApp_path=bin\KhernetInstaller-0.17.0.0.exe
set installerDest=..\bin\Khernet.Installer
if not exist %installerDest% mkdir %installerDest%
if exist %mainApp_path% copy %mainApp_path% %installerDest%

echo -------------------------------------------------------------
echo  Build finished.
echo -------------------------------------------------------------