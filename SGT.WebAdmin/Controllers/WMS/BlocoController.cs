using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize(new string[] { "BuscarBlocos" }, "WMS/Deposito", "WMS/Posicao", "WMS/Bloco", "WMS/Rua")]
    public class BlocoController : BaseController
    {
		#region Construtores

		public BlocoController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.WMS.DepositoBloco repDeposito = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco = new Dominio.Entidades.Embarcador.WMS.DepositoBloco();

                // Preenche entidade com dados
                PreencheEntidade(ref bloco, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(bloco, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Inserir(bloco, Auditado);
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
                Repositorio.Embarcador.WMS.DepositoBloco repDeposito = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco = repDeposito.BuscarPorCodigo(codigo, true);

                // Valida
                if (bloco == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref bloco, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(bloco, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Atualizar(bloco, Auditado);

                unitOfWork.CommitChanges();

                // Replica Cascata
                Servicos.Embarcador.WMS.Deposito.AtulizarAbreviacaoBloco(bloco, unitOfWork);

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
                Repositorio.Embarcador.WMS.DepositoBloco repDeposito = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco = repDeposito.BuscarPorCodigo(codigo);

                // Valida
                if (bloco == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Valida entidade
                if (!ValidaExclusao(bloco, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Deletar(bloco, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */
            Repositorio.Embarcador.WMS.DepositoRua repDepositoRua = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            int.TryParse(Request.Params("Rua"), out int rua);

            // Vincula dados
            bloco.Rua = repDepositoRua.BuscarPorCodigo(rua);
            bloco.Ativo = ativo;
            bloco.Descricao = descricao;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (bloco.Descricao.Length == 0)
            {
                msgErro = "Descrição é Obrigatória.";
                return false;
            }

            return true;
        }

        private bool ValidaExclusao(Dominio.Entidades.Embarcador.WMS.DepositoBloco bloco, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

            // Busca informacoes
            int posicoes = repDepositoPosicao.ContarPosicoesPorBloco(bloco.Codigo);

            if (posicoes > 0)
            {
                msgErro = "Não é possível excluir uma bloco quando já existe posições vinculadas.";
                return false;
            }

            return true;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.WMS.DepositoBloco repDepositoBloco = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);

            // Dados do filtro
            string descricao = Request.Params("DescricaoBusca") ?? string.Empty;

            int.TryParse(Request.Params("Deposito"), out int deposito);
            int.TryParse(Request.Params("Rua"), out int rua);

            // Consulta
            List<Dominio.Entidades.Embarcador.WMS.DepositoBloco> listaGrid = repDepositoBloco.Consultar(descricao, deposito, rua, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDepositoBloco.ContarConsulta(descricao, deposito, rua);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            Status = obj.Ativo,
                            obj.DescricaoAtivo,
                        };

            return lista.ToList();
        }

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
            grid.AdicionarCabecalho("Status", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Status", "DescricaoAtivo", 10, Models.Grid.Align.left, true);

            return grid;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "DescricaoAtivo")
                propOrdenar = "Ativo";
        }
        #endregion
    }
}
