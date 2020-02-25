﻿using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.IoC;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Khernet.UI.Managers
{
    public enum UserChangeType
    {
        /// <summary>
        /// User has changed profile
        /// </summary>
        ProfileChange = 0,

        /// <summary>
        /// Request to load profile
        /// </summary>
        ProfileLoading = 1,

        /// <summary>
        /// User has changed avatar
        /// </summary>
        AvatarChange = 2,

        /// <summary>
        /// Request to load avatar
        /// </summary>
        AvatarLoading = 4
    }
    public class UserState
    {
        /// <summary>
        /// The user name
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The token of user
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The type of chenge for user
        /// </summary>
        public UserChangeType Change { get; set; }

    }

    public class UserManager : IDisposable
    {
        /// <summary>
        /// The process to manage states
        /// </summary>
        private Thread userStateMonitor;

        /// <summary>
        /// Controls when to start to upload text message
        /// </summary>
        private ManualResetEvent userStateManualReset;

        /// <summary>
        /// Indicates if state manager should continue running
        /// </summary>
        private bool stopMonitoring = false;

        private ConcurrentQueue<UserState> userStateMessageList;

        public UserManager()
        {
            userStateManualReset = new ManualResetEvent(false);
        }

        public void ProcessState(UserState state)
        {
            if (userStateMessageList == null)
                userStateMessageList = new ConcurrentQueue<UserState>();

            userStateMessageList.Enqueue(state);

            StartUserStateMonitor();
        }

        private void StartUserStateMonitor()
        {
            if (userStateMonitor == null)
            {
                userStateMonitor = new Thread(new ThreadStart(ProcessUserState));
                userStateMonitor.Start();
            }

            userStateManualReset.Set();
            userStateManualReset.Reset();
        }

        private void ProcessUserState()
        {
            while (!stopMonitoring)
            {
                try
                {
                    while (!userStateMessageList.IsEmpty)
                    {
                        UserState state;
                        try
                        {
                            userStateMessageList.TryPeek(out state);

                            if (state == null)
                                continue;

                            UserItemViewModel user = GetUserFromList(state.Token, state.Username);

                            if (user == null)
                                continue;

                            switch (state.Change)
                            {
                                case UserChangeType.AvatarChange:

                                    try
                                    {
                                        IoCContainer.Get<Messenger>().UpdatePeerAvatar(user.Token);
                                    }
                                    catch (Exception error)
                                    {
                                        LogDumper.WriteLog(error);
                                    }
                                    user.SetAvatarThumbnail(IoCContainer.Get<Messenger>().GetPeerAvatar(user.Token));

                                    break;

                                case UserChangeType.ProfileChange:

                                    try
                                    {
                                        IoCContainer.Get<Messenger>().UpdatePeerProfile(user.Token);
                                    }
                                    catch (Exception error2)
                                    {
                                        LogDumper.WriteLog(error2);
                                    }
                                    FillUserProfile(user);

                                    break;

                                case UserChangeType.ProfileLoading:

                                    FillUserProfile(user);

                                    break;

                                case UserChangeType.AvatarLoading:
                                    user.SetAvatarThumbnail(IoCContainer.Get<Messenger>().GetPeerAvatar(user.Token));
                                    break;
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        finally
                        {
                            userStateMessageList.TryDequeue(out state);
                        }
                    }

                    userStateManualReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        private UserItemViewModel GetUserFromList(string userToken, string username)
        {
            var userList = IoCContainer.Get<UserListViewModel>();

            UserItemViewModel user = userList.FindUser(userToken);

            if (user == null)
            {
                user = new UserItemViewModel
                {
                    Token = userToken,
                    Username = username,
                };

                user.BuildDisplayName();

                //Add new user to list
                IoCContainer.Get<UserListViewModel>().AddUser(user);
            }

            return user;
        }

        private void FillUserProfile(UserItemViewModel userModel)
        {
            Peer peer = IoCContainer.Get<Messenger>().GetPeerProfile(userModel.Token);

            userModel.Username = peer.UserName;
            userModel.Group = peer.Group;
            userModel.Slogan = peer.Slogan;
            userModel.Initials = peer.Initials;
            userModel.ReadFullName(peer.FullName);
            userModel.ReadDisplayName(peer.DisplayName);
            userModel.ColorHex = peer.HexColor;

            userModel.BuildDisplayName();

            var unreadMessages = IoCContainer.Get<Messenger>().GetUnreadMessages(userModel.Token);
            if (unreadMessages == null)
                userModel.SetUnReadMessages(0);
            else
                userModel.SetUnReadMessages(unreadMessages.Count);
        }

        public void StopProcessor()
        {
            try
            {
                userStateMessageList = null;
                stopMonitoring = true;
                if (userStateMonitor != null && userStateMonitor.ThreadState != ThreadState.Unstarted)
                {
                    userStateManualReset.Set();
                    userStateMonitor.Interrupt();

                    //If thread does not stop through 1 minute, abort thread
                    if (!userStateMonitor.Join(TimeSpan.FromMinutes(1)))
                        userStateMonitor.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                userStateMonitor = null;

                if (userStateManualReset != null)
                    userStateManualReset.Close();
            }
        }

        #region IDisposable Support

        /// <summary>
        /// Variable to detect reentry calls
        /// </summary>
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopProcessor();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Cleans resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}