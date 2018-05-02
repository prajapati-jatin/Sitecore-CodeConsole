using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.CodeConsole.Client.Applications
{
    public class RunnerOutput
    {
        public List<String> CloseMessages { get; set; }        

        public Boolean CloseRunner { get; set; }

        public Exception Exception { get; set; }

        public Boolean HasErrors { get; set; }

        public String Output { get; set; }

        public RunnerOutput() { }
    }
}