using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Frotas
{
    [CustomAuthorize("Frotas/BombaAbastecimento")]
    public class BombaAbastecimentoController : BaseController
    {
		#region Construtores

		public BombaAbastecimentoController(Conexao conexao) : base(conexao) { }

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
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frotas.BombaAbastecimento repBombaAbastecimento = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento bombaAbastecimento = new Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento();

                PreencherEntidade(bombaAbastecimento, unitOfWork);

                unitOfWork.Start();

                repBombaAbastecimento.Inserir(bombaAbastecimento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.BombaAbastecimento repBombaAbastecimento = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento bombaAbastecimento = repBombaAbastecimento.BuscarPorCodigo(codigo, true);

                if (bombaAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(bombaAbastecimento, unitOfWork);

                unitOfWork.Start();

                repBombaAbastecimento.Atualizar(bombaAbastecimento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.BombaAbastecimento repBombaAbastecimento = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento bombaAbastecimento = repBombaAbastecimento.BuscarPorCodigo(codigo, true);

                if (bombaAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repBombaAbastecimento.Deletar(bombaAbastecimento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Frotas.BombaAbastecimento repBombaAbastecimento = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento bombaAbastecimento = repBombaAbastecimento.BuscarPorCodigo(codigo, false);

                if (bombaAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var testeLocalArmazenamento = bombaAbastecimento.LocalArmazenamentoProduto?.Codigo ?? 0;
                var testeLocalAmrzenamento2 = bombaAbastecimento.LocalArmazenamentoProduto?.Codigo > 0 ? bombaAbastecimento.LocalArmazenamentoProduto?.Descricao : string.Empty;

                return new JsonpResult(new
                {
                    bombaAbastecimento.Codigo,
                    bombaAbastecimento.Descricao,
                    bombaAbastecimento.CodigoBombaIntegracao,
                    bombaAbastecimento.CodigoBicoIntegracao,
                    LocalArmazenamento = new { Codigo = bombaAbastecimento.LocalArmazenamentoProduto?.Codigo ?? 0, Descricao = bombaAbastecimento.LocalArmazenamentoProduto?.Codigo > 0 ? bombaAbastecimento.LocalArmazenamentoProduto?.Descricao : string.Empty },
            });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento bombaAbastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(unitOfWork);

            int.TryParse(Request.Params("LocalArmazenamento"), out int codigoLocalArmazenamento);
            string descricao = Request.GetStringParam("Descricao");
            string codigoBombaIntegracao = Request.GetStringParam("CodigoBombaIntegracao");
            string codigoBicoIntegracao = Request.GetStringParam("CodigoBicoIntegracao");

            bombaAbastecimento.Descricao = !String.IsNullOrEmpty(descricao) ? descricao : throw new ControllerException("A descrição não pode ser nula ou vazia!");
            bombaAbastecimento.LocalArmazenamentoProduto = codigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(codigoLocalArmazenamento) : throw new ControllerException("Local de Armazenamento não cadastrado!");
            bombaAbastecimento.CodigoBombaIntegracao = codigoBombaIntegracao;
            bombaAbastecimento.CodigoBicoIntegracao = codigoBicoIntegracao;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            int.TryParse(Request.Params("LocalArmazenamento"), out int codigoLocalArmazenamento);

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                LocalArmazenamento = codigoLocalArmazenamento
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Código integração", "CodigoBombaIntegracao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Local de Armazenamento", "LocalArmazenamentoProduto", 15, Models.Grid.Align.center, false);

            return grid;
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frotas.BombaAbastecimento repBombaAbastecimento = new Repositorio.Embarcador.Frotas.BombaAbastecimento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaBombaAbastecimento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            List<Dominio.Entidades.Embarcador.Frotas.BombaAbastecimento> listaBombaAbastecimento = repBombaAbastecimento.Consultar(filtrosPesquisa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repBombaAbastecimento.ContarConsulta(filtrosPesquisa);

            var dynListaBombaAbastecimento = from bombaAbastecimento in listaBombaAbastecimento
                                    select new
                                    {
                                        bombaAbastecimento.Codigo,
                                        bombaAbastecimento.Descricao,
                                        bombaAbastecimento.CodigoBombaIntegracao,
                                        LocalArmazenamentoProduto = bombaAbastecimento.LocalArmazenamentoProduto.Codigo > 0 ? bombaAbastecimento.LocalArmazenamentoProduto.Descricao : string.Empty,
                                    };

            return dynListaBombaAbastecimento.ToList();
        }

        #endregion
    }
}
