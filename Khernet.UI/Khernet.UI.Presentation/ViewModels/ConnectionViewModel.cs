using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.Windows.Input;

namespace Khernet.UI
{
    public class ConnectionViewModel : BaseModel
    {
        /// <summary>
        /// The setting option name.
        /// </summary>
        private string hostname;

        /// <summary>
        /// The setting option name.
        /// </summary>
        private string ipAddress;

        /// <summary>
        /// The Hexadecimal value for color, for example: F5A2D8.
        /// </summary>
        private int port;

        /// <summary>
        /// Indicates whether gateway service is online or offline.
        /// </summary>
        private string status;

        /// <summary>
        /// The dialog where settings are shown.
        /// </summary>
        private readonly IPagedDialog pagedDialog;

        public string Hostname
        {
            get { return hostname; }
            set
            {
                if (hostname != value)
                {
                    hostname = value;
                    OnPropertyChanged(nameof(Hostname));
                }
            }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                if (ipAddress != value)
                {
                    ipAddress = value;
                    OnPropertyChanged(nameof(IPAddress));
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
        public string Status
        {
            get => status;
            set 
            { 
                if(status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        /// <summary>
        /// Command to open setting.
        /// </summary>
        public ICommand RefreshStateCommand { get; private set; }

        public ConnectionViewModel()
        {
            RefreshStateCommand = new RelayCommand(RefreshState);
            Status = "Offline";
        }

        public ConnectionViewModel(IPagedDialog pagedDialog)
        {
            this.pagedDialog = pagedDialog ?? throw new Exception($"{nameof(IPagedDialog)} cannot be null");
            RefreshStateCommand = new RelayCommand(RefreshState);
        }

        /// <summary>
        /// Open a specific setting.
        /// </summary>
        private async void RefreshState()
        {
            try
            {
                string gatewayAddress = IoCContainer.Get<Messenger>().GetGatewayAddress();
                if (!string.IsNullOrEmpty(gatewayAddress))
                {
                    Uri gateway = new Uri(IoCContainer.Get<Messenger>().GetGatewayAddress());

                    Hostname = gateway.Host;
                    Port = gateway.Port;

                    if (Port > 0)
                        Status = "Online";
                }
            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = "Error while retrieving state, try again later.",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }
    }
}
