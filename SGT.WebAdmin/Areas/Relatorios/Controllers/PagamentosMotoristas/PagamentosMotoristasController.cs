using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Ocorrencias
{
	[Area("Relatorios")]
	public class PagamentosMotoristasController : BaseController
    {
		#region Construtores

		public PagamentosMotoristasController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/PagamentosMotoristas/PagamentoMotoristaTMS")]
        public async Task<IActionResult> PagamentoMotoristaTMS()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/PagamentosMotoristas/PendenciaMotorista")]
        public async Task<IActionResult> PendenciaMotorista()
        {
            return View();
        }
    }
}
