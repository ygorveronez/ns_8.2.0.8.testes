using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracoes.NFSeSaoPauloSP.Schemas;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize("Pessoas/TipoTerceiro")]
    public class TipoTerceiroController : BaseController
    {
		#region Construtores

		public TipoTerceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro = new Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro();

                PreencherTipoTerceiro(tipoTerceiro, unitOfWork);

                unitOfWork.Start();

                new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork).Inserir(tipoTerceiro, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
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
                Repositorio.Embarcador.Pessoas.TipoTerceiro repositorio = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (tipoTerceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherTipoTerceiro(tipoTerceiro, unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.Auditoria.HistoricoObjeto historicoObjeto = repositorio.Atualizar(tipoTerceiro, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
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
                Repositorio.Embarcador.Pessoas.TipoTerceiro repositorio = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro = repositorio.BuscarPorCodigo(codigo);

                if (tipoTerceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    tipoTerceiro.Codigo,
                    tipoTerceiro.Descricao,
                    tipoTerceiro.CodigoIntegracao,
                    tipoTerceiro.Situacao,
                    tipoTerceiro.ReterImpostosContratoFrete,
                    ConfiguracaoCIOT = tipoTerceiro.ConfiguracaoCIOT != null ? new { tipoTerceiro.ConfiguracaoCIOT.Codigo, Descricao = tipoTerceiro.ConfiguracaoCIOT.Descricao } : null,
                    ConfiguracaoContratoFrete = new
                    {
                        tipoTerceiro.AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado,
                        tipoTerceiro.EspecificarConfiguracaoContratoFreteTipoTerceiro,
                        ConfiguracaoPercentualAdiantamentoContratoFrete = tipoTerceiro.ConfiguracaoPercentualAdiantamentoContratoFrete ?? TipoTerceiroConfiguracaoContratoFrete.PorPessoa,
                        ConfiguracaoPercentualAbastecimentoContratoFrete = tipoTerceiro.ConfiguracaoPercentualAbastecimentoContratoFrete ?? TipoTerceiroConfiguracaoContratoFrete.PorPessoa,
                        ConfiguracaoDiasVencimentoAdiantamentoContratoFrete = tipoTerceiro.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete ?? TipoTerceiroConfiguracaoContratoFrete.PorPessoa,
                        ConfiguracaoDiasVencimentoSaldoContratoFrete = tipoTerceiro.ConfiguracaoDiasVencimentoSaldoContratoFrete ?? TipoTerceiroConfiguracaoContratoFrete.PorPessoa,
                        tipoTerceiro.PercentualAdiantamentoFretesTerceiro,
                        tipoTerceiro.PercentualAbastecimentoFretesTerceiro,
                        DiasVencimentoAdiantamentoContratoFrete = tipoTerceiro.DiasVencimentoAdiantamentoContratoFrete ?? 0,
                        DiasVencimentoSaldoContratoFrete = tipoTerceiro.DiasVencimentoSaldoContratoFrete ?? 0
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Pessoas.TipoTerceiro repositorio = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (tipoTerceiro == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                repositorio.Deletar(tipoTerceiro, Auditado);

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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherTipoTerceiro(Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro tipoTerceiro, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.CIOT.ConfiguracaoCIOT repConfiguracaoCIOT = new Repositorio.Embarcador.CIOT.ConfiguracaoCIOT(unitOfWork);
            int ConfiguracaoCIOT = Request.GetIntParam("ConfiguracaoCIOT");

            tipoTerceiro.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException(Localization.Resources.Consultas.CategoriaPessoa.DescricaoObrigatoria);
            tipoTerceiro.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");
            tipoTerceiro.Situacao = Request.GetBoolParam("Situacao");
            tipoTerceiro.ReterImpostosContratoFrete = Request.GetNullableBoolParam("ReterImpostosContratoFrete");
            tipoTerceiro.ConfiguracaoCIOT = ConfiguracaoCIOT > 0 ? repConfiguracaoCIOT.BuscarPorCodigo(ConfiguracaoCIOT)  : null;
            dynamic dynConfiguracaoContratoFrete = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ConfiguracaoContratoFrete"));

            tipoTerceiro.AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado = (bool)dynConfiguracaoContratoFrete.AdicionarPercentualAbastecimentoAdiantamentoCartaoNaoInformado;
            tipoTerceiro.EspecificarConfiguracaoContratoFreteTipoTerceiro = (bool)dynConfiguracaoContratoFrete.EspecificarConfiguracaoContratoFreteTipoTerceiro;

            TipoTerceiroConfiguracaoContratoFrete configuracaoPercentualAdiantamentoContratoFrete = TipoTerceiroConfiguracaoContratoFrete.PorPessoa;
            TipoTerceiroConfiguracaoContratoFrete configuracaoPercentualAbastecimentoContratoFrete = TipoTerceiroConfiguracaoContratoFrete.PorPessoa;
            TipoTerceiroConfiguracaoContratoFrete configuracaoDiasVencimentoAdiantamentoContratoFrete = TipoTerceiroConfiguracaoContratoFrete.PorPessoa;
            TipoTerceiroConfiguracaoContratoFrete configuracaoDiasVencimentoSaldoContratoFrete = TipoTerceiroConfiguracaoContratoFrete.PorPessoa;

            if (tipoTerceiro.EspecificarConfiguracaoContratoFreteTipoTerceiro)
            {
                if (dynConfiguracaoContratoFrete.ConfiguracaoPercentualAdiantamentoContratoFrete != null)
                    Enum.TryParse((string)dynConfiguracaoContratoFrete.ConfiguracaoPercentualAdiantamentoContratoFrete, out configuracaoPercentualAdiantamentoContratoFrete);
                tipoTerceiro.ConfiguracaoPercentualAdiantamentoContratoFrete = configuracaoPercentualAdiantamentoContratoFrete;

                if (dynConfiguracaoContratoFrete.ConfiguracaoPercentualAbastecimentoContratoFrete != null)
                    Enum.TryParse((string)dynConfiguracaoContratoFrete.ConfiguracaoPercentualAbastecimentoContratoFrete, out configuracaoPercentualAbastecimentoContratoFrete);
                tipoTerceiro.ConfiguracaoPercentualAbastecimentoContratoFrete = configuracaoPercentualAbastecimentoContratoFrete;

                if (dynConfiguracaoContratoFrete.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete != null)
                    Enum.TryParse((string)dynConfiguracaoContratoFrete.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete, out configuracaoDiasVencimentoAdiantamentoContratoFrete);
                tipoTerceiro.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete = configuracaoDiasVencimentoAdiantamentoContratoFrete;

                if (dynConfiguracaoContratoFrete.ConfiguracaoDiasVencimentoSaldoContratoFrete != null)
                    Enum.TryParse((string)dynConfiguracaoContratoFrete.ConfiguracaoDiasVencimentoSaldoContratoFrete, out configuracaoDiasVencimentoSaldoContratoFrete);
                tipoTerceiro.ConfiguracaoDiasVencimentoSaldoContratoFrete = configuracaoDiasVencimentoSaldoContratoFrete;

                if (!string.IsNullOrWhiteSpace((string)dynConfiguracaoContratoFrete.PercentualAdiantamentoFretesTerceiro))
                    tipoTerceiro.PercentualAdiantamentoFretesTerceiro = decimal.Parse(dynConfiguracaoContratoFrete.PercentualAdiantamentoFretesTerceiro.ToString());

                if (!string.IsNullOrWhiteSpace((string)dynConfiguracaoContratoFrete.PercentualAbastecimentoFretesTerceiro))
                    tipoTerceiro.PercentualAbastecimentoFretesTerceiro = decimal.Parse(dynConfiguracaoContratoFrete.PercentualAbastecimentoFretesTerceiro.ToString());

                tipoTerceiro.DiasVencimentoAdiantamentoContratoFrete = (int?)dynConfiguracaoContratoFrete.DiasVencimentoAdiantamentoContratoFrete ?? 0;
                tipoTerceiro.DiasVencimentoSaldoContratoFrete = (int?)dynConfiguracaoContratoFrete.DiasVencimentoSaldoContratoFrete ?? 0;
            }
            else
            {
                tipoTerceiro.ConfiguracaoPercentualAdiantamentoContratoFrete = configuracaoPercentualAdiantamentoContratoFrete;
                tipoTerceiro.ConfiguracaoPercentualAbastecimentoContratoFrete = configuracaoPercentualAbastecimentoContratoFrete;
                tipoTerceiro.ConfiguracaoDiasVencimentoAdiantamentoContratoFrete = configuracaoDiasVencimentoAdiantamentoContratoFrete;
                tipoTerceiro.ConfiguracaoDiasVencimentoSaldoContratoFrete = configuracaoDiasVencimentoSaldoContratoFrete;
                tipoTerceiro.PercentualAdiantamentoFretesTerceiro = 0;
                tipoTerceiro.PercentualAbastecimentoFretesTerceiro = 0;
                tipoTerceiro.DiasVencimentoAdiantamentoContratoFrete = 0;
                tipoTerceiro.DiasVencimentoSaldoContratoFrete = 0;
            }
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");

                SituacaoAtivoPesquisa? situacao = null;
                SituacaoAtivoPesquisa situacaoAux;
                if (Enum.TryParse(Request.Params("Status"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoSituacao", 50, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("ReterImpostosContratoFrete", false);
                grid.AdicionarCabecalho("CodigoConfiguracaoCIOT", false);
                grid.AdicionarCabecalho("DescricaoConfiguracaoCIOT", false);


                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Pessoas.TipoTerceiro repositorio = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(descricao, situacao);
                List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro> listaTipoTerceiro = totalRegistros > 0 ? repositorio.Consultar(descricao, situacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Pessoas.TipoTerceiro>();

                var listaTipoTerceiroRetornar = (
                    from tipoTerceiro in listaTipoTerceiro
                    select new
                    {
                        tipoTerceiro.Codigo,
                        tipoTerceiro.Descricao,
                        xx = "TESTE",
                        tipoTerceiro.DescricaoSituacao,
                        tipoTerceiro.ReterImpostosContratoFrete,
                        ConfiguracaoCIOT = tipoTerceiro.ConfiguracaoCIOT != null ? new { tipoTerceiro.ConfiguracaoCIOT.Codigo, Descricao = tipoTerceiro.ConfiguracaoCIOT.Descricao } : null,
                        CodigoConfiguracaoCIOT = tipoTerceiro.ConfiguracaoCIOT?.Codigo ?? null,
                        DescricaoConfiguracaoCIOT = tipoTerceiro.ConfiguracaoCIOT?.Descricao ?? null
                    }
                ).ToList();

                grid.AdicionaRows(listaTipoTerceiroRetornar);
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

        #endregion
    }
}
