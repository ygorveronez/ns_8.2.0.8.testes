using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Produtos
{
    [CustomAuthorize("Produtos/ProdutoNCMAbastecimento")]
    public class ProdutoNCMAbastecimentoController : BaseController
    {
		#region Construtores

		public ProdutoNCMAbastecimentoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento produtoNCMAbastecimento = repProdutoNCMAbastecimento.BuscarPorCodigo(codigo);

                // Valida
                if (produtoNCMAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    produtoNCMAbastecimento.Codigo,
                    produtoNCMAbastecimento.NCM,
                    Status = produtoNCMAbastecimento.Ativo,
                    produtoNCMAbastecimento.TipoAbastecimento
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
                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento produtoNCMAbastecimento = new Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento();

                // Preenche entidade com dados
                PreencheEntidade(ref produtoNCMAbastecimento, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(produtoNCMAbastecimento, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repProdutoNCMAbastecimento.Inserir(produtoNCMAbastecimento, Auditado);
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
                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento produtoNCMAbastecimento = repProdutoNCMAbastecimento.BuscarPorCodigo(codigo);

                // Valida
                if (produtoNCMAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref produtoNCMAbastecimento, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(produtoNCMAbastecimento, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repProdutoNCMAbastecimento.Atualizar(produtoNCMAbastecimento);
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
                Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento produtoNCMAbastecimento = repProdutoNCMAbastecimento.BuscarPorCodigo(codigo);

                // Valida
                if (produtoNCMAbastecimento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repProdutoNCMAbastecimento.Deletar(produtoNCMAbastecimento, Auditado);
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
            grid.Prop("Codigo");
            grid.Prop("NCM").Nome("NCM").Tamanho(50).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoTipoAbastecimento").Nome("Tipo").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Status").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento repProdutoNCMAbastecimento = new Repositorio.Embarcador.Produtos.ProdutoNCMAbastecimento(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);
            Enum.TryParse(Request.Params("TipoAbastecimento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento);

            string descricao = Request.Params("NCM");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento> listaGrid = repProdutoNCMAbastecimento.Consultar(codigoEmpresa, descricao, status, tipoAbastecimento, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repProdutoNCMAbastecimento.ContarConsulta(codigoEmpresa, descricao, status, tipoAbastecimento);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.NCM,
                            obj.DescricaoTipoAbastecimento,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento produtoNCMAbastecimento, Repositorio.UnitOfWork unitOfWork)
        {
            string ncm = Request.Params("NCM") ?? string.Empty;            
            Enum.TryParse(Request.Params("TipoAbastecimento"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento tipoAbastecimento);
            bool.TryParse(Request.Params("Status"), out bool ativo);

            // Vincula dados
            produtoNCMAbastecimento.NCM = ncm;            
            produtoNCMAbastecimento.Ativo = ativo;
            produtoNCMAbastecimento.TipoAbastecimento = tipoAbastecimento;
            produtoNCMAbastecimento.Empresa = this.Usuario.Empresa;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Produtos.ProdutoNCMAbastecimento produtoNCMAbastecimento, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(produtoNCMAbastecimento.NCM))
            {
                msgErro = "NCM é obrigatório.";
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
        }
        #endregion
    }
}
