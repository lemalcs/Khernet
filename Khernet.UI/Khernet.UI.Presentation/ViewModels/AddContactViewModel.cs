using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.Windows.Input;

namespace Khernet.UI
{
    public class AddContactViewModel : BaseModel
    {
        /// <summary>
        /// The ip address or hostname of gateway service exposed by contact (peer).
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
            try
            {
                IoCContainer.Get<Messenger>().AddContact(HostNameIp,Port);
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Cannot add contact, verify if hostname or IP and port are correct.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }
    }
}
