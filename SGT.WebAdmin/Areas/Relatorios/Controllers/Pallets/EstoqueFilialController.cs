using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Entidades.Embarcador.Relatorios;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Pallets
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Pallets/EstoqueFilial")]
    public class EstoqueFilialController : BaseController
    {
		#region Construtores

		public EstoqueFilialController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R048_Pallets_Estoque_Filial;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                var codigoRelatorio = Request.GetIntParam("Codigo");
                var serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                var relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Estoque de Pallets por Filial", "Pallets", "EstoqueFilial.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                var gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

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

                var filiais = Request.GetListParam<int>("Filial");
                var dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                var dataFim = Request.GetNullableDateTimeParam("DataFim");
                var repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var propriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(grid.group.enable ? grid.group.propAgrupa : "");
                var propriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(grid.header[grid.indiceColunaOrdena].data);
                var movimentacaoEstoquePalletsFilial = repositorioEstoque.ConsultarFilial(filiais, dataInicio, dataFim, propriedadeAgrupar, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repositorioEstoque.ContarConsultaFilial(filiais, dataInicio, dataFim));

                grid.AdicionaRows((
                    from movimentacao in movimentacaoEstoquePalletsFilial
                    select new
                    {
                        movimentacao.Codigo,
                        Data = movimentacao.Data.ToString("dd/MM/yyyy HH:mm"),
                        Filial = movimentacao.Filial?.Descricao,
                        FilialCnpj = movimentacao.Filial?.CNPJ_Formatado,
                        FilialCodigoIntegracao = movimentacao.Filial?.CodigoFilialEmbarcador,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        movimentacao.Observacao,
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        Descarte = (movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? movimentacao.QuantidadeDescartada : movimentacao.QuantidadeDescartada * -1),
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
                var filiais = Request.GetListParam<int>("Filial");
                var dataInicio = Request.GetNullableDateTimeParam("DataInicio");
                var dataFim = Request.GetNullableDateTimeParam("DataFim");

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                var dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                var relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                var relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                var relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                var mdlRelatorio = new Models.Grid.Relatorio();
                var grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                var propOrdena = relatorioTemp.PropriedadeOrdena;
                var stringConexao = _conexao.StringConexao;
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, relatorioTemp.PropriedadeAgrupa);

                _ = Task.Factory.StartNew(() => GerarRelatorioEstoqueFilial(agrupamentos, filiais, dataInicio, dataFim, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

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
            grid.AdicionarCabecalho("Data", "Data", 12, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Filial", "Filial", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ", "FilialCnpj", 13, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Código Integração", "FilialCodigoIntegracao", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Lançamento", "TipoLancamento", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Entrada", "Entrada", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saída", "Saida", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Descarte", "Descarte", 8, Models.Grid.Align.right, false, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Saldo Total", "SaldoTotal", 8, Models.Grid.Align.right, false, false, false, false, true);

            return grid;
        }

        private async Task GerarRelatorioEstoqueFilial(List<PropriedadeAgrupamento> agrupamentos, List<int> codigosFilial, DateTime? dataInicio, DateTime? dataFim, RelatorioControleGeracao relatorioControleGeracao, Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            var servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                var repositorioEstoque = new Repositorio.Embarcador.Pallets.EstoquePallet(unitOfWork);
                var propriedadeAgrupar = ObterPropriedadeOrdenarOuAgrupar(relatorioTemp.PropriedadeAgrupa);
                var propriedadeOrdenar = ObterPropriedadeOrdenarOuAgrupar(relatorioTemp.PropriedadeOrdena);
                var movimentacaoEstoquePalletsFilial = repositorioEstoque.ConsultarFilial(codigosFilial, dataInicio, dataFim, propriedadeAgrupar, propriedadeOrdenar, relatorioTemp.OrdemOrdenacao, 0, 0);

                List<Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueFilial> dynEstoque = (
                    from movimentacao in movimentacaoEstoquePalletsFilial
                    select new Dominio.Relatorios.Embarcador.DataSource.Pallets.EstoqueFilial()
                    {
                        Codigo = movimentacao.Codigo,
                        Data = movimentacao.Data,
                        Filial = movimentacao.Filial?.Descricao,
                        FilialCodigoIntegracao = movimentacao.Filial?.CodigoFilialEmbarcador,
                        FilialCnpj = movimentacao.Filial?.CNPJ_Formatado,
                        TipoLancamento = movimentacao.ObterTipoLancamento(),
                        Observacao = movimentacao.Observacao,
                        Entrada = movimentacao.ObterQuantidadeEntrada(),
                        Saida = movimentacao.ObterQuantidadeSaida(),
                        Descarte = (movimentacao.TipoMovimentacao == TipoMovimentacaoEstoquePallet.Entrada ? movimentacao.QuantidadeDescartada : movimentacao.QuantidadeDescartada * -1),
                        SaldoTotal = movimentacao.SaldoTotal,
                    }
                ).ToList();

                var parametros = new List<Parametro>();

                if (codigosFilial?.Count() > 0)
                {
                    if (codigosFilial.Count() == 1)
                    {
                        var repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                        var filial = repositorioFilial.BuscarPorCodigo(codigosFilial.FirstOrDefault());

                        parametros.Add(new Parametro("Filial", filial.Descricao, true));
                    }
                    else
                        parametros.Add(new Parametro("Filial", "Múltiplos Registros Selecionados", true));
                }
                else
                    parametros.Add(new Parametro("Filial", false));

                if (dataInicio.HasValue || dataFim.HasValue)
                {
                    string data = "";
                    data += dataInicio.HasValue ? dataInicio.Value.ToString("dd/MM/yyyy") + " " : "";
                    data += dataFim.HasValue ? "até " + dataFim.Value.ToString("dd/MM/yyyy") : "";
                    parametros.Add(new Parametro("Periodo", data, true));
                }
                else
                    parametros.Add(new Parametro("Periodo", false));

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Pallets/EstoqueFilial",parametros,relatorioControleGeracao, relatorioTemp, dynEstoque, unitOfWork, null, null, true, TipoServicoMultisoftware);

            }
            catch (Exception ex)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string nomePropriedadeOrdenar)
        {
            if (nomePropriedadeOrdenar == "Filial")
                return "Filial.Descricao";

            return nomePropriedadeOrdenar;
        }

        #endregion
    }
}
