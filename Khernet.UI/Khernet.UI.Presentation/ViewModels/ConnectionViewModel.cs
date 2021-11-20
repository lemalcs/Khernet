using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.UI.IoC;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Khernet.UI
{
    public class ConnectionViewModel : BaseModel,IDisposable
    {
        #region Properties

        /// <summary>
        /// The token of the current user.
        /// </summary>
        private string token;

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
        /// Indicates whether gateway services is online or not.
        /// </summary>
        private bool isGatewayOnline;

        private CancellationTokenSource cancellationToken;

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

        public string Token
        {
            get => token;
            set
            {
                if (token != value)
                {
                    token = value;
                    OnPropertyChanged(nameof(Token));
                }
            }
        }
        public bool IsGatewayOnline
        {
            get => isGatewayOnline;
            set 
            { 
                if(isGatewayOnline != value)
                {
                    isGatewayOnline = value;
                    OnPropertyChanged(nameof(IsGatewayOnline));
                }
            }
        }

        #endregion


        public ConnectionViewModel()
        {
            Token = IoCContainer.Get<IIdentity>().Token;
            cancellationToken = new CancellationTokenSource();
            RefreshState();
        }

        /// <summary>
        /// Open a specific setting.
        /// </summary>
        private void RefreshState()
        {
            Task.Run(
                () =>
                {
                    try
                    {

                        string gatewayAddress = null;
                        IsGatewayOnline = false;

                        while (string.IsNullOrEmpty(gatewayAddress))
                        {
                            gatewayAddress = IoCContainer.Get<Messenger>().GetSelfGatewayAddress();
                            if (!string.IsNullOrEmpty(gatewayAddress))
                            {
                                Uri gateway = new Uri(gatewayAddress);

                                Hostname = gateway.Host;
                                Port = gateway.Port;

                                if (Port > 0)
                                {
                                    IsGatewayOnline = true;
                                    return;
                                }
                            }

                            if (cancellationToken.Token.IsCancellationRequested)
                                return;

                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception error)
                    {
                        LogDumper.WriteLog(error);
                    }
                },
            cancellationToken.Token);
        }

        #region IDisposable Support

        /// <summary>
        /// Variable to detect reentry calls.
        /// </summary>
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cancellationToken.Cancel();
                    cancellationToken.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Cleans resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        ~ConnectionViewModel()
        {
            Dispose(false);
        }
    }
}
