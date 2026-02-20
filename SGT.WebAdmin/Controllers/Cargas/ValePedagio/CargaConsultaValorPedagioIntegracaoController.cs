using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ValePedagio
{
    [CustomAuthorize("Cargas/CargaConsultaValorPedagioIntegracao", "Cargas/Carga")]
    public class CargaConsultaValorPedagioIntegracaoController : BaseController
    {
		#region Construtores

		public CargaConsultaValorPedagioIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaIntegracaoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaIntegracaoValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaConsultaIntegracaoValePedagioCarga = repCargaConsultaIntegracaoValePedagio.ConsultaIntegracaoPorCarga(codigoCarga, null);

                if (cargaConsultaIntegracaoValePedagioCarga == null)
                    return new JsonpResult(false, true, "Nenhum registro encontrado");

                return new JsonpResult(new
                {
                    Codigo = cargaConsultaIntegracaoValePedagioCarga.Codigo,
                    SituacaoConsulta = cargaConsultaIntegracaoValePedagioCarga.DescricaoSituacaoIntegracao,
                    Situacao = cargaConsultaIntegracaoValePedagioCarga.SituacaoIntegracao,
                    Integradora = cargaConsultaIntegracaoValePedagioCarga.TipoIntegracao.Descricao,
                    ValorPedagio = cargaConsultaIntegracaoValePedagioCarga.ValorValePedagio.ToString("C"),
                    DataIntegracao = cargaConsultaIntegracaoValePedagioCarga.DataIntegracao.ToDateTimeString(),
                    cargaConsultaIntegracaoValePedagioCarga.ProblemaIntegracao,
                    RotaFreteExclusiva = cargaConsultaIntegracaoValePedagioCarga?.RotaFrete?.Descricao ?? string.Empty,
                    ListaArquivos = (from arquivo in cargaConsultaIntegracaoValePedagioCarga.ArquivosTransacao
                                     select new
                                     {
                                         arquivo.Codigo,
                                         arquivo.Mensagem,
                                         Data = arquivo.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                         arquivo.DescricaoTipo,
                                     }).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar a integração da consulta de valores do vale pedágio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosIntegracao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao integracao = repCargaConsultaValorPedagio.BuscarPorCodigoArquivo(codigo);

                if (integracao == null)
                    return new JsonpResult(true, false, "Integração não encontrada.");

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaValePedagioIntegracaoArquivo arquivoIntegracao = integracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de consulta.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Consulta Integração Vale Pedágio " + integracao.Carga.CodigoCargaEmbarcador + ".zip");
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

        public async Task<IActionResult> LiberarSemConsulta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar essa ação.");

                // Valida informações
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                carga.LiberadoComProblemaValePedagio = true;
                carga.ProblemaIntegracaoValePedagio = false;
                carga.PossuiPendencia = false;
                carga.MotivoPendencia = "";
                repCarga.Atualizar(carga);

                if (carga.PossuiPendencia)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com consulta valor pedágio Rejeitados.", unitOfWork);
                }

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReenviarIntegracaoConsultaValePedagio()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao repCargaConsultaValorPedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaConsultaValorPedagioIntegracao cargaIntegracaoValorPedagio = repCargaConsultaValorPedagio.BuscarPorCodigo(codigo, false);

                if (cargaIntegracaoValorPedagio == null)
                    return new JsonpResult(false, true, "Integracao não encontrada.");

                if (cargaIntegracaoValorPedagio.SituacaoIntegracao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao)
                    return new JsonpResult(false, true, "Não é possível reenviar quando a situação é diferente de Falha ao Integrar.");

                cargaIntegracaoValorPedagio.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

                // Atualiza situação da carga
                if (cargaIntegracaoValorPedagio.Carga.PossuiPendencia)
                {
                    cargaIntegracaoValorPedagio.Carga.PossuiPendencia = false;
                    cargaIntegracaoValorPedagio.Carga.ProblemaIntegracaoValePedagio = false;
                    cargaIntegracaoValorPedagio.Carga.IntegrandoValePedagio = true;
                    cargaIntegracaoValorPedagio.Carga.MotivoPendencia = "";

                    repCarga.Atualizar(cargaIntegracaoValorPedagio.Carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaIntegracaoValorPedagio, null, "Reenviou integração rejeitada.", unidadeDeTrabalho);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaIntegracaoValorPedagio.Carga, null, "Reenviou integração rejeitada.", unidadeDeTrabalho);

                }
                repCargaConsultaValorPedagio.Atualizar(cargaIntegracaoValorPedagio);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao reenviar a integração da consulta vale pedágio.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion
    }
}
