using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Escrituracao
{
    [CustomAuthorize("Escrituracao/CancelamentoProvisao")]
    public class CancelamentoProvisaoFechamentoController : BaseController
    {
		#region Construtores

		public CancelamentoProvisaoFechamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> ObterDetalhesFechamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Escrituracao.CancelamentoProvisao repositorioCancelamentoProvisao = new Repositorio.Embarcador.Escrituracao.CancelamentoProvisao(unitOfWork);
                Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisao cancelamentoProvisao = repositorioCancelamentoProvisao.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil repositorioDocumentoContabil = new Repositorio.Embarcador.ConfiguracaoContabil.DocumentoContabil(unitOfWork);
                List<Dominio.ObjetosDeValor.Embarcador.Escrituracao.DocumentoContabil> documentosContabeis = repositorioDocumentoContabil.BuscarSumarizadoPorCancelamentoProvisao(codigo);

                var retorno = new
                {
                    DocumentosContabeis = documentosContabeis,
                    DataLancamento = cancelamentoProvisao.DataLancamento.HasValue ? cancelamentoProvisao.DataLancamento.Value.ToString("dd/MM/yyyy") : cancelamentoProvisao.DataCriacao.ToString("dd/MM/yyyy")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os detalhes do Fechamento para o cancelamento da provisão");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
