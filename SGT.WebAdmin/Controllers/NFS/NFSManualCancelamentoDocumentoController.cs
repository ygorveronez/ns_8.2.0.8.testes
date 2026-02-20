using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/NFSManualCancelamento")]
    public class NFSManualCancelamentoDocumentoController : BaseController
    {
		#region Construtores

		public NFSManualCancelamentoDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> DesabilitarDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int codigoCTe = Request.GetIntParam("Codigo");

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);

                if (cte == null)
                    return new JsonpResult(false, true, "NFS Manual não encontrado.");

                if (cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Cancelada &&
                    cte.SituacaoCTeSefaz != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Anulado)
                    return new JsonpResult(false, true, "A situação do documento não permite que o mesmo seja desabilitado.");

                unitOfWork.Start();

                cte.Desabilitado = true;

                repCTe.Atualizar(cte);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Desabilitou a NFS manual.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao validar os dados da carga para cancelamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
