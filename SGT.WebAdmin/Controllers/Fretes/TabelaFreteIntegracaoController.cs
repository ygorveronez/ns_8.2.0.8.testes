using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Fretes/TabelaFrete")]
    public class TabelaFreteIntegracaoController : BaseController
    {
		#region Construtores

		public TabelaFreteIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigo);

                if (tabelaFrete == null)
                    throw new ControllerException(Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                Servicos.Embarcador.Frete.TabelaFreteIntegracao servicoTabelaFreteIntegracao = new Servicos.Embarcador.Frete.TabelaFreteIntegracao(unitOfWork);

                servicoTabelaFreteIntegracao.AdicionarIntegracoes(tabelaFrete);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao tabelaFreteIntegracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                var arquivosIntegracaoRetornar = (
                    from arquivoIntegracao in tabelaFreteIntegracao.ArquivosTransacao.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                    select new
                    {
                        arquivoIntegracao.Codigo,
                        Data = arquivoIntegracao.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                        arquivoIntegracao.DescricaoTipo,
                        arquivoIntegracao.Mensagem
                    }
                ).ToList();

                grid.AdicionaRows(arquivosIntegracaoRetornar);
                grid.setarQuantidadeTotal(tabelaFreteIntegracao.ArquivosTransacao.Count());

                return new JsonpResult(grid);
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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repositorioArquivoIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repositorioArquivoIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                if ((arquivoIntegracao == null) || ((arquivoIntegracao.ArquivoRequisicao == null) && (arquivoIntegracao.ArquivoResposta == null)))
                    return new JsonpResult(true, false, Localization.Resources.Gerais.Geral.NaoHaArquivosDisponiveisParaDownload);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", $"Arquivos de Integração da Tabela de Frete.zip");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTabelaFreteIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 13, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tentativas", "NumeroTentativas", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);

                int codigoTabelaFrete = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(unitOfWork);
                int totalIntegracoes = repositorioIntegracao.ContarConsultaPorTabelaFrete(codigoTabelaFrete, situacao);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao> listaIntegracoes = (totalIntegracoes > 0) ? repositorioIntegracao.ConsultarPorTabelaFrete(codigoTabelaFrete, situacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao>();

                var listaIntegracoesRetornar = (
                    from integracao in listaIntegracoes
                    select new
                    {
                        integracao.Codigo,
                        Situacao = integracao.SituacaoIntegracao,
                        SituacaoIntegracao = integracao.DescricaoSituacaoIntegracao,
                        TipoIntegracao = integracao.TipoIntegracao.DescricaoTipo,
                        Retorno = integracao.ProblemaIntegracao,
                        integracao.NumeroTentativas,
                        DataIntegracao = integracao.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte(),
                    }
                ).ToList();

                grid.AdicionaRows(listaIntegracoesRetornar);
                grid.setarQuantidadeTotal(totalIntegracoes);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = Request.GetIntParam("Codigo");
                int totalAguardandoIntegracao = 0;
                int totalIntegrado = 0;
                int totalProblemaIntegracao = 0;
                int totalAguardandoRetorno = 0;

                if (codigoTabelaFrete > 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(unitOfWork);

                    totalAguardandoIntegracao = repositorioIntegracao.ContarConsultaPorTabelaFrete(codigoTabelaFrete, SituacaoIntegracao.AgIntegracao);
                    totalIntegrado = repositorioIntegracao.ContarConsultaPorTabelaFrete(codigoTabelaFrete, SituacaoIntegracao.Integrado);
                    totalProblemaIntegracao = repositorioIntegracao.ContarConsultaPorTabelaFrete(codigoTabelaFrete, SituacaoIntegracao.ProblemaIntegracao);
                    totalAguardandoRetorno = repositorioIntegracao.ContarConsultaPorTabelaFrete(codigoTabelaFrete, SituacaoIntegracao.AgRetorno);
                }

                var retorno = new
                {
                    TotalAguardandoIntegracao = totalAguardandoIntegracao,
                    TotalAguardandoRetorno = totalAguardandoRetorno,
                    TotalIntegrado = totalIntegrado,
                    TotalProblemaIntegracao = totalProblemaIntegracao,
                    TotalGeral = totalAguardandoIntegracao + totalIntegrado + totalProblemaIntegracao + totalAguardandoRetorno
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoObterOsTotaisDasIntegracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao repositorioIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteIntegrarAlteracaoIntegracao tabelaFreteIntegracao = repositorioIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                if (tabelaFreteIntegracao == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                tabelaFreteIntegracao.DataIntegracao = DateTime.Now;
                tabelaFreteIntegracao.NumeroTentativas = 0;
                tabelaFreteIntegracao.ProblemaIntegracao = "";
                tabelaFreteIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                repositorioIntegracao.Atualizar(tabelaFreteIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}

