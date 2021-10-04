﻿using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using Khernet.UI.Resources;
using System;
using System.Collections.Generic;

namespace Khernet.UI
{
    public enum AppOptions
    {
        /// <summary>
        /// View and edit logged user profile.
        /// </summary>
        Profile = 0,

        /// <summary>
        /// Clear cache folder used by application.
        /// </summary>
        Cache = 1,

        /// <summary>
        /// Close current session.
        /// </summary>
        CloseSession = 2,

        /// <summary>
        /// Opens the about page.
        /// </summary>
        About = 3,

        /// <summary>
        /// View informacion about services used to connect network.
        /// </summary>
        Connection = 4,
    }


    public class SettingControllerViewModel : BaseModel, ISettingController
    {
        /// <summary>
        /// The dialog where settings are shown.
        /// </summary>
        private readonly IPagedDialog pagedDialog;

        /// <summary>
        /// The user list for chat.
        /// </summary>
        public List<SettingItemViewModel> Items { get; private set; }

        public SettingControllerViewModel()
        {
            Items = new List<SettingItemViewModel>();
            Items.Add(new SettingItemViewModel(OpenProfileSetting)
            {
                Name = "Profile",
                Setting = AppOptions.Profile,
                IconName = "AccountCircle",
            });
            Items.Add(new SettingItemViewModel(OpenConnectionPage)
            {
                Name = "Connection",
                Setting = AppOptions.Connection,
                IconName = "Archive",
            });
            Items.Add(new SettingItemViewModel(OpenCacheSetting)
            {
                Name = "Cache",
                Setting = AppOptions.Cache,
                IconName = "Archive",
            });

            Items.Add(new SettingItemViewModel(OpenSessionSetting)
            {
                Name = "Close session",
                Setting = AppOptions.CloseSession,
                IconName = "ExitToApp",
            });

            Items.Add(new SettingItemViewModel(OpenAboutPage)
            {
                Name = "About",
                Setting = AppOptions.About,
                IconName = "InformationOutline",
            });
        }

        private void OpenConnectionPage()
        {
            ConnectionViewModel connectionViewModel = new ConnectionViewModel();
            
            string gatewayAddress = IoCContainer.Get<Messenger>().GetGatewayAddress();
            if (!string.IsNullOrEmpty(gatewayAddress))
            {
                Uri gateway = new Uri(IoCContainer.Get<Messenger>().GetGatewayAddress());

                connectionViewModel.Hostname = gateway.Host;
                connectionViewModel.Port = gateway.Port;
                string adressses = null;
                foreach (string address in NetworkHelper.GetIPAddresses(gateway.Host, System.Net.Sockets.ProtocolFamily.InterNetwork))
                {
                    adressses += $"{address};";
                }

                foreach (string address in NetworkHelper.GetIPAddresses(gateway.Host, System.Net.Sockets.ProtocolFamily.InterNetworkV6))
                {
                    adressses += $"{address};";
                }

                if (adressses.EndsWith(";"))
                    adressses = adressses.Remove(adressses.Length - 1);
                connectionViewModel.IPAddress = adressses;
            }

            pagedDialog.GoToPage(Converters.ApplicationPage.Connection, connectionViewModel, "Connection");
        }

        public SettingControllerViewModel(IPagedDialog pagedDialog) : this()
        {
            this.pagedDialog = pagedDialog ?? throw new Exception($"{nameof(IPagedDialog)} cannot be null");
        }

        public void SetItems(IEnumerable<SettingItemViewModel> settingsList)
        {
            Items = (List<SettingItemViewModel>)settingsList;
            OnPropertyChanged(nameof(Items));
        }

        public void OpenProfileSetting()
        {
            Peer peer = IoCContainer.Get<Messenger>().GetProfile();

            ProfileViewModel profile = new ProfileViewModel();
            profile.User = new UserItemViewModel
            {
                Username = peer.UserName,
                Token = peer.AccountToken,
                Group = peer.Group,
                Slogan = peer.Slogan,
                State = peer.State.ToString(),
            };

            profile.User.SetFullName(peer.FullName);

            profile.User.SetAvatarThumbnail(IoCContainer.Get<Messenger>().GetAvatar());

            pagedDialog.GoToPage(Converters.ApplicationPage.Profile, profile, "Profile");
        }

        public void OpenCacheSetting()
        {
            pagedDialog.GoToPage(Converters.ApplicationPage.Cache, new CacheViewModel(pagedDialog), "Cache");
        }

        public async void OpenSessionSetting()
        {
            try
            {
                var messageBox = new MessageBoxViewModel
                {
                    Message = "Do you want to close session?",
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    ShowCancelOption = true,
                    AcceptOptionLabel = "Yes",
                    CancelOptionLabel = "No",
                };
                await IoCContainer.UI.ShowMessageBox(messageBox);

                if (messageBox.Result == MessageBoxResponse.Accept)
                {
                    IoCContainer.Get<ApplicationViewModel>().SignOut();
                }

            }
            catch (Exception error)
            {
                LogDumper.WriteLog(error);
                await IoCContainer.UI.ShowMessageBox(new MessageBoxViewModel
                {
                    Message = Constants.ErrorMessage,
                    Title = "Khernet",
                    ShowAcceptOption = true,
                    AcceptOptionLabel = "OK",
                    ShowCancelOption = false,
                });
            }
        }

        private void OpenAboutPage()
        {
            pagedDialog.GoToPage(Converters.ApplicationPage.About, new AboutViewModel(), "About");
        }
    }
}
