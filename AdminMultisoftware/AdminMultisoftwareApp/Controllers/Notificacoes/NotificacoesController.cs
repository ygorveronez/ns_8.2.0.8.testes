using System.Web.Mvc;

namespace AdminMultisoftwareApp.Controllers.Notificacoes
{
    public class NotificacoesController : BaseController
    {
        [CustomAuthorize("Notificacoes/MensagemAviso")]
        public ActionResult MensagemAviso()
        {
            return View();
        }
    }
}