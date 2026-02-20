using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize(new string[] { "ObterTotais", "Download" }, "NFS/NFSManualCancelamento")]
    public class NFSManualCancelamentoIntegracaoController : BaseController
    {
		#region Construtores

		public NFSManualCancelamentoIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDadosIntegracoes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI repNFSManualCancelamentoIntegracaoEDI = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoEDI(unidadeDeTrabalho);
                Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe repNFSManualCancelamentoIntegracaoCTe = new Repositorio.Embarcador.NFS.NFSManualCancelamentoIntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesCTe = repNFSManualCancelamentoIntegracaoCTe.BuscarTipoIntegracaoPorNFSManualCancelamento(codigoNFSManualCancelamento);
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> tiposIntegracoesEDI = repNFSManualCancelamentoIntegracaoEDI.BuscarTipoIntegracaoPorNFSManualCancelamento(codigoNFSManualCancelamento);

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
                int codigoNFSManualCancelamento = Request.GetIntParam("NFSManualCancelamento");

                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeDeTrabalho);
                
                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento = repNFSManualCancelamento.BuscarPorCodigo(codigoNFSManualCancelamento);

                if (nfsManualCancelamento == null)
                    return new JsonpResult(true, false, "Cancelamento de NFS manual não encontrado.");

                if (nfsManualCancelamento.SituacaoNFSManualCancelamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.AgIntegracao)
                    return new JsonpResult(true, false, "A situação do cancelamento da NFS manual não permite a finalização da etapa.");

                if (nfsManualCancelamento.GerandoIntegracoes)
                    return new JsonpResult(false, true, "O sistema ainda está gerando as integrações, não sendo possível finalizar a etapa. Aguarde alguns minutos e tente novamente.");

                unidadeDeTrabalho.Start();

                nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.Cancelada;

                repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManualCancelamento, null, "Finalizou.", unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

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
