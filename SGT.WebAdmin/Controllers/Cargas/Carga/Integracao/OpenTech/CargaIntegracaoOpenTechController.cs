using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.Integracao.OpenTech
{
    [CustomAuthorize("Cargas/Carga", "Logistica/JanelaCarregamento")]
    public class CargaIntegracaoOpenTechController : BaseController
    {
		#region Construtores

		public CargaIntegracaoOpenTechController(Conexao conexao) : base(conexao) { }

		#endregion

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadAutorizacaoEmbarque()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string mensagemErro = string.Empty;

                byte[] arquivo = new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unidadeDeTrabalho).ObterAutorizacaoEmbarque(cargaIntegracao, out mensagemErro);

                if (arquivo == null)
                    return new JsonpResult(false, true, mensagemErro);

                return Arquivo(arquivo, "application/pdf", "AE_" + cargaIntegracao.Protocolo + ".pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download da autorização de embarque.");
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> EnviarEmailAutorizacaoEmbarque()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaCargaIntegracao repIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unidadeDeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao = repIntegracao.BuscarPorCodigo(codigo);

                if (cargaIntegracao == null)
                    return new JsonpResult(false, "Integração não encontrada.");

                string mensagemErro = string.Empty;

                byte[] arquivo = new Servicos.Embarcador.Integracao.OpenTech.IntegracaoCargaOpenTech(unidadeDeTrabalho).ObterAutorizacaoEmbarque(cargaIntegracao, out mensagemErro);

                if (arquivo == null)
                    return new JsonpResult(false, true, mensagemErro);

                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();
                if (email == null)
                    return new JsonpResult(false, "Configuração de e-mail não encontrada.");

                List<Dominio.Entidades.Cliente> remetente = (from obj in cargaIntegracao.Carga.Pedidos select obj.Pedido.Remetente ?? null).ToList();
                List<Dominio.Entidades.Cliente> destinatario = (from obj in cargaIntegracao.Carga.Pedidos select obj.Pedido.Destinatario ?? null).ToList();
                List<Dominio.Entidades.Cliente> recebedor = (from obj in cargaIntegracao.Carga.Pedidos select obj.Pedido.Recebedor ?? null).ToList();
                List<Dominio.Entidades.Cliente> expedidor = (from obj in cargaIntegracao.Carga.Pedidos select obj.Pedido.Expedidor ?? null).ToList();

                List<string> emails = new List<string>();
                if (remetente != null && remetente.Count > 0)
                {
                    foreach (var rem in remetente)
                    {
                        for (int a = 0; a < rem?.Emails?.Count; a++)
                        {
                            if (!string.IsNullOrWhiteSpace(rem.Emails[a].Email) && rem.Emails[a].TipoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros)
                                emails.Add(rem.Emails[a].Email);
                        }
                    }
                }

                if (destinatario != null && destinatario.Count > 0)
                {
                    foreach (var des in destinatario)
                    {
                        for (int a = 0; a < des?.Emails?.Count; a++)
                        {
                            if (!string.IsNullOrWhiteSpace(des.Emails[a].Email) && des.Emails[a].TipoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros)
                                emails.Add(des.Emails[a].Email);
                        }
                    }
                }

                if (recebedor != null && recebedor.Count > 0)
                {
                    foreach (var rec in recebedor)
                    {
                        for (int a = 0; a < rec?.Emails?.Count; a++)
                        {
                            if (!string.IsNullOrWhiteSpace(rec.Emails[a].Email) && rec.Emails[a].TipoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros)
                                emails.Add(rec.Emails[a].Email);
                        }
                    }
                }

                if (expedidor != null && expedidor?.Count > 0)
                {
                    foreach (var exp in expedidor)
                    {
                        for (int a = 0; a < exp?.Emails?.Count; a++)
                        {
                            if (!string.IsNullOrWhiteSpace(exp.Emails[a].Email) && exp.Emails[a].TipoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmail.Outros)
                                emails.Add(exp.Emails[a].Email);
                        }
                    }
                }

                emails = emails.Count > 0 ? emails.Distinct().ToList() : null;
                if (emails.Count > 0)
                {
                    string assunto = "Autorização Embarque Carga " + cargaIntegracao.Carga.CodigoCargaEmbarcador;
                    string mensagemEmail = "Segue em anexo a autorização de embarque da OpenTech da carga " + cargaIntegracao.Carga.CodigoCargaEmbarcador + " em anexo.";
                    string mensagemErroEmail = "";
                    var listaAnexo = new List<System.Net.Mail.Attachment>();

                    listaAnexo.Add(new System.Net.Mail.Attachment(new System.IO.MemoryStream(arquivo), "AutorizacaoEmbarque.pdf", "application/pdf"));

                    List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                    Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, emails.Count == 1 ? emails[0] : null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErroEmail, email.DisplayEmail,
                        listaAnexo.Count > 0 ? listaAnexo : null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho);

                    if (!string.IsNullOrWhiteSpace(mensagemErroEmail))
                        return new JsonpResult(false, "mensagemErroEmail");
                    else
                        return new JsonpResult(true, "Email enviado com sucesso!");
                }

                return new JsonpResult(true, "Email");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao tentar enviar e-mail com o arquivo da autorização de embarque.");
            }
        }
    }
}
