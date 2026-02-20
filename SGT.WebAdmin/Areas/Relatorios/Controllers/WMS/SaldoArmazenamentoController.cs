using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.WMS;
using Repositorio;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.WMS
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/WMS/SaldoArmazenamento")]
    public class SaldoArmazenamentoController : Relatorios.AutomatizacaoGeracaoRelatorioController<Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento>
    {
		#region Construtores

		public SaldoArmazenamentoController(Conexao conexao) : base(conexao) { }

		#endregion

		#region Atributos Privados

		Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R104_SaldoArmazenamento;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Saldo Armazenamento", "WMS", "SaldoArmazenamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.WMS.SaldoArmazenamento servicoRelatorioSaldoArmazenamento = new Servicos.Embarcador.Relatorios.WMS.SaldoArmazenamento(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioSaldoArmazenamento.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.WMS.SaldoArmazenamento> listaSaldoArmazenamento, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(listaSaldoArmazenamento);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.WMS.FiltroPesquisaRelatorioSaldoArmazenamento()
            {
                CodigoProdutoEmbarcador = Request.GetIntParam("ProdutoEmbarcador"),
                CodigoDeposito = Request.GetIntParam("Deposito"),
                CodigoBloco = Request.GetIntParam("Bloco"),
                CodigoPosicao = Request.GetIntParam("Posicao"),
                CodigoRua = Request.GetIntParam("Rua"),
                SaldoDisponivel = Request.GetBoolParam("SaldoDisponivel"),
                CodigoBarras = Request.GetStringParam("CodigoBarras"),
                Descricao = Request.GetStringParam("Descricao"),
                DataVencimentoInicial = Request.GetDateTimeParam("DataVencimentoInicial"),
                DataVencimentoFinal = Request.GetDateTimeParam("DataVencimentoFinal"),
                TipoRecebimento = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria>("TipoRecebimento"),
            };

            return filtrosPesquisa;
        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Produto Embarcador", "ProdutoEmbarcador", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Depósito", "Deposito", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Bloco", "Bloco", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Rua", "Rua", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Posição", "Posicao", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Local", "Abreviacao", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Barras", "CodigoBarras", 10, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimentoFormatada", 12, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Mercadoria", "TipoRecebimento", 12, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("UN", "UnidadeMedida", 6, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Qtd. Lote", "QuantidadeLote", 10, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Qtd. Disponível", "QuantidadeAtual", 10, Models.Grid.Align.right, true, false, false, false, true);

            return grid;
        }

        protected override Task<FiltroPesquisaRelatorioSaldoArmazenamento> ObterFiltrosPesquisaAsync(UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
