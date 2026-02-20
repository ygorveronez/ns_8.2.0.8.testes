using System;
using System.Web;

namespace EmissaoCTe.InternalWebAdmin
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.lnkNomeUsuario.Text = HttpContext.Current.User.Identity.Name;
            this.lnkNomeEmpresa.Text = "Administrativo Interno";
        }
    }
}