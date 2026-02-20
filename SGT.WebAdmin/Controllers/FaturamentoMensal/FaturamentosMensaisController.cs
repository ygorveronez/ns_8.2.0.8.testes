using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.FaturamentoMensal
{
    public class FaturamentosMensaisController : BaseController
    {
		#region Construtores

		public FaturamentosMensaisController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("FaturamentosMensais/FaturamentoMensalGrupo")]
        public async Task<IActionResult> FaturamentoMensalGrupo()
        {
            return View();
        }

        [CustomAuthorize("FaturamentosMensais/FaturamentoMensalCliente")]
        public async Task<IActionResult> FaturamentoMensalCliente()
        {
            return View();
        }

        [CustomAuthorize("FaturamentosMensais/FaturamentoMensal")]
        public async Task<IActionResult> FaturamentoMensal()
        {
            return View();
        }

        [CustomAuthorize("FaturamentosMensais/PlanoEmissaoFaturamento")]
        public async Task<IActionResult> PlanoEmissaoFaturamento()
        {
            return View();
        }

        [CustomAuthorize("FaturamentosMensais/ValidacaoFaturamentoMensal")]
        public async Task<IActionResult> ValidacaoFaturamentoMensal()
        {
            return View();
        }
        
    }
}
