using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.AbastecimentoInterno
{
    public class AbastecimentoInternoController : BaseController
    {
        #region Construtores

        public AbastecimentoInternoController(Conexao conexao) : base(conexao) { }

        #endregion

        [CustomAuthorize("AbastecimentoInterno/MovimentacaoAbastecimento")]
        public async Task<IActionResult> MovimentacaoAbastecimento()
        {
            return View();
        }

        [CustomAuthorize("AbastecimentoInterno/MovimentoEntradaTanque")]
        public async Task<IActionResult> MovimentoEntradaTanque()
        {
            return View();
        }

        [CustomAuthorize("AbastecimentoInterno/LiberacaoAbastecimentoAutomatizado")]
        public async Task<IActionResult> LiberacaoAbastecimentoAutomatizado()
        {
            return View();
        }

        [CustomAuthorize("AbastecimentoInterno/MovimentacaoTanquesDetalhes")]
        public ActionResult MovimentacaoTanquesDetalhes()
        {
            return View();
        }
    }
}