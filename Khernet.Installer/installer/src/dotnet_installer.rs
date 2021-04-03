extern crate winreg;

use winreg::enums::*;
use winreg::RegKey;
use std::error;

pub type Result<T> = std::result::Result<T, Box<dyn error::Error>>;

/// Gets the current dotnet version installed.
pub fn get_dotnet_version()-> Result<u32>
{
    let hklm = RegKey::predef(HKEY_LOCAL_MACHINE);
    let framework_ver = hklm.open_subkey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full")?;
    let rel:u32 = framework_ver.get_value("Release")?;
    Ok(rel)
}