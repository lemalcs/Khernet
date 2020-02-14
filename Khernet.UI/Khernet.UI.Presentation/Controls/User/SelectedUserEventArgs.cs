using System;

namespace Khernet.UI.Controls
{
    /// <summary>
    /// Holds the informations about selected user
    /// </summary>
    public class SelectedUserEventArgs : EventArgs
    {
        public string UserToken { get; private set; }
        public SelectedUserEventArgs(string userToken)
        {
            UserToken = userToken;
        }
    }
}
