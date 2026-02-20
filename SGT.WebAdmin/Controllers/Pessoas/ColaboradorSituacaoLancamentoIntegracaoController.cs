using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pessoas
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao", "ObterTotaisIntegracoes" }, "Pessoas/ColaboradorSituacaoLancamento")]
    public class ColaboradorSituacaoLancamentoIntegracaoController : BaseController
    {
		#region Construtores

		public ColaboradorSituacaoLancamentoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo repColaboradorSituacaoLancamentoIntegracaoArquivo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo(unidadeDeTrabalho);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 60, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo> arquivos = repColaboradorSituacaoLancamentoIntegracaoArquivo.BuscarPorIntegracao(codigo);                
                grid.setarQuantidadeTotal(arquivos?.Count() ?? 0);

                var retorno = (from obj in arquivos.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
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
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo repColaboradorSituacaoLancamentoIntegracaoArquivo = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracaoArquivo arquivoIntegracao = repColaboradorSituacaoLancamentoIntegracaoArquivo.BuscarPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");                

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração Situação Colaborador " + arquivoIntegracao.ColaboradorSituacaoLancamentoIntegracao.ColaboradorLancamento.Colaborador.Descricao + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos jsons de integração.");
            }
        }

        public async Task<IActionResult> Integrar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao integracao = repColaboradorSituacaoLancamentoIntegracao.BuscarPorCodigo(codigo);

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;

                repColaboradorSituacaoLancamentoIntegracao.Atualizar(integracao);

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

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(unitOfWork);
                string propriedadeOrdenar = ObterPrioriedadeOrdenarPesquisaIntegracao(grid.header[grid.indiceColunaOrdena].data);

                List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao> listaIntegracoes = repColaboradorSituacaoLancamentoIntegracao.Consultar(codigo, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repColaboradorSituacaoLancamentoIntegracao.ContarConsulta(codigo, situacao);

                var lista = (from obj in listaIntegracoes
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

        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, "Motivo deve ser informado");

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao colaboradorSituacaoLancamentoIntegracao = repColaboradorSituacaoLancamentoIntegracao.BuscarPorCodigo(codigo);

                if (colaboradorSituacaoLancamentoIntegracao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                colaboradorSituacaoLancamentoIntegracao.DataIntegracao = DateTime.Now;
                colaboradorSituacaoLancamentoIntegracao.NumeroTentativas += 1;
                colaboradorSituacaoLancamentoIntegracao.ProblemaIntegracao = motivo.Trim();
                colaboradorSituacaoLancamentoIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                repColaboradorSituacaoLancamentoIntegracao.Atualizar(colaboradorSituacaoLancamentoIntegracao);

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

        public async Task<IActionResult> ObterTotaisIntegracoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repColaboradorSituacaoLancamentoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repColaboradorSituacaoLancamentoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repColaboradorSituacaoLancamentoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repColaboradorSituacaoLancamentoIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

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
