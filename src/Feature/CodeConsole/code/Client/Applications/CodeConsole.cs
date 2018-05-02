using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Mvc.Controllers;
using Sitecore.Security;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls.Ribbons;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Sitecore.Feature.CodeConsole.Client.Applications
{
    public class CodeConsole : BaseForm
    {
        protected Literal CoveringClass;

        protected Border RibbonPanel;

        protected Memo Editor;

        protected String AppName
        {
            get
            {
                String str = StringUtil.GetString(Context.ClientPage.ServerProperties["AppName"]);
                return (String.IsNullOrEmpty(str) ? "Code Console" : str);
            }
            set
            {
                Context.ClientPage.ServerProperties["AppName"] = value ?? "Code Console";
            }
        }

        protected Boolean ScriptRunning
        {
            get
            {
                Boolean str = StringUtil.GetString(Context.ClientPage.ServerProperties["ScriptRunning"]) == "1";
                return str;
            }
            set
            {
                Context.ClientPage.ServerProperties["ScriptRunning"] = (value ? "1" : String.Empty);
            }
        }

        public static Boolean CodeModified
        {
            get
            {
                Boolean str = StringUtil.GetString(Context.ClientPage.ServerProperties["CodeModified"]) == "1";
                return str;
            }
            set
            {
                Context.ClientPage.ServerProperties["CodeModified"] = (value ? "1" : String.Empty);
            }
        }

        public Boolean Compiled
        {
            get
            {
                Boolean str = StringUtil.GetString(Context.ClientPage.ServerProperties["Compiled"]) == "1";
                return str;
            }
            set
            {
                Context.ClientPage.ServerProperties["Compiled"] = (value ? "1" : String.Empty);
            }
        }

        public static String AssemblyPath
        {
            get
            {
                return StringUtil.GetString(Context.ClientPage.ServerProperties["AssemblyPath"], String.Empty);
            }
            set
            {
                Context.ClientPage.ServerProperties["AssemblyPath"] = value;
            }
        }

        private readonly Sitecore.Foundation.CodeConsole.SCCodeConsole compiler;

        public CodeConsole()
        {
            compiler = new Sitecore.Foundation.CodeConsole.SCCodeConsole();
            this.Compiled = false;
        }

        [HandleMessage("codeconsole:compile", true)]
        protected virtual void Compile(ClientPipelineArgs args)
        {
            this.Compiled = false;
            args.Parameters.Add("message", "codeconsole:compile");            
            UpdateElementText("ScriptResult", "Build started...");
            this.CompileCode(this.Editor.Value);
            CodeModified = false;
        }

        [HandleMessage("codeconsole:execute", true)]
        protected virtual void Execute(ClientPipelineArgs args)
        {
            if (!Compiled)
            {
                this.CompileCode(this.Editor.Value);
            }
            if (Compiled)
            {
                args.Parameters.Add("message", "codeconsole:execute");
                try
                {
                    var output = this.compiler.ExecuteAssembly(AssemblyPath);
                    UpdateElementText("ScriptResult", output);
                }
                catch (Exception ex)
                {
                    var runtimeError = $"Runtime error:\n{ex.InnerException.Message} -> {ex.InnerException.StackTrace}";
                    UpdateElementText("ScriptResult", runtimeError);
                }
            }
        }

        private void CompileCode(String codeToCompile)
        {
            this.ScriptRunning = true;            
            CompilerResults results = this.compiler.Compile(codeToCompile);
            if(results.NativeCompilerReturnValue == 0)
            {
                AssemblyPath = results.PathToAssembly;
                this.UpdateElementText("ScriptResult", "Build succeeded");
                this.ScriptRunning = false;
                this.Compiled = true;
            }
            else
            {
                var errors = new System.Text.StringBuilder("Build error(s) found!").AppendLine();
                foreach (CompilerError error in results.Errors)
                {
                    if (error.IsWarning)
                    {
                        errors.AppendLine($"<div class=\"warning\"><strong>{error.ErrorNumber}</strong> {error.ErrorText}</div>");
                    }
                    else
                    {
                        errors.AppendLine($"<div class=\"error\"><strong>{error.ErrorNumber}</strong> {error.ErrorText}</div>");
                    }
                }
                this.UpdateElementText("ScriptResult", errors.ToString());
                this.ScriptRunning = false;
                this.Compiled = false;
            }
        }

        private void UpdateElementText(String elementName, String message)
        {
            Context.ClientPage.ClientResponse.SetInnerHtml(elementName, message);
        }

        protected override void OnLoad(EventArgs e)
        {            
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                this.UpdateRibbon();
                this.Editor.Value = GetDefaultClassCode();
            }
        }

        private String GetDefaultClassCode()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using Sitecore;");
            sb.AppendLine("namespace SCCodeConsole {");
            sb.AppendLine("\tpublic class SCCode {");
            sb.AppendLine("\t\tpublic String Execute() {");
            sb.AppendLine("\t\t\tvar db = Sitecore.Configuration.Factory.GetDatabase(\"master\");");
            sb.AppendLine("\t\t\treturn \"Implement the method\";");
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        [HandleMessage("codeconsole:codemodified")]
        protected void SetCodeModified(Message message)
        {
            CodeModified = true;
            Compiled = false;
        }

        [HandleMessage("codeconsole:updateribbon")]
        protected void UpdateRibbon(Message message)
        {
            this.UpdateRibbon();
        }

        private void UpdateRibbon()
        {
            Ribbon ribbon = new Ribbon() { ID = "CodeConsoleRibbon" };
            ribbon.CommandContext = new Shell.Framework.Commands.CommandContext();
            Item itemRibbon = Context.Database.GetItem("/sitecore/content/Applications/Code Console/Code console/Ribbon");
            ribbon.CommandContext.RibbonSourceUri = itemRibbon.Uri;
            ribbon.CommandContext.Parameters["ScriptRunning"] = (this.ScriptRunning ? "1" : "0");
            this.RibbonPanel.InnerHtml = HtmlUtil.RenderControl(ribbon);
        }
    }
}