using Sitecore.Feature.CodeConsole.Client.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.CodeConsole.Client.Controls
{
    public class SessionCompleteEventArgs : EventArgs
    {
        public RunnerOutput RunnerOutput { get; set; }

        public SessionCompleteEventArgs() { }
    }
}