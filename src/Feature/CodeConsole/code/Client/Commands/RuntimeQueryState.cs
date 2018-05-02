using Sitecore.Shell.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Feature.CodeConsole.Client.Commands
{
    [Serializable]
    public class RuntimeQueryState : Command
    {
        public RuntimeQueryState()
        {

        }

        public override void Execute(CommandContext context)
        {
            context.CustomData = "Result String";
        }

        public override CommandState QueryState(CommandContext context)
        {
            return (context.Parameters["ScriptRunning"] == "1" ? CommandState.Disabled : CommandState.Enabled);
        }
    }
}