using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/ConfiguracaoSubcontratacaoTabelaFrete")]
    public class ConfiguracaoSubcontratacaoTabelaFreteController : BaseController
    {
		#region Construtores

		public ConfiguracaoSubcontratacaoTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Enum.TryParse(Request.Params("TipoArquivo"), out Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo);
                bool tipoArquivoPDF = tipoArquivo == Dominio.Enumeradores.TipoArquivoRelatorio.PDF;
                
                byte[] relatorio = ReportRequest.WithType(ReportType.ConfiguracaoSubcontratacaoTabelaFrete)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("GrupoPessoas", Request.Params("GrupoPessoas"))
                    .AddExtraData("TabelaFrete", Request.Params("TabelaFrete"))
                    .AddExtraData("TipoArquivoPDF", tipoArquivoPDF)
                    .CallReport()
                    .GetContentFile();

                if (tipoArquivoPDF)
                    return Arquivo(relatorio, "application/pdf", "Configuração de Subcontratação da Tabela de Frete.pdf");
                else
                    return Arquivo(relatorio, "application/xls", "Configuração de Subcontratação da Tabela de Frete.xls");
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}
