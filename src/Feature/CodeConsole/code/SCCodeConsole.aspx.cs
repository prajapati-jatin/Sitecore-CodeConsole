using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sitecore.Feature.CodeConsole
{
    public partial class SCCodeConsole : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Sitecore.Context.User.IsAuthenticated)
            {
                Response.Redirect("/sitecore/login?returnUrl=/sccodeconsole.aspx");
            }
        }
    }
}