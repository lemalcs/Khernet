use std::fs;
use std::process::Command;
use std::path::Path;

/// The path of application to compress and decompress files
const COMPRESSOR : &str="..\\..\\tools\\7z.exe";

fn main()
{
    let native_launcher="khernetlauncher.exe";
    let updater="Update.exe";
    let dotnet_launcher="_khernetlauncher.exe";
    let dotnet_installer="Setup.exe";
    let main_app="Khernet.exe";

    let source_dotnet_launcher_path=String::from(format!("{}\\{}","..\\..\\bin\\Khernet.Installer",&dotnet_launcher));
    let source_main_app_path=String::from(format!("{}\\{}","..\\..\\bin\\Khernet.UI",&main_app));
    let source_native_launcher_path=String::from(format!("{}\\{}","..\\..\\bin\\Khernet.Installer",&native_launcher));
    let source_updater_path=String::from(format!("{}\\{}","..\\..\\bin\\Khernet.Installer",&updater));
    let source_dotnet_installer_path=String::from(format!("{}\\{}","..\\..\\bin\\Khernet.Installer",&dotnet_installer));
    let source_icon_path=String::from("..\\..\\Khernet.UI\\Khernet.UI.Presentation\\LogoIcon.ico");

    // Download again files when there is a changed in some of the following paths
    println!("cargo:rerun-if-changed={0}",source_dotnet_launcher_path);
    println!("cargo:rerun-if-changed={0}",source_main_app_path);
    println!("cargo:rerun-if-changed={0}",source_native_launcher_path);
    println!("cargo:rerun-if-changed={0}",source_updater_path);
    println!("cargo:rerun-if-changed={0}",source_dotnet_installer_path);
    println!("cargo:rerun-if-changed={0}",source_icon_path);

    // Folder for files that will be embedded into main executable
    let assets_folder ="bag";

    let assets_path =Path::new(&assets_folder);
    if !assets_path.exists()
    {
        match fs::create_dir(assets_path)
        {
            Ok(_)=>println!("Assets folder created successfully."),
            Err(reason)=>panic!("Error while creating assets folder: {}",reason),
        }
    }

    download_file(&source_dotnet_launcher_path,format!("{}\\{}", &assets_folder, &dotnet_launcher).as_str());
    download_file(&source_main_app_path,format!("{}\\{}", &assets_folder, &main_app).as_str());
    download_file(&source_native_launcher_path,format!("{}\\{}", &assets_folder, &native_launcher).as_str());
    download_file(&source_updater_path,format!("{}\\{}", &assets_folder, &updater).as_str());
    download_file(&source_dotnet_installer_path,format!("{}\\{}", &assets_folder, &dotnet_installer).as_str());

    download_and_zip_file("..\\..\\Khernet.UI\\packages\\Khernet.UI.Lib.1.0.0\\build\\libvlc\\*",
                          format!("{}\\{}", &assets_folder, "libvlc_win-x86.zip").as_str());
    download_and_zip_file("..\\..\\Khernet.UI\\packages\\Khernet.UI.Lib.1.0.0\\build\\media\\*",
                          format!("{}\\{}", &assets_folder, "media.zip").as_str());
    download_and_zip_file("..\\..\\Khernet.UI\\packages\\Khernet.UI.Lib.1.0.0\\build\\firebird\\*",
                          format!("{}\\{}", &assets_folder, "firebird.zip").as_str());

    // Compile resources file for icon of application
    let mut res = winres::WindowsResource::new();
    res.set_icon(&source_icon_path);

    // Version must be equals to APP_VERSION variable located in main.rs
    res.set_version_info(winres::VersionInfo::FILEVERSION, 0x0000000F00040000);

    match res.compile()
    {
        Ok(_)=> println!("Icon added successfully."),
        Err(reason)=> panic!("Error while adding icon {} : {}",source_icon_path,reason),
    }
}

/// Downloads a file from a remote location to a local path.
fn download_file(remote_path:&str, local_path:&str)
{
    match fs::copy(remote_path, local_path)
    {
        Ok(_)=> println!("\"{}\" copied.",local_path),
        Err(reason)=> panic!("Error while copying \"{}\": {}", remote_path, reason),
    }
}

/// Downloads a file from a remote location, saves to local path and compress it with ZIP format.
fn download_and_zip_file(remote_path:&str, local_path:&str)
{
    match Command::new(&COMPRESSOR)
        .args(&["a",local_path,&remote_path])
        .output()
    {
        Ok(_)=>println!("\"{}\" packed successfully",local_path),
        Err(reason)=> panic!("Error while packing \"{}\": {}", remote_path, reason),
    }
}