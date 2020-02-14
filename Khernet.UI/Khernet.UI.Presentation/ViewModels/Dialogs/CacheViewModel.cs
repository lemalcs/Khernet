using Khernet.UI.Cache;
using Khernet.UI.IoC;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// View model for user profile
    /// </summary>
    public class CacheViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The total size of files in bytes
        /// </summary>
        private long size;

        /// <summary>
        /// Indicates if cleaning operation is executing
        /// </summary>
        private bool isCleaning;

        /// <summary>
        /// Message to be show about cleaning process
        /// </summary>
        private string textProgress;

        /// <summary>
        /// The dialog where settings are shown
        /// </summary>
        private readonly IPagedDialog pagedDialog;

        public long Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                    OnPropertyChanged(nameof(Size));
                }
            }
        }

        public bool IsCleaning
        {
            get => isCleaning;
            set
            {
                if (isCleaning != value)
                {
                    isCleaning = value;
                    OnPropertyChanged(nameof(IsCleaning));
                }
            }
        }

        public string TextProgress
        {
            get => textProgress;
            set
            {
                if (textProgress != value)
                {
                    textProgress = value;
                    OnPropertyChanged(nameof(TextProgress));
                }

            }
        }

        #endregion


        #region Commands

        /// <summary>
        /// Clear the files located in cache directory
        /// </summary>
        public ICommand ClearCacheCommand { get; private set; }

        /// <summary>
        /// Get the total size of cache in bytes
        /// </summary>
        public ICommand GetCacheSizeCommand { get; private set; }

        #endregion

        public CacheViewModel()
        {
            ClearCacheCommand = new UI.RelayCommand(ClearCache);
            GetCacheSizeCommand = new UI.RelayCommand(GetCacheSize);

            GetCacheSize(null);
        }

        public CacheViewModel(IPagedDialog pagedDialog) : this()
        {
            this.pagedDialog = pagedDialog ?? throw new Exception($"{nameof(IPagedDialog)} cannot be null");
        }

        /// <summary>
        /// Get total size of file in cache directory
        /// </summary>
        /// <param name="obj"></param>
        private void GetCacheSize(object obj)
        {
            string[] files = Directory.GetFiles(Configurations.CacheDirectory.FullName);

            long totalSize = 0;

            for (int i = 0; i < files.Length; i++)
            {
                totalSize += (new FileInfo(files[i])).Length;
            }

            Size = totalSize;
        }

        /// <summary>
        /// Delete files in cache directory
        /// </summary>
        /// <param name="obj"></param>
        private async void ClearCache(object obj)
        {
            try
            {
                TextProgress = "Clearing cache...";
                IsCleaning = true;
                pagedDialog.ShowChildDialog(this);

                await DeleteFiles();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
            finally
            {
                IsCleaning = false;
                TextProgress = "Cache cleared successfully";

                GetCacheSize(null);
            }
        }

        /// <summary>
        /// Delete files lacated in cache folder
        /// </summary>
        /// <returns></returns>
        private Task DeleteFiles()
        {
            return Task.Factory.StartNew(() =>
            {
                string[] files = Directory.GetFiles(Configurations.CacheDirectory.FullName);

                for (int i = 0; i < files.Length; i++)
                {
                    if (File.Exists(files[i]))
                    {
                        try
                        {
                            File.Delete(files[i]);
                        }
                        catch (Exception error)
                        {
                            Debug.WriteLine(error.Message);
                            Debugger.Break();
                        }
                    }
                }
            });
        }
    }
}
