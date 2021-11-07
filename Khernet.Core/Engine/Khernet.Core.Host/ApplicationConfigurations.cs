using Khernet.Core.Common;
using Khernet.Core.Utility;
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
    }
}
