using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/TipoContratoFrete")]
    public class TipoContratoFreteController : BaseController
    {
		#region Construtores

		public TipoContratoFreteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);

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
                bool aditivos = Request.GetBoolParam("Aditivos");
                Enum.TryParse(Request.Params("Ativo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo);

                // Consulta
                List<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete> listaGrid = repTipoContratoFrete.Consultar(descricao, ativo, aditivos, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repTipoContratoFrete.ContarConsulta(descricao, ativo, aditivos);

                var lista = from obj in listaGrid
                            select new
                            {
                                obj.Codigo,
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
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContratoFrete = repTipoContratoFrete.BuscarPorCodigo(codigo);

                // Valida
                if (tipoContratoFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    tipoContratoFrete.Codigo,
                    tipoContratoFrete.Descricao,
                    tipoContratoFrete.Ativo,
                    tipoContratoFrete.Observacao,
                    tipoContratoFrete.TipoAditivo,
                    ContratoFreteAditivo = new { Codigo = tipoContratoFrete.ContratoFreteAditivo?.Codigo ?? 0, Descricao = tipoContratoFrete.ContratoFreteAditivo?.Descricao ?? "" }
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
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContratoFrete = new Dominio.Entidades.Embarcador.Frete.TipoContratoFrete();

                // Preenche entidade com dados
                PreencheEntidade(ref tipoContratoFrete, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(tipoContratoFrete, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repTipoContratoFrete.Inserir(tipoContratoFrete, Auditado);
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
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContratoFrete = repTipoContratoFrete.BuscarPorCodigo(codigo, true);

                // Valida
                if (tipoContratoFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref tipoContratoFrete, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(tipoContratoFrete, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repTipoContratoFrete.Atualizar(tipoContratoFrete, Auditado);
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
                Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContratoFrete = repTipoContratoFrete.BuscarPorCodigo(codigo);

                // Valida
                if (tipoContratoFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repTipoContratoFrete.Deletar(tipoContratoFrete, Auditado);
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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContratoFrete, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.TipoContratoFrete repTipoContratoFrete = new Repositorio.Embarcador.Frete.TipoContratoFrete(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfig = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            var config = repConfig.BuscarConfiguracaoPadrao();

            // Converte valores
            bool.TryParse(Request.Params("Ativo"), out bool ativo);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            // Vincula dados
            tipoContratoFrete.Descricao = descricao;
            tipoContratoFrete.Observacao = observacao;
            tipoContratoFrete.Ativo = ativo;

            if(config.UsarContratoFreteAditivo)
            {
                tipoContratoFrete.ContratoFreteAditivo = repTipoContratoFrete.BuscarPorCodigo(Request.GetIntParam("ContratoFreteAditivo"));
                tipoContratoFrete.TipoAditivo = Request.GetBoolParam("TipoAditivo");
            }
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frete.TipoContratoFrete tipoContratoFrete, out string msgErro)
        {
            /* ValidaEntidade
             * Recebe uma instancia da entidade
             * Valida informacoes
             * Retorna de entidade e valida ou nao e retorna erro (se tiver)
             */
            msgErro = "";

            if (tipoContratoFrete.Descricao.Length == 0)
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
