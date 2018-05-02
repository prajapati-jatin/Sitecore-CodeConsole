using Sitecore.Feature.CodeConsole.Client.Applications;
using Sitecore.Jobs.AsyncUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.CodeConsole.Client.Controls
{
    public class CompleteMessage : FlushMessage
    {
        public RunnerOutput RunnerOutput { get; set; }

        public CompleteMessage() { }
    }
}