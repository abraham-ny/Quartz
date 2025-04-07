using System;

namespace Origin.Networking
{
    public class UserAgent
    {
        public string AgentString { get; private set; }

        public UserAgent()
        {
            // Default Vostro user agent string
            AgentString = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                         "Vostro/1.0 (KHTML, like Gecko) " +
                         "OriginBrowser/0.3.0";
        }

        public void SetUserAgent(string agentString)
        {
            AgentString = agentString;
        }

        public void SetVostroVersion(string version)
        {
            AgentString = AgentString.Replace("Vostro/1.0", $"Vostro/{version}");
        }

        public void SetBrowserVersion(string version) 
        {
            AgentString = AgentString.Replace("OriginBrowser/0.3.0", $"OriginBrowser/{version}");
        }
    }
}
