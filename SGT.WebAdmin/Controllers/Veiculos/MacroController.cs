using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Veiculos
{
    [CustomAuthorize("Veiculos/Macro")]
    public class MacroController : BaseController
    {
		#region Construtores

		public MacroController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Veiculos.Macro macro = repMacro.BuscarPorCodigo(codigo);

                // Valida
                if (macro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    macro.Codigo,
                    macro.Descricao,
                    macro.CodigoIntegracao,
                    macro.Observacao,
                    Status = macro.Ativo,
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
                // Instancia repositorios
                Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Veiculos.Macro macro = new Dominio.Entidades.Embarcador.Veiculos.Macro();

                // Preenche entidade com dados
                PreencheEntidade(ref macro, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(macro, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repMacro.Inserir(macro, Auditado);
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
                // Instancia repositorios
                Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Veiculos.Macro macro = repMacro.BuscarPorCodigo(codigo, true);

                // Valida
                if (macro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref macro, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(macro, out string erro, unitOfWork))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                unitOfWork.Start();
                repMacro.Atualizar(macro, Auditado);
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
                // Instancia repositorios
                Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Veiculos.Macro macro = repMacro.BuscarPorCodigo(codigo);

                // Valida
                if (macro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                unitOfWork.Start();
                repMacro.Deletar(macro, Auditado);
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
            grid.Prop("Descricao").Nome("Descrição").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("DescricaoAtivo").Nome("Status").Tamanho(15).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);

            // Dados do filtro
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;

            if (!Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status))
                status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Veiculos.Macro> listaGrid = repMacro.Consultar(descricao, codigoIntegracao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repMacro.ContarConsulta(descricao, codigoIntegracao, status);

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
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Veiculos.Macro macro, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;
            bool.TryParse(Request.Params("Status"), out bool ativo);
            
            // Vincula dados
            macro.Descricao = descricao;
            macro.CodigoIntegracao = codigoIntegracao;
            macro.Ativo = ativo;
            macro.Observacao = observacao;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Veiculos.Macro macro, out string msgErro, Repositorio.UnitOfWork unitOfWork)
        {
            msgErro = "";
            Repositorio.Embarcador.Veiculos.Macro repMacro = new Repositorio.Embarcador.Veiculos.Macro(unitOfWork);

            if (string.IsNullOrEmpty(macro.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (string.IsNullOrEmpty(macro.CodigoIntegracao))
            {
                msgErro = "Código Integração é obrigatória.";
                return false;
            }

            if (!repMacro.ValidarCodigoIntegracao(macro.CodigoIntegracao, macro.Codigo))
            {
                msgErro = "Já existe uma macro com esse Código Integração.";
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
            if (propOrdenar == "Relacional") propOrdenar = "Relacional.Codigo";
        }
        #endregion
    }
}
