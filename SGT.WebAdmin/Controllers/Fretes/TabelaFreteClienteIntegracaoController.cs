using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Fretes/TabelaFreteCliente")]
    public class TabelaFreteClienteIntegracaoController : BaseController
    {
		#region Construtores

		public TabelaFreteClienteIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", "Codigo", 20, Models.Grid.Align.left, true, true);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false, true);
                grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracao", 20, Models.Grid.Align.left, false, true);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelaFreteClienteArquivos = repositorioTabelaFreteClienteIntegracao.BuscarPorCodigo(codigo, false);

                grid.setarQuantidadeTotal(tabelaFreteClienteArquivos.ArquivosTransacao.Count);
                var retorno = (from obj in tabelaFreteClienteArquivos.ArquivosTransacao
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   SituacaoIntegracao = obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

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

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteArquivo arquivoIntegracaoTabelaFreteClienteArquivo = repositorioTabelaFreteClienteIntegracao.BuscarIntegracaoPorCodigo(codigo);

                if (arquivoIntegracaoTabelaFreteClienteArquivo == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracaoTabelaFreteClienteArquivo.ArquivoRequisicao == null && arquivoIntegracaoTabelaFreteClienteArquivo.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracaoTabelaFreteClienteArquivo.ArquivoRequisicao, arquivoIntegracaoTabelaFreteClienteArquivo.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Tabela Frete Cliente.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTabelaFreteClienteIntegracoes()
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

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);
                int totalIntegracoes = repositorioTabelaFreteClienteIntegracao.ContarConsulta(codigo, situacao);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> listaIntegracoes = (totalIntegracoes > 0) ? repositorioTabelaFreteClienteIntegracao.Consultar(codigo, situacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao>();

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
                return new JsonpResult(false, "Ocorre uma falha ao Consultar.");
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
                int codigo = Request.GetIntParam("Codigo");
                int totalAguardandoIntegracao = 0;
                int totalIntegrado = 0;
                int totalProblemaIntegracao = 0;
                int totalAguardandoRetorno = 0;

                if (codigo > 0)
                {
                    Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);

                    totalAguardandoIntegracao = repositorioTabelaFreteClienteIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                    totalIntegrado = repositorioTabelaFreteClienteIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                    totalProblemaIntegracao = repositorioTabelaFreteClienteIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                    totalAguardandoRetorno = repositorioTabelaFreteClienteIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);
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
                return new JsonpResult(false, "Ocorreu uma falha ao obter os totais das integrações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelasParaReenviarIntegracao = repositorioTabelaFreteClienteIntegracao.BuscarPorCodigo(codigo, auditavel: false);

                if(tabelasParaReenviarIntegracao == null)
                    return new JsonpResult(true, "Integração não encontrada");

                tabelasParaReenviarIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                tabelasParaReenviarIntegracao.ProblemaIntegracao = "";

                repositorioTabelaFreteClienteIntegracao.Atualizar(tabelasParaReenviarIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarTodasProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao repositorioTabelaFreteClienteIntegracao = new Repositorio.Embarcador.Frete.TabelaFreteClienteIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao> tabelasParaReenviarIntegracao = repositorioTabelaFreteClienteIntegracao.BuscarIntegracoesComProblema();

                foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteIntegracao tabelaFrete in tabelasParaReenviarIntegracao)
                {
                    tabelaFrete.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    tabelaFrete.ProblemaIntegracao = "";

                    repositorioTabelaFreteClienteIntegracao.Atualizar(tabelaFrete);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar as integrações.");
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

