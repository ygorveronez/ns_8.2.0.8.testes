using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Filiais
{
    [CustomAuthorize("Filiais/GestaoArmazem")]
    public class GestaoArmazemController : BaseController
    {
        #region Construtores

        public GestaoArmazemController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterGridPesquisa());
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> HistoricoArmazemProduto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazemHistorico repProdutoEmbarcadorEstoqueArmazemHistorico = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazemHistorico(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Auditado", "Auditado", 30, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Quantidade Anterior", "QuantidadeAnterior", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Quantidade Atualizada", "QuantidadeAtualizada", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Ação", "Acao", 15, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repProdutoEmbarcadorEstoqueArmazemHistorico.ContarConsulta(codigo);
                List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico> lista = (totalRegistros) > 0 ? repProdutoEmbarcadorEstoqueArmazemHistorico.Consultar(codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorEstoqueArmazemHistorico>();

                var listaRetornar = (
                    from item in lista
                    select new
                    {
                        Codigo = item.Codigo,
                        Data = item.DataAlteracao.ToDateTimeString(),
                        Auditado = item.Auditado,
                        QuantidadeAnterior = item.QuantidadeAnterior.ToString("n2"),
                        QuantidadeAtualizada = item.QuantidadeAtualizada.ToString("n2"),
                        Acao = item.Acao
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem repositorioGestaoArmazem = new Repositorio.Embarcador.Filiais.ProdutoEmbarcadorEstoqueArmazem(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código Filial", "CodigoFilial", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Filial", "DescricaoFilial", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Código Produto", "CodigoProduto", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Descrição do Produto", "DescricaoProduto", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Código do Armazém", "CodigoArmazem", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Estoque Disponível", "EstoqueDisponivel", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Estoque Sessão Roterização", "EstoqueSessaoRoterizacao", 20, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Estoque Final Previsto", "EstoqueFinalPrevisto", 20, Models.Grid.Align.center, false);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            int totalRegistros = repositorioGestaoArmazem.ContarGestaoArmazem(filtrosPesquisa);
            IList<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoArmazem> lista = totalRegistros > 0 ? repositorioGestaoArmazem.ConsultarGestaoArmazem(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoArmazem>();

            grid.AdicionaRows(lista);
            grid.setarQuantidadeTotal(totalRegistros);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaGestaoArmazem()
            {
                CodigosFilial = Request.GetListParam<int>("Filial"),
                CodigosProdutoEmbarcador = Request.GetListParam<int>("ProdutoEmbarcador"),
                CodigosArmazem = Request.GetListParam<int>("Armazem")
            };

            int codigoProdutoEmbarcador = Request.GetIntParam("CodigoProdutoEmbarcador");
            int codigoFilial = Request.GetIntParam("CodigoFilial");

            if (codigoProdutoEmbarcador > 0)
                filtrosPesquisa.CodigosProdutoEmbarcador.Add(codigoProdutoEmbarcador);

            if (codigoFilial > 0)
                filtrosPesquisa.CodigosFilial.Add(codigoFilial);

            return filtrosPesquisa;
        }

        #endregion
    }
}
