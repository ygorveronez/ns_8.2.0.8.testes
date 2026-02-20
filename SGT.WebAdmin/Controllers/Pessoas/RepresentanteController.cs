using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/Representante")]
    public class RepresentanteController : BaseController
    {
		#region Construtores

		public RepresentanteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Ativo", "Ativo", 20, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                string descricao = Request.Params("Descricao") ?? string.Empty;
                Enum.TryParse(Request.Params("Ativo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo);

                // Consulta
                List<Dominio.Entidades.Embarcador.Pessoas.Representante> listaGrid = repRepresentante.Consultar(descricao, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repRepresentante.ContarConsulta(descricao, ativo);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                obj.Descricao,
                                Ativo = obj.DescricaoAtivo
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.Representante representante = repRepresentante.BuscarPorCodigo(codigo);

                // Valida
                if (representante == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    representante.Codigo,
                    representante.Descricao,
                    representante.Ativo,
                    representante.Observacao,
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
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.Representante representante = new Dominio.Entidades.Embarcador.Pessoas.Representante();

                // Preenche entidade com dados
                PreencheEntidade(ref representante, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(representante, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repRepresentante.Inserir(representante, Auditado);
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
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.Representante representante = repRepresentante.BuscarPorCodigo(codigo, true);

                // Valida
                if (representante == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref representante, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(representante, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repRepresentante.Atualizar(representante, Auditado);
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
                Repositorio.Embarcador.Pessoas.Representante repRepresentante = new Repositorio.Embarcador.Pessoas.Representante(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.Representante representante = repRepresentante.BuscarPorCodigo(codigo);

                // Valida
                if (representante == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repRepresentante.Deletar(representante, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pessoas.Representante representante, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Converte valores
            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            // Vincula dados
            representante.Descricao = descricao;
            representante.Observacao = observacao;
            representante.Ativo = ativo;

            // Dados Criacao 
            if (representante.Codigo == 0)
            {
                representante.DataCadastro = DateTime.Now;
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pessoas.Representante representante, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (representante.Descricao.Length == 0)
            {
                msgErro = "Descrição é obrigatório.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            /* PropOrdena
             * Recebe o campo ordenado na grid
             * Retorna o elemento especifico da entidade para ordenacao
             */
        }
        #endregion
    }
}
