using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Mvc.Controllers;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls.Ribbons;
using System;
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

        protected Memo UsingStatements;
        protected Memo MethodCode;
        protected Memo Code;

        private String DefaultClass = @"using System;
using Sitecore.Mvc.Presentation;
using Sitecore.Data;

namespace TestControllers{
	public class TestController : Controller{
		public ActionResult Test(){
			return Content(Sitecore.Context.Site.Name);
		}
	}
}";

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

        public CodeConsole()
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            
            this.UpdateRibbon();
            this.Code.Value = GetDefaultClassCode();
        }

        private String GetDefaultClassCode()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using Sitecore;");
            sb.AppendLine("namespace SCCodeConsole {");
            sb.AppendLine("\tpublic class SCCode {");
            sb.AppendLine("\t\tvar db = Sitecore.Configuration.Factory.GetDatabase(\"master\");");
            sb.AppendLine("\t\t");
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private void UpdateRibbon()
        {
            Ribbon ribbon = new Ribbon() { ID = "CodeConsoleRibbon" };
            ribbon.CommandContext = new Shell.Framework.Commands.CommandContext();
            Item item2 = Context.Database.GetItem("/sitecore/content/Applications/Code Console/Code console/Ribbon");
            ribbon.CommandContext.RibbonSourceUri = item2.Uri;
            this.RibbonPanel.InnerHtml = HtmlUtil.RenderControl(ribbon);
        }
    }
}