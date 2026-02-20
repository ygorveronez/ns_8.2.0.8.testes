using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/DREGerencial")]
    public class DREGerencialController : BaseController
    {
		#region Construtores

		public DREGerencialController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BaixarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito repMovimento = new Repositorio.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                DateTime dataInicial = Request.GetDateTimeParam("DataInicial");
                DateTime dataFinal = Request.GetDateTimeParam("DataFinal");

                int codigoEmpresa = 0;
                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R127_DREGerencial, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R127_DREGerencial, TipoServicoMultisoftware, "Relatório de DRE Gerencial", "Financeiros", "DREGerencial.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial> dadosDREGerencial = repMovimento.ConsultarRelatorioDREGerencial(dataInicial, dataFinal, codigoEmpresa, tipoAmbiente);
                if (dadosDREGerencial.Count > 0)
                {
                    _ = Task.Factory.StartNew(() => GerarRelatorioDREGerencial(dataInicial, dataFinal, stringConexao, relatorioControleGeracao, dadosDREGerencial));
                    return new JsonpResult(true);
                }
                else
                    return new JsonpResult(false, true, "Nenhum registro de movimentações financeiras para regar o relatório.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorioDREGerencial(DateTime dataInicial, DateTime dataFinal, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial> dadosDREGerencial)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                ReportRequest.WithType(ReportType.DREGerencial)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DataInicial", dataInicial)
                    .AddExtraData("DataFinal", dataFinal)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .AddExtraData("DadosDREGerencial", dadosDREGerencial.ToJson())
                    .AddExtraData("CodigoEmpresa", Empresa.Codigo)
                    .CallReport();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion
    }
}
