using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/MotivoCancelamentoPagamento")]
    public class MotivoCancelamentoPagamentoController : BaseController
    {
		#region Construtores

		public MotivoCancelamentoPagamentoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento motivo = repMotivoCancelamentoPagamento.BuscarPorCodigo(codigo);

                // Valida
                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    motivo.Codigo,
                    motivo.Descricao,
                    Status = motivo.Ativo,
                    Observacao = motivo.Observacao ?? string.Empty
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
                Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento motivo = new Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento();

                // Preenche entidade com dados
                PreencheEntidade(ref motivo, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoCancelamentoPagamento.Inserir(motivo, Auditado);
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
                Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento motivo = repMotivoCancelamentoPagamento.BuscarPorCodigo(codigo, true);

                // Valida
                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref motivo, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoCancelamentoPagamento.Atualizar(motivo, Auditado);
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
                Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento motivo = repMotivoCancelamentoPagamento.BuscarPorCodigo(codigo);

                // Valida
                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoCancelamentoPagamento.Deletar(motivo, Auditado);
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
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento repMotivoCancelamentoPagamento = new Repositorio.Embarcador.Escrituracao.MotivoCancelamentoPagamento(unitOfWork);

            // Dados do filtro
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
            if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                Enum.TryParse(Request.Params("Status"), out status);
            else
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            string descricao = Request.Params("Descricao");

            // Consulta
            List<Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento> listaGrid = repMotivoCancelamentoPagamento.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoCancelamentoPagamento.ContarConsulta(descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento motivo, Repositorio.UnitOfWork unitOfWork)
        {
            // Converte valores
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            bool ativo = false;
            bool.TryParse(Request.Params("Status"), out ativo);

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            // Vincula dados
            motivo.Descricao = descricao;
            motivo.Ativo = ativo;
            motivo.Observacao = observacao;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Escrituracao.MotivoCancelamentoPagamento motivo, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(motivo.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (motivo.Descricao.Length > 200)
            {
                msgErro = "Descrição não pode passar de 200 caracteres.";
                return false;
            }

            if (motivo.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode passar de 2000 caracteres.";
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
            if (propOrdenar == "DescricaoAtivo") propOrdenar = "Ativo";
        }
        #endregion
    }
}
