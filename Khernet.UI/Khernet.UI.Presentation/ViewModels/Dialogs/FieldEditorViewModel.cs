using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Khernet.UI
{
    public class FieldEditorViewModel : BaseModel
    {
        #region Properties

        /// <summary>
        /// The token of user.
        /// </summary>
        private ProfileViewModel userProfile;

        /// <summary>
        /// The name of field.
        /// </summary>
        private string fieldName;

        /// <summary>
        /// Indicates is emoji palette is open.
        /// </summary>
        private bool isMediaGalleryOpen;

        private string sourceDataField;

        /// <summary>
        /// The content of field.
        /// </summary>
        public ReadOnlyCollection<byte> DataField
        {
            get;
            private set;
        }

        public string FieldName
        {
            get => fieldName;
            set
            {
                if (fieldName != value)
                {
                    fieldName = value;
                    OnPropertyChanged(nameof(FieldName));
                }
            }
        }

        public bool IsMediaGalleryOpen
        {
            get => isMediaGalleryOpen;
            set
            {
                if (isMediaGalleryOpen != value)
                {
                    isMediaGalleryOpen = value;
                    OnPropertyChanged(nameof(IsMediaGalleryOpen));
                }
            }
        }

        public string SourceDataField
        {
            get => sourceDataField;
            set
            {
                if (sourceDataField != value)
                {
                    sourceDataField = value;
                    OnPropertyChanged(nameof(SourceDataField));
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to update the current field.
        /// </summary>
        public ICommand UpdateFieldCommand { get; private set; }

        /// <summary>
        /// Command to open emoji gallery.
        /// </summary>
        public ICommand OpenMediaGalleryCommand { get; private set; }

        #endregion

        public FieldEditorViewModel(ProfileViewModel profileModel)
        {
            UpdateFieldCommand = new RelayCommand(UpdateField);
            OpenMediaGalleryCommand = new RelayCommand(OpenMediaGallery);
            userProfile = profileModel;
        }

        private void OpenMediaGallery()
        {
            IsMediaGalleryOpen = true;
        }

        /// <summary>
        /// Update the value of field.
        /// </summary>
        /// <param name="newValue">The new value of type <see cref="byte[]"/>.</param>
        private async void UpdateField(object newValue)
        {
            try
            {
                //Save new value of field
                IDocumentContainer document = (IDocumentContainer)newValue;

                //Save new value if it exist
                if (document.HasDocument())
                {
                    IoCContainer.Get<Messenger>().SavePeerDisplayname(userProfile.User.Token, document.GetDocument(Media.MessageType.Html));

                    //Update the field with new value
                    userProfile.User.SetDisplayName(document.GetDocument(Media.MessageType.Html));
                }
                else
                {
                    //Clear value of display name
                    IoCContainer.Get<Messenger>().SavePeerDisplayname(userProfile.User.Token, null);
                    userProfile.User.SetDisplayName(null);
                    userProfile.User.BuildDisplayName();
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error when updating value",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }

        }

        public void SetDataField(byte[] dataFieldBytes)
        {
            DataField = new ReadOnlyCollection<byte>(dataFieldBytes);
            OnPropertyChanged(nameof(DataField));
        }
    }
}
