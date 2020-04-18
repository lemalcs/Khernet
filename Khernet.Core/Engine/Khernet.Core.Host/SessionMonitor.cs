using Khernet.Core.Entity;
using Khernet.Core.Processor;
using Khernet.Core.Utility;
using Khernet.Services.Client;
using Khernet.Services.Messages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Khernet.Core.Host
{
    internal delegate void SessionClosingEventHandler(object sender, EventArgs eventArgs);

    internal class SessionMonitor
    {
        PeerIdentity identity;
        public event SessionClosingEventHandler SessionClosing;
        public SessionMonitor(PeerIdentity peerIdentity)
        {
            identity = peerIdentity;
        }

        protected void OnSessionClosing()
        {
            SessionClosing?.Invoke(this, new EventArgs());
        }

        public void Start()
        {
            try
            {
                SystemEvents.SessionSwitch += SystemEvents_SessionSwitch;
                SystemEvents.SessionEnded += SystemEvents_SessionEnded;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw;
            }
        }

        private void SystemEvents_SessionEnded(object sender, SessionEndedEventArgs e)
        {
            //There is a session ending (log out) event on current Operating System
            //Notificate to application that it must exit
            SendNotification(PeerState.Offline);
        }

        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            PeerState state = PeerState.Idle;//Idle state
            if (e.Reason == SessionSwitchReason.RemoteConnect ||
                e.Reason == SessionSwitchReason.ConsoleConnect ||
                e.Reason == SessionSwitchReason.SessionLogon ||
                e.Reason == SessionSwitchReason.SessionUnlock)
                state = PeerState.Online;//Online state

            SendNotification(state);
        }

        /// <summary>
        /// Send a notification of peers when its state is changed
        /// </summary>
        /// <param name="state">
        /// States:
        /// 0: Offline
        /// 1: Idle
        /// 2: Online
        /// </param>
        private void SendNotification(PeerState state)
        {
            try
            {
                Communicator communicator = new Communicator();
                List<Peer> addressList = communicator.GetPeers();
                addressList.ForEach((peer) =>
                {
                    Task.Factory.StartNew((p) =>
                    {
                        try
                        {
                            EventNotifierClient notifierClient = new EventNotifierClient(((Peer)p).AccountToken);

                            Notification notification = new Notification();
                            notification.Token = identity.Token;
                            notification.Type = NotificationType.StateChange;
                            notification.Content = state.ToString();

                            notifierClient.ProcessContactChange(notification);
                        }
                        catch (Exception exception)
                        {
                            //Do not throw an exception if notification could no be sent
                            LogDumper.WriteLog(exception);
                        }
                    }, peer);
                });
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
                throw exception;
            }
        }

        public void Stop()
        {
            try
            {
                SystemEvents.SessionSwitch -= SystemEvents_SessionSwitch;
                SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
        }
    }
}
