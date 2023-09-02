![Logo](Logo.png)
# **Khernet**

Khernet is a standalone chat application for LAN, just click the application and start to chat.

## Main features

- Portable mode and installed mode.
- No centralized server.
- Use emojis on contact names 🙂.
- End to end encryption.
- Supported types of messages:

  - Text (includes emojis)
  - Markdown (includes emojis)
  - Image
  - GIF
  - Audio
  - Video
  - Any binary file
  
- Online and offline updates (for restricted environments 🤐)

> You can send and receive files with a size up to 2 GB.

## How to distinguish portable application from installer?

The portable application (portable mode) does not have a wizard based installer and can be launched directly from executable file named:

`Khernet.exe`

The installer (installed mode) launches a wizard to install the application and it's named:

`KhernetInstaller-*.*.*.*.exe`

`*.*.*.*` : stands for version number, for instance: 0.18.0.0

It offers the following features over the portable mode:
- Creation of shortcuts on start menu.
- Creation of uninstaller (when portable mode is not selected).
- .NET Framework (see Prerequisites) is installed automatically if your system does not have it or no later version exists (Internet connection needed to download [installer]((https://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe))).


## Prerequisites

- .NET Framework 4.5.2 or later.
- 241 MB of available space.

## Supported systems
- Windows Vista SP2 (KB2533623)
- Windows 7
- Windows 8
- Windows 8.1
- Windows 10 (not RT)
- Windows 11

## Contributions

This application is made to help people keep in touch with their peers,
why not help them to make it even better and easier.
These are some contributions you can do:

- Fix a bug.
- Code a new feature.
- Improve documentation.
- Give the application a face wash.

To edit documentation go to **gh-pages** branch of this repository and find the project website made with [Jekyll](https://jekyllrb.com/) static content generator.


## How to build

1. Clone this repository:
   ```
   git clone https://github.com/lemalcs/Khernet.git
   ```
3. Go to the folder where you cloned repository.
2. Execute **build.bat** in [Visual Studio Command Prompt](https://docs.microsoft.com/en-us/visualstudio/ide/reference/command-prompt-powershell?view=vs-2019).

> Nuget packages are available on [nuget](https://www.nuget.org) and attached in the [latest release](https://github.com/lemalcs/Khernet/releases).
