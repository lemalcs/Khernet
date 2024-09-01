using Khernet.Core.Common;
using System;

namespace Khernet.Core.Host
{
    public class ApplicationConfigurations
    {
        /// <summary>
        /// Gets the prefered source to get updates from.
        /// </summary>
        /// <returns>True to get online updates. False get updates from a local file.</returns>
        public bool GetUpdateSource()
        {
            string configMode = Configuration.GetValue(Constants.UpdateSource);
            return Convert.ToBoolean(configMode);
        }

        /// <summary>
        /// Sets the update prefered source to get updated from. 
        /// </summary>
        /// <param name="updateSource">True to get online updates. False to get updates from a local file.</param>
        public void SetUpdateSource(bool updateSource)
        {
            Configuration.SetValue(Constants.UpdateSource, updateSource ? "True" : "False");
        }

        /// <summary>
        /// Enable or disable launch the application at system start up.
        /// </summary>
        /// <param name="enable">True to enable start up otherwise False</param>
        public void SetAutoRun(bool enable)
        {
            Configuration.SetPlainValue(
                    Constants.AutoRun,
                    BitConverter.GetBytes(enable)
                    );
        }

        /// <summary>
        /// Get the current setting whether the application launches at system start up or not.
        /// </summary>
        /// <returns>True when the application runs at system start up, otherwise False</returns>
        public bool GetAutoRun()
        {
            byte[] rawValue = Configuration.GetPlainValue(Constants.AutoRun);

            if (rawValue == null)
                return false;

            bool configValue = BitConverter.ToBoolean(
                Configuration.GetPlainValue(Constants.AutoRun),
                0
                );

            return configValue;
        }

        public void SetStartInBackGround(bool enable)
        {
            Configuration.SetPlainValue(
                Constants.StartInBackground,
                BitConverter.GetBytes(enable)
                );
        }

        public bool GetStartInBackGround()
        {
            byte[] rawValue = Configuration.GetPlainValue(Constants.StartInBackground);

            if (rawValue == null)
                return false;

            bool configValue = BitConverter.ToBoolean(
                Configuration.GetPlainValue(Constants.StartInBackground),
                0
                );

            return configValue;
        }
    }
}
