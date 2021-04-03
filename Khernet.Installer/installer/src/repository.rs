extern crate rust_embed;

use rust_embed::RustEmbed;
use std::path::Path;
use std::path::PathBuf;
use std::fs::File;
use std::io::prelude::*;
use std::error;
use std::fmt;

pub type Result<T> = std::result::Result<T, Box<dyn error::Error>>;

#[derive(Debug)]
struct NotFoundFile;

impl fmt::Display for NotFoundFile
{
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result 
    {
        write!(f, "Embedded file was not found.")
    }
}

impl error::Error for NotFoundFile
{}

#[derive(RustEmbed)]
#[folder = "bag"]
struct Asset;

/// Embeds a file into main executable.
pub fn save_embedded_file(file_name: String,file_path:&Path) -> Result<PathBuf>
{
    // Get file from current binary
    let file_asset = match Asset::get(&file_name)
    {
        Some(file)=>file,
        None=> return Err(Box::new(NotFoundFile)),
    };
    let content:Vec<u8>=file_asset.into_owned();

    // Save file to local folder
    let mut new_file=File::create(file_path.join(&file_name))?;
    new_file.write_all(&content)?;

    // Return path where file was saved
    let mut final_path= PathBuf::new();
    final_path.push(file_path.join(&file_name));
    Ok(final_path)
}

/// Get the path on entry executable including the name of the latter.
pub fn get_current_executable_path()->PathBuf
{
    // Build path for directory of application
    let app_directory= match std::env::current_exe()
    {
        Ok(path)=> path,
        Err(why)=>
            {
                println!("Cannot get current directory path: {}",why);
                PathBuf::new()
            }
    };
    app_directory
}