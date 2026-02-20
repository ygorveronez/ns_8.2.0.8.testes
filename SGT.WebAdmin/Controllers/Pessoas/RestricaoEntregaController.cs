using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    public class RestricaoEntregaController : BaseController
    {
		#region Construtores

		public RestricaoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Ativo, "Ativo", 20, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                string descricao = Request.Params("Descricao") ?? string.Empty;
                Enum.TryParse(Request.Params("Ativo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo);

                // Consulta
                List<Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega> listaGrid = repRestricaoEntrega.Consultar(descricao, ativo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repRestricaoEntrega.ContarConsulta(descricao, ativo);

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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega = repRestricaoEntrega.BuscarPorCodigo(codigo);

                // Valida
                if (restricaoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    restricaoEntrega.Codigo,
                    restricaoEntrega.Descricao,
                    restricaoEntrega.Ativo,
                    restricaoEntrega.PrimeiraEntrega,
                    restricaoEntrega.CorVisualizacao,
                    restricaoEntrega.CodigoIntegracao,
                    restricaoEntrega.Email,
                    restricaoEntrega.Observacao,
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
                Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega = new Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega();

                // Preenche entidade com dados
                PreencheEntidade(ref restricaoEntrega, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(restricaoEntrega, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repRestricaoEntrega.Inserir(restricaoEntrega, Auditado);
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
                Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega = repRestricaoEntrega.BuscarPorCodigo(codigo, true);

                // Valida
                if (restricaoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref restricaoEntrega, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(restricaoEntrega, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repRestricaoEntrega.Atualizar(restricaoEntrega, Auditado);
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
                Repositorio.Embarcador.Pessoas.RestricaoEntrega repRestricaoEntrega = new Repositorio.Embarcador.Pessoas.RestricaoEntrega(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega = repRestricaoEntrega.BuscarPorCodigo(codigo);

                // Valida
                if (restricaoEntrega == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repRestricaoEntrega.Deletar(restricaoEntrega, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Converte valores
            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            bool.TryParse(Request.Params("PrimeiraEntrega"), out bool primeiraEntrega);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;
            string corVisualizacao = Request.Params("CorVisualizacao") ?? string.Empty;
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;
            string email = Request.Params("Email") ?? string.Empty;
            // Vincula dados
            restricaoEntrega.Descricao = descricao;
            restricaoEntrega.Observacao = observacao;
            restricaoEntrega.Ativo = ativo;
            restricaoEntrega.PrimeiraEntrega = primeiraEntrega;
            restricaoEntrega.CorVisualizacao = corVisualizacao;
            restricaoEntrega.CodigoIntegracao = codigoIntegracao;
            restricaoEntrega.Email = email;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Pessoas.RestricaoEntrega restricaoEntrega, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (restricaoEntrega.Descricao.Length == 0)
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
