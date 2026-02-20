using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Controllers.Cargas.CTeManual
{
    [CustomAuthorize(new string[] { "ConsultarHistoricoIntegracao", "DownloadArquivosHistoricoIntegracao" }, "Cargas/CargaCTeManual")]
    public class CargaCTeManualIntegracaoController : BaseController
    {
		#region Construtores

		public CargaCTeManualIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> ConsultarHistoricoIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoesArquivos = repCarga.BuscarArquivosPorIntergacao(codigo, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCarga.ContarBuscarArquivosPorIntergacao(codigo));

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

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = repCarga.BuscarIntergacaoPorCodigo(codigo);

                if (arquivoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico não encontrado.");

                if (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null)
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integração Motorista.zip");
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

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao cargaCTeManualIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaCTeManualIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                cargaCTeManualIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeManualIntegracao.CTe, null, "Reenviou Integração do CT-e Manual.", unidadeDeTrabalho);
                repIntegracao.Atualizar(cargaCTeManualIntegracao);

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

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                int codigoCarga = 0;
                int.TryParse(Request.Params("Carga"), out codigoCarga);

                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> integracoes = repCargaCTeManualIntegracao.BuscarPorCarga(codigoCarga, situacao);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao in integracoes)
                {
                    if (integracao.SituacaoIntegracao != SituacaoIntegracao.Integrado)
                    {
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                        repCargaCTeManualIntegracao.Atualizar(integracao);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, integracao.CTe, null, "Reenviou Integração do CT-e manual.", unidadeDeTrabalho);
                    }
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Reenviou as integrações da Carga", unidadeDeTrabalho);

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
        public async Task<IActionResult> PesquisaCargaCTeManualIntegracoes()
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
                grid.AdicionarCabecalho("Nº CT-e", "CTe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Tentativas", "NumeroTentativas", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data do Envio", "DataIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoIntegracao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Retorno", "Retorno", 30, Models.Grid.Align.left, false);

                int codigoCarga = Request.GetIntParam("Codigo");
                SituacaoIntegracao? situacao = Request.GetNullableEnumParam<SituacaoIntegracao>("Situacao");

                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);
                string propriedadeOrdenar = grid.header[grid.indiceColunaOrdena].data;

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> listaIntegracoes = repCargaCTeManualIntegracao.Consultar(codigoCarga, situacao, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int totalIntegracoes = repCargaCTeManualIntegracao.ContarConsulta(codigoCarga, situacao);

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
                        CTe = integracao.CTe.Numero,
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

        public async Task<IActionResult> ObterStatusEtapas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = int.Parse(Request.Params("Carga"));

                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> listaIntegracoes = repCargaCTeManualIntegracao.BuscarPorCarga(codigoCarga, null);
                SituacaoIntegracao situacaoIntegracao = SituacaoIntegracao.AgRetorno;

                if (listaIntegracoes.Count <= 0)
                    situacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                else
                {
                    situacaoIntegracao = listaIntegracoes
                        .Exists(integracao => integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao) ? SituacaoIntegracao.ProblemaIntegracao : SituacaoIntegracao.Integrado;
                }

                var retorno = new
                {
                    SituacaoIntegracao = situacaoIntegracao
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao definir etapa do processo");
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

                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repositorioCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);

                int totalAguardandoIntegracao = repositorioCargaCTeManualIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgIntegracao);
                int totalIntegrado = repositorioCargaCTeManualIntegracao.ContarConsulta(codigo, SituacaoIntegracao.Integrado);
                int totalProblemaIntegracao = repositorioCargaCTeManualIntegracao.ContarConsulta(codigo, SituacaoIntegracao.ProblemaIntegracao);
                int totalAguardandoRetorno = repositorioCargaCTeManualIntegracao.ContarConsulta(codigo, SituacaoIntegracao.AgRetorno);

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

        public async Task<IActionResult> ProblemaIntegracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Motorista.MotivoDeveSerInformado);

                Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repositorio = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao = repositorio.BuscarPorCodigo(codigo);

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

        #endregion
    }
}

