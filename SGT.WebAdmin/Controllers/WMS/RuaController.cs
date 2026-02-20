using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/Deposito", "WMS/Posicao", "WMS/Bloco", "WMS/Rua")]
    public class RuaController : BaseController
    {
		#region Construtores

		public RuaController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.WMS.DepositoRua repDeposito = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoRua rua = new Dominio.Entidades.Embarcador.WMS.DepositoRua();

                // Preenche entidade com dados
                PreencheEntidade(ref rua, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(rua, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Inserir(rua, Auditado);
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
                Repositorio.Embarcador.WMS.DepositoRua repDeposito = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoRua rua = repDeposito.BuscarPorCodigo(codigo, true);

                // Valida
                if (rua == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref rua, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(rua, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Atualizar(rua, Auditado);

                unitOfWork.CommitChanges();

                // Replica Cascata
                Servicos.Embarcador.WMS.Deposito.AtulizarAbreviacaoRua(rua, unitOfWork);

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
                Repositorio.Embarcador.WMS.DepositoRua repDeposito = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoRua rua = repDeposito.BuscarPorCodigo(codigo);

                // Valida
                if (rua == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Valida entidade
                if (!ValidaExclusao(rua, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Deletar(rua, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.WMS.DepositoRua rua, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */
            Repositorio.Embarcador.WMS.Deposito repDeposito = new Repositorio.Embarcador.WMS.Deposito(unitOfWork);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            int.TryParse(Request.Params("Deposito"), out int deposito);

            // Vincula dados
            rua.Deposito = repDeposito.BuscarPorCodigo(deposito);
            rua.Ativo = ativo;
            rua.Descricao = descricao;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.DepositoRua rua, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (rua.Descricao.Length == 0)
            {
                msgErro = "Descrição é Obrigatória.";
                return false;
            }

            return true;
        }

        private bool ValidaExclusao(Dominio.Entidades.Embarcador.WMS.DepositoRua rua, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            Repositorio.Embarcador.WMS.DepositoBloco repDepositoRua = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);

            // Busca informacoes
            int ruas = repDepositoRua.ContarBlocosPorRua(rua.Codigo);

            if (ruas > 0)
            {
                msgErro = "Não é possível excluir uma rua quando já existe blocos vinculados.";
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
            Repositorio.Embarcador.WMS.DepositoRua repDepositoRua = new Repositorio.Embarcador.WMS.DepositoRua(unitOfWork);

            // Dados do filtro
            string descricao = Request.Params("DescricaoBusca") ?? string.Empty;

            int.TryParse(Request.Params("Deposito"), out int deposito);

            // Consulta
            List<Dominio.Entidades.Embarcador.WMS.DepositoRua> listaGrid = repDepositoRua.Consultar(descricao, deposito, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDepositoRua.ContarConsulta(descricao, deposito);

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
