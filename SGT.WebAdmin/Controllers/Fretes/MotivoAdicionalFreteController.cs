using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/MotivoAdicionalFrete")]
    public class MotivoAdicionalFreteController : BaseController
    {
		#region Construtores

		public MotivoAdicionalFreteController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                // Cabecalhos grid
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 50, Models.Grid.Align.left, true);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Dados do filtro
                string descricao = Request.Params("Descricao");
                if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status;
                Enum.TryParse(Request.Params("Status"), out status);

                // Consulta
                List<Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete> listaGrid = repMotivoAdicionalFrete.Consultar(descricao, status, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repMotivoAdicionalFrete.ContarConsulta(descricao, status);

                var lista = from obj in listaGrid
                            select new
                            {
                                Codigo = obj.Codigo,
                                Descricao = obj.Descricao,
                                Observacao = obj.Observacao,
                                DescricaoAtivo = obj.DescricaoAtivo
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
                Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete motivoAdicionalFrete = repMotivoAdicionalFrete.BuscarPorCodigo(codigo);

                // Valida
                if (motivoAdicionalFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    Codigo = motivoAdicionalFrete.Codigo,
                    Descricao = motivoAdicionalFrete.Descricao,
                    Observacao = motivoAdicionalFrete.Observacao,
                    Ativo = motivoAdicionalFrete.Ativo
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
                Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete motivoAdicionalFrete = new Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete();

                // Preenche entidade com dados
                PreencheEntidade(ref motivoAdicionalFrete, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoAdicionalFrete, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoAdicionalFrete.Inserir(motivoAdicionalFrete);
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
                Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete motivoAdicionalFrete = repMotivoAdicionalFrete.BuscarPorCodigo(codigo);

                // Valida
                if (motivoAdicionalFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref motivoAdicionalFrete, unitOfWork);

                // Valida entidade
                string erro;
                if (!ValidaEntidade(motivoAdicionalFrete, out erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoAdicionalFrete.Atualizar(motivoAdicionalFrete);
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
                Repositorio.Embarcador.Frete.MotivoAdicionalFrete repMotivoAdicionalFrete = new Repositorio.Embarcador.Frete.MotivoAdicionalFrete(unitOfWork);

                // Parametros
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete motivoAdicionalFrete = repMotivoAdicionalFrete.BuscarPorCodigo(codigo);

                // Valida
                if (motivoAdicionalFrete == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoAdicionalFrete.Deletar(motivoAdicionalFrete);
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

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete motivoAdicionalFrete, Repositorio.UnitOfWork unitOfWork)
        {
            // Converte valores
            string descricao = Request.Params("Descricao");
            if (string.IsNullOrWhiteSpace(descricao)) descricao = string.Empty;

            string observacao = Request.Params("Observacao");
            if (string.IsNullOrWhiteSpace(observacao)) observacao = string.Empty;

            bool ativo;
            bool.TryParse(Request.Params("Ativo"), out ativo);


            // Vincula dados
            motivoAdicionalFrete.Descricao = descricao;
            motivoAdicionalFrete.Observacao = observacao;
            motivoAdicionalFrete.Ativo = ativo;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frete.MotivoAdicionalFrete motivoAdicionalFrete, out string msgErro)
        {
            msgErro = "";

            if (motivoAdicionalFrete.Descricao.Length == 0)
            {
                msgErro = "Descrição é obrigatório.";
                return false;
            }

            if (motivoAdicionalFrete.Descricao.Length > 250)
            {
                msgErro = "Descrição não pode ser maior que 250 caracteres.";
                return false;
            }

            if (motivoAdicionalFrete.Observacao.Length > 2000)
            {
                msgErro = "Observação não pode ser maior que 2000 caracteres.";
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
