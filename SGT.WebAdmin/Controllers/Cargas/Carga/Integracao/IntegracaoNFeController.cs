using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Integracao
{
    [CustomAuthorize(new string[] { "DownloadArquivosIntegracao", "DownloadArquivosHistoricoIntegracao", "ConsultarHistoricoIntegracao" }, "Cargas/IntegracaoNFe")]
    public class IntegracaoNFeController : BaseController
    {
		#region Construtores

		public IntegracaoNFeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Publicos

        public async Task<IActionResult> Pesquisar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid();

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao();
                if (configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono)
                {
                    Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repositorioIntegracao = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtroPequisa = ObterFiltrosPesquisa();
                    grid = ObterGrid(configuracaoGeral);
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                    int totalRegitrosIntegracao = repositorioIntegracao.ContarConsulta(filtroPequisa);
                    IList<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe> integracoes = totalRegitrosIntegracao > 0 ? repositorioIntegracao.Consultar(filtroPequisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe>();

                    grid.setarQuantidadeTotal(totalRegitrosIntegracao);
                    grid.AdicionaRows(FormatarRetorno(integracoes));
                }
                else
                {
                    Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repositorioIntegracao = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(unitOfWork);

                    Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe filtroPequisa = ObterFiltrosPesquisa();
                    grid = ObterGrid(configuracaoGeral);
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                    int totalRegitrosIntegracao = repositorioIntegracao.ContarConsulta(filtroPequisa);
                    IList<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe> integracoes = totalRegitrosIntegracao > 0 ? repositorioIntegracao.Consultar(filtroPequisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe>();

                    grid.setarQuantidadeTotal(totalRegitrosIntegracao);
                    grid.AdicionaRows(FormatarRetornoArquivoXMLNotaFiscalIntegracao(integracoes));
                }

                return new JsonpResult(grid);
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu um erro ao processar a consulta");
            }
        }
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repIntegracao = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono integracao = repIntegracao.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincronoArquivo arquivoIntegracao = integracao.ArquivosIntegracaoRetorno.FirstOrDefault(o => o.Codigo == codigo);

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.NaoHaRegistrosParaEsseHistorico);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Retorno - " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repIntegracao = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono integracao = repIntegracao.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.CancelamentoCarga.HistoricoNaoEncontrado);

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { integracao.ArquivoRequisicao, integracao.ArquivoResposta }, unitOfWork);

                return Arquivo(arquivo, "application/zip", "Arquivos Integração - " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repIntegracao = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono integracao = repIntegracao.BuscarPorCodigo(codigo, false);
                grid.setarQuantidadeTotal(integracao.ArquivosIntegracaoRetorno.Count());

                var retorno = (from obj in integracao.ArquivosIntegracaoRetorno.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork).BuscarConfiguracaoPadrao();

                if (configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono)
                {
                    //METODO UTILIZADO PELA ARCELORMITTAL

                    int codigo = Request.GetIntParam("Codigo");

                    Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono repositorioIntegracao = new Repositorio.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.ProcessamentoNotaFiscalAssincrono integracao = repositorioIntegracao.BuscarPorCodigo(codigo, false);

                    integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    integracao.Mensagem = "";
                    integracao.Tentativas = 0;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenvio de integração", unitOfWork);

                    repositorioIntegracao.Atualizar(integracao);
                }
                else
                {
                    int codigo = Request.GetIntParam("Codigo");

                    Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao repositorioIntegracao = new Repositorio.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao(unitOfWork);
                    Dominio.Entidades.Embarcador.NotaFiscal.ArquivoXMLNotaFiscalIntegracao integracao = repositorioIntegracao.BuscarPorCodigo(codigo, false);

                    if (integracao.Situacao == SituacaoProcessamentoRegistro.FalhaLiberacao)
                    {
                        //vamos processar a liberacao novamente
                        integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Sucesso;
                        integracao.Mensagem = "";
                        integracao.TentativasLiberacao = 0;
                    }
                    else if (integracao.Situacao != SituacaoProcessamentoRegistro.Sucesso)
                    {
                        //vamos processar tudo novamente apenas se for diferente de sucesso (primeira etapa ok)
                        integracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoRegistro.Pendente;
                        integracao.Mensagem = "";
                        integracao.Tentativas = 0;
                        integracao.TentativasLiberacao = 0;
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenvio de integração", unitOfWork);

                    repositorioIntegracao.Atualizar(integracao);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, Localization.Resources.Cargas.MontagemCargaMapa.OcorreuUmaFalhaAoReenviarArquivoParaIntegracao);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaIntegracaoNFe()
            {
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                SituacaoIntegracao = Request.GetListEnumParam<SituacaoIntegracao>("SituacaoIntegracao"),
                SituacaoProcessamentoRegistro = Request.GetListEnumParam<SituacaoProcessamentoRegistro>("SituacaoProcessamentoRegistro"),
                DataInicial = Request.GetNullableDateTimeParam("DataIntegracaoInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataIntegracaoFinal"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                Chave = Request.GetStringParam("Chave"),
            };
        }

        private Models.Grid.Grid ObterGrid(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número Carga", "NumeroCarga", 10, Models.Grid.Align.left, true);
            if (configuracaoGeral.ProcessarXMLNotasFiscaisAssincrono)
            {
                grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação Integração", "SituacaoIntegracaoDescricao", 10, Models.Grid.Align.left, true);
            }
            else
                grid.AdicionarCabecalho("Situação Integração", "SituacaoProcessamentoRegistro", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Integração", "DataIntegracaoDescricao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Retorno", "Retorno", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic FormatarRetorno(IList<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe> integracoes)
        {
            return from obj in integracoes
                   select new
                   {
                       obj.Codigo,
                       obj.NumeroCarga,
                       obj.NumeroPedido,
                       obj.Retorno,
                       SituacaoIntegracaoDescricao = obj.SituacaoIntegracao.ObterDescricao(),
                       DataIntegracaoDescricao = obj.DataIntegracao.ToString("G"),
                       DT_RowColor = obj.SituacaoIntegracao.ObterCorLinha(),
                       DT_FontColor = obj.SituacaoIntegracao.ObterCorFonte(),
                   };
        }

        private dynamic FormatarRetornoArquivoXMLNotaFiscalIntegracao(IList<Dominio.ObjetosDeValor.Embarcador.Carga.IntegracaoNFe> integracoes)
        {
            return from obj in integracoes
                   select new
                   {
                       obj.Codigo,
                       obj.NumeroCarga,
                       obj.Retorno,
                       SituacaoProcessamentoRegistro = obj.SituacaoProcessamentoRegistro.ObterDescricao(),
                       DataIntegracaoDescricao = obj.DataIntegracao.ToString("G"),
                       DT_RowColor = obj.SituacaoProcessamentoRegistro.ObterCorLinha(),
                       DT_FontColor = obj.SituacaoProcessamentoRegistro.ObterCorFonte(),
                   };
        }

        #endregion
    }
}
