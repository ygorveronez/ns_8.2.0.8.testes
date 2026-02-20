using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFe
{
    [CustomAuthorize("NFe/ConsultaSiteSEFAZ")]
    public class ConsultaSiteSEFAZController : BaseController
    {
		#region Construtores

		public ConsultaSiteSEFAZController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterRequisicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.ServicoConsultaNFe.RetornoOfRequisicaoSefazgj5B5PAD retorno = Servicos.Embarcador.NFe.NFe.ObterRequisicaoConsultaNFeSiteSEFAZ(unitOfWork);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter a requisição da SEFAZ.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarNFePelaChave()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string chave = Utilidades.String.OnlyNumbers(Request.Params("Chave"));

                chave = chave.Trim();
                chave = Utilidades.String.OnlyNumbers(chave);
                if (string.IsNullOrWhiteSpace(chave) || chave.Length != 44)
                    return new JsonpResult(false, true, "A chave digitada está inválida.");

                string tipoDocumento = chave.Substring(20, 2);
                if (tipoDocumento != "55")
                    return new JsonpResult(false, true, "A chave digitada não é de nota fiscal eletrônica.");

                object dadosNFe = null;
                string erro = string.Empty;

                if (Servicos.Embarcador.NFe.NFe.ObterDadosNFePelaChave(chave, out erro, out dadosNFe, Empresa.Codigo, unitOfWork, _conexao.StringConexao, TipoServicoMultisoftware, Auditado))
                    return new JsonpResult(dadosNFe);
                else
                    return new JsonpResult(false, true, erro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter a nota fiscal pela chave.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz>(Request.Params("Requisicao"));

                string chave = Utilidades.String.OnlyNumbers(Request.Params("Chave"));
                string captcha = Request.Params("Captcha");

                Servicos.ServicoConsultaNFe.RetornoOfConsultaSefazgj5B5PAD retorno = Servicos.Embarcador.NFe.NFe.ConsultarNFeSiteSEFAZ(requisicaoSefaz, chave, captcha, unitOfWork);

                object dadosNFe = null;
                string erro = string.Empty;

                if (Servicos.Embarcador.NFe.NFe.ObterDadosNFeSiteSEFAZ(retorno, out erro, out dadosNFe, Empresa.Codigo, unitOfWork, _conexao.StringConexao))
                    return new JsonpResult(dadosNFe);
                else
                    return new JsonpResult(false, true, erro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao obter a requisição da SEFAZ.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
