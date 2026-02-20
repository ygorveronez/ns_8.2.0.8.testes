using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/EstoqueTransportador")]
    public class EstoqueTransportadorController : BaseController
    {
		#region Construtores

		public EstoqueTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R049_Pallets_Estoque_Transportador;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Estoque de Pallets por Transportador", "Pallets", "EstoqueTransportador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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
                var grid = new Models.Grid.Grid(Request);
                var repositorioEstoqueTransportador = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var propruiedadeAgrupar = grid.group.enable ? grid.group.propAgrupa : "";
                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                var movimentacoesEstoquePalletsTransportador = repositorioEstoqueTransportador.ConsultarTransportador(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorioEstoqueTransportador.ContarConsultaTransportador(filtrosPesquisa));

                grid.AdicionaRows((
                    from movimentacao in movimentacoesEstoquePalletsTransportador
                    select new
                    {
                        movimentacao.Codigo,
                        Data = movimentacao.Data.ToString("dd/MM/yyyy HH:mm"),
                        Transportador = movimentacao.Transportador.RazaoSocial,
                        TransportadorCnpj = movimentacao.Transportador.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = movimentacao.Transportador.CodigoIntegracao,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        movimentacao.Observacao,
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        Descarte = (movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? movimentacao.QuantidadeDescartada * -1 : movimentacao.QuantidadeDescartada),
                        movimentacao.SaldoTotal
                    }
                ).ToList());

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
                Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                string propOrdena = relatorioTemp.PropriedadeOrdena;

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorioEstoqueTransportador(agrupamentos, filtrosPesquisa, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "Data", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ", "TransportadorCnpj", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração", "TransportadorCodigoIntegracao", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Lançamento", "TipoLancamento", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Entrada", "Entrada", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saída", "Saida", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Descarte", "Descarte", 10, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 10, Models.Grid.Align.right, false, false, false, false, true);

            return grid;
        }

        private async Task GerarRelatorioEstoqueTransportador(List<PropriedadeAgrupamento> agrupamentos, Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo filtrosPesquisa, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioEstoqueTransportador = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var propriedadeOrdenar = ObterPropriedadeOrdenar(relatorioTemp.PropriedadeOrdena);
                var movimentacoesEstoquePalletsTransportador = repositorioEstoqueTransportador.ConsultarTransportador(filtrosPesquisa, propriedadeOrdenar, relatorioTemp.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueTransportador> dynEstoque = (
                    from movimentacao in movimentacoesEstoquePalletsTransportador
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueTransportador()
                    {
                        Codigo = movimentacao.Codigo,
                        Data = movimentacao.Data,
                        Transportador = movimentacao.Transportador.RazaoSocial,
                        TransportadorCnpj = movimentacao.Transportador.CNPJ_Formatado,
                        TransportadorCodigoIntegracao = movimentacao.Transportador.CodigoIntegracao ?? "",
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        Observacao = movimentacao.Observacao,
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        Descarte = (movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? movimentacao.QuantidadeDescartada * -1 : movimentacao.QuantidadeDescartada),
                        SaldoTotal = movimentacao.SaldoTotal
                    }
                ).ToList();

                var parametros = new List<Parametro>();

                if (filtrosPesquisa.CodigosTransportador?.Count() > 0)
                {
                    if (filtrosPesquisa.CodigosTransportador.Count() == 1)
                    {
                        var repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                        var transportador = repositorioEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigosTransportador.FirstOrDefault());

                        parametros.Add(new Parametro("Transportador", transportador.RazaoSocial, true));
                    }
                    else
                        parametros.Add(new Parametro("Transportador", "Múltiplos Registros selecionados", true));
                }
                else
                    parametros.Add(new Parametro("Transportador", false));

                if (filtrosPesquisa.DataMovimentoInicial.HasValue || filtrosPesquisa.DataMovimentoFinal.HasValue)
                {
                    string data = "";
                    data += filtrosPesquisa.DataMovimentoInicial.HasValue ? filtrosPesquisa.DataMovimentoInicial.Value.ToString("dd/MM/yyyy") + " " : "";
                    data += filtrosPesquisa.DataMovimentoFinal.HasValue ? "até " + filtrosPesquisa.DataMovimentoFinal.Value.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Parametro("Periodo", data, true));
                }
                else
                    parametros.Add(new Parametro("Periodo", false));

                serRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/EstoqueTransportador",parametros,relatorioControleGeracao, relatorioTemp, dynEstoque, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private string ObterPropriedadeOrdenar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Transportador")
                return "Transportador.RazaoSocial";

            return nomePropriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAjusteSaldo()
            {
                CodigosTransportador = Request.GetListParam<int>("Empresa"),
                DataMovimentoInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataMovimentoFinal = Request.GetNullableDateTimeParam("DataFim")
            };
        }

        #endregion
    }
}
