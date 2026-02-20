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
	[CustomAuthorize("Relatorios/Financeiros/PlanoOrcamentario")]
    public class PlanoOrcamentarioController : BaseController
    {
		#region Construtores

		public PlanoOrcamentarioController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BaixarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Financeiro.PlanoOrcamentario repPlanoOrcamentario = new Repositorio.Embarcador.Financeiro.PlanoOrcamentario(unitOfWork);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoEmpresa = 0, codigoCentroResultado;
                codigoEmpresa = int.Parse(Request.Params("Empresa"));
                codigoCentroResultado = int.Parse(Request.Params("CentroResultado"));

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.Enumeradores.TipoAmbiente tipoAmbiente = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    tipoAmbiente = this.Usuario.Empresa.TipoAmbiente;
                }

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                string nomeEmpresa = empresa.RazaoSocial;
                string cnpjEmpresa = empresa.CNPJ_Formatado;
                string stringConexao = _conexao.StringConexao;

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = repRelatorio.BuscarPadraoPorCodigoControleRelatorio(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R128_PlanoOrcamentario, TipoServicoMultisoftware);
                if (relatorio == null)
                    relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R128_PlanoOrcamentario, TipoServicoMultisoftware, "Relatório de Plano Orçamentário", "Financeiros", "PlanoOrcamentario.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", 0, unitOfWork, false, false);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorio, this.Usuario, Dominio.Enumeradores.TipoArquivoRelatorio.PDF, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario> dadosPlanoOrcamentario = repPlanoOrcamentario.ConsultarRelatorioPlanoOrcamentario(dataInicial, dataFinal, codigoCentroResultado, codigoEmpresa, tipoAmbiente);
                if (dadosPlanoOrcamentario.Count > 0)
                {
                    _ = Task.Factory.StartNew(() => GerarRelatorioPlanoOrcamentario(dataInicial, dataFinal, codigoCentroResultado, cnpjEmpresa, nomeEmpresa, stringConexao, relatorioControleGeracao, dadosPlanoOrcamentario));
                    return new JsonpResult(true);
                }
                else
                {
                    return new JsonpResult(false, false, "Nenhum registro de movimentações financeiras para regar o relatório.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o relatório.");
            }
        }

        private async Task GerarRelatorioPlanoOrcamentario(DateTime dataInicial, DateTime dataFinal, int codigoCentroResultado, string cnpjEmpresa, string nomeEmpresa, string stringConexao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario> dadosPlanoOrcamentario)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
            try
            {
                var result = ReportRequest.WithType(ReportType.PlanoOrcamentario)
                    .WithExecutionType(ExecutionType.Async)
                    .AddExtraData("DataInicial", dataInicial)
                    .AddExtraData("DataFinal", dataFinal)
                    .AddExtraData("CodigoCentroResultado", codigoCentroResultado)
                    .AddExtraData("CnpjEmpresa", cnpjEmpresa)
                    .AddExtraData("NomeEmpresa", nomeEmpresa)
                    .AddExtraData("DadosPlanoOrcamentario", dadosPlanoOrcamentario.ToJson())
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
