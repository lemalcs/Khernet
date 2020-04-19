using Khernet.Core.Host;
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
        AvatarLoading = 4,

        /// <summary>
        /// Peer has changed its state
        /// </summary>
        StateChange=5
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
        private AutoResetEvent userStateAutoReset;

        /// <summary>
        /// Indicates if state manager should continue running
        /// </summary>
        private bool stopMonitoring = false;

        private ConcurrentQueue<UserState> userStateMessageList;

        public UserManager()
        {
            userStateAutoReset = new AutoResetEvent(false);
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

            userStateAutoReset.Set();
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

                                case UserChangeType.StateChange:
                                    break;
                            }
                        }
                        catch (ThreadAbortException)
                        {
                            return;
                        }
                        catch (ThreadInterruptedException)
                        {
                            return;
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        finally
                        {
                            if (userStateMessageList != null)
                                userStateMessageList.TryDequeue(out state);
                        }
                    }

                    if (userStateMessageList.IsEmpty)
                        userStateAutoReset.WaitOne();
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Get a peer from peer list if it does not exist add the new peer to list
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="username"></param>
        /// <returns>A <see cref="UserItemViewModel"/> of peer</returns>
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
                var unreadMessages = IoCContainer.Get<Messenger>().GetUnreadMessages(userToken);

                if (unreadMessages == null)
                    user.SetUnReadMessages(0);
                else
                    user.SetUnReadMessages(unreadMessages.Count);

                if (unreadMessages != null && unreadMessages.Count > 0)
                    IoCContainer.UI.ShowUnreadMessagesNumber(IoCContainer.Get<UserListViewModel>().TotalUnreadMessages);

                //Add new user to list
                IoCContainer.Get<UserListViewModel>().AddUser(user);
            }

            return user;
        }

        /// <summary>
        /// Fill profile of a peer.
        /// </summary>
        /// <param name="userModel">The object profile of peer</param>
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
        }

        public void StopProcessor()
        {
            try
            {
                stopMonitoring = true;
                if (userStateMonitor != null && userStateMonitor.ThreadState != ThreadState.Unstarted)
                {
                    userStateAutoReset.Set();
                    userStateMonitor.Interrupt();
                    userStateMonitor.Abort();
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                userStateMonitor = null;

                if (userStateAutoReset != null)
                    userStateAutoReset.Close();

                userStateMessageList = null;
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
