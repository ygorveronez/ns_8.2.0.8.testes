using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/MagazineLuiza")]
    public class MagazineLuizaController : BaseController
    {
		#region Construtores

		public MagazineLuizaController(Conexao conexao) : base(conexao) { }

		#endregion

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadAsync()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                byte[] relatorioAsync = ReportRequest.WithType(ReportType.MagazineLuiza)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("DataInicial", dataInicial)
                    .AddExtraData("DataFinal", dataFinal)
                    .CallReport()
                    .GetContentFile();

                return Arquivo(relatorioAsync, "application/msexcel", "Relatório Magazine Luiza.xls");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }
    }
}
