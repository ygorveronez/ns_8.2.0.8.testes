using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/MotivoRejeicaoLancamentoNFS")]
    public class MotivoRejeicaoLancamentoNFSController : BaseController
    {
		#region Construtores

		public MotivoRejeicaoLancamentoNFSController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicaoLancamentoNFS = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS motivoRejeicao = repMotivoRejeicaoLancamentoNFS.BuscarPorCodigo(codigo);

                // Valida
                if (motivoRejeicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    motivoRejeicao.Codigo,
                    motivoRejeicao.Descricao,
                    motivoRejeicao.Observacao,
                    Status = motivoRejeicao.Ativo
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
                Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicaoLancamentoNFS = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS motivoRejeicao = new Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS();

                // Preenche entidade com dados
                PreencheEntidade(ref motivoRejeicao, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoRejeicao, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoRejeicaoLancamentoNFS.Inserir(motivoRejeicao, Auditado);
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
                Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicaoLancamentoNFS = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS motivoRejeicao = repMotivoRejeicaoLancamentoNFS.BuscarPorCodigo(codigo, true);

                // Valida
                if (motivoRejeicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref motivoRejeicao, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoRejeicao, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoRejeicaoLancamentoNFS.Atualizar(motivoRejeicao, Auditado);
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
                Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicaoLancamentoNFS = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS motivoRejeicao = repMotivoRejeicaoLancamentoNFS.BuscarPorCodigo(codigo);

                // Valida
                if (motivoRejeicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoRejeicaoLancamentoNFS.Deletar(motivoRejeicao, Auditado);
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
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 30, Models.Grid.Align.left, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS repMotivoRejeicaoLancamentoNFS = new Repositorio.Embarcador.NFS.MotivoRejeicaoLancamentoNFS(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao") ?? string.Empty;

            // Consulta
            List<Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS> listaGrid = repMotivoRejeicaoLancamentoNFS.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoRejeicaoLancamentoNFS.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            Codigo = obj.Codigo,
                            Descricao = obj.Descricao,
                            Situacao = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS motivoRejeicao, Repositorio.UnitOfWork unitOfWork)
        {
            // Converte valores
            bool.TryParse(Request.Params("Status"), out bool ativo);

            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            // Vincula dados
            motivoRejeicao.Ativo = ativo;
            motivoRejeicao.Descricao = descricao;
            motivoRejeicao.Observacao = observacao;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.NFS.MotivoRejeicaoLancamentoNFS motivoRejeicao, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(motivoRejeicao.Descricao))
            {
                msgErro = "Descriação é obrigatório.";
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
