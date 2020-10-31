using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Cache;
using Khernet.UI.Files;
using Khernet.UI.IoC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Khernet.UI
{
    /// <summary>
    /// View model for image messages.
    /// </summary>
    public class MediaGalleryViewModel : BaseModel
    {
        /// <summary>
        /// List of UNICODE values for emojis.
        /// </summary>
        ObservableCollection<GIFItemViewModel> animationList;

        /// <summary>
        /// Indicates if GIF list is loading.
        /// </summary>
        private bool isLoading;

        public ObservableCollection<GIFItemViewModel> AnimationList
        {
            get => animationList;
            private set
            {
                if (animationList != value)
                {
                    animationList = value;
                    OnPropertyChanged(nameof(AnimationList));
                }
            }
        }

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public MediaGalleryViewModel()
        {
            IsLoading = true;
            IoCContainer.UI.ExecuteAsync(LoadGIFs);
        }

        public void LoadGIFs()
        {
            try
            {
                if (AnimationList == null)
                    AnimationList = new ObservableCollection<GIFItemViewModel>();

                List<int> gifList = IoCContainer.Get<Messenger>().GetAnimationList();

                for (int i = 0; i < gifList.Count; i++)
                {
                    AnimationList.Add(GetAnimationModel(gifList[i]));
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();

            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Add new animation to the list.
        /// </summary>
        /// <param name="idAnimation">The id of animation.</param>
        public void AddAnimation(int idAnimation)
        {
            try
            {
                AnimationList.Add(GetAnimationModel(idAnimation));
            }
            catch (Exception error)
            {

                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
        }


        /// <summary>
        /// Get animation details with the animation itself.
        /// </summary>
        /// <param name="id">The id of animation.</param>
        /// <returns>An <see cref="AnimationChatMessageViewModel"/> object.</returns>
        private GIFItemViewModel GetAnimationModel(int id)
        {
            //Retrieve the original message detail of animation
            AnimationDetail animation = IoCContainer.Get<Messenger>().GetAnimationContent(id);

            FileInformation info = JSONSerializer<FileInformation>.DeSerialize(IoCContainer.Get<Messenger>().GetMessageContent(animation.IdMessage));

            string outFile = IoCContainer.Get<Messenger>().GetCacheFilePath(animation.IdMessage);

            if (string.IsNullOrEmpty(outFile) || !Directory.Exists(outFile))
                outFile = Path.Combine(Configurations.CacheDirectory.FullName, System.IO.Path.GetFileName(info.FileName));

            FileOperations operations = new FileOperations();

            if (!operations.VerifyFileIntegrity(outFile, info.Size, animation.IdMessage))
            {
                using (Stream st = IoCContainer.Get<Messenger>().DownloadLocalFile(animation.IdMessage))
                using (FileStream fStream = new FileStream(outFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    int chunk = 1048576;
                    byte[] buffer = new byte[chunk];

                    int readBytes = st.Read(buffer, 0, chunk);

                    while (readBytes > 0)
                    {
                        fStream.Write(buffer, 0, readBytes);

                        readBytes = st.Read(buffer, 0, chunk);
                    }
                }
            }

            GIFItemViewModel animationViewModel = new GIFItemViewModel
            {
                Id = animation.IdMessage,
                FileName = info.FileName,
                Width = info.Width,
                Height = info.Height,
                ThumbNailWidth = animation.Width,
                ThumbNailHeight = animation.Height,
                FilePath = outFile,
            };
            var th = IoCContainer.Get<Messenger>().GetThumbnail(animation.IdMessage);
            animationViewModel.SetThumbnail(th);

            return animationViewModel;
        }
    }
}