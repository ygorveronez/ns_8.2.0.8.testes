using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Relatorios.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/ExtratoAcertoViagem")]
    public class ExtratoAcertoViagemController : BaseController
    {
		#region Construtores

		public ExtratoAcertoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly CodigoControleRelatorios _codigoControleRelatorio = CodigoControleRelatorios.R244_ExtratoAcertoViagem;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Extrato Acerto Viagem", "Financeiros", "ExtratoAcertoViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Data", "asc", "", "", Codigo, unitOfWork, false, false, 10, "Arial", false, 0);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.ExtratoAcertoViagem servicoRelatorioExtratoAcertoViagem = new Servicos.Embarcador.Relatorios.Financeiros.ExtratoAcertoViagem(ConfiguracaoEmbarcador.NaoDescontarValorSaldoMotorista, unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioExtratoAcertoViagem.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoAcertoViagem> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Código", "Codigo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data", "Data", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Documento", "TipoDocumento", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nº Documento", "NumeroDocumento", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor Saída", "ValorSaida", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Entrada", "ValorEntrada", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Nº Frota", "NumeroFrota", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("CPF Motorista", "CPFMotorista", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Veículos", "Veiculos", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Saldo Anterior", "SaldoAnterior", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo", "Saldo", _tamanhoColunaMedia, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Numero Acerto", "NumeroAcerto", _tamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            List<int> justificativas = Request.GetListParam<int>("Justificativa");

            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Motorista = Request.GetIntParam("Motorista"),
                Veiculo = Request.GetIntParam("Veiculo"),
                SegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo"),
                SituacaoAcerto = Request.GetListParam<int>("SituacaoAcerto"),
                TipoLancamento = Request.GetStringParam("TipoLancamento"),
                Justificativas = Request.GetListParam<int>("Justificativa"),
                CentroResultado = Request.GetIntParam("CentroResultado"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
