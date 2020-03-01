using Khernet.UI.Resources;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace Khernet.UI
{
    public static class Program
    {
        private static ConcurrentDictionary<string, Assembly> assemblyList;

        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //System.Threading.Thread.Sleep(20000);

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (assemblyList == null)
                assemblyList = new ConcurrentDictionary<string, Assembly>();

            Assembly executingAssembly = Assembly.GetExecutingAssembly();

            AssemblyName assemblyName;

            //Load System.Core library for .Net Framework 4.0 if a previuos version is requested
            if (args.Name == "System.Core, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e, Retargetable=Yes")
            {
                assemblyName = new AssemblyName("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

                if (assemblyList.ContainsKey(assemblyName.FullName))
                    return assemblyList[assemblyName.FullName];

                assemblyList.TryAdd(assemblyName.FullName, Assembly.Load(assemblyName));
                return assemblyList[assemblyName.FullName];
            }
            else
                assemblyName = new AssemblyName(args.Name);

            string dllPath;
            Assembly parentAssembly = executingAssembly;

            if (assemblyName.Name == "Khernet.resources")
            {
                dllPath = "Khernet.g.resources";
            }
            else if (assemblyName.Name == "Khernet.UI.Container.resources")
            {
                dllPath = "Khernet.UI.Container.g.resources";
            }
            else if (assemblyName.Name == "MahApps.Metro.resources")
            {
                dllPath = "MahApps.Metro.g.resources";
                if (assemblyList.ContainsKey(Constants.MetroLibrary))
                    parentAssembly = assemblyList[Constants.MetroLibrary];
            }
            else
            {
                dllPath = assemblyName.Name + ".dll";
            }

            if (assemblyList.ContainsKey(dllPath))
                return assemblyList[dllPath];
            else
            {
                assemblyList.TryAdd(dllPath, LoadDependency(parentAssembly, dllPath));
                return assemblyList[dllPath];
            }
        }

        /// <summary>
        /// Load assembly from resources located in entry assembly
        /// </summary>
        /// <param name="parentAssembly">The assembly where to search for</param>
        /// <param name="assemblyName">The requested assembly embedded as resource</param>
        /// <returns></returns>
        private static Assembly LoadDependency(Assembly parentAssembly, string assemblyName)
        {
            using (Stream stream = parentAssembly.GetManifestResourceStream(assemblyName))
            {
                if (stream == null)
                    return null;

                byte[] assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);

                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
