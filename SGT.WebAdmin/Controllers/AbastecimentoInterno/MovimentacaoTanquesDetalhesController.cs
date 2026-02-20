using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.AbastecimentoInterno
{
    [CustomAuthorize("AbastecimentoInterno/MovimentacaoTanquesDetalhes")]
    public class MovimentacaoTanquesDetalhesController : BaseController
    {
        #region Construtores

        public MovimentacaoTanquesDetalhesController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                var propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, ref grid, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Manipula grids
                Models.Grid.Grid gridDetalhes = GridPesquisaDetalhes();

                // Ordenacao da grid
                var propOrdenar = gridDetalhes.header[gridDetalhes.indiceColunaOrdena].data;

                // Busca Dados
                int totalRegistrosDetalhes = 0;
                var listaDetalhes = ExecutaPesquisaDetalhes(ref totalRegistrosDetalhes, ref gridDetalhes, unitOfWork);

                // Seta valores na grid
                gridDetalhes.AdicionaRows(listaDetalhes);
                gridDetalhes.setarQuantidadeTotal(totalRegistrosDetalhes);

                return new JsonpResult(gridDetalhes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques()
            {
                LocalArmazenamento = Request.GetIntParam("LocalArmazenamento"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicialMovimentacao"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinalMovimentacao")
            };
            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques ObterFiltrosPesquisaDetalhes(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques()
            {
                LocalArmazenamento = Request.GetIntParam("LocalArmazenamentoDetalhes"),
                DataInicial = Request.GetDateTimeParam("DataInicialMovimentacaoDetalhes"),
                DataFinal = Request.GetDateTimeParam("DataFinalMovimentacaoDetalhes"),
            };
            return filtrosPesquisa;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("CodigoLocalArmazenamento", false);
            grid.AdicionarCabecalho("DataInicialMovimentacaoDetalhes", false);
            grid.AdicionarCabecalho("DataFinalMovimentacaoDetalhes", false);

            grid.AdicionarCabecalho("Local de Armazenamento", "LocalArmazenamento", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saldo Inicial", "SaldoInicialTanque", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Entrada", "ValorEntradaMovimentacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saída", "ValorSaidaMovimentacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saldo Atual", "SaldoAtualTanque", 10, Models.Grid.Align.left, true);
            return grid;
        }

        private Models.Grid.Grid GridPesquisaDetalhes()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Data", "DiaMesAnoDataExibirDetalhes", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Hora", "HorasMinutosDataExibirDetalhes", 5, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saldo Inicial", "SaldoInicialTanque", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Entrada", "ValorEntradaMovimentacaoDetalhes", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saída", "ValorSaidaMovimentacaoDetalhes", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Saldo Atual", "SaldoAtualTanque", 10, Models.Grid.Align.left, true);
            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistrosDetalhes, ref Models.Grid.Grid gridDetalhes, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamento = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques> listaLocalArmazenamento = repLocalArmazenamento.ConsultaMovimentacaoTanques(filtrosPesquisa);
            totalRegistrosDetalhes = listaLocalArmazenamento.Count();

            var dynListaLocalArmazenamento = from local in listaLocalArmazenamento
                                             select new
                                             {
                                                 local.CodigoLocalArmazenamento,
                                                 local.LocalArmazenamento,
                                                 local.SaldoInicialTanque,
                                                 local.ValorEntradaMovimentacao,
                                                 local.ValorSaidaMovimentacao,
                                                 local.SaldoAtualTanque,
                                                 DataInicialMovimentacaoDetalhes = filtrosPesquisa.DataInicial != null ? filtrosPesquisa.DataInicial : null,
                                                 DataFinalMovimentacaoDetalhes = filtrosPesquisa.DataFinal != null ? filtrosPesquisa.DataFinal : null,
                                             };
            return dynListaLocalArmazenamento.ToList();
        }

        private dynamic ExecutaPesquisaDetalhes(ref int totalRegistros, ref Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamento = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaMovimentacaoTanques filtrosPesquisa = ObterFiltrosPesquisaDetalhes(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            
            IList<Dominio.Relatorios.Embarcador.DataSource.Frota.MovimentacaoTanques> listaLocalArmazenamento = repLocalArmazenamento.ConsultaMovimentacaoTanquesDetalhes(filtrosPesquisa);
            totalRegistros = listaLocalArmazenamento.Count();

            var dynListaLocalArmazenamento = from local in listaLocalArmazenamento
                                             select new
                                             {
                                                 DiaMesAnoDataExibirDetalhes = local.DataExibirDetalhes.ToString("dd/MM/yyyy"),
                                                 HorasMinutosDataExibirDetalhes = local.DataExibirDetalhes.ToString("HH:mm"),
                                                 local.SaldoInicialTanque,
                                                 local.ValorEntradaMovimentacaoDetalhes,
                                                 local.ValorSaidaMovimentacaoDetalhes,
                                                 local.SaldoAtualTanque
                                             };
            return dynListaLocalArmazenamento.ToList();
        }

        #endregion
    }
}