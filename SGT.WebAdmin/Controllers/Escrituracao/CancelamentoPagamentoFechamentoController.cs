using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/CancelamentoPagamento")]
    public class CancelamentoPagamentoFechamentoController : BaseController
    {
		#region Construtores

		public CancelamentoPagamentoFechamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDetalhesFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentosContabeis = repDocumentoContabil.BuscarSumarizadoPorCancelamentoPagamento(codigo);

                return new JsonpResult(documentosContabeis);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para o cancelamento");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        
    }
}
