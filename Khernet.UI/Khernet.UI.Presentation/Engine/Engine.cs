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
                listener.PeerChanged += Listener_PeerChanged;
                listener.WritingMessage += Listener_WritingMessage;
                listener.MessageArrived += Listener_MessageArrived;
                listener.FileArrived += Listener_FileArrived;
                listener.BeginSendingFile += Listener_BeginSendingFile;
                listener.EndSendingFile += Listener_EndSendingFile;
                listener.MessageSent += Listener_MessageSent;
                listener.Start();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void Listener_MessageSent(object sender, MessageSentEventArgs e)
        {
            IoCContainer.Get<ChatMessageStateManager>().ProcessState(e.IdMessage);
        }

        public static void Stop()
        {
            if (State == EngineState.Stopping)
                return;

            State = EngineState.Stopping;

            try
            {
                StopServer();

                StopClient();

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
                try
                {
                    IoCContainer.Get<EventListenerClient>().Stop();
                }
                catch (Exception error)
                {
                    Debug.WriteLine(error.Message);
                    Debugger.Break();
                }
                finally
                {
                    var listener = IoCContainer.Get<EventListenerClient>();

                    //Attach events to listen notifications
                    listener.PeerChanged -= Listener_PeerChanged;
                    listener.WritingMessage -= Listener_WritingMessage;
                    listener.MessageArrived -= Listener_MessageArrived;
                    listener.FileArrived -= Listener_FileArrived;
                    listener.BeginSendingFile -= Listener_BeginSendingFile;
                    listener.EndSendingFile -= Listener_EndSendingFile;
                    listener.MessageSent -= Listener_MessageSent;
                }

                IoCContainer.UnConfigure<AccountIdentity>();
                IoCContainer.UnConfigure<EventListenerClient>();

                IoCContainer.UnConfigure<Messenger>();
                IoCContainer.UnConfigure<ChatCache>();
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
        }

        private static void Listener_EndSendingFile(object sender, SendingFileEventArgs e)
        {
            if (State != EngineState.Executing)
                return;

            var user = IoCContainer.Get<UserListViewModel>().FindUser(e.AccountToken);
            if (user != null)
                user.HideUserSendingFile();
        }

        private static void Listener_BeginSendingFile(object sender, SendingFileEventArgs e)
        {
            if (State != EngineState.Executing)
                return;

            var user = IoCContainer.Get<UserListViewModel>().FindUser(e.AccountToken);
            if (user != null)
                user.ShowUserSendingFile();
        }

        private static void Listener_FileArrived(object sender, FileArrivedEventArgs e)
        {
            var currentUser = IoCContainer.Get<ChatMessageListViewModel>().UserContext;

            if (State != EngineState.Executing)
                return;

            if (!IoCContainer.UI.IsMainWindowActive() ||
                currentUser == null ||
                currentUser.User.Token != e.File.SenderToken
                )
            {
                var user = IoCContainer.Get<UserListViewModel>().FindUser(e.File.SenderToken);
                user.IncreaseUnReadMessages();
                IoCContainer.UI.ShowNotification(new NotificationViewModel
                {
                    User = user,
                    MessageType = (MessageType)(int)e.File.Type,
                });
            }
            else
            {
                IoCContainer.UI.ShowUnReadMessage(e.File.Id);
            }
        }

        private static void Listener_MessageArrived(object sender, MessageArrivedEventArgs e)
        {
            var currentUser = IoCContainer.Get<ChatMessageListViewModel>().UserContext;

            if (State != EngineState.Executing)
                return;

            if (!IoCContainer.UI.IsMainWindowActive() ||
                currentUser == null ||
                currentUser.User.Token != e.Message.SenderToken
                )
            {
                var user = IoCContainer.Get<UserListViewModel>().FindUser(e.Message.SenderToken);
                user.IncreaseUnReadMessages();
                IoCContainer.UI.ShowNotification(new NotificationViewModel
                {
                    User = user,
                    MessageType = (MessageType)(int)e.Message.Type,
                });
            }
            else
            {
                IoCContainer.UI.ShowUnReadMessage(e.Message.Id);
            }
        }

        private static void Listener_WritingMessage(object sender, WritingMessageEventArgs e)
        {
            if (State != EngineState.Executing)
                return;

            var user = IoCContainer.Get<UserListViewModel>().FindUser(e.AccountToken);
            if (user != null)
                user.ShowUserWriting();
        }

        private static void Listener_PeerChanged(object sender, ContactChangedEventArgs e)
        {
            LogDumper.WriteInformation("NEW contact " + e.EventInformation.Token);
            if (State != EngineState.Executing)
            {
                LogDumper.WriteInformation("ENGINE stopped " + e.EventInformation.Token);
                return;
            }

            try
            {
                IoCContainer.Get<UserManager>().ProcessState(
                    new UserState
                    {
                        Token = e.EventInformation.Token,
                        Username = e.EventInformation.Type == NotificationType.StateChange ? "" : e.EventInformation.Content,
                        Change = e.EventInformation.Type == NotificationType.AvatarChange ? UserChangeType.AvatarChange : UserChangeType.ProfileChange,
                    });
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
                Debugger.Break();
            }
        }
    }
}
