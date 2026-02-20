using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CRM
{
    [CustomAuthorize("CRM/OrigemContatoClienteProspect")]
    public class OrigemContatoClienteProspectController : BaseController
    {
		#region Construtores

		public OrigemContatoClienteProspectController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContatoClienteProspect = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect origemContatoClienteProspect = new Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect();

                // Preenche entidade com dados
                PreencheEntidade(ref origemContatoClienteProspect, unitOfWork);

                // Persiste dados
                repOrigemContatoClienteProspect.Inserir(origemContatoClienteProspect, Auditado);

                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(origemContatoClienteProspect.Codigo);
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
                Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContatoClienteProspect = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect origemContatoClienteProspect = repOrigemContatoClienteProspect.BuscarPorCodigo(codigo, true);

                // Valida
                if (origemContatoClienteProspect == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref origemContatoClienteProspect, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(origemContatoClienteProspect, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repOrigemContatoClienteProspect.Atualizar(origemContatoClienteProspect, Auditado);

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
                Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContatoClienteProspect = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect origemContatoClienteProspect = repOrigemContatoClienteProspect.BuscarPorCodigo(codigo, true);

                // Valida
                if (origemContatoClienteProspect == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Persiste dados
                repOrigemContatoClienteProspect.Deletar(origemContatoClienteProspect, Auditado);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContatoClienteProspect = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect produto = repOrigemContatoClienteProspect.BuscarPorCodigo(codigo);

                // Valida
                if (produto == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    produto.Codigo,
                    Status = produto.Ativo,
                    Descricao = produto.Nome
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
        #endregion

        #region Métodos Privados

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect origemContatoClienteProspect, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string observacao = Request.Params("Observacao") ?? string.Empty;

            bool.TryParse(Request.Params("Status"), out bool ativo);

            // Vincula dados
            origemContatoClienteProspect.Nome = descricao;            
            origemContatoClienteProspect.Ativo = ativo;
            origemContatoClienteProspect.Empresa = this.Usuario.Empresa;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect origemContatoClienteProspect, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(origemContatoClienteProspect.Nome))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            return true;
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
            grid.Prop("Codigo");
            grid.Prop("Descricao").Nome("Nome").Tamanho(70).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.CRM.OrigemContatoClienteProspect repOrigemContatoClienteProspect = new Repositorio.Embarcador.CRM.OrigemContatoClienteProspect(unitOfWork);

            // Dados do filtro
            //int.TryParse(Request.Params("Filtro"), out int codigoFiltro);
            string descricao = Request.Params("Descricao") ?? string.Empty;
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);
            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.CRM.OrigemContatoClienteProspect> listaGrid = repOrigemContatoClienteProspect.Consultar(status, descricao, codigoEmpresa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repOrigemContatoClienteProspect.ContarConsulta(status, descricao, codigoEmpresa);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Descricao = obj.Nome
                        };

            return lista.ToList();
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Descricao") propOrdenar = "Nome";
        }
        #endregion
    }
}
