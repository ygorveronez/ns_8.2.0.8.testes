using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Contabil
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotaisIntegracoes" }, "Contabils/AlteracaoArquivoMercante")]
    public class AlteracaoArquivoMercanteIntegracaoController : BaseController
    {
		#region Construtores

		public AlteracaoArquivoMercanteIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(true);
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
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(true);
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
        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                unitOfWork.Rollback();

                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracao()
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
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.Integracao, "TipoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.Tentativas, "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Motorista.DataDoEnvio, "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Mensagem, "Retorno", 30, Models.Grid.Align.left, false);

                int codigo = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");
                TipoIntegracao? tipoIntegracao = Request.GetNullableEnumParam<TipoIntegracao>("TipoIntegracao");
                
                Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPrioriedadeOrdenarPesquisaIntegracao(grid.header[grid.indiceColunaOrdena].data);

                var listaIntegracoes = repArquivoMercanteIntegracao.Consultar(situacao, tipoIntegracao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = listaIntegracoes.Total;

                var lista = (from obj in listaIntegracoes.Arquivos
                             select new
                             {
                                 obj.Codigo,
                                 Situacao = obj.SituacaoIntegracao,
                                 SituacaoIntegracao = obj.DescricaoSituacaoIntegracao,
                                 TipoIntegracao = obj.TipoIntegracao.DescricaoTipo,
                                 Retorno = obj.ProblemaIntegracao,
                                 obj.NumeroTentativas,
                                 DataIntegracao = obj.DataIntegracao.ToString("dd/MM/yyyy HH:mm:ss"),
                                 DT_ROWColor = obj.SituacaoIntegracao.ObterCorLinha(),
                                 DT_FontColor = obj.SituacaoIntegracao.ObterCorFonte(),
                             }).ToList();

                grid.AdicionaRows(lista);
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
        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if(string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, "Motivo deve ser informado");

                Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao arquivoMercanteIntegracao = repArquivoMercanteIntegracao.BuscarPorCodigo(codigo);

                if (arquivoMercanteIntegracao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                arquivoMercanteIntegracao.DataIntegracao = DateTime.Now;
                arquivoMercanteIntegracao.NumeroTentativas += 1;
                arquivoMercanteIntegracao.ProblemaIntegracao = motivo.Trim();
                arquivoMercanteIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repArquivoMercanteIntegracao.Atualizar(arquivoMercanteIntegracao);

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
        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                TipoIntegracao? tipoIntegracao = Request.GetNullableEnumParam<TipoIntegracao>("TipoIntegracao");

                Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(unitOfWork);

                var resultado = repArquivoMercanteIntegracao.ConsultaTotaisPorSituacao(tipoIntegracao: tipoIntegracao);

                var totaisPorSituacao = resultado.ToDictionary(r => r.Situacao, r => r.Total);

                var retorno = new
                {
                    TotalAguardandoIntegracao = totaisPorSituacao.GetValueOrDefault(SituacaoIntegracao.AgIntegracao),
                    TotalAguardandoRetorno = totaisPorSituacao.GetValueOrDefault(SituacaoIntegracao.AgRetorno),
                    TotalIntegrado = totaisPorSituacao.GetValueOrDefault(SituacaoIntegracao.Integrado),
                    TotalProblemaIntegracao = totaisPorSituacao.GetValueOrDefault(SituacaoIntegracao.ProblemaIntegracao),
                    TotalGeral = resultado.Sum(r => r.Total)
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

        #endregion

        #region Métodos Privados

        private string ObterPrioriedadeOrdenarPesquisaIntegracao(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TipoIntegracao")
                return "TipoIntegracao.Tipo";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
