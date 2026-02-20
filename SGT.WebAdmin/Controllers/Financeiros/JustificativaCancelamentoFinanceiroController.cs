using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Financeiros/JustificativaCancelamentoFinanceiro")]
    public class JustificativaCancelamentoFinanceiroController : BaseController
    {
		#region Construtores

		public JustificativaCancelamentoFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamento = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro JustificativaCancelamento = new Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro();

                PreencherEntidade(JustificativaCancelamento, unitOfWork);

                unitOfWork.Start();

                repJustificativaCancelamento.Inserir(JustificativaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamento = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro JustificativaCancelamento = repJustificativaCancelamento.BuscarPorCodigo(codigo, true);

                if (JustificativaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherEntidade(JustificativaCancelamento, unitOfWork);

                unitOfWork.Start();

                repJustificativaCancelamento.Atualizar(JustificativaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamento = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro JustificativaCancelamento = repJustificativaCancelamento.BuscarPorCodigo(codigo, false);

                if (JustificativaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    JustificativaCancelamento.Codigo,
                    JustificativaCancelamento.Descricao,
                    Situacao = JustificativaCancelamento.Ativo,
                    JustificativaCancelamento.Observacao,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamento = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro JustificativaCancelamento = repJustificativaCancelamento.BuscarPorCodigo(codigo, true);

                if (JustificativaCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                repJustificativaCancelamento.Deletar(JustificativaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRemover);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private void PreencherEntidade(Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro justificativaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {

            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");

            justificativaCancelamento.Ativo = ativo;
            justificativaCancelamento.Descricao = descricao;
            justificativaCancelamento.Observacao = observacao;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Situacao", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro repJustificativaCancelamento = new Repositorio.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.JustificativaCancelamentoFinanceiro> listaJustificativaCancelamento = repJustificativaCancelamento.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repJustificativaCancelamento.ContarConsulta(descricao, status);

                var retorno = (from motivo in listaJustificativaCancelamento
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoAtivo,
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
