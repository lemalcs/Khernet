---
layout: splash
permalink: /
hidden: true
classes: wide
title: " "
intro:
  - excerpt: <p style="margin-top:35px; font-size:30px;font-weight:lighter;color:#6244BB">Why use Khernet to chat?</p>

feature_row2:
  - image_path: /assets/images/app-installer.png
    alt: "App installation"
    title: "Easy to start"
    excerpt: "Just click the application and start to chat."
    url: "/docs/installation/"     
feature_row3:
  - image_path: /assets/images/text-emoji.png
    alt: "Send text messages"
    title: "Send text messages"
    excerpt: "Messages can contain emojis 😃 or just plain text."
    url: "/docs/text-message/"    
feature_row4:
  - image_path: /assets/images/file-message.png
    alt: "Send files"
    title: "Send any type of files"
    excerpt: "Share images, audios , videos or any binary file you wish. GIF files are also supported."
    url: "/docs/file-message/" 
feature_row5:
  - image_path: /assets/images/text-markdown.png
    alt: "Markdown message"
    title: "Markdown format"
    excerpt: "Give a rich format to your text messages with Markdown."
    url: "/docs/markdown-message/"
feature_row6:
  - image_path: /assets/images/secure-app.png
    alt: "Encryption"
    title: "End to end encryption"
    excerpt: "All messages are end to end encrypted to keep you away from prying eyes."
feature_row7:
  - image_path: /assets/images/source-code.png
    alt: "Source code"
    title: "Source code"
    excerpt: "Want to review how application works internally? Feel free to take a look at [source code](https://github.com/lemalcs/Khernet)."
    url: "/docs/development/"
---


<div style="border-bottom: 1px solid #f2f3f3">
  <div style="display:inline-block;min-height: 400px;padding:20px;padding-top:150px;padding-bottom:200px;vertical-align:middle">
    <h1 style="font-size: 60px">Stay in touch<br />without servers</h1>

    <p>The secure standalone chat application made for LAN.</p>

    <a class="btn btn--info"
      href="https://github.com/lemalcs/Khernet/releases/download/v0.20.0.0/KhernetInstaller-0.20.0.0.exe">
      Download for Windows
    </a>
    <br/>
    <br/>
    <a href="https://github.com/lemalcs/Khernet/releases/download/v0.20.0.0/Khernet.exe" class="btn">Get portable application</a>
  </div>


  <div style="display:inline-block;vertical-align:middle;padding:50px 0px 50px 40px:width=400px">
    <img src="{{ '/assets/images/main-window.png' | relative_url }}" alt="Main window"/>
  </div>
</div>



{% include feature_row id="intro" type="center" %}

{% include feature_row id="feature_row2" type="left"%}

{% include feature_row id="feature_row3" type="right"%}

{% include feature_row id="feature_row4" type="left"%}

{% include feature_row id="feature_row5" type="right"%}

{% include feature_row id="feature_row6" type="left"%}

{% include feature_row id="feature_row7" type="center"%}
