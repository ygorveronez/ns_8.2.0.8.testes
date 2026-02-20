using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/MotivoRejeicaoAjuste")]
    public class MotivoRejeicaoAjusteController : BaseController
    {
		#region Construtores

		public MotivoRejeicaoAjusteController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste repMotivoRejeicaoAjuste = new Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivo = repMotivoRejeicaoAjuste.BuscarPorCodigo(codigo);

                // Valida
                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    motivo.Codigo,
                    motivo.Ativo,
                    motivo.Descricao,
                    motivo.Observacao
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
                Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste repMotivoRejeicaoAjuste = new Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivo = new Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste();

                // Preenche entidade com dados
                PreencheEntidade(ref motivo, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoRejeicaoAjuste.Inserir(motivo, Auditado);
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
                Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste repMotivoRejeicaoAjuste = new Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivo = repMotivoRejeicaoAjuste.BuscarPorCodigo(codigo, true);

                // Valida
                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref motivo, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(motivo, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repMotivoRejeicaoAjuste.Atualizar(motivo, Auditado);
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
                Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste repMotivoRejeicaoAjuste = new Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivo = repMotivoRejeicaoAjuste.BuscarPorCodigo(codigo);

                // Valida
                if (motivo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repMotivoRejeicaoAjuste.Deletar(motivo, Auditado);
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
            grid.Prop("Descricao").Nome("Descrição").Tamanho(30).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoAtivo").Nome("Status").Tamanho(5).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste repMotivoRejeicaoAjuste = new Repositorio.Embarcador.Frete.MotivoRejeicaoAjuste(unitOfWork);

            // Dados do filtro
            //int.TryParse(Request.Params("Filtro"), out int codigoFiltro);

            // Consulta
            List<Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste> listaGrid = repMotivoRejeicaoAjuste.Consultar(propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMotivoRejeicaoAjuste.ContarConsulta();

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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivo, Repositorio.UnitOfWork unitOfWork)
        {
            motivo.Descricao = Request.GetStringParam("Descricao");
            motivo.Ativo = Request.GetBoolParam("Status");
            motivo.Observacao = Request.GetStringParam("Observacao");
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Frete.MotivoRejeicaoAjuste motivo, out string msgErro)
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
