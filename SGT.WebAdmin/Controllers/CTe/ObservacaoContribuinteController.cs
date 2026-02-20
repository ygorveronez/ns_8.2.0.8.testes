using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/ObservacaoContribuinte")]
    public class ObservacaoContribuinteController : BaseController
    {
		#region Construtores

		public ObservacaoContribuinteController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacaoContribuinte = repObservacaoContribuinte.BuscarPorCodigo(codigo);

                // Valida
                if (observacaoContribuinte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    observacaoContribuinte.Codigo,
                    observacaoContribuinte.Texto,
                    observacaoContribuinte.Ativo,
                    observacaoContribuinte.Identificador,
                    observacaoContribuinte.Tipo
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
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacaoContribuinte = new Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte();

                // Preenche entidade com dados
                PreencheEntidade(ref observacaoContribuinte, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(observacaoContribuinte, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repObservacaoContribuinte.Inserir(observacaoContribuinte, Auditado);
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
                Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacaoContribuinte = repObservacaoContribuinte.BuscarPorCodigo(codigo, true);

                // Valida
                if (observacaoContribuinte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref observacaoContribuinte, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(observacaoContribuinte, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repObservacaoContribuinte.Atualizar(observacaoContribuinte, Auditado);
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
                Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacaoContribuinte = repObservacaoContribuinte.BuscarPorCodigo(codigo);

                // Valida
                if (observacaoContribuinte == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repObservacaoContribuinte.Deletar(observacaoContribuinte, Auditado);
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
            grid.Prop("Texto").Nome("Texto").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("Ativo").Nome("Ativo").Tamanho(25).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CTe.ObservacaoContribuinte repObservacaoContribuinte = new Repositorio.Embarcador.CTe.ObservacaoContribuinte(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Ativo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Texto");

            // Consulta
            List<Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte> listaGrid = repObservacaoContribuinte.Consultar(descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repObservacaoContribuinte.ContarConsulta( descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Texto,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacaoContribuinte, Repositorio.UnitOfWork unitOfWork)
        {
            // Vincula dados
            observacaoContribuinte.Texto = Request.GetStringParam("Texto");
            observacaoContribuinte.Identificador = Request.GetStringParam("Identificador");
            observacaoContribuinte.Ativo = Request.GetBoolParam("Ativo");
            observacaoContribuinte.Tipo = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoCTe>("Tipo");
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.CTe.ObservacaoContribuinte observacaoContribuinte, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(observacaoContribuinte.Texto))
            {
                msgErro = "Texto é obrigatório.";
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
