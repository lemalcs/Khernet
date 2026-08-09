namespace Khernet.UI
{
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; set; } = new ViewModelLocator();

        public static ApplicationViewModel ApplicationViewModel => IoC.IoCContainer.Get<ApplicationViewModel>();
    }
}
