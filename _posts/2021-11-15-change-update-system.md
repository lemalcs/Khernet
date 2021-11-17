---
title: "Improved update system for Khernet"
last_modified_at: 2021-11-17
categories:
  - Blog
tags:
  - Post Formats
  - readability
  - standard
---

Despite some users avoid to update their applications because of fear to left them unstable, an update system is a major feature of any software that wants to evolve and get better every time (don't be afraid, updates are your friends 🙂).

Of course Khernet was released with the capability to get updates from extern or local sources but it was some tricky to install them.

## Antivirus security alerts

Automatic updates was enabled by default and couldn't be disabled unless you didn't have an internet connection but the technic used to accomplish this produced false positives raised by certain antivirus software and security alerts every time the application was launched 😮. 

Some messages showed by an antivirus could be:
- Suspicious activity.
- Blocked access to internet for Khernet.
- Network activity detected by a not certified or signed application.

Maybe you already had to tell the antivirus that let the application to launch because you know that Khernet is harmless.


## Workaround for false positives 

Antivirus false positives was a serious issue because you couldn't launch Khernet unless you go to the inner folder where the actual application was located (Khernet file system is detailed in the next section). 

The above was only possible to do if you installed the application before an antivirus software was installed (or it was more permissive) otherwise you couldn't even launch the main executable and never see the main window.

## Khernet file system

There are two **Khernet.exe** files that are launched one after another:

- **Main executable:** named **Khernet.exe** which is the installer and launcher of the actual application.

- **Actual application:** named **Khernet.exe** which is the actual windows where chats are displayed and sent.

Every time you clicked the former the latter was the window that you ended up seeing and using for chatting 😉... and yes it can be a little confusing but let's explain that:

If the application was installed in the following path:

`C:\mychat`

Then the full path of *main executable* is:

`C:\mychat\Khernet.exe`


The *actual application* is located at:

`C:\mychat\khernet-app\app`

So the full path is:

`C:\mychat\khernet-app\app\Khernet.exe`


This was the result of the update system used for Khernet and as seen before this can lead to issues on some environments and it does not offer the best user experience.

## Changing updates

It was necessary to implement a new update system for Khernet and now it is done! it can be updated with a single click and you decide when to launch the installation. More details in [Updating]({{ "/docs/updating/" | relative_url }}). 

You can get the update files from internet or a file already downloaded (please only get update files from a trusted source) it's a useful feature when your machine does not have internet access.

[Version 0.16.0]({{ "/docs/Version-0160/" | relative_url }}) of Khernet won't receive new updates so you have to [update to version 0.17.0.0]({{ "/docs/updating/#update-from-version-0160" | relative_url }}). 

We hope that you enjoy this new version and let us know what can be improved for future releases 🖐.

## Feedback requested

🤔... By the way I'm starting to think that I should sign Khernet with a certificate. What do you think?