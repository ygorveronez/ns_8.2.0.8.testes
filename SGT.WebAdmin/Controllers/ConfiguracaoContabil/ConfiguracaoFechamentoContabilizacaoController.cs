using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.ConfiguracaoContabil
{
    [CustomAuthorize("ConfiguracaoContabil/ConfiguracaoFechamentoContabilizacao")]
    public class ConfiguracaoFechamentoContabilizacaoController : BaseController
    {
		#region Construtores

		public ConfiguracaoFechamentoContabilizacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao repConfiguracaoFechamentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao(unitOfWork);

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao configuracaoFechamentoContabil = new Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao();

                PreencherEntidade(configuracaoFechamentoContabil, unitOfWork);

                if (repConfiguracaoFechamentoContabil.ExistePorMesEAnoReferencia(0, configuracaoFechamentoContabil.MesReferencia, configuracaoFechamentoContabil.AnoReferencia))
                    return new JsonpResult(false, true, "Já existe um registro com o mesmo mês/ano de referência.");

                unitOfWork.Start();

                repConfiguracaoFechamentoContabil.Inserir(configuracaoFechamentoContabil, Auditado);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao repConfiguracaoFechamentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao(unitOfWork);

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao configuracaoFechamentoContabil = repConfiguracaoFechamentoContabil.BuscarPorCodigo(codigo, true);

                if (configuracaoFechamentoContabil == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherEntidade(configuracaoFechamentoContabil, unitOfWork);

                if (repConfiguracaoFechamentoContabil.ExistePorMesEAnoReferencia(configuracaoFechamentoContabil.Codigo, configuracaoFechamentoContabil.MesReferencia, configuracaoFechamentoContabil.AnoReferencia))
                    return new JsonpResult(false, true, "Já existe um registro com o mesmo mês/ano de referência.");

                unitOfWork.Start();

                repConfiguracaoFechamentoContabil.Atualizar(configuracaoFechamentoContabil, Auditado);

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao repConfiguracaoFechamentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao(unitOfWork);

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao configuracaoFechamentoContabil = repConfiguracaoFechamentoContabil.BuscarPorCodigo(codigo, false);

                if (configuracaoFechamentoContabil == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    configuracaoFechamentoContabil.Codigo,
                    configuracaoFechamentoContabil.AnoReferencia,
                    configuracaoFechamentoContabil.MesReferencia,
                    UltimoDiaEnvio = configuracaoFechamentoContabil.UltimoDiaEnvio.ToString("dd/MM/yyyy")
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao repConfiguracaoFechamentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao(unitOfWork);

                Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao configuracaoFechamentoContabil = repConfiguracaoFechamentoContabil.BuscarPorCodigo(codigo, true);

                if (configuracaoFechamentoContabil == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repConfiguracaoFechamentoContabil.Deletar(configuracaoFechamentoContabil, Auditado);

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
                Models.Grid.Grid grid = ObterGridPesquisa();

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

        private void PreencherEntidade(Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao configuracaoFechamentoContabilizacao, Repositorio.UnitOfWork unitOfWork)
        {
            configuracaoFechamentoContabilizacao.MesReferencia = Request.GetIntParam("MesReferencia");
            configuracaoFechamentoContabilizacao.AnoReferencia = Request.GetIntParam("AnoReferencia");
            configuracaoFechamentoContabilizacao.UltimoDiaEnvio = Request.GetDateTimeParam("UltimoDiaEnvio");
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int anoReferencia = Request.GetIntParam("AnoReferencia");
                int mesReferencia = Request.GetIntParam("MesReferencia");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Mês", "MesReferencia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Ano", "AnoReferencia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Último Dia Envio", "UltimoDiaEnvio", 70, Models.Grid.Align.left, true);

                string propriedadeOrdenar = ObterPropriedadeOrdenar(grid.header[grid.indiceColunaOrdena].data);

                Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao repConfiguracaoFechamentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.ConfiguracaoContabil.ConfiguracaoFechamentoContabilizacao> listaConfiguracaoFechamentoContabilizacao = repConfiguracaoFechamentoContabil.Consultar(mesReferencia, anoReferencia, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalRegistros = repConfiguracaoFechamentoContabil.ContarConsulta(mesReferencia, anoReferencia);

                var retorno = (from configuracao in listaConfiguracaoFechamentoContabilizacao
                               select new
                               {
                                   configuracao.Codigo,
                                   configuracao.MesReferencia,
                                   configuracao.AnoReferencia,
                                   UltimoDiaEnvio = configuracao.UltimoDiaEnvio.ToString("dd/MM/yyyy")
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
            return propriedadeOrdenar;
        }

        #endregion
    }
}
