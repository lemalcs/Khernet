--- 
title: "Updating"
permalink: /docs/updating/
excerpt: "How to update Khernet."
last_modified_at: 2021-06-19
redirect_from:
  - /Khernet-updating/
toc: true
---

You can update the application at any time with just a single click.

**Note:** Starting with version 0.17.0.0 updates are not installed without user consent.
{: .notice--info}

There are two sources to get update file for Khernet:
1. **Online:** this is the recommended one, you will be notified when a new update is available and install it when you are ready with a single click. Internet connection is needed.
2. **Offline:** you need get the update file and set Khernet to install the update using that.

    **Caution:** For security reasons only get [update files](https://github.com/lemalcs/Khernet/releases) from a trusted source to avoid damage the application.
    {: .notice--warning}

## Online update

When a new update is available the **Update** button is showed at the top right corner of main window. Click to install update and the application will restart after installation.

![update-button]({{ "/assets/images/update-button.png" | relative_url }})

Be sure that you have Internet connection to download [updates](https://github.com/lemalcs/Khernet/releases).
{: .notice--info}


## Offline update

Useful when you are in an environment without internet access. 
You need a machine with internet access to get the update files in order to use them on other machines.

1. Download the following file from [latest release](https://github.com/lemalcs/Khernet/releases/latest).
   - `KhernetUpdate-*.*.*.*.exe`


    *Version number `*.*.*.*` changes with every update, for instance: 0.18.0.0*

2. Launch **Khernet.exe**, sign in a click on settings button.

    ![settings-button]({{ "/assets/images/settings-button.png" | relative_url }})

3. In the dialog click on **Updates** option.

    ![updates-option]({{ "/assets/images/updates-option.png" | relative_url }})

4. In the **Updates** dialog click on **Browse file** and select the update file you got in step one. Also you can type the path in **Path of update file** field.

    ![update-dialog]({{ "/assets/images/update-dialog.png" | relative_url }})
    ![select-update-file]({{ "/assets/images/select-update-file.png" | relative_url }})


5. Click **Update** button to install the updates.

    ![start-update]({{ "/assets/images/start-update.png" | relative_url }})

6. When update is installed Khernet will be restarted.


## Update from version 0.16.0

If you have version 0.16.0 you have to use the [installer](https://github.com/lemalcs/Khernet) to update to version 0.17.0.0.

1. Launch the installer and place the path where **Khernet.exe** files is located. Click **Browse** button to find the path:

    ![select-path-installer]({{ "/assets/images/select-path-installer.png" | relative_url }})

    To identify the right path be sure that **Khernet.exe** file weights 85 MB (87104 KB) and is next to a folder called **khernet-app** like in the figure:

    ![app-path]({{ "/assets/images/app-path.png" | relative_url }})

    In the example above the path is *C:/other*

2. After selected the path of application click **Next**.
3. In the **Select Start Menu Folder** type a name for shortcut or check the option **Don't create a Start Menu Folder**. Click **Next**.

    ![create-shortcuts]({{ "/assets/images/create-shortcuts.png" | relative_url }})

4. Check **Portable mode** if you want to install only the main application without an uninstaller otherwise left it unchecked. Click **Next**.

    ![portable-mode-option]({{ "/assets/images/portable-mode-option.png" | relative_url }})

5. Click **Install** to start installation.

    ![start-install]({{ "/assets/images/start-install.png" | relative_url }})

6. When installation is finished check **View Khernet.exe** to launch application otherwise left it unchecked. Click **Finish**.

    ![finish-install]({{ "/assets/images/finish-install.png" | relative_url }})

