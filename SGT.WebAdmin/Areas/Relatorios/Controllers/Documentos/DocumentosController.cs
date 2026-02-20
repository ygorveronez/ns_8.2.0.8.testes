using SGT.WebAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Documentos
{
	[Area("Relatorios")]
	public class DocumentosController : BaseController
    {
		#region Construtores

		public DocumentosController(Conexao conexao) : base(conexao) { }

		#endregion

        [CustomAuthorize("Relatorios/Documentos/FaturaCIOT")]
        public async Task<IActionResult> FaturaCIOT()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Documentos/CargaCIOT")]
        public async Task<IActionResult> CargaCIOT()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Documentos/PagamentoProvisao")]
        public async Task<IActionResult> PagamentoProvisao()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Documentos/DadosDocsys")]
        public async Task<IActionResult> DadosDocsys()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Documentos/SerieDocumentos")]
        public async Task<IActionResult> SerieDocumentos()
        {
            return View();
        }

        [CustomAuthorize("Relatorios/Documentos/CargaCIOTPedido")]
        public async Task<IActionResult> CargaCIOTPedido()
        {
            return View();
        }

    }
}
