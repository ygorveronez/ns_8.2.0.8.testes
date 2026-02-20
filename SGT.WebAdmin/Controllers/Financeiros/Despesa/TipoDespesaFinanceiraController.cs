using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Despesa
{
    [CustomAuthorize("Financeiros/TipoDespesaFinanceira")]
    public class TipoDespesaFinanceiraController : BaseController
    {
		#region Construtores

		public TipoDespesaFinanceiraController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = new Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira();

                PreencherEntidade(tipoDespesa, unitOfWork);

                unitOfWork.Start();

                repTipoDespesa.Inserir(tipoDespesa, Auditado);

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

                Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoDespesa.BuscarPorCodigo(codigo, true);

                if (tipoDespesa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(tipoDespesa, unitOfWork);

                unitOfWork.Start();

                repTipoDespesa.Atualizar(tipoDespesa, Auditado);

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

                Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoDespesa.BuscarPorCodigo(codigo, false);

                if (tipoDespesa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoDespesa.Codigo,
                    tipoDespesa.Descricao,
                    Situacao = tipoDespesa.Ativo,
                    tipoDespesa.Observacao,
                    GrupoDespesa = new
                    {
                        Descricao = tipoDespesa?.GrupoDespesa?.Descricao ?? string.Empty,
                        Codigo = tipoDespesa?.GrupoDespesa?.Codigo ?? 0
                    }
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

                Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa = repTipoDespesa.BuscarPorCodigo(codigo, true);

                if (tipoDespesa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repTipoDespesa.Deletar(tipoDespesa, Auditado);

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira tipoDespesa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira repGrupoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.GrupoDespesaFinanceira(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            long codigoGrupoDespesa = Request.GetLongParam("GrupoDespesa");
            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            tipoDespesa.Ativo = ativo;
            tipoDespesa.Descricao = descricao;
            tipoDespesa.Observacao = observacao;
            tipoDespesa.GrupoDespesa = codigoGrupoDespesa > 0 ? repGrupoDespesa.BuscarPorCodigo(codigoGrupoDespesa, false) : null;
            tipoDespesa.Empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                long codigoGrupoDespesa = Request.GetLongParam("GrupoDespesa");
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
                grid.AdicionarCabecalho("Grupo de Despesas", "GrupoDespesa", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira repTipoDespesa = new Repositorio.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira(unitOfWork);
                int totalRegistros = repTipoDespesa.ContarConsulta(descricao, codigoGrupoDespesa, status, codigoEmpresa);
                List<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira> listaTipoDespesa = totalRegistros > 0 ? repTipoDespesa.Consultar(descricao, codigoGrupoDespesa, status, codigoEmpresa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Financeiro.Despesa.TipoDespesaFinanceira>();

                var retorno = (from obj in listaTipoDespesa
                               select new
                               {
                                   obj.Codigo,
                                   obj.Descricao,
                                   GrupoDespesa = obj.GrupoDespesa?.Descricao,
                                   obj.DescricaoAtivo
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
