using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS.Integracao
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "NFS/NFSManual")]
    public class NFSManualIntegracaoController : BaseController
    {
		#region Construtores

		public NFSManualIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLancamentoNFSManual;
                int.TryParse(Request.Params("LancamentoNFSManual"), out codigoLancamentoNFSManual);

                Repositorio.Embarcador.NFS.NFSManualEDIIntegracao repNFSManualEDIIntegracao = new Repositorio.Embarcador.NFS.NFSManualEDIIntegracao(unidadeDeTrabalho);
                Repositorio.Embarcador.NFS.NFSManualCTeIntegracao repNFSManualCTeIntegracao = new Repositorio.Embarcador.NFS.NFSManualCTeIntegracao(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = repNFSManualCTeIntegracao.BuscarTipoIntegracaoPorLancamentoNFSManual(codigoLancamentoNFSManual);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repNFSManualEDIIntegracao.BuscarTipoIntegracaoPorLancamentoNFSManual(codigoLancamentoNFSManual);
                
                return new JsonpResult(new
                {
                    TiposIntegracoesCTe = tiposIntegracoesCTe,
                    TiposIntegracoesEDI = tiposIntegracoesEDI
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados das integrações.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Finalizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                //if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_ConfirmarIntegracao))
                //    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int codigoLancamentoNFSManual;
                int.TryParse(Request.Params("LancamentoNFSManual"), out codigoLancamentoNFSManual);

                Servicos.Embarcador.Carga.Carga svcCarga = new Servicos.Embarcador.Carga.Carga(unidadeDeTrabalho);
                //Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeDeTrabalho);
                //Repositorio.Embarcador.Cargas.CargaLog repLog = new Repositorio.Embarcador.Cargas.CargaLog(unidadeDeTrabalho);
                Servicos.Embarcador.Hubs.NFSManual svcNFSManual = new Servicos.Embarcador.Hubs.NFSManual();

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoLancamentoNFSManual);

                if (lancamentoNFSManual == null)
                    return new JsonpResult(true, false, "NFS manual não encontrada.");

                if (lancamentoNFSManual.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.AgIntegracao)
                    return new JsonpResult(true, false, "A situação da NFS manual não permite a finalização da etapa.");

                if (lancamentoNFSManual.GerandoIntegracoes)
                    return new JsonpResult(false, true, "O sistema ainda está gerando as integrações, não sendo possível finalizar a etapa. Aguarde alguns minutos e tente novamente.");

                unidadeDeTrabalho.Start();

                //Dominio.Entidades.Embarcador.Cargas.CargaLog log = new Dominio.Entidades.Embarcador.Cargas.CargaLog();

                //log.Acao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCarga.FinalizarEtapaIntegracao;
                //log.Carga = lancamentoNFSManual;
                //log.Data = DateTime.Now;
                //log.Usuario = Usuario;

                //repLog.Inserir(log);

                lancamentoNFSManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLancamentoNFSManual.Finalizada;
                lancamentoNFSManual = Servicos.Embarcador.Integracao.IntegracaoNFSManual.AtualizarNumeracaoNFSManualIntegracao(lancamentoNFSManual, unidadeDeTrabalho);

                // Integracao com SignalR
                svcNFSManual.InformarLancamentoNFSManualAtualizada(lancamentoNFSManual.Codigo);

                repLancamentoNFSManual.Atualizar(lancamentoNFSManual);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, lancamentoNFSManual, null, "Finalizou.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                //svcHubCarga.InformarCargaAtualizada(lancamentoNFSManual.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao finalizar a etapa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
