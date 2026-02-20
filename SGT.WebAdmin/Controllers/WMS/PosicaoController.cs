using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.WMS
{
    [CustomAuthorize("WMS/Deposito", "WMS/Posicao", "WMS/Bloco", "WMS/Rua")]
    public class PosicaoController : BaseController
    {
		#region Construtores

		public PosicaoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.WMS.DepositoPosicao repDeposito = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao = repDeposito.BuscarPorCodigo(codigo);

                // Valida
                if (posicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    posicao.Codigo,
                    posicao.Descricao,
                    posicao.Ativo,
                    QuantidadePaletsMaximo = posicao.QuantidadePaletes.ToString("n2"),
                    MetroCubicoMaximo = posicao.MetroCubicoMaximo.ToString("n2"),
                    PesoMaximo = posicao.PesoMaximo.ToString("n2"),
                    ProdutoEmbarcador = posicao.Produto != null ? new { posicao.Produto.Codigo, posicao.Produto.Descricao } : null,
                    posicao.Abreviacao
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
                Repositorio.Embarcador.WMS.DepositoPosicao repDeposito = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao = new Dominio.Entidades.Embarcador.WMS.DepositoPosicao();

                // Preenche entidade com dados
                PreencheEntidade(ref posicao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(posicao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Inserir(posicao, Auditado);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.WMS.Deposito.AtulizarAbreviacaoPosicao(posicao, unitOfWork);

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
                Repositorio.Embarcador.WMS.DepositoPosicao repDeposito = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao = repDeposito.BuscarPorCodigo(codigo, true);

                // Valida
                if (posicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref posicao, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(posicao, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repDeposito.Atualizar(posicao, Auditado);

                unitOfWork.CommitChanges();

                Servicos.Embarcador.WMS.Deposito.AtulizarAbreviacaoPosicao(posicao, unitOfWork);

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
                Repositorio.Embarcador.WMS.DepositoPosicao repDeposito = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao = repDeposito.BuscarPorCodigo(codigo);

                // Valida
                if (posicao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repDeposito.Deletar(posicao, Auditado);
                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                //return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");

                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, false, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }

            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao, Repositorio.UnitOfWork unitOfWork)
        {
            /* PreencheEntidade
             * Recebe uma instancia da entidade
             * Converte parametros recebido por request
             * Atribui a entidade
             */
            Repositorio.Embarcador.WMS.DepositoBloco repDepositoBloco = new Repositorio.Embarcador.WMS.DepositoBloco(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            int bloco = Request.GetIntParam("Bloco");
            int produtoEmbarcador = Request.GetIntParam("ProdutoEmbarcador");
            string descricao = Request.GetStringParam("Descricao");
            bool ativo = Request.GetBoolParam("Ativo");
            decimal quantidadePaletsMaximo = Request.GetDecimalParam("QuantidadePaletsMaximo");
            decimal metroCubicoMaximo = Request.GetDecimalParam("MetroCubicoMaximo");
            decimal pesoMaximo = Request.GetDecimalParam("PesoMaximo");

            // Vincula dados
            posicao.Bloco = repDepositoBloco.BuscarPorCodigo(bloco);
            posicao.Produto = repProdutoEmbarcador.BuscarPorCodigo(produtoEmbarcador);
            posicao.Ativo = ativo;
            posicao.QuantidadePaletes = quantidadePaletsMaximo;
            posicao.MetroCubicoMaximo = metroCubicoMaximo;
            posicao.PesoMaximo = pesoMaximo;
            posicao.Descricao = descricao;
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.WMS.DepositoPosicao posicao, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (posicao.Descricao.Length == 0)
            {
                msgErro = "Descrição é Obrigatória.";
                return false;
            }

            if (posicao.QuantidadePaletesAtual > posicao.QuantidadePaletes)
            {
                msgErro = "Quantidade de Palets excedeu a capacidade máxima.";
                return false;
            }

            if (posicao.PesoAtual > posicao.PesoMaximo)
            {
                msgErro = "Peso excedeu a capacidade máxima.";
                return false;
            }

            if (posicao.MetroCubicoAtual > posicao.MetroCubicoMaximo)
            {
                msgErro = "Metro Cúbico excedeu a capacidade máxima.";
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
            Repositorio.Embarcador.WMS.DepositoPosicao repDepositoPosicao = new Repositorio.Embarcador.WMS.DepositoPosicao(unitOfWork);

            // Dados do filtrodescricao
            string descricao = Request.Params("DescricaoBusca") ?? string.Empty;
            string apreviacao = Request.Params("Abreviacao") ?? string.Empty;

            int.TryParse(Request.Params("Deposito"), out int deposito);
            int.TryParse(Request.Params("Rua"), out int rua);
            int.TryParse(Request.Params("Bloco"), out int bloco);

            // Consulta
            List<Dominio.Entidades.Embarcador.WMS.DepositoPosicao> listaGrid = repDepositoPosicao.ConsultarPosicoes(apreviacao, descricao, deposito, rua, bloco, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repDepositoPosicao.ContarConsultaPosicoes(apreviacao, descricao, deposito, rua, bloco);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Abreviacao,
                            Descricao = obj.Abreviacao,
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
            grid.AdicionarCabecalho("Abreviação", "Abreviacao", 30, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 12, Models.Grid.Align.left, true);
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
