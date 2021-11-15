---
title: "Installation"
permalink: /docs/installation/
excerpt: "How to install Khernet."
last_modified_at: 2021-11-14
redirect_from:
  - /Khernet-installation/
toc: true
---

There are 2 modes of installation:
- **Portable mode:** no installer wizard.
- **Installed mode:** uses a wizard where additional features are available.

Supported operating systems:
- Windows Vista SP2 (KB2533623)
- Windows 7
- Windows 8
- Windows 8.1
- Windows 10 (not RT)

## Portable mode
This mode does not use a wizard based installer just copy the main executable to any location you want and start it.

Useful when you don't need the application to create things outside the folder where it is placed, for instance when you need to carry it in a flash drive.

**Main executable file name:** `Khernet.exe`


### Prerequisites

- .NET Framework 4.5.2 or later must exists on system beforehand. [Offline installer](https://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe) is recommended to avoid compatibility issues.
- 240 MB of available space.


## Installed mode
A wizard is used to install this application where the following features are available:
- Creation of shortcuts on start menu (optional).
- Specify a custom location to install application.
- Uninstaller is created automatically when portable mode is not selected.
- Portable mode is available where no shortcuts and uninstaller are created (optional).
- .NET Framework is installed automatically if your system does not have it or an earlier version exists (Internet connection needed to download [installer]((https://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe))).

**Installer name:** `KhernetInstaller-*.*.*.*.exe`

`*.*.*.*` : stands for version number, for instance: 0.18.0.0

<div class="notice--success" markdown="1">
  <h4 class="no_toc"><i class="fas fa-lightbulb"></i> Tip</h4>
  File **Khernet.exe** created by the installer is the portable version of application thereby you can copy and distribute this file to other machines or even other locations of the same machine and launch another instance of Khernet.
</div>


### Prerequisites

- Administrative privileges **only** if .NET Framework 4.5.2 or later is not installed.
- 241 MB of available space. Up to 1 GB of additional space is needed if .NET Framework is not installed.


## Account creation

1. Start the application **Khernet** clicking the executable file of using start menu shortcut if created.

    If a message pops up saying that application have to be executed with administrator privileges 
    please restart the application as administrator otherwise it won't be able to communicate with peers over network.
    {: .notice--info}


2. The main screen will appear asking you to create an account. Type an user name and password.

    ![create-account]({{ "/assets/images/create-account.png" | relative_url }})



The next time you start the application type your username and password to sign in to.

![sign-in]({{ "/assets/images/sign-in.png" | relative_url }})

**Caution:** Do not lose password otherwise you won't be able to access to application and thereby lose your chats too.
{: .notice--warning}