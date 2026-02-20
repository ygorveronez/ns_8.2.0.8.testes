using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/FalhaNumeracaoCTe")]
    public class FalhaNumeracaoCTeController : BaseController
    {
		#region Construtores

		public FalhaNumeracaoCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> Download()
        {
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                if (dataInicial <= DateTime.MinValue)
                    return new JsonpResult(false, false, "Favor informe a data inicial.");
                if (dataFinal <= DateTime.MinValue)
                    return new JsonpResult(false, false, "Favor informe a data final.");

                byte[] relatorio = ReportRequest.WithType(ReportType.FalhaNumeracaoCTe)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("DataInicial", dataInicial)
                    .AddExtraData("DataFinal", dataFinal)
                    .CallReport()
                    .GetContentFile();

                return Arquivo(relatorio, "application/pdf", "Relatório Falha Numeração CT-e.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }
    }
}
