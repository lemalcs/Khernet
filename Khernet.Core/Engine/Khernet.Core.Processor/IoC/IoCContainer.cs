﻿using Khernet.Core.Processor.Managers;
using Ninject;

namespace Khernet.Core.Host.IoC
{
    /// <summary>
    /// Provides dependency injection using Ninject framework
    /// </summary>
    public class IoCContainer
    {
        /// <summary>
        /// The kernel of dependency injection framework
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// Configure binding for objects
        /// </summary>
        public static void Configure()
        {
            Kernel.Bind<FileManager>().ToConstant(new FileManager());
            Kernel.Bind<MessageManager>().ToConstant(new MessageManager());
            Kernel.Bind<NotificationManager>().ToConstant(new NotificationManager());
            Kernel.Bind<TextMessageManager>().ToConstant(new TextMessageManager());
        }

        /// <summary>
        /// Configures an instance of type T
        /// </summary>
        /// <typeparam name="T">the type of object</typeparam>
        public static void Configure<T>() where T : new()
        {
            Kernel.Bind<T>().ToConstant(new T());
        }

        /// <summary>
        /// Unconfigure ans instance of given type
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        public static void UnConfigure<T>() where T : new()
        {
            var bindings = Kernel.GetBindings(typeof(T));

            foreach (var b in bindings)
            {
                Kernel.RemoveBinding(b);
            }
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