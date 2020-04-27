using Khernet.Core.Entity;
using Khernet.Core.Host;
using Khernet.Core.Utility;
using Khernet.Services.Messages;
using Khernet.UI.Cache;
using Khernet.UI.IoC;
using Khernet.UI.Managers;
using Khernet.UI.Media;
using System;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;

namespace Khernet.UI
{
    /// <summary>
    /// The states of the chat engine
    /// </summary>
    public enum EngineState
    {
        Executing = 0,
        Stopping = 1,
        Stopped = 2
    }

    public static class Engine
    {
        public static EngineState State { get; private set; } = EngineState.Stopped;

        public static void CreateUser(string userName, SecureString password)
        {
            Authenticator auth = new Authenticator();
            auth.CreateUser(userName, password);
        }

        public static PeerIdentity AuthenticateUser(string userName, SecureString password)
        {
            Authenticator auth = new Authenticator();
            var peer = auth.Login(userName, password);
            return peer;
        }

        /// <summary>
        /// Start server core
        /// </summary>
        /// <param name="currentUser">The identity for this server</param>
        public static Task Start(PeerIdentity currentUser)
        {
            return TaskEx.Run(() =>
            {
                try
                {
                    IoCContainer.Configure<Initializer>();

                    //Set current user as identity for server
                    IoCContainer.Get<Initializer>().SetUserIdentity(currentUser);

                    //Get identity to use along application user interface
                    AccountIdentity identity = new AccountIdentity();
                    identity.Token = currentUser.Token;
                    identity.Username = currentUser.UserName;

                    IoCContainer.Configure<IIdentity, AccountIdentity>(identity);

                    //Start chat engine
                    IoCContainer.Get<Initializer>().Init();

                    IoCContainer.Configure<IFileObservable, FileManager>(new FileManager());

                    IoCContainer.Configure<ITextObservable, TextManager>(new TextManager());

                    IoCContainer.Configure<IAudioObservable, AudioManager>(new AudioManager());

                    IoCContainer.Configure<ChatMessageStateManager>();
                    IoCContainer.Configure<UserManager>();

                    //Create agent to manage messaging
                    IoCContainer.Configure<Messenger>();

                    //Create a cache for chat messages
                    IoCContainer.Configure<IChatList, ChatCache>(new ChatCache());

                    //Create view model for sending messages
                    IoCContainer.Configure<ChatMessageListViewModel>();

                    IoCContainer.Configure<UserListViewModel>();

                    StartClient();

                    State = EngineState.Executing;
                }
                catch (Exception)
                {

                    throw;
                }
            });
        }

        /// <summary>
        /// Creates client to listen notifications from core
        /// </summary>
        private static void StartClient()
        {
            try
            {
                IoCContainer.Configure<EventListenerClient>();

                var listener = IoCContainer.Get<EventListenerClient>();

                //Attach events to listen notifications
                listener.ProcessingMessage += Listener_ProcessingMessage;
                listener.MessageArrived += Listener_MessageArrived;
                listener.PeerChanged += Listener_PeerChanged;
                listener.MessageStateChanged += Listener_MessageStateChanged;

                listener.Start();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void Listener_MessageStateChanged(object sender, MessageStateChangedEventArgs e)
        {
            IoCContainer.Get<ChatMessageStateManager>().ProcessState(new MessageStateInfo 
            {
                Id=e.Notification.MessageId,
                State=e.Notification.State,
            });
        }

        private static void Listener_PeerChanged(object sender, ContactChangedEventArgs e)
        {
            if (State != EngineState.Executing)
            {
                return;
            }

            try
            {

                var userState = new UserState
                {
                    Token = e.EventInformation.Token,
                    Change=e.EventInformation.Change==PeerChangeType.AvatarChange?UserChangeType.AvatarChange:UserChangeType.ProfileChange,
                };

                if (e.EventInformation.Change == PeerChangeType.AvatarChange)
                    userState.Change = UserChangeType.AvatarChange;
                else if (e.EventInformation.Change == PeerChangeType.ProfileChange)
                {
                    userState.Change = UserChangeType.ProfileChange;
                }
                else if (e.EventInformation.Change == PeerChangeType.StateChange)
                {
                    userState.Change = UserChangeType.StateChange;
                }

                IoCContainer.Get<UserManager>().ProcessState(userState);
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
        }

        private static void Listener_MessageArrived(object sender, MessageArrivedEventArgs e)
        {
            var currentUser = IoCContainer.Get<ChatMessageListViewModel>().UserContext;

            if (State != EngineState.Executing)
                return;


            if (!IoCContainer.UI.IsMainWindowActive() ||
                currentUser == null ||
                currentUser.User.Token != e.Notification.SenderToken
                )
            {
                var user = IoCContainer.Get<UserListViewModel>().FindUser(e.Notification.SenderToken);

                if (user == null)
                    return;

                user.IncreaseUnReadMessages();
                IoCContainer.UI.ShowNotification(new NotificationViewModel
                {
                    User = user,
                    MessageType = (MessageType)(int)e.Notification.Format,
                });
            }
            else
            {
                IoCContainer.UI.ShowUnReadMessage(e.Notification.MessageId);
            }

        }

        private static void Listener_ProcessingMessage(object sender, MessageProcessingEventArgs e)
        {
            if (State != EngineState.Executing)
                return;

            var user = IoCContainer.Get<UserListViewModel>().FindUser(e.Notification.SenderToken);
            if (user == null)
                return;

            switch(e.Notification.Process)
            {
                case MessageProcessing.WritingText:
                    user.ShowUserWriting();
                    break;

                case MessageProcessing.BeginSendingFile:
                    user.ShowUserSendingFile();
                    break;

                case MessageProcessing.EndSendingFile:
                    user.HideUserSendingFile();
                    break;
            }
        }

        public static void Stop()
        {
            if (State == EngineState.Stopping)
                return;

            State = EngineState.Stopping;

            try
            {
                StopClient();

                StopServer();

                State = EngineState.Stopped;
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
        }

        private static void StopServer()
        {

            try
            {
                IoCContainer.Get<Initializer>().Stop();
                IoCContainer.UnConfigure<Initializer>();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
            }
        }

        private static void StopClient()
        {
            try
            {
                var listener = IoCContainer.Get<EventListenerClient>();
                listener.Stop();

                //Detach listener events
                listener.PeerChanged -= Listener_PeerChanged;
                listener.ProcessingMessage -= Listener_ProcessingMessage;
                listener.MessageArrived -= Listener_MessageArrived;
                listener.MessageStateChanged -= Listener_MessageStateChanged;

                IoCContainer.UnConfigure<AccountIdentity>();
                IoCContainer.UnConfigure<EventListenerClient>();

                IoCContainer.UnConfigure<Messenger>();
                IoCContainer.UnConfigure<ChatMessageListViewModel>();
                IoCContainer.UnConfigure<ChatMessageStateManager>();
                IoCContainer.UnConfigure<UserManager>();

                IoCContainer.UnConfigure<UserListViewModel>();

                IoCContainer.UnBind<IChatList>();
                IoCContainer.UnBind<IIdentity>();

                IoCContainer.UnBind<IFileObservable>();
                IoCContainer.UnBind<ITextObservable>();
                IoCContainer.UnBind<IAudioObservable>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debugger.Break();
            }
            finally
            {
                IoCContainer.UI.ClearNotificationNewMessageIcon();
            }
        }
    }
}
