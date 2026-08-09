using Khernet.UI.IoC;

namespace Khernet.UI
{
    public class AccountIdentity : IIdentity
    {
        public string Token { get; set; }

        public string Username { get; set; }
    }
}
