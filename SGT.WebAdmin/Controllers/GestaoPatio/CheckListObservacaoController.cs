using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/CheckListObservacao")]
    public class CheckListObservacaoController : BaseController
    {
		#region Construtores

		public CheckListObservacaoController(Conexao conexao) : base(conexao) { }

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
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListObservacao repCheckListObservacao = new Repositorio.Embarcador.GestaoPatio.CheckListObservacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao checkListObservacao = repCheckListObservacao.BuscarPorCodigo(codigo);

                // Valida
                if (checkListObservacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    checkListObservacao.Codigo,
                    checkListObservacao.Descricao,
                    checkListObservacao.Ativo,
                    checkListObservacao.Categoria,
                    checkListObservacao.Observacao
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListObservacao repCheckListObservacao = new Repositorio.Embarcador.GestaoPatio.CheckListObservacao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao checkListObservacao = new Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao();

                // Preenche entidade com dados
                PreencheEntidade(ref checkListObservacao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(checkListObservacao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCheckListObservacao.Inserir(checkListObservacao);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListObservacao repCheckListObservacao = new Repositorio.Embarcador.GestaoPatio.CheckListObservacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao checkListObservacao = repCheckListObservacao.BuscarPorCodigo(codigo);

                // Valida
                if (checkListObservacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref checkListObservacao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(checkListObservacao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repCheckListObservacao.Atualizar(checkListObservacao);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
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
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.CheckListObservacao repCheckListObservacao = new Repositorio.Embarcador.GestaoPatio.CheckListObservacao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao checkListObservacao = repCheckListObservacao.BuscarPorCodigo(codigo);

                // Valida
                if (checkListObservacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repCheckListObservacao.Deletar(checkListObservacao);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados


        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Categoria", "Categoria", 15, Models.Grid.Align.left, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.GestaoPatio.CheckListObservacao repCheckListObservacao = new Repositorio.Embarcador.GestaoPatio.CheckListObservacao(unitOfWork);

            // Dados do filtro
            SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);
            else
                status = SituacaoAtivoPesquisa.Ativo;
            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao> listaGrid = repCheckListObservacao.Consultar(descricao, status,  propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repCheckListObservacao.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            Categoria = obj.Categoria.ObterDescricao(),
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao checkListObservacao, Repositorio.UnitOfWork unitOfWork)
        {
            // Converte valores
            string observacao = Request.Params("Observacao") ?? string.Empty;
            string descricao = Request.Params("Descricao") ?? string.Empty;
            Enum.TryParse(Request.Params("Categoria"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList categoria);
            bool.TryParse(Request.Params("Ativo"), out bool ativo);


            // Vincula dados
            checkListObservacao.Categoria = categoria;
            checkListObservacao.Descricao = descricao;
            checkListObservacao.Observacao = observacao;
            checkListObservacao.Ativo = ativo;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao checkListObservacao, out string msgErro)
        {
            msgErro = "";

            if (checkListObservacao.Ativo && checkListObservacao.Observacao.Length == 0)
            {
                msgErro = "Observação é obrigatória.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Relacional") propOrdenar = "Relacional.Codigo";
        }
        #endregion
    }
}
