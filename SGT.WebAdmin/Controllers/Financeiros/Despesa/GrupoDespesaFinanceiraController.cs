using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Despesa
{
    [CustomAuthorize("Financeiros/GrupoDespesaFinanceira")]
    public class GrupoDespesaFinanceiraController : BaseController
    {
		#region Construtores

		public GrupoDespesaFinanceiraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira grupoDespesa = new Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira();

                PreencherEntidade(grupoDespesa, unitOfWork);

                unitOfWork.Start();

                repGrupoDespesa.Inserir(grupoDespesa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira grupoDespesa = repGrupoDespesa.BuscarPorCodigo(codigo, true);

                if (grupoDespesa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(grupoDespesa, unitOfWork);

                unitOfWork.Start();

                repGrupoDespesa.Atualizar(grupoDespesa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira grupoDespesa = repGrupoDespesa.BuscarPorCodigo(codigo, false);

                if (grupoDespesa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    grupoDespesa.Codigo,
                    grupoDespesa.Descricao,
                    Situacao = grupoDespesa.Ativo,
                    grupoDespesa.Observacao
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira grupoDespesa = repGrupoDespesa.BuscarPorCodigo(codigo, true);

                if (grupoDespesa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repGrupoDespesa.Deletar(grupoDespesa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira grupoDespesa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            grupoDespesa.Ativo = ativo;
            grupoDespesa.Descricao = descricao;
            grupoDespesa.Observacao = observacao;
            grupoDespesa.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira> listaGrupoDespesa = repGrupoDespesa.Consultar(descricao, status, codigoEmpresa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repGrupoDespesa.ContarConsulta(descricao, status, codigoEmpresa);

                var retorno = (from motivo in listaGrupoDespesa
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoAtivo
                               }).ToList();

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
