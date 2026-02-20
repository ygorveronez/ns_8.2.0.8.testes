using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeAquaviarioManual
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Cargas/CargaCartaCorrecaoIntegracao", "Cargas/Carga")]
    public class CargaCartaCorrecaoIntegracaoController : BaseController
    {
		#region Construtores

		public CargaCartaCorrecaoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.CartaDeCorrecaoEletronica repCartaDeCorrecaoEletronica = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo> integracoesArquivos = repCartaDeCorrecaoEletronica.BuscarArquivosPorIntergacao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCartaDeCorrecaoEletronica.ContarBuscarArquivosPorIntergacao(codigo));

                var retorno = (from obj in integracoesArquivos
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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.CartaDeCorrecaoEletronica repCartaDeCorrecaoEletronica = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoCCEIntegracaoArquivo arquivoIntegracao = repCartaDeCorrecaoEletronica.BuscarIntergacaoCartaCorrecaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração CC-e de CT-e.zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos xmls de integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaDeCorrecaoEletronica = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao cartaCorrecaoIntegracao = repCartaDeCorrecaoEletronica.BuscarPorCodigo(codigo);

                if (cartaCorrecaoIntegracao == null)
                    return new JsonpResult(false, true, "Integração não encontrada.");

                cartaCorrecaoIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cartaCorrecaoIntegracao, null, "Reenviou Integração.", unidadeDeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cartaCorrecaoIntegracao.CartaCorrecao.CTe, null, "Reenviou Integração da carta de correção.", unidadeDeTrabalho);
                repCartaDeCorrecaoEletronica.Atualizar(cartaCorrecaoIntegracao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unidadeDeTrabalho);

                int codigoCTe = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> integracoes = repCartaCorrecaoIntegracao.BuscarPorCTe(codigoCTe, situacao);

                foreach (Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao integracao in integracoes)
                {
                    if (integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        repCartaCorrecaoIntegracao.Atualizar(integracao);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao, null, "Reenviou Integração.", unidadeDeTrabalho);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, null, "Reenviou as integrações de CC-e", unidadeDeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a integração.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCartaCorrecaoIntegracoes()
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
                grid.AdicionarCabecalho("Integração", "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Evento", "CartaCorrecao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Nº Tentativas", "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unitOfWork);
                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> listaIntegracoes = repCartaCorrecaoIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repCartaCorrecaoIntegracao.ContarConsulta(codigo, situacao);

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
                        CartaCorrecao = integracao.CartaCorrecao.NumeroSequencialEvento,
                        DT_RowColor = integracao.SituacaoIntegracao.ObterCorLinha(),
                        DT_FontColor = integracao.SituacaoIntegracao.ObterCorFonte()
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
                int codigoCTe = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repCartaDeCorrecaoEletronica = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repCartaDeCorrecaoEletronica.ContarConsulta(codigoCTe, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repCartaDeCorrecaoEletronica.ContarConsulta(codigoCTe, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repCartaDeCorrecaoEletronica.ContarConsulta(codigoCTe, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repCartaDeCorrecaoEletronica.ContarConsulta(codigoCTe, SituacaoIntegracao.AgRetorno);

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
        
        [AllowAuthenticate]
        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotivoDeveSerInformado);

                Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repositorio = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao integracao = repositorio.BuscarPorCodigo(codigo);

                if (integracao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas += 1;
                integracao.ProblemaIntegracao = motivo.Trim();
                integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repositorio.Atualizar(integracao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracoesIntegracaoIntercab()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();

                var dyn = new
                {
                    AtivarIntegracaoCartaCorrecao = integracaoIntercab?.AtivarIntegracaoCartaCorrecao ?? false
                };

                return new JsonpResult(dyn);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}

