namespace Khernet.UI.IoC
{
    /// <summary>
    /// The identity for current logged user
    /// </summary>
    public interface IIdentity
    {
        string Token { get; }
        string Username { get; }
    }
}
