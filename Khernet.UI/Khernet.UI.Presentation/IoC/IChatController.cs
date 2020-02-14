namespace Khernet.UI.IoC
{
    interface IChatController
    {
        void OpenChat(UserItemViewModel user);

        void LoadUnReadMessages();

        void LoadLastMessages(int messageQuantity);
    }
}
