using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escalas
{
    [CustomAuthorize("Escalas/Escala")]
    public class EscalaController : BaseController
    {
		#region Construtores

		public EscalaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Escalas.Escala repEscala = new Repositorio.Embarcador.Escalas.Escala(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Centro Carregamento", "CentroCarregamento", 30, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Centro Descarregamento", "CentroDescarregamento", 30, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Classificação", "Classificacao", 10, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.left, true, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                string descricao = Request.Params("Descricao") ?? string.Empty;

                Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala? classificacao = null;
                if (Enum.TryParse(Request.Params("Classificacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala classificacaoAux))
                    classificacao = classificacaoAux;

                // Consulta
                List<Dominio.Entidades.Embarcador.Escalas.Escala> listaGrid = repEscala.Consultar(descricao, classificacao, status, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repEscala.ContarConsulta(descricao, classificacao, status);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                Descricao = obj.Descricao,
                                CentroCarregamento = obj.CentroCarregamento.Descricao,
                                CentroDescarregamento = obj.CentroDescarregamento.Descricao,
                                Classificacao = obj.DescricaoClassificacao,
                                Status = obj.DescricaoStatus,
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
                Repositorio.Embarcador.Escalas.Escala repEscala = new Repositorio.Embarcador.Escalas.Escala(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escalas.Escala escala = repEscala.BuscarPorCodigo(codigo);

                // Valida
                if (escala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    escala.Codigo,
                    escala.Descricao,
                    escala.Status,
                    escala.Classificacao,
                    CentroCarregamento = escala.CentroCarregamento != null ? new { escala.CentroCarregamento.Codigo, escala.CentroCarregamento.Descricao } : null,
                    CentroDescarregamento = escala.CentroDescarregamento != null ? new { escala.CentroDescarregamento.Codigo, escala.CentroDescarregamento.Descricao } : null,
                    escala.Observacao
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
                Repositorio.Embarcador.Escalas.Escala repEscala = new Repositorio.Embarcador.Escalas.Escala(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escalas.Escala escala = new Dominio.Entidades.Embarcador.Escalas.Escala();

                // Preenche entidade com dados
                PreencheEntidade(ref escala, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(escala, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repEscala.Inserir(escala, Auditado);
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
                Repositorio.Embarcador.Escalas.Escala repEscala = new Repositorio.Embarcador.Escalas.Escala(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escalas.Escala escala = repEscala.BuscarPorCodigo(codigo, true);

                // Valida
                if (escala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref escala, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(escala, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repEscala.Atualizar(escala, Auditado);
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
                Repositorio.Embarcador.Escalas.Escala repEscala = new Repositorio.Embarcador.Escalas.Escala(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escalas.Escala escala = repEscala.BuscarPorCodigo(codigo);

                // Valida
                if (escala == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repEscala.Deletar(escala, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Escalas.Escala escala, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */

            // Instancia Repositorios
            Repositorio.Embarcador.Logistica.CentroCarregamento repCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

            // Converte valores
            int.TryParse(Request.Params("CentroCarregamento"), out int codigoCentroCarregamento);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento);

            int.TryParse(Request.Params("CentroDescarregamento"), out int codigoCentroDescarregamento);
            Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento);

            Enum.TryParse(Request.Params("Classificacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.ClassificacaoEscala classificao);

            bool.TryParse(Request.Params("Status"), out bool status);

            // Vincula dados
            escala.Descricao = Request.Params("Descricao") ?? string.Empty;
            escala.Observacao = Request.Params("Observacao") ?? string.Empty;
            escala.Classificacao = classificao;
            escala.CentroCarregamento = centroCarregamento;
            escala.CentroDescarregamento = centroDescarregamento;
            escala.Status = status;

            // Dados Criacao 
            if (escala.Codigo == 0)
            {
                escala.DataCriacao = DateTime.Now;
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Escalas.Escala escala, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (string.IsNullOrWhiteSpace(escala.Descricao))
            {
                msgErro = "Descrição é obrigatório.";
                return false;
            }

            if (escala.CentroCarregamento == null)
            {
                msgErro = "Nenhum Centro de Carregamento selecionado.";
                return false;
            }

            if (escala.CentroDescarregamento == null)
            {
                msgErro = "Nenhum Centro de Descarregamento selecionado.";
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
            if (propOrdenar == "CentroCarregamento") propOrdenar = "CentroCarregamento.Descricao";
            else if (propOrdenar == "CentroDescarregamento") propOrdenar = "CentroDescarregamento.Descricao";
        }
        #endregion
    }
}
