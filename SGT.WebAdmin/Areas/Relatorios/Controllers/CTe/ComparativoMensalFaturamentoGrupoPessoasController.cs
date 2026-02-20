using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/ComparativoMensalFaturamentoGrupoPessoas")]
    public class ComparativoMensalFaturamentoGrupoPessoasController : BaseController
    {
		#region Construtores

		public ComparativoMensalFaturamentoGrupoPessoasController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime.TryParseExact(Request.Params("DataInicialEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicialEmissao);
                DateTime.TryParseExact(Request.Params("DataFinalEmissao"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinalEmissao);

                string propriedadeVeiculo = Request.Params("PropriedadeVeiculo");

                int.TryParse(Request.Params("QuantidadeMesesAnteriores"), out int quantidadeMesesAnteriores);

                Enum.TryParse(Request.Params("TipoArquivo"), out Dominio.Enumeradores.TipoArquivoRelatorio tipoArquivo);

                bool tipoArquivoPDF = tipoArquivo == Dominio.Enumeradores.TipoArquivoRelatorio.PDF;

                if (dataInicialEmissao.Month != dataFinalEmissao.Month || dataInicialEmissao.Year != dataFinalEmissao.Year)
                    return new JsonpResult(false, true, "A data inicial e final devem estar no mesmo mês/ano.");

                byte[] relatorio = ReportRequest.WithType(ReportType.ComparativoMensalFaturamentoGrupoPessoas)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("GruposPessoas", Request.Params("GruposPessoas"))
                    .AddExtraData("DataInicialEmissao", dataInicialEmissao)
                    .AddExtraData("DataFinalEmissao", dataFinalEmissao)
                    .AddExtraData("PropriedadeVeiculo", propriedadeVeiculo)
                    .AddExtraData("QuantidadeMesesAnteriores", quantidadeMesesAnteriores)
                    .AddExtraData("TipoArquivoPDF", tipoArquivoPDF)
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport()
                    .GetContentFile();
                
                if (tipoArquivoPDF)
                    return Arquivo(relatorio, "application/pdf", "Comparativo Mensal de Faturamento por Grupo de Pessoas.pdf");
                else
                    return Arquivo(relatorio, "application/xls", "Comparativo Mensal de Faturamento por Grupo de Pessoas.xls");
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
