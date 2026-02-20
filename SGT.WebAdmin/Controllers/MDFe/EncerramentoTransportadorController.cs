using SGTAdmin.Controllers;
using System;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SGT.WebAdmin.Controllers.MDFe
{
    [CustomAuthorize("MDFe/EncerramentoTransportador")]
    public class EncerramentoTransportadorController : BaseController
    {
		#region Construtores

		public EncerramentoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string dados = Servicos.Criptografia.Descriptografar(WebUtility.UrlDecode(Request.Params("x")), "Encerramento##@");

                string cnpjEmpresa = dados.Substring(0, 14);
                int codigoMDFe = int.Parse(dados.Substring(14));

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (mdfe.Empresa.CNPJ != cnpjEmpresa)
                    return new JsonpResult(false, true, "MDF-e não pertence à empresa informada.");

                string veiculo = mdfe.Veiculos.FirstOrDefault()?.Placa ?? string.Empty;

                if (mdfe.Reboques.Count > 0)
                    veiculo += " / " + string.Join(" / ", mdfe.Reboques.Select(o => o.Placa));

                return new JsonpResult(new
                {
                    mdfe.Codigo,
                    mdfe.Chave,
                    UFOrigem = mdfe.EstadoCarregamento.Sigla,
                    UFDestino = mdfe.EstadoDescarregamento.Sigla,
                    Data = mdfe.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm"),
                    Numero = mdfe.Numero + " - " + mdfe.Serie.Numero,
                    mdfe.Protocolo,
                    Veiculo = veiculo,
                    Motorista = mdfe.Motoristas.FirstOrDefault()?.Nome ?? string.Empty,
                    mdfe.Status,
                    mdfe.DescricaoStatus
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados do MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosCaptcha()
        {
            try
            {
                string captcha = Utilidades.Captcha.Captcha.GenerateRandomCode();

                RandomImage imagemCaptcha = new RandomImage(captcha, 240, 75);
                using System.IO.MemoryStream stream = new System.IO.MemoryStream();
                imagemCaptcha.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                string hashCaptcha = Servicos.Criptografia.GerarHashMD5(captcha.ToUpper());

                return new JsonpResult(new
                {
                    ImagemCaptcha = Convert.ToBase64String(stream.ToArray()),
                    Captcha = hashCaptcha
                });

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao gerar o captcha.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> EncerrarMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params("MDFe"), out codigoMDFe);

                string captcha = Request.Params("CaptchaInformado");
                string hashCaptcha = Request.Params("Captcha");

                string hash = Servicos.Criptografia.GerarHashMD5(captcha.ToUpper());

                if (hash != hashCaptcha)
                    return new JsonpResult(false, true, "O captcha digitado é inválido.");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, DateTime.Now, unidadeTrabalho))
                {
                    mdfe.Log = "Encerrado manualmente pelo transportador (usuário " + this.Usuario.Nome + ") as " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    repMDFe.Atualizar(mdfe);

                    svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, DateTime.Now, mdfe.Empresa, mdfe.Empresa.Localidade, mdfe.Log, unidadeTrabalho);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Encerramento manual solicitado", unidadeTrabalho);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados do MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }


    }
}
