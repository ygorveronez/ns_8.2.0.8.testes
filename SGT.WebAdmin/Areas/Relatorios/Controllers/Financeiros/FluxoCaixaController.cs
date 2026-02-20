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
	public class FluxoCaixaController : BaseController
    {
		#region Construtores

		public FluxoCaixaController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BaixarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoEmpresa = 0, codigoTipoPagamentoRecebimento;
                codigoEmpresa = int.Parse(Request.Params("Empresa"));
                codigoTipoPagamentoRecebimento = int.Parse(Request.Params("TipoPagamentoRecebimento"));

                bool tituloPendente = true, limiteConta = true;
                bool.TryParse(Request.Params("TituloPendente"), out tituloPendente);
                bool.TryParse(Request.Params("LimiteConta"), out limiteConta);

                DateTime dataVencimentoInicial, dataVencimentoFinal;
                DateTime.TryParseExact(Request.Params("DataVencimentoInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoInicial);
                DateTime.TryParseExact(Request.Params("DataVencimentoFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataVencimentoFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo provisaoPesquisaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo.ComProvisao;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico analiticoSintetico;
                Enum.TryParse(Request.Params("Provisao"), out provisaoPesquisaTitulo);
                Enum.TryParse(Request.Params("AnaliticoSintetico"), out analiticoSintetico);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Dominio.Entidades.Empresa empresa;
                if (codigoEmpresa > 0)
                    empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                else
                    empresa = this.Usuario.Empresa;
                string nomeEmpresa = empresa.RazaoSocial;
                string cnpjEmpresa = empresa.CNPJ_Formatado;
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R130_FluxoCaixa, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R130_FluxoCaixa, TipoServicoMultisoftware, "Relatório de Fluxo de Caixa", "Financeiros", "FluxoCaixa.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa> dadosFluxoCaixa = repTitulo.ConsultarRelatorioFluxoCaixa(dataVencimentoInicial, dataVencimentoFinal, codigoEmpresa, tipoAmbiente, provisaoPesquisaTitulo, TipoServicoMultisoftware);
                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta> dadosFluxoCaixaConta = repTitulo.ConsultarRelatorioFluxoCaixaConta(codigoEmpresa, codigoTipoPagamentoRecebimento, tipoAmbiente, TipoServicoMultisoftware);
                if (dadosFluxoCaixa.Count > 0 && dadosFluxoCaixaConta.Count > 0)
                {
                    _ = Task.Factory.StartNew(() => GerarRelatorioFluxoCaixa(dataVencimentoInicial, dataVencimentoFinal, codigoTipoPagamentoRecebimento, tituloPendente, limiteConta, provisaoPesquisaTitulo, analiticoSintetico, cnpjEmpresa, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosFluxoCaixa, dadosFluxoCaixaConta, codigoEmpresa, tipoAmbiente));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, true, "Nenhum registro de títulos e movimentos nas contas de pagamento/Recebimento para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        private async Task GerarRelatorioFluxoCaixa(DateTime dataVencimentoInicial, DateTime dataVencimentoFinal, int codigoTipoPagamentoRecebimento, bool tituloPendente, bool limiteConta, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProvisaoPesquisaTitulo provisaoPesquisaTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico analiticoSintetico, string cnpjEmpresa, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixa> dadosFluxoCaixa, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.FluxoCaixaConta> dadosFluxoCaixaConta, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.FluxoCaixa)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DataVencimentoInicial", dataVencimentoInicial)
                    .AddExtraData("DataVencimentoFinal", dataVencimentoFinal)
                    .AddExtraData("CodigoTipoPagamentoRecebimento", codigoTipoPagamentoRecebimento)
                    .AddExtraData("TituloPendente", tituloPendente)
                    .AddExtraData("LimiteConta", limiteConta)
                    .AddExtraData("ProvisaoPesquisaTitulo", provisaoPesquisaTitulo.ToJson())
                    .AddExtraData("AnaliticoSintetico", analiticoSintetico.ToJson())
                    .AddExtraData("CnpjEmpresa", cnpjEmpresa)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("DadosFluxoCaixa", dadosFluxoCaixa.ToJson())
                    .AddExtraData("DadosFluxoCaixaConta", dadosFluxoCaixaConta.ToJson())
                    .AddExtraData("CodigoEmpresa", codigoEmpresa)
                    .AddExtraData("TipoAmbiente", tipoAmbiente)
                    .AddExtraData("RelatorioControleGeracao", relatorioControleGeracao.Codigo)
                    .CallReport();

                if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                    serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, result.ErrorMessage);
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
    }
}
