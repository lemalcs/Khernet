using Khernet.UI.Cache;
using Khernet.UI.Managers;
using Ninject;

namespace Khernet.UI.IoC
{
    /// <summary>
    /// Provides dependency injection using Ninject framework
    /// </summary>
    public static class IoCContainer
    {
        /// <summary>
        /// The kernel of dependency injection framework
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// Get an instance of <see cref="UIManager"/>
        /// </summary>
        public static IUIManager UI => Get<IUIManager>();

        /// <summary>
        /// Get an instance of <see cref="FileManager"/>
        /// </summary>
        public static IFileObservable Media => Get<IFileObservable>();

        /// <summary>
        /// Gets an instance of <see cref="ChatCache"/>
        /// </summary>
        public static IChatList Chat => Get<IChatList>();


        /// <summary>
        /// Gets an instance of <see cref="ChatCache"/>
        /// </summary>
        public static ITextObservable Text => Get<ITextObservable>();

        /// <summary>
        /// Configure binding for objects
        /// </summary>
        public static void Configure()
        {
            Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
        }

        /// <summary>
        /// Configures an instance of type T
        /// </summary>
        /// <typeparam name="T">the type of object</typeparam>
        public static void Configure<T>() where T : new()
        {
            Kernel.Bind<T>().ToConstant(new T());
        }

        public static void UnConfigure<T>() where T : new()
        {
            var bindings = Kernel.GetBindings(typeof(T));

            foreach (var b in bindings)
            {
                Kernel.RemoveBinding(b);
            }

        }

        public static void UnBind<T>()
        {
            Kernel.Unbind<T>();
        }

        /// <summary>
        /// Configures and instance that implementes and interface
        /// </summary>
        /// <typeparam name="T">The interface that object must implement</typeparam>
        /// <typeparam name="U">The type that implements the interface</typeparam>
        /// <param name="obj"></param>
        public static void Configure<T, U>(U obj) where U : T, new()
        {
            Kernel.Bind<T>().ToConstant(obj);
        }

        /// <summary>
        /// Gets an instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
