using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Servicos.Embarcador.Notificacao
{
    public sealed class NotificacaoEmailDados
    {
        public string De { get; set; }

        public string Login { get; set; }

        public string Nome { get; set; }

        public int Porta { get; set; } = 587;

        public bool PossuiSSL { get; set; }

        public string Senha { get; set; }

        public string ServidorSMTP { get; set; }
    }

    public sealed class NotificacaoEmail
    {
        #region Atributos Privados

        private Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte _configuracaoEmail;
        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public NotificacaoEmail(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoEmail: null) { }

        public NotificacaoEmail(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail)
        {
            _configuracaoEmail = configuracaoEmail;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte ObterConfiguracaoEmail()
        {
            if (_configuracaoEmail == null)
                _configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork).BuscarEmailEnviaDocumentoAtivo();

            return _configuracaoEmail;
        }

        private string ObterBodyEmailNotificacao(Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmpresa)
        {
            StringBuilder body = new StringBuilder();

            body.AppendLine(@"<div style=""font-family: Arial;"">");
            body.AppendLine($@"    <p><strong>{notificacaoEmpresa.CabecalhoMensagem}</strong></p>", !string.IsNullOrWhiteSpace(notificacaoEmpresa.CabecalhoMensagem));
            body.AppendLine($@"    <p style=""margin:0px"">{notificacaoEmpresa.Mensagem}</p>");
            body.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            body.AppendLine("    <p></p>");
            body.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            body.AppendLine("</div>");

            return body.ToString();
        }

        #endregion

        #region Métodos Públicos

        public bool EnviarEmail(string para, string[] copiaOcultaPara, string[] copiaPara, string assunto, string body, string nome, List<Attachment> anexos = null, String assinatura = "", string responderPara = "", int codigoEmpresa = 0)
        {
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = ObterConfiguracaoEmail();

            if (configuracaoEmail == null)
                return false;

            NotificacaoEmailDados dados = new NotificacaoEmailDados()
            {
                De = configuracaoEmail.Email,
                Login = configuracaoEmail.Email,
                Nome = configuracaoEmail.DisplayEmail,
                Porta = configuracaoEmail.PortaSmtp,
                PossuiSSL = configuracaoEmail.RequerAutenticacaoSmtp,
                Senha = configuracaoEmail.Senha,
                ServidorSMTP = configuracaoEmail.Smtp
            };

            string de = configuracaoEmail.Email;
            string login = configuracaoEmail.Email;
            string senha = configuracaoEmail.Senha;
            int porta = configuracaoEmail.PortaSmtp;
            bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
            string servidorSMTP = configuracaoEmail.Smtp;

            string corpo = ObterBodyEmailNotificacao(null);

            try
            {
                if (string.IsNullOrEmpty(de) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha) || string.IsNullOrEmpty(servidorSMTP))
                    throw new ServicoException("A configuração para envio dos e-mails está incorreta.");

                if (string.IsNullOrEmpty(assunto))
                    throw new ServicoException("O assunto do e-mail não foi informado.");

                if (string.IsNullOrEmpty(corpo))
                    throw new ServicoException("O conteúdo do e-mail não foi informado.");

                MailMessage mensagemEmail = new MailMessage();

                // Define o endereço do remetente
                if (!string.IsNullOrWhiteSpace(nome))
                    mensagemEmail.From = new MailAddress(de, nome);
                else
                    mensagemEmail.From = new MailAddress(de);

                if (!string.IsNullOrWhiteSpace(responderPara))
                    mensagemEmail.ReplyToList.Add(new MailAddress(responderPara));

                //Quando for homologação, envia e-mails somente para os que estão configurado na empresa
                var enviaEmailEmpresaHomologacao = false;
                if (codigoEmpresa > 0)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                    {
                        enviaEmailEmpresaHomologacao = true;
                        List<string> emails = new List<string>();

                        var emailsEmpresa = empresa.Email;
                        if (!string.IsNullOrWhiteSpace(emailsEmpresa) && empresa.StatusEmail == "A")
                        {
                            if (emailsEmpresa.Contains(";"))
                            {
                                string[] emailsSeparados = emailsEmpresa.Split(';');
                                for (int k = 0; k < emailsSeparados.Count(); k++)
                                {
                                    if (!string.IsNullOrWhiteSpace(emailsSeparados[k].Trim()))
                                        emails.Add(emailsSeparados[k].Trim());
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(emailsEmpresa.Trim()))
                                emails.Add(emailsEmpresa.Trim());
                        }

                        var emailsAdmEmpresa = empresa.EmailAdministrativo;
                        if (!string.IsNullOrWhiteSpace(emailsAdmEmpresa) && empresa.StatusEmailAdministrativo == "A")
                        {
                            if (emailsAdmEmpresa.Contains(";"))
                            {
                                string[] emailsSeparados = emailsAdmEmpresa.Split(';');
                                for (int k = 0; k < emailsSeparados.Count(); k++)
                                {
                                    if (!string.IsNullOrWhiteSpace(emailsSeparados[k].Trim()))
                                        emails.Add(emailsSeparados[k].Trim());
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(emailsAdmEmpresa.Trim()))
                                emails.Add(emailsAdmEmpresa.Trim());
                        }

                        emails = emails.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                        para = string.Empty;
                        copiaOcultaPara = null;
                        copiaPara = emails.ToArray();

                        if (copiaPara == null || copiaPara.Count() == 0)
                            throw new ServicoException("Nenhum e-mail informado no cadastro da empresa.");
                    }
                }

                if (!string.IsNullOrWhiteSpace(para))
                {
                    string[] emailsRecepient = para.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var emailRecepient in emailsRecepient)
                        mensagemEmail.To.Add(new MailAddress(emailRecepient.Trim()));
                }

                if (copiaOcultaPara != null && copiaOcultaPara.Count() > 0)
                {
                    foreach (string ebcc in copiaOcultaPara)
                    {
                        if (!string.IsNullOrWhiteSpace(ebcc) && ebcc != "")
                        {
                            if (string.IsNullOrWhiteSpace(para))
                                mensagemEmail.To.Add(new MailAddress(ebcc.Trim()));
                            else
                                mensagemEmail.Bcc.Add(new MailAddress(ebcc.Trim()));
                        }
                    }
                }

                if (copiaPara?.Count() > 0)
                {
                    foreach (string ecc in copiaPara)
                    {
                        if (!string.IsNullOrWhiteSpace(ecc) && ecc != "")
                            mensagemEmail.CC.Add(new MailAddress(ecc.Trim()));
                    }
                }

                if (anexos != null)
                {
                    foreach (Attachment attach in anexos)
                        mensagemEmail.Attachments.Add(attach);
                }

                StringBuilder bodyEmail = new StringBuilder();

                bodyEmail.Append(corpo.Replace(System.Environment.NewLine, "<br />"));

                if (!string.IsNullOrWhiteSpace(assinatura))
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("--").Append("<br />");
                    bodyEmail.Append(assinatura.Replace(System.Environment.NewLine, "<br />"));
                }

                if (enviaEmailEmpresaHomologacao && copiaPara != null && copiaPara.Count() > 0)
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("E-mails: " + string.Join(", ", copiaPara));
                }

                mensagemEmail.Subject = assunto;
                mensagemEmail.Body = bodyEmail.ToString();
                mensagemEmail.IsBodyHtml = true;
                mensagemEmail.Priority = MailPriority.Normal;

                // Cria uma instância de SmtpClient - Nota - Define qual o host a ser usado para envio
                // de mensagens, no local de smtp.server.com use o nome do SEU servidor
                SmtpClient mSmtpClient = new SmtpClient(servidorSMTP);

                mSmtpClient.Port = porta <= 0 ? 587 : porta;
                mSmtpClient.Credentials = new System.Net.NetworkCredential(login, senha);
                mSmtpClient.EnableSsl = possuiSSL;

                System.Net.SecurityProtocolType oldSecurityProtocol = System.Net.ServicePointManager.SecurityProtocol;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                mSmtpClient.Send(mensagemEmail);

                System.Net.ServicePointManager.SecurityProtocol = oldSecurityProtocol;

                //Salva log de envio de email
                Global.LogEnvioEmail.SalvarLogEnvioEmail(de, para, copiaOcultaPara, copiaPara, assunto, corpo, anexos, responderPara, codigoEmpresa, _unitOfWork);

                // Email enviado com sucesso!
                return true;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(new Exception("Falha ao enviar o e-mail para '" + para + "', '" + (copiaOcultaPara != null ? string.Join("', '", copiaOcultaPara) : "") + "'", excecao), "NotificacaoEmail");
                throw new ServicoException($"Falha ao enviar o e-mail para '{para}': {excecao.Message}");
            }
        }

        #endregion
    }
}
