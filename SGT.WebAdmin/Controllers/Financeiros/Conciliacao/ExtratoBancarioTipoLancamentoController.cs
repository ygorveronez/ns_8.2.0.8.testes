using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Conciliacao
{
    [CustomAuthorize("Financeiros/ExtratoBancarioTipoLancamento")]
    public class ExtratoBancarioTipoLancamentoController : BaseController
    {
		#region Construtores

		public ExtratoBancarioTipoLancamentoController(Conexao conexao) : base(conexao) { }

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
                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento extratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigo(codigo);

                // Valida
                if (extratoBancarioTipoLancamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    extratoBancarioTipoLancamento.Codigo,
                    extratoBancarioTipoLancamento.Descricao,
                    Status = extratoBancarioTipoLancamento.Situacao,
                    extratoBancarioTipoLancamento.CodigoIntegracao,
                    extratoBancarioTipoLancamento.NaoImportarRegistroAoEstrato
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

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento extratoBancarioTipoLancamento = new Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento();

                PreencheEntidade(ref extratoBancarioTipoLancamento, unitOfWork);

                if (!ValidaEntidade(extratoBancarioTipoLancamento, out string erro))
                    return new JsonpResult(false, true, erro);

                repExtratoBancarioTipoLancamento.Inserir(extratoBancarioTipoLancamento, Auditado);

                unitOfWork.CommitChanges();

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
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento extratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigo(codigo, true);

                if (extratoBancarioTipoLancamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencheEntidade(ref extratoBancarioTipoLancamento, unitOfWork);

                if (!ValidaEntidade(extratoBancarioTipoLancamento, out string erro))
                    return new JsonpResult(false, true, erro);

                repExtratoBancarioTipoLancamento.Atualizar(extratoBancarioTipoLancamento, Auditado);

                unitOfWork.CommitChanges();

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
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento extratoBancarioTipoLancamento = repExtratoBancarioTipoLancamento.BuscarPorCodigo(codigo);

                if (extratoBancarioTipoLancamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                repExtratoBancarioTipoLancamento.Deletar(extratoBancarioTipoLancamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
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

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento repExtratoBancarioTipoLancamento = new Repositorio.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("Status"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status);

            string descricao = Request.Params("Descricao");
            string codigoIntegracao = Request.Params("CodigoIntegracao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento> listaGrid = repExtratoBancarioTipoLancamento.Consultar(codigoEmpresa, codigoIntegracao, descricao, status, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repExtratoBancarioTipoLancamento.ContarConsulta(codigoEmpresa, codigoIntegracao, descricao, status);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            Descricao = obj.DescricaoCompleta,
                            obj.CodigoIntegracao,
                            Ativo = obj.DescricaoAtivo
                        };

            return lista.ToList();
        }

        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento extratoBancarioTipoLancamento, Repositorio.UnitOfWork unitOfWork)
        {
            string descricao = Request.Params("Descricao") ?? string.Empty;
            string codigoIntegracao = Request.Params("CodigoIntegracao") ?? string.Empty;

            bool.TryParse(Request.Params("Status"), out bool ativo);

            extratoBancarioTipoLancamento.Descricao = descricao;
            extratoBancarioTipoLancamento.CodigoIntegracao = codigoIntegracao;
            extratoBancarioTipoLancamento.Situacao = ativo;
            if (extratoBancarioTipoLancamento.Codigo == 0)
                extratoBancarioTipoLancamento.Empresa = this.Usuario.Empresa;
            extratoBancarioTipoLancamento.NaoImportarRegistroAoEstrato = Request.GetBoolParam("NaoImportarRegistroAoEstrato");
        }

        private bool ValidaEntidade(Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancarioTipoLancamento extratoBancarioTipoLancamento, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(extratoBancarioTipoLancamento.Descricao))
            {
                msgErro = "Descrição é obrigatória.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(extratoBancarioTipoLancamento.CodigoIntegracao))
            {
                msgErro = "Código de Integração é obrigatório.";
                return false;
            }

            return true;
        }

        private void PropOrdena(ref string propOrdenar)
        {
        }

        #endregion
    }
}
