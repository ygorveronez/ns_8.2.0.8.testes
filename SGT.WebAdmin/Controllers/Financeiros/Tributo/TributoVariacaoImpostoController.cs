using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Tributo
{
    [CustomAuthorize("Financeiros/TributoVariacaoImposto")]
    public class TributoVariacaoImpostoController : BaseController
    {
		#region Construtores

		public TributoVariacaoImpostoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto tributoVariacaoImposto = repTributoVariacaoImposto.BuscarPorCodigo(codigo);

                // Valida
                if (tributoVariacaoImposto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    tributoVariacaoImposto.Codigo,
                    tributoVariacaoImposto.Descricao,
                    Status = tributoVariacaoImposto.Situacao,
                    tributoVariacaoImposto.CodigoIntegracao
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
                Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto tributoVariacaoImposto = new Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto();

                // Preenche entidade com dados
                PreencheEntidade(ref tributoVariacaoImposto, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(tributoVariacaoImposto, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repTributoVariacaoImposto.Inserir(tributoVariacaoImposto, Auditado);
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
                Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto tributoVariacaoImposto = repTributoVariacaoImposto.BuscarPorCodigo(codigo);

                // Valida
                if (tributoVariacaoImposto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref tributoVariacaoImposto, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(tributoVariacaoImposto, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repTributoVariacaoImposto.Atualizar(tributoVariacaoImposto);
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
                Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto tributoVariacaoImposto = repTributoVariacaoImposto.BuscarPorCodigo(codigo);

                // Valida
                if (tributoVariacaoImposto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repTributoVariacaoImposto.Deletar(tributoVariacaoImposto, Auditado);
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
            grid.Prop("Descricao").Nome("Descrição").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("CodigoIntegracao").Nome("Cod. Integração").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Status").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto repTributoVariacaoImposto = new Repositorio.Embarcador.Financeiro.Tributo.TributoVariacaoImposto(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto> listaGrid = repTributoVariacaoImposto.Consultar(codigoEmpresa, codigoIntegracao, descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repTributoVariacaoImposto.ContarConsulta(codigoEmpresa, codigoIntegracao, descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.CodigoIntegracao,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto tributoVariacaoImposto, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;

            bool.TryParse(Request.Params("Status"), out bool ativo);

            // Vincula dados
            tributoVariacaoImposto.Descricao = descricao;
            tributoVariacaoImposto.CodigoIntegracao = codigoIntegracao;
            tributoVariacaoImposto.Situacao = ativo;
            tributoVariacaoImposto.Empresa = this.Usuario.Empresa;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Financeiro.Tributo.TributoVariacaoImposto tributoVariacaoImposto, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(tributoVariacaoImposto.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(tributoVariacaoImposto.CodigoIntegracao))
            {
                msgErro = "Código de Integração é obrigatório.";
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
