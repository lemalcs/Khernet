using NetFwTypeLib;
using System;

namespace Khernet.Core.Utility
{
    public static class FirewallHelper
    {
        public static bool ExistsFirewallRule(string applicationName, string ruleName, int protocol)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            foreach (INetFwRule rule in firewallPolicy.Rules)
            {
                //Compare properties ignoring case
                if (string.Compare(rule.ApplicationName, applicationName, true) == 0 &
                    string.Compare(rule.Name, ruleName, true) == 0 &
                    rule.Protocol == protocol)
                    return true;
            }
            return false;
        }

        public static void CreateFirewallRule(string applicationPath, string ruleName, int protocol)
        {
            INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(
                      Type.GetTypeFromProgID("HNetCfg.FWRule"));

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(
                Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

            firewallRule.ApplicationName = applicationPath;
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.Protocol = protocol;
            firewallRule.Name = ruleName;

            firewallPolicy.Rules.Add(firewallRule);
        }
    }
}
