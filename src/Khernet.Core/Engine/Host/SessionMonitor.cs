using Khernet.Core.Utility;
using Microsoft.Win32;
using System;

namespace Khernet.Core.Host
{
    internal delegate void SessionClosingEventHandler(object sender, EventArgs eventArgs);

    internal class SessionMonitor
    {
        public event SessionClosingEventHandler SessionClosing;

        protected void OnSessionClosing()
        {
            SessionClosing?.Invoke(this, new EventArgs());
        }

        public void Start()
        {
            try
            {
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
            OnSessionClosing();
        }

        public void Stop()
        {
            try
            {
                SystemEvents.SessionEnded -= SystemEvents_SessionEnded;
            }
            catch (Exception exception)
            {
                LogDumper.WriteLog(exception);
            }
        }
    }
}
