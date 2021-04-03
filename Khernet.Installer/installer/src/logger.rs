use std::path::PathBuf;
use std::fs::OpenOptions;
use std::io::Write;
use chrono::Utc;

pub struct Logger
{
    pub log_path:PathBuf
}

impl Logger
{
    /// Writes a log to text file.
    pub fn log(&self, message:&str)
    {
        let mut log_file= match OpenOptions::new().create(true).append(true).open(&self.log_path)
        {
            Ok(f)=>f,
            Err(why)=>
                {
                    print!("Error while opening log file: {}",why);
                    return
                },
        };

        let formatted_message=format!("{} > {}\r\n",Utc::now().to_rfc3339(),message);
        match log_file.write_all(formatted_message.as_bytes())
        {
            Err(why)=>print!("Error while saving log: {}",why),
            _=>(),
        };
    }
}
