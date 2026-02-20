using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contatos
{
    [CustomAuthorize("Contatos/SituacaoContato", "SAC/AtendimentoCliente")]
    public class SituacaoContatoController : BaseController
    {
		#region Construtores

		public SituacaoContatoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 20, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                string descricao = Request.Params("Descricao");
                if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
                Enum.TryParse(Request.Params("Status"), out status);

                // Consulta
                List<Dominio.Entidades.Embarcador.Contatos.SituacaoContato> listaGrid = repSituacaoContato.Consultar(descricao, status, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repSituacaoContato.ContarConsulta(descricao, status);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
                                obj.Descricao,
                                obj.Observacao,
                                obj.DescricaoAtivo
                            };

                // Seta valores na grid e rotarna conteudo
                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista.ToList());

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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);

                // Busca informacoes
                List<Dominio.Entidades.Embarcador.Contatos.SituacaoContato> situacoesContato = repSituacaoContato.BuscarTodos(true);

                // Formata retorno
                var retorno = (from obj in situacoesContato
                               select new
                               {
                                   value = obj.Codigo,
                                   text = obj.Descricao
                               }).ToList();

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.SituacaoContato situacaoContato = repSituacaoContato.BuscarPorCodigo(codigo);

                // Valida
                if (situacaoContato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    situacaoContato.Codigo,
                    situacaoContato.Descricao,
                    situacaoContato.Observacao,
                    situacaoContato.Ativo
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
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.SituacaoContato situacaoContato = new Dominio.Entidades.Embarcador.Contatos.SituacaoContato();

                // Preenche entidade com dados
                PreencheEntidade(ref situacaoContato, unitOfWork);

                // Persiste dados
                repSituacaoContato.Inserir(situacaoContato, Auditado);

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
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.SituacaoContato situacaoContato = repSituacaoContato.BuscarPorCodigo(codigo, true);

                // Valida
                if (situacaoContato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref situacaoContato, unitOfWork);

                // Persiste dados
                repSituacaoContato.Atualizar(situacaoContato, Auditado);

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
                Repositorio.Embarcador.Contatos.SituacaoContato repSituacaoContato = new Repositorio.Embarcador.Contatos.SituacaoContato(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Contatos.SituacaoContato situacaoContato = repSituacaoContato.BuscarPorCodigo(codigo);

                // Valida
                if (situacaoContato == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repSituacaoContato.Deletar(situacaoContato, Auditado);

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

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Contatos.SituacaoContato situacaoContato, Repositorio.UnitOfWork unitOfWork)
        {
            // Converte valores
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            bool ativo;
            bool.TryParse(Request.Params("Ativo"), out ativo);


            // Vincula dados
            situacaoContato.Descricao = descricao;
            situacaoContato.Observacao = observacao;
            situacaoContato.Ativo = ativo;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }

        #endregion
    }
}
