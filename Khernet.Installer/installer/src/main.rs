#[macro_use]
extern crate lazy_static;
extern crate wildmatch;

use std::fs;
use std::path::{Path,PathBuf};
use std::process::Command;
use std::fs::{rename};
use std::env;
use crate::logger::Logger;
use crate::repository::get_current_executable_path;
use std::ffi::OsStr;

mod logger;
mod dotnet_installer;
mod repository;

const APP_VERSION:&str="0.15.12";
const APP_DIR:&str="khernet-app";
const INNER_APP_DIR_PREFIX:&str="app-";
const LAUNCHER_FILE:&str="khernetlauncher.exe";
const UPDATER_FILE:&str="Update.exe";

lazy_static!
{
    static ref LOGGER: logger::Logger =init_log();
}

/// Setup log file.
fn init_log()->Logger
{
    let mut app_log_path=get_current_executable_path();
    app_log_path.pop();
    app_log_path.push("KhernetSetup.log");
    logger::Logger{log_path:app_log_path}
}

/// Compile with these command to hide console window
/// cargo rustc -- -Clink-args="/SUBSYSTEM:WINDOWS /ENTRY:mainCRTStartup"
///
/// Compile for windows x86:
/// cargo rustc --target=i686-pc-windows-msvc -- -Clink-args="/SUBSYSTEM:WINDOWS /ENTRY:mainCRTStartup"
///
/// Compile for release
/// cargo rustc --release --target=i686-pc-windows-msvc -- -Clink-args="/SUBSYSTEM:WINDOWS /ENTRY:mainCRTStartup"
fn main() 
{
    let args:Vec<String>=env::args().collect();

    match args.len()
    {
        1 => run_application(),
        3=>
            {
                let file_name=&args[1];
                let mut destination_path=PathBuf::new();
                destination_path.push(&args[2]);
                extract_files(file_name.to_string(),destination_path.as_path());
            },
        _ => (),
    }
}

/// Installs the application if it is not. Otherwise it executes the launcher of main application.
fn run_application()
{
    // Build path for directory of application
    let mut app_directory= get_current_executable_path();
    app_directory.pop();
    app_directory.push(APP_DIR);

    // Create directory of application
    if !app_directory.exists()
    {
        if !create_directory(app_directory.as_path())
        {
            LOGGER.log("Application directory could not be created");
            return
        }
    }

    // Try to install dotnet 4.5
    let dotnet_version=match dotnet_installer::get_dotnet_version()
    {
        Ok(val) => val,
        Err(desc) =>
            {
                LOGGER.log(&format!("Dotnet framework not found: {}",desc));
                0
            },
    };

    // Verify if dotnet 4.5 is installed
    if dotnet_version<378389u32 // Version number
    {
        let installer_file=String::from("Setup.exe");
        let installer_path=extract_files(installer_file.to_string(),app_directory.as_path());
        if !installer_path.exists()
        {
            return
        }
        let mut install_process = match Command::new(app_directory.join(installer_file)).spawn()
        {
            Ok(child_process)=>child_process,
            Err(why)=>
                {
                    LOGGER.log(&format!("Error while installing dotnet 4.5.: {:?}",why));
                    return
                },
        };
        let _result =match install_process.wait()
        {
            Ok(res)=>res,
            Err(why)=>
                {
                    LOGGER.log(&format!("Dotnet 4.5 installation failed: {:?}",why));
                    return
                },
        };
    }

    // Extract native launcher
    let launcher_file=String::from(LAUNCHER_FILE);

    if !app_directory.join(&launcher_file).as_path().exists()
    {
        extract_files(launcher_file.to_string(),app_directory.as_path());
        LOGGER.log(&format!("{} native launcher installed successfully.",launcher_file));
    }
    if !app_directory.join(UPDATER_FILE).as_path().exists()
    {
        extract_files(UPDATER_FILE.to_string(),app_directory.as_path());
        LOGGER.log(&format!("{} installed successfully.",UPDATER_FILE));
    }

    // Create directory for dotnet launcher
    let dotnet_launcher_path =app_directory.join(format!("{}{}", INNER_APP_DIR_PREFIX, APP_VERSION)).as_path().to_owned();

    if !exists_inner_app_directory(&app_directory)
    {
        create_inner_app_directory(dotnet_launcher_path,&launcher_file);
    }

    // Run native launcher
    let mut _launcher_process = match Command::new(app_directory.join(launcher_file)).spawn()
    {
        Ok(child_process)=> child_process,
        Err(why)=>
            {
                LOGGER.log(&format!("Error while starting launcher: {:?}",why));
                return
            },
    };
}

/// Create the directory where launcher will be placed on.
fn create_inner_app_directory(dotnet_launcher_path:PathBuf, launcher_file:&String)
{
    if !dotnet_launcher_path.exists()
    {
        create_directory(&dotnet_launcher_path);
        LOGGER.log(&format!("Dotnet directory created at: {}",dotnet_launcher_path.display()));
    }

    // Extract dotnet launcher
    if !dotnet_launcher_path.join(&launcher_file).exists()
    {
        let dotnet_launcher_file=String::from(format!("_{}",LAUNCHER_FILE));
        extract_files(dotnet_launcher_file.to_string(),&dotnet_launcher_path);

        let _result=rename(dotnet_launcher_path.join(&dotnet_launcher_file).as_path(),
                           dotnet_launcher_path.join(&launcher_file).as_path());

        LOGGER.log(&format!("{} dotnet launcher installed successfully.",LAUNCHER_FILE));
    }
}

/// Creates a directory in the given path.
fn create_directory(path:&Path) -> bool
{
    match fs::create_dir(path)
    {
        Ok(_)=> true,
        Err(desc)=>
            {
                LOGGER.log(&format!("Create directory: {:?}",desc.kind()));
                false
            },
    }
}

/// Checks if exists the application directory where launcher will be placed on.
/// Directory will be named using the version number for instance: app-1.0.0
fn exists_inner_app_directory(dir_path:&PathBuf) -> bool
{
    if dir_path.is_dir()
    {
        let dir_reader=match fs::read_dir(dir_path)
        {
            Ok(r)=>r,
            Err(why)=>
                {
                    LOGGER.log(&format!("Error while reading directory: {}",why));
                    panic!("Error while reading directory: {}",why);
                },
        };

        for entry in dir_reader
        {
            let item=match entry
            {
                Ok(it)=>it,
                Err(why)=>
                    {
                        LOGGER.log(&format!("Error while reading file: {}",why));
                        continue;
                    },
            };
            let path=item.path();
            if !path.is_dir()
            {
                continue;
            }

            let exists_path=wildmatch::WildMatch::new(format!("{}{}", dir_path.display().to_string().as_str(),"app-*").as_str())
                .matches(path.display().to_string().as_str());

            if exists_path
            {
                return true;
            }
        }
    }
    return false
}

/// Extracts the embedded file and saves it to file system.
fn extract_files(file_name:String,path:&Path)->PathBuf
{
    let final_path=match repository::save_embedded_file(file_name,path)
    {
        Ok(file_path)=>file_path,
        Err(why)=>
            {
                // Get file name
                let os_file_name =match path.file_name()
                {
                    Some(f)=>f,
                    None => OsStr::new(""),
                };

                let f_name=match os_file_name.to_str()
                {
                    Some(f)=>f,
                    None=>"",
                };
                println!("Error while saving embedded file {}: {}",f_name,why);
                LOGGER.log(&format!("Error while saving embedded file {}: {}",path.file_name().unwrap().to_str().unwrap(),why));
                PathBuf::new()
            },
    };
    final_path
}