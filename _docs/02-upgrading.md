--- 
title: "Upgrading"
permalink: /docs/upgrading/
excerpt: "How to quickly install and setup Minimal Mistakes for use with GitHub Pages."
last_modified_at: 2021-06-19
redirect_from:
  - /theme-setup/
toc: true
---

There are two ways to upgrade Khernet:
1. **Online:** no manual intervention needed.
2. **Offline:** upgrade files have to be downloaded manually.

## Online upgrading

This is the default behavior.

Application updates itself if there is internet access so you don't have to do anything.

> Updates are downloaded from [GitHub](https://github.com/lemalcs/Khernet)

## Offline upgrading

Useful when you are in an environment without internet access. 
You need a machine with internet access to get the update files in order to use them on other machines.

1. Download the following files from [latest release](https://github.com/lemalcs/Khernet).
   - `Khernet-*.*.*-full.nupkg`
   - `Khernet-*.*.*-delta.nupkg`
   - `RELEASES`


    *Version number `*.*.*` changes with every update, for instance: 0.16.1.*

2. Create a folder named **khernetupgrade** in the same folder where **Khernet.exe** executable is located.
3. Place the files (downloaded on step one) within the folder created on step two.
4. Launch **Khernet.exe** and wait till the main screen appears. If update files contain a newer version then the application is updated.