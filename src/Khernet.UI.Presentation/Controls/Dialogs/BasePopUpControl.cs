using System;
using System.Windows.Controls;

namespace Khernet.UI.Controls
{
    public class BasePopUpControl : UserControl
    {
        /// <summary>
        /// Event fired when any data is confirmed.
        /// </summary>
        public event EventHandler Commited;

        /// <summary>
        /// Executes <see cref="Commited"/> event.
        /// </summary>
        protected void OnCommited()
        {
            Commited?.Invoke(this, new EventArgs());
        }

        public BasePopUpControl() : base()
        {

        }
    }
}
