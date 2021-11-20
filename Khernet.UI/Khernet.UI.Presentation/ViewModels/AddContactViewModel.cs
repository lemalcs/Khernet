using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    public class AddContactViewModel : BaseModel
    {
        /// <summary>
        /// The token of the user.
        /// </summary>
        private string userToken;

        /// <summary>
        /// The IP address or host name of gateway service exposed by contact (peer).
        /// </summary>
        private string hostnameIp;

        /// <summary>
        /// The port of gateway service exposed by contact (peer).
        /// </summary>
        private int port;

        /// <summary>
        /// The dialog where this page will be shown.
        /// </summary>
        private readonly IPagedDialog pagedDialog;

        /// <summary>
        /// Indicates whether a peer is being added to users list.
        /// </summary>
        private bool isAttemptingAddContact;

        /// <summary>
        /// The description of current operation performed.
        /// </summary>
        private string progressDescription;

        /// <summary>
        /// Indicates whether a contact has been found or not.
        /// </summary>
        private bool foundContact;


        public string HostNameIp
        {
            get { return hostnameIp; }
            set
            {
                if (hostnameIp != value)
                {
                    hostnameIp = value;
                    OnPropertyChanged(nameof(HostNameIp));
                }
            }
        }

        public int Port
        {
            get { return port; }
            set
            {
                if (port != value)
                {
                    port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }
        public bool IsAttemptingAddContact 
        { 
            get => isAttemptingAddContact;
            set 
            {
                if(isAttemptingAddContact != value) 
                {
                    isAttemptingAddContact = value;
                    OnPropertyChanged(nameof(IsAttemptingAddContact));
                }
            }
        }
        public string ProgressDescription 
        { 
            get => progressDescription;
            set 
            { 
                if (progressDescription != value)
                {
                    progressDescription = value;
                    OnPropertyChanged(nameof(ProgressDescription));
                }
            }
        }
        public bool FoundContact 
        { 
            get => foundContact;
            set 
            { 
                if(foundContact != value)
                {
                    foundContact = value;
                    OnPropertyChanged(nameof(FoundContact));
                }
            }
        }
        public string UserToken 
        { 
            get => userToken;
            set 
            { 
                if(userToken != value)
                {
                    userToken = value;
                    OnPropertyChanged(nameof(UserToken));
                }
            }
        }

        /// <summary>
        /// Command to add contact.
        /// </summary>
        public ICommand AddContactCommand { get; private set; }

        public AddContactViewModel()
        {
            AddContactCommand = new RelayCommand(AddContact);
        }

        public AddContactViewModel(IPagedDialog pagedDialog)
        {
            this.pagedDialog = pagedDialog ?? throw new Exception($"{nameof(IPagedDialog)} cannot be null");
            AddContactCommand = new RelayCommand(AddContact);
        }

        /// <summary>
        /// Add contact to peer list.
        /// </summary>
        private async void AddContact()
        {
            FoundContact = false;
            IsAttemptingAddContact = true;
            ProgressDescription = "Searching contact...";

            ProgressDialogViewModel progressDialogView = new ProgressDialogViewModel
            {
                TextProgress = this.ProgressDescription,
                IsExecuting = true,
            };

            try
            {
                pagedDialog.ShowChildDialog(progressDialogView);

                await TryAddContact();
                FoundContact = true;
                progressDialogView.IsExecuting = false;
                progressDialogView.Result = ProgressResultIcon.Success;
                progressDialogView.TextProgress = "Contact added.";
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                ProgressDescription = "Cannot add contact, verify if host name or IP and port are correct.";
                progressDialogView.TextProgress = ProgressDescription;
                progressDialogView.Result = ProgressResultIcon.Error;
            }
            finally
            {
                IsAttemptingAddContact = false;
                progressDialogView.IsExecuting = false;
            }
        }

        public Task TryAddContact()
        {

            return Task.Run(() =>
            {
                try
                {
                    IoCContainer.Get<Messenger>().AddContact(UserToken, HostNameIp, Port);
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }
    }
}
