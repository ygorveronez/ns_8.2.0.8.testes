using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Cancelamento
{
    [CustomAuthorize("Cargas/JustificativaCancelamentoCarga")]
    public class JustificativaCancelamentoCargaController : BaseController
    {
		#region Construtores

		public JustificativaCancelamentoCargaController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repGrupoCancelamentoCarga = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga grupoCancelamento = new Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga();

                PreencherEntidade(grupoCancelamento, unitOfWork);

                unitOfWork.Start();

                repGrupoCancelamentoCarga.Inserir(grupoCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaAoAdicionarDados);
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

                Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repGrupoCancelamentoCarga = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga grupoCancelamento = repGrupoCancelamentoCarga.BuscarPorCodigo(codigo, true);

                if (grupoCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                PreencherEntidade(grupoCancelamento, unitOfWork);

                unitOfWork.Start();

                repGrupoCancelamentoCarga.Atualizar(grupoCancelamento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.CancelamentoCarga.OcorreuUmaFalhaAoAtualizarDados);
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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repGrupoCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga grupoCancelamento = repGrupoCancelamento.BuscarPorCodigo(codigo, false);

                if (grupoCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    grupoCancelamento.Codigo,
                    grupoCancelamento.Descricao,
                    Situacao = grupoCancelamento.Ativo,
                    grupoCancelamento.Observacao,
                    grupoCancelamento.MotivoCancelamento,
                    grupoCancelamento.CodigoIntegracao
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
                long codigo = Request.GetLongParam("Codigo");

                Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repGrupoCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga grupoCancelamento = repGrupoCancelamento.BuscarPorCodigo(codigo, true);

                if (grupoCancelamento == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                repGrupoCancelamento.Deletar(grupoCancelamento, Auditado);

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
        private void PreencherEntidade(Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga grupoCancelamento, Repositorio.UnitOfWork unitOfWork)
        {

            bool ativo = Request.GetBoolParam("Situacao");
            string descricao = Request.Params("Descricao");
            string observacao = Request.Params("Observacao");
            string motivoCancelamento = Request.Params("MotivoCancelamento");

            grupoCancelamento.Ativo = ativo;
            grupoCancelamento.Descricao = descricao;
            grupoCancelamento.Observacao = observacao;
            grupoCancelamento.MotivoCancelamento = motivoCancelamento;
            grupoCancelamento.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
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
                grid.AdicionarCabecalho("MotivoCancelamento", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.CancelamentoCarga.CodigoIntegracao, "CodigoIntegracao", 50, Models.Grid.Align.left, true);

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 25, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga repGrupoCancelamento = new Repositorio.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.Cancelamento.JustificativaCancelamentoCarga> listaCancelamentoCarga = repGrupoCancelamento.Consultar(descricao, status, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repGrupoCancelamento.ContarConsulta(descricao, status);

                var retorno = (from motivo in listaCancelamentoCarga
                               select new
                               {
                                   motivo.Codigo,
                                   motivo.Descricao,
                                   motivo.DescricaoAtivo,
                                   motivo.MotivoCancelamento,
                                   motivo.CodigoIntegracao
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
