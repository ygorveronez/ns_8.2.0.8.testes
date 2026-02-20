using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Imposto
{
    public class ImpostoController : BaseController
    {
        public ImpostoController(Conexao conexao) : base(conexao)
        {
        }

        [CustomAuthorize("Imposto/OutrasAliquotas")]
        public async Task<IActionResult> OutrasAliquotas()
        {
            return View();
        }
    }
}
