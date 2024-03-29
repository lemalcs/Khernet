﻿using Khernet.UI.Resources;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Khernet.UI
{
    public static class Program
    {
        private static ConcurrentDictionary<string, Assembly> assemblyList;

        [STAThread]
        [DebuggerStepThrough]
        public static void Main()
        {
            IntPtr winHandle = CheckRunningInstance();
            if (winHandle != IntPtr.Zero)
            {
                //Show window of the previous instance.
                //Only a single instance of the same assembly is allowed to execute
                NativeMethods.ShowWindow(winHandle, NativeMethods.SW_SHOW);
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        /// <summary>
        /// Check if other instance of this application is already executing.
        /// </summary>
        /// <returns>The <see cref="IntPtr"/> window handle or <see cref="IntPtr.Zero"/> if not found.</returns>
        private static IntPtr CheckRunningInstance()
        {
            Process[] processList = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

            foreach (Process p in processList)
            {
                if (p.MainModule.FileName == Assembly.GetExecutingAssembly().Location
                    && p.Id != Process.GetCurrentProcess().Id)
                {
                    return p.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (assemblyList == null)
                assemblyList = new ConcurrentDictionary<string, Assembly>();

            AssemblyName assemblyName = new AssemblyName(args.Name);
            string dllPath;
            Assembly parentAssembly = Assembly.GetExecutingAssembly();

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
        /// Load assembly from resources located in entry assembly.
        /// </summary>
        /// <param name="parentAssembly">The assembly where to search for.</param>
        /// <param name="assemblyName">The requested assembly embedded as resource.</param>
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
