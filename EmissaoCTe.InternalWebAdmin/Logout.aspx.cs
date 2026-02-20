using System;
using System.Web.Security;

namespace EmissaoCTe.InternalWebAdmin
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Redirect("Logon.aspx");
        }
    }
}