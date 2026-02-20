using AdminMultisoftware.Dominio.Enumeradores;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Infrastructure.Services.HttpClientFactory;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using Newtonsoft.Json.Linq;
using Repositorio;
using Servicos.Cache;
using Servicos.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos
{
    public class Email : ServicoBase
    {
        #region Construtores

        public Email() : base() { }        

        public Email(UnitOfWork unitOfWork) : base(unitOfWork) { }        

        private string ChaveChilkat = "MAILT34MB34N_191176400UHJ";

        #endregion

        #region Métodos Públicos

        public Boolean EnviarEmailImagemTexto(String from, String user, String password, String recepient, String bcc, String cc, String subject, String body, String servidorSMTP, string imagemBody, String signature = "", bool possuiSSL = false, string replyTo = "", int porta = 587, Repositorio.UnitOfWork unitOfWork = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user) && unitOfWork != null)
                {
                    Repositorio.ConfiguracaoEmail repConfiguracaoEmail = new Repositorio.ConfiguracaoEmail(unitOfWork);
                    Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = repConfiguracaoEmail.BuscarConfiguracao();
                    if (configuracaoEmail != null)
                    {
                        from = configuracaoEmail.Email;
                        user = string.IsNullOrEmpty(configuracaoEmail.User) ? configuracaoEmail.Email : configuracaoEmail.User;
                        password = configuracaoEmail.Senha;
                        servidorSMTP = configuracaoEmail.Smtp;
                        porta = configuracaoEmail.PortaSmtp;
                        possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                    }
                }

                //Verifica se os campos obrigatórios estão preenchidos
                if (String.IsNullOrEmpty(from) || String.IsNullOrEmpty(user) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(recepient) || String.IsNullOrEmpty(subject) || String.IsNullOrEmpty(body) || String.IsNullOrEmpty(servidorSMTP))
                    return false;

                // Cria uma instância do objeto MailMessage
                MailMessage mMailMessage = new MailMessage();

                // Define o endereço do remetente
                mMailMessage.From = new MailAddress(from);

                // Define o destinario da mensagem
                mMailMessage.To.Add(new MailAddress(recepient));

                if (!string.IsNullOrWhiteSpace(replyTo))
                    // Define o endereço para resposta das mensagens
                    mMailMessage.ReplyToList.Add(new MailAddress(replyTo));

                if (!String.IsNullOrEmpty(bcc))
                    // Define o endereço bcc
                    mMailMessage.Bcc.Add(new MailAddress(bcc));


                if (!String.IsNullOrEmpty(cc))
                    // Define o endereço cc
                    mMailMessage.CC.Add(cc);


                //if (attachments != null)
                //    // Adiciona os anexos
                //    foreach (Attachment attach in attachments)
                //        mMailMessage.Attachments.Add(attach);


                StringBuilder bodyEmail = new StringBuilder();
                bodyEmail.Append("<html>");
                bodyEmail.Append(body.Replace(System.Environment.NewLine, "<br />"));
                bodyEmail.Append("<br /><br />");
                bodyEmail.Append("<img src=\"cid:imagem\" title=\"imagem\" />");
                if (!String.IsNullOrWhiteSpace(signature))
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("--").Append("<br />");
                    bodyEmail.Append(signature.Replace(System.Environment.NewLine, "<br />"));
                }

                // Define o assunto
                mMailMessage.Subject = subject;

                // Define o corpo da mensagem
                LinkedResource lr = new LinkedResource(imagemBody, "image/gif");
                lr.ContentId = "imagem";

                AlternateView av = AlternateView.CreateAlternateViewFromString(bodyEmail.ToString(), new ContentType("text/html"));
                av.LinkedResources.Add(lr);

                mMailMessage.AlternateViews.Add(av);

                //' Define o formato do email como HTML
                mMailMessage.IsBodyHtml = true;
                // Define a prioridade da mensagem como normal
                mMailMessage.Priority = MailPriority.Normal;

                // Cria uma instância de SmtpClient - Nota - Define qual o host a ser usado para envio
                // de mensagens, no local de smtp.server.com use o nome do SEU servidor
                SmtpClient mSmtpClient = new SmtpClient(servidorSMTP);

                //Porta

                if (porta == 0)
                    porta = 587;

                mSmtpClient.Port = porta;

                // Seta as credenciais do e-mail (usuário e senha) para autenticar no servidor de envio
                mSmtpClient.Credentials = new System.Net.NetworkCredential(user, password);

                // Define se a conexão utiliza SSL
                mSmtpClient.EnableSsl = possuiSSL;

                // Envia o email
                mSmtpClient.Send(mMailMessage);

                // Email enviado com sucesso!
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarEmail");

                return false;
            }
        }

        public Boolean EnviarEmail(String from, String user, String password, String recepient, String bcc, String cc, String subject, String body, String servidorSMTP, List<Attachment> attachments = null, String signature = "", bool possuiSSL = false, string replyTo = "", int porta = 587, Repositorio.UnitOfWork unitOfWork = null, int codigoEmpresa = 0, bool nl2br = true, List<string> emails = null, bool gerarLog = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user) && unitOfWork != null)
                {
                    Repositorio.ConfiguracaoEmail repConfiguracaoEmail = new Repositorio.ConfiguracaoEmail(unitOfWork);
                    Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = repConfiguracaoEmail.BuscarConfiguracao();
                    if (configuracaoEmail != null)
                    {
                        from = configuracaoEmail.Email;
                        user = string.IsNullOrEmpty(configuracaoEmail.User) ? configuracaoEmail.Email : configuracaoEmail.User;
                        password = configuracaoEmail.Senha;
                        servidorSMTP = configuracaoEmail.Smtp;
                        porta = configuracaoEmail.PortaSmtp;
                        possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                    }
                }

                if (String.IsNullOrEmpty(from) || String.IsNullOrEmpty(user) || String.IsNullOrEmpty(password) || (String.IsNullOrEmpty(recepient) && (emails == null || emails?.Count == 0)) || String.IsNullOrEmpty(subject) || String.IsNullOrEmpty(body) || String.IsNullOrEmpty(servidorSMTP))
                    return false;

                MailMessage mMailMessage = new MailMessage();

                mMailMessage.From = new MailAddress(from);

                if (!string.IsNullOrWhiteSpace(recepient))
                {
                    if (recepient.Contains(";"))
                    {
                        string[] emailsRecepient = recepient.Split(';');
                        if (emailsRecepient != null)
                        {
                            foreach (var emailRecepient in emailsRecepient)
                            {
                                if (!string.IsNullOrWhiteSpace(emailRecepient))
                                    mMailMessage.To.Add(new MailAddress(emailRecepient.Replace(";", "")));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(recepient.Replace(";", "")))
                            mMailMessage.To.Add(new MailAddress(recepient.Replace(";", "")));
                    }
                    else if (!string.IsNullOrWhiteSpace(recepient))
                        mMailMessage.To.Add(new MailAddress(recepient));
                }


                if (emails != null && emails.Count > 0)
                {
                    foreach (string emailEnviar in emails)
                        mMailMessage.To.Add(new MailAddress(emailEnviar));
                }

                if (!string.IsNullOrWhiteSpace(replyTo))
                    mMailMessage.ReplyToList.Add(new MailAddress(replyTo));

                if (!string.IsNullOrWhiteSpace(bcc))
                    mMailMessage.Bcc.Add(new MailAddress(bcc));


                if (!string.IsNullOrWhiteSpace(cc))
                    mMailMessage.CC.Add(cc);


                if (attachments != null)
                    foreach (Attachment attach in attachments)
                        mMailMessage.Attachments.Add(attach);


                StringBuilder bodyEmail = new StringBuilder();
                if (nl2br)
                {
                    bodyEmail.Append(body.Replace(System.Environment.NewLine, "<br />"));
                }
                else
                {
                    bodyEmail.Append(body);
                }

                if (!String.IsNullOrWhiteSpace(signature))
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("--").Append("<br />");
                    bodyEmail.Append(signature.Replace(System.Environment.NewLine, "<br />"));
                }

                mMailMessage.Subject = subject;
                mMailMessage.Body = bodyEmail.ToString();
                mMailMessage.IsBodyHtml = true;
                mMailMessage.Priority = MailPriority.Normal;


                SmtpClient mSmtpClient = new SmtpClient(servidorSMTP);

                if (porta == 0)
                    porta = 587;

                mSmtpClient.Port = porta;

                mSmtpClient.Credentials = new System.Net.NetworkCredential(user, password);

                mSmtpClient.EnableSsl = possuiSSL;

                mSmtpClient.Send(mMailMessage);

                //Salva log de envio de email
                if (gerarLog)
                    LogEnvioEmail.SalvarLogEnvioEmail(from, recepient, !string.IsNullOrEmpty(bcc) ? new[] { bcc } : null, !string.IsNullOrEmpty(cc) ? new[] { cc } : null, subject, body, attachments, replyTo, codigoEmpresa, unitOfWork);

                // Email enviado com sucesso!
                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "EnviarEmail");

                return false;
            }
        }

        public static bool EnviarEmail(string from, string user, string password, string recepient, string[] bcc, string[] cc, string subject, string body, string servidorSMTP, out string erro, string displayEmail, List<Attachment> attachments = null, String signature = "", bool possuiSSL = false, string replyTo = "", int porta = 587, Repositorio.UnitOfWork unitOfWork = null, int codigoEmpresa = 0, bool enviarapenasdestinatariooculto = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user) && unitOfWork != null)
                {
                    Repositorio.ConfiguracaoEmail repConfiguracaoEmail = new Repositorio.ConfiguracaoEmail(unitOfWork);
                    Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = repConfiguracaoEmail.BuscarConfiguracao();
                    if (configuracaoEmail != null)
                    {
                        from = configuracaoEmail.Email;
                        user = string.IsNullOrEmpty(configuracaoEmail.User) ? configuracaoEmail.Email : configuracaoEmail.User;
                        password = configuracaoEmail.Senha;
                        servidorSMTP = configuracaoEmail.Smtp;
                        porta = configuracaoEmail.PortaSmtp;
                        possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                    }
                }

                //Verifica se os campos obrigatórios estão preenchidos
                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body) || string.IsNullOrEmpty(servidorSMTP))
                {
                    erro = "A configuração para envio dos e-mails está incorreta.";
                    return false;
                }

                // Cria uma instância do objeto MailMessage
                MailMessage mMailMessage = new MailMessage();

                if (string.IsNullOrWhiteSpace(from))
                {
                    erro = "Remetente não informado";
                    return false;
                }

                from = from?.Replace(";", "") ?? "";
                replyTo = replyTo?.Replace(";", "") ?? "";

                // Define o endereço do remetente
                if (!string.IsNullOrWhiteSpace(displayEmail))
                    mMailMessage.From = new MailAddress(from, displayEmail);
                else
                    mMailMessage.From = new MailAddress(from);


                // Define o endereço para resposta das mensagens
                if (!string.IsNullOrWhiteSpace(replyTo))
                    mMailMessage.ReplyToList.Add(new MailAddress(replyTo));

                //Quando for homologação, envia e-mails somente para os que estão configurado na empresa
                var enviaEmailEmpresaHomologacao = false;
                if (unitOfWork != null && codigoEmpresa > 0)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (empresa != null && empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
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
                        recepient = string.Empty;
                        bcc = null;
                        cc = emails.ToArray();

                        if (cc == null || cc.Count() == 0)
                        {
                            erro = "Nenhum e-mail informado no cadastro da empresa.";
                            return false;
                        }
                    }
                }

                // Define o destinario da mensagem
                if (!string.IsNullOrWhiteSpace(recepient))
                {
                    if (recepient.Contains(";"))
                    {
                        string[] emailsRecepient = recepient.Split(';');
                        if (emailsRecepient != null)
                        {
                            foreach (var emailRecepient in emailsRecepient)
                            {
                                if (!string.IsNullOrWhiteSpace(emailRecepient))
                                    mMailMessage.To.Add(new MailAddress(emailRecepient.Replace(";", "")));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(recepient.Replace(";", "")))
                            mMailMessage.To.Add(new MailAddress(recepient.Replace(";", "")));
                    }
                    else if (!string.IsNullOrWhiteSpace(recepient))
                        mMailMessage.To.Add(new MailAddress(recepient));
                }

                if (bcc != null && bcc.Count() > 0)
                {
                    foreach (string ebcc in bcc)
                    {
                        string stEbcc = ebcc.Replace(";", "");
                        if (!string.IsNullOrWhiteSpace(stEbcc))
                        {
                            if (string.IsNullOrWhiteSpace(recepient?.Replace(";", "")) && !enviarapenasdestinatariooculto)
                                mMailMessage.To.Add(new MailAddress(stEbcc.Trim()));
                            else
                                mMailMessage.Bcc.Add(new MailAddress(stEbcc.Trim()));
                        }
                    }
                }

                if (cc != null && cc.Count() > 0)
                {
                    foreach (string ecc in cc)
                    {
                        string stEcc = ecc.Replace(";", "");
                        if (!string.IsNullOrWhiteSpace(stEcc))
                            mMailMessage.CC.Add(new MailAddress(stEcc.Trim()));
                    }
                }

                if (mMailMessage.To.Count() <= 0 && mMailMessage.Bcc.Count() <= 0 && mMailMessage.CC.Count() <= 0)
                {
                    erro = "Nenhum e-mail valido foi informado para o envio.";
                    return false;
                }


                if (attachments != null)
                    // Adiciona os anexos
                    foreach (Attachment attach in attachments)
                        mMailMessage.Attachments.Add(attach);


                StringBuilder bodyEmail = new StringBuilder();
                bodyEmail.Append(body.Replace(System.Environment.NewLine, "<br />"));
                if (!string.IsNullOrWhiteSpace(signature))
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("--").Append("<br />");
                    bodyEmail.Append(signature.Replace(System.Environment.NewLine, "<br />"));
                }
                if (enviaEmailEmpresaHomologacao && cc != null && cc.Count() > 0)
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("E-mails: " + string.Join(", ", cc));
                }

                // Define o assunto
                mMailMessage.Subject = subject;
                // Define o corpo da mensagem
                mMailMessage.Body = bodyEmail.ToString();
                //' Define o formato do email como HTML
                mMailMessage.IsBodyHtml = true;
                // Define a prioridade da mensagem como normal
                mMailMessage.Priority = MailPriority.Normal;

                // Cria uma instância de SmtpClient - Nota - Define qual o host a ser usado para envio
                // de mensagens, no local de smtp.server.com use o nome do SEU servidor
                SmtpClient mSmtpClient = new SmtpClient(servidorSMTP);

                mSmtpClient.Port = porta <= 0 ? 587 : porta;

                // Seta as credenciais do e-mail (usuário e senha) para autenticar no servidor de envio                
                mSmtpClient.Credentials = new System.Net.NetworkCredential(user, password);

                // Define se a conexão utiliza SSL
                mSmtpClient.EnableSsl = possuiSSL;

                System.Net.SecurityProtocolType oldSecurityProtocol = System.Net.ServicePointManager.SecurityProtocol;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                // Envia o email
                mSmtpClient.Send(mMailMessage);
                System.Net.ServicePointManager.SecurityProtocol = oldSecurityProtocol;

                erro = string.Empty;

                //Salva log de envio de email
                LogEnvioEmail.SalvarLogEnvioEmail(from, recepient, bcc, cc, subject, body, attachments, replyTo, codigoEmpresa, unitOfWork);

                // Email enviado com sucesso!
                return true;
            }
            catch (Exception ex)
            {
                erro = "Falha ao enviar o e-mail para '" + recepient + "': " + ex.Message;

                Servicos.Log.TratarErro(new Exception("Falha ao enviar o e-mail para '" + recepient + "', '" + (bcc != null ? string.Join("', '", bcc) : "") + "'", ex), "EnviarEmail");

                return false;
            }
        }

        public static async Task<(bool Success, string Error)> EnviarEmailComApiAsync(string from, string user, string password, string recepient, string[] bcc, string[] cc, string subject, string body, string servidorSMTP, string displayEmail, List<Attachment> attachments = null, string signature = "", bool possuiSSL = false, string replyTo = "", int porta = 587, Repositorio.UnitOfWork unitOfWork = null, int codigoEmpresa = 0, bool enviarapenasdestinatariooculto = false)
        {
            try
            {
                bool IsApi = false;
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email != null && !string.IsNullOrWhiteSpace(email.Email) && !string.IsNullOrWhiteSpace(user) && email.Email.Equals(user, StringComparison.OrdinalIgnoreCase) && email.ApiEnvioEmail)
                {
                    IsApi = true;
                    from = email.Email;
                }
                if (string.IsNullOrWhiteSpace(user) && unitOfWork != null)
                {
                    Repositorio.ConfiguracaoEmail repConfiguracaoEmail = new Repositorio.ConfiguracaoEmail(unitOfWork);
                    Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = repConfiguracaoEmail.BuscarConfiguracao();
                    if (configuracaoEmail != null)
                    {
                        from = configuracaoEmail.Email;
                        user = string.IsNullOrEmpty(configuracaoEmail.User) ? configuracaoEmail.Email : configuracaoEmail.User;
                        password = configuracaoEmail.Senha;
                        servidorSMTP = configuracaoEmail.Smtp;
                        porta = configuracaoEmail.PortaSmtp;
                        possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                    }
                }


                //Verifica se os campos obrigatórios estão preenchidos
                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body) || string.IsNullOrEmpty(servidorSMTP))
                {
                    return (false, "A configuração para envio dos e-mails está incorreta.");
                }

                // Cria uma instância do objeto MailMessage
                MailMessage mMailMessage = new MailMessage();

                if (string.IsNullOrWhiteSpace(from))
                {
                    return (false, "Remetente não informado");

                }

                from = from?.Replace(";", "") ?? "";
                replyTo = replyTo?.Replace(";", "") ?? "";

                // Define o endereço do remetente
                if (!string.IsNullOrWhiteSpace(displayEmail))
                    mMailMessage.From = new MailAddress(from, displayEmail);
                else
                    mMailMessage.From = new MailAddress(from);


                // Define o endereço para resposta das mensagens
                if (!string.IsNullOrWhiteSpace(replyTo))
                    mMailMessage.ReplyToList.Add(new MailAddress(replyTo));

                //Quando for homologação, envia e-mails somente para os que estão configurado na empresa
                var enviaEmailEmpresaHomologacao = false;
                if (unitOfWork != null && codigoEmpresa > 0)
                {
                    Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                    if (empresa != null && empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
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
                        recepient = string.Empty;
                        bcc = null;
                        cc = emails.ToArray();

                        if (cc == null || cc.Count() == 0)
                        {
                            return (false, "Nenhum e-mail informado no cadastro da empresa.");
                        }
                    }
                }

                // Define o destinario da mensagem
                if (!string.IsNullOrWhiteSpace(recepient))
                {
                    if (recepient.Contains(";"))
                    {
                        string[] emailsRecepient = recepient.Split(';');
                        if (emailsRecepient != null)
                        {
                            foreach (var emailRecepient in emailsRecepient)
                            {
                                if (!string.IsNullOrWhiteSpace(emailRecepient))
                                    mMailMessage.To.Add(new MailAddress(emailRecepient.Replace(";", "")));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(recepient.Replace(";", "")))
                            mMailMessage.To.Add(new MailAddress(recepient.Replace(";", "")));
                    }
                    else if (!string.IsNullOrWhiteSpace(recepient))
                        mMailMessage.To.Add(new MailAddress(recepient));
                }

                if (bcc != null && bcc.Count() > 0)
                {
                    foreach (string ebcc in bcc)
                    {
                        string stEbcc = ebcc.Replace(";", "");
                        if (!string.IsNullOrWhiteSpace(stEbcc))
                        {
                            if (string.IsNullOrWhiteSpace(recepient?.Replace(";", "")) && !enviarapenasdestinatariooculto)
                                mMailMessage.To.Add(new MailAddress(stEbcc.Trim()));
                            else
                                mMailMessage.Bcc.Add(new MailAddress(stEbcc.Trim()));
                        }
                    }
                }

                if (cc != null && cc.Count() > 0)
                {
                    foreach (string ecc in cc)
                    {
                        string stEcc = ecc.Replace(";", "");
                        if (!string.IsNullOrWhiteSpace(stEcc))
                            mMailMessage.CC.Add(new MailAddress(stEcc.Trim()));
                    }
                }

                if (mMailMessage.To.Count() <= 0 && mMailMessage.Bcc.Count() <= 0 && mMailMessage.CC.Count() <= 0)
                {
                    return (false, "Nenhum e-mail valido foi informado para o envio.");
                }


                if (attachments != null)
                    // Adiciona os anexos
                    foreach (Attachment attach in attachments)
                        mMailMessage.Attachments.Add(attach);


                StringBuilder bodyEmail = new StringBuilder();
                bodyEmail.Append(body.Replace(System.Environment.NewLine, "<br />"));
                if (!string.IsNullOrWhiteSpace(signature))
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("--").Append("<br />");
                    bodyEmail.Append(signature.Replace(System.Environment.NewLine, "<br />"));
                }
                if (enviaEmailEmpresaHomologacao && cc != null && cc.Count() > 0)
                {
                    bodyEmail.Append("<br /><br />");
                    bodyEmail.Append("E-mails: " + string.Join(", ", cc));
                }

                // Define o assunto
                mMailMessage.Subject = subject;
                // Define o corpo da mensagem
                mMailMessage.Body = bodyEmail.ToString();
                //' Define o formato do email como HTML
                mMailMessage.IsBodyHtml = true;
                // Define a prioridade da mensagem como normal
                mMailMessage.Priority = MailPriority.Normal;

                // Cria uma instância de SmtpClient - Nota - Define qual o host a ser usado para envio
                // de mensagens, no local de smtp.server.com use o nome do SEU servidor
                SmtpClient mSmtpClient = new SmtpClient(servidorSMTP);

                mSmtpClient.Port = porta <= 0 ? 587 : porta;

                // Seta as credenciais do e-mail (usuário e senha) para autenticar no servidor de envio                
                mSmtpClient.Credentials = new System.Net.NetworkCredential(user, password);

                // Define se a conexão utiliza SSL
                mSmtpClient.EnableSsl = possuiSSL;

                System.Net.SecurityProtocolType oldSecurityProtocol = System.Net.ServicePointManager.SecurityProtocol;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                // Envia o email
                if (IsApi)
                {
                    var gmail = new Servicos.Embarcador.Integracao.Gmail.IntegracaoGmail(unitOfWork);
                    // Envia via Gmail API
                    bool sucesso = await gmail.EnviarEmailViaGmailApiAsync(email.ClientId, email.ClientSecret, email.caminhoTokenResposta, email.Codigo, from, mMailMessage.To.ToString(), cc, bcc, subject, mMailMessage.Body, attachments, email.UrlEnvio, signature);
                }

                else
                {
                    mSmtpClient.Send(mMailMessage);
                    System.Net.ServicePointManager.SecurityProtocol = oldSecurityProtocol;
                }
                LogEnvioEmail.SalvarLogEnvioEmail(from, recepient, bcc, cc, subject, body, attachments, replyTo, codigoEmpresa, unitOfWork);
                return (true, "");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(new Exception("Falha ao enviar o e-mail para '" + recepient + "', '" + (bcc != null ? string.Join("', '", bcc) : "") + "'", ex));
                return (false, "Falha ao enviar o e-mail para '" + recepient + "': " + ex.Message);
            }
        }

        public static bool EnviarEmailAutenticado(string para, string assunto, string corpo, Repositorio.UnitOfWork unitOfWork, out string msgErro, string displayEmail, List<Attachment> attachments = null, string[] bcc = null, string[] cc = null, string signature = "", string replyTo = "")
        {
            msgErro = "";
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmailDocTransporte = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            if (configuracaoEmailDocTransporte == null)
            {
                msgErro = "Nenhuma configuração de e-mail padrão.";
                return false;
            }

            return Servicos.Email.EnviarEmail(configuracaoEmailDocTransporte.Email, !string.IsNullOrWhiteSpace(configuracaoEmailDocTransporte.Usuario) ? configuracaoEmailDocTransporte.Usuario : configuracaoEmailDocTransporte.Email, configuracaoEmailDocTransporte.Senha, para, bcc, cc, assunto, corpo, configuracaoEmailDocTransporte.Smtp, out msgErro, displayEmail, attachments, signature, configuracaoEmailDocTransporte.RequerAutenticacaoSmtp, replyTo, configuracaoEmailDocTransporte.PortaSmtp, unitOfWork);
        }

        public static void EnviarMensagem(Dominio.ObjetosDeValor.Email.Mensagem mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Email serEmail = new Servicos.Email(unitOfWork);
            serEmail.EnviarEmail(string.Empty, string.Empty, string.Empty, "", "", "", mensagem.Assunto, mensagem.Corpo, string.Empty, mensagem.Anexos, "", true, string.Empty, 0, unitOfWork, 0, false, mensagem.Destinatarios);
        }

        public static void EnviarMensagensAsync(List<Dominio.ObjetosDeValor.Email.Mensagem> mensagens, Repositorio.UnitOfWork unitOfWork)
        {
            int total = mensagens?.Count() ?? 0;
            if (total > 0)
            {
                Repositorio.ConfiguracaoEmail repConfiguracaoEmail = new Repositorio.ConfiguracaoEmail(unitOfWork);
                Dominio.Entidades.ConfiguracaoEmail configuracaoEmail = repConfiguracaoEmail.BuscarConfiguracao();
                if (configuracaoEmail != null)
                {
                    for (int i = 0; i < total; i++)
                    {
                        Dominio.ObjetosDeValor.Email.Mensagem mensagem = mensagens[i];
                        Task task = new Task(() =>
                        {
                            try
                            {
                                Servicos.Email serEmail = new Servicos.Email(unitOfWork);
                                serEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, "", "", "", mensagem.Assunto, mensagem.Corpo, configuracaoEmail.Smtp, null, "", configuracaoEmail.RequerAutenticacaoSmtp, string.Empty, configuracaoEmail.PortaSmtp, unitOfWork, 0, false, mensagem.Destinatarios);
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex);
                            }
                        });
                        task.Start();
                    }
                }
            }

        }

        public static string TemplateCorpoEmail(string titulo, List<KeyValuePair<string, string>> mensagemCorpoLinhas, string mensagemCorpoSuperior = null, string mensagemCorpoInferior = null, string mensagemCorpoRodape = null, string mensagemCorpoObservacao = null)
        {
            int totalLinhas = mensagemCorpoLinhas?.Count() ?? 0;
            string mensagemLinhas = string.Empty;
            if (totalLinhas > 0)
            {
                mensagemLinhas += $@"<p style=""margin:10px 0; padding:10px; border: 1px solid #DDD; border-radius: 4px;line-height: 150%; "">";
                for (int i = 0; i < totalLinhas; i++)
                {
                    mensagemLinhas += $@"<label style=""display: inline-block; width: 120px; font-style: italic;"">{mensagemCorpoLinhas[i].Key}:</label> {mensagemCorpoLinhas[i].Value}<br/>";
                }
                mensagemLinhas += $@"</p>";
            }

            string mensagem = $@"<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"">
    <tr>
        <td style=""padding:20px; background-color:#EEE;""> 
            <table border=""0"" cellspacing=""0"" cellpadding=""0"" style=""width:600px"" align=""center"">
                <tr>
                    <td>
                        <div style=""border-bottom: 1px solid #EEE;padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 36px; font-weight: bold;"">{titulo}</div>
                        <div style=""border-bottom: 1px solid #EEE;padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%; "">
                            <p style=""margin:10px 0"">{mensagemCorpoSuperior}</p>
                            {mensagemLinhas}
                            <p style=""margin:10px 0"">{mensagemCorpoInferior}</p>
                        </div>
                        <div style=""padding:40px; background-color:#FFF; font-family: Arial, Helvetica, sans-serif; font-size: 14px;line-height: 150%;"">
                            <strong>{mensagemCorpoRodape}</strong><br/>
                            <span style=""font-style:italic"">Multisoftware</span>
                        </div>
                        <div style=""padding:20px; font-family: Arial, Helvetica, sans-serif; font-size: 14px; text-align: center; font-size: 10px; color:#CCC"">{mensagemCorpoObservacao}</div>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>";
            return mensagem;
        }

        #region MailKit

        public void ReceberEmail(Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, String user, String password, String servidorPOP3, bool possuiSSL = false, int porta = 995, Repositorio.UnitOfWork unitOfWork = null, bool TesteLeitura = false)
        {
            //if (configEmail.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Gmail)
            //    ReceberEmailGmailAsync(configEmail, tipoServicoMultisoftware, user, password, servidorPOP3, possuiSSL, porta, unitOfWork);
            //else
            //{

            if (configEmail.TipoConexaoEmail == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConexaoEmail.Exchange)
            {
                ReceberEmailExchange(configEmail, tipoServicoMultisoftware, unitOfWork, TesteLeitura);
            }
            else
            {
                Repositorio.Embarcador.Email.EmailCaixaEntrada repCaixaEntrada = new Repositorio.Embarcador.Email.EmailCaixaEntrada(unitOfWork);
                if (porta == 0)
                    porta = 995;

                using (var client = new MailKit.Net.Pop3.Pop3Client())
                {
                    try
                    {
                        client.SslProtocols = System.Security.Authentication.SslProtocols.Ssl3 |
                                              System.Security.Authentication.SslProtocols.Tls |
                                              System.Security.Authentication.SslProtocols.Tls11 |
                                              System.Security.Authentication.SslProtocols.Tls12;

                        if (user.Contains("@multicte.com.br"))
                            client.Connect(servidorPOP3, porta, SecureSocketOptions.None);
                        else
                            client.Connect(servidorPOP3, porta, possuiSSL);
                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                        client.Authenticate(user, password);

                        var qtde = client.Count;
                        for (int i = 0; i < qtde; i++)
                        {
                            try
                            {
                                var email = client.GetMessage(i);

                                ProcessarEmailRecebido(configEmail, tipoServicoMultisoftware, email, unitOfWork);

                                // marca a mensagem para exclusão
                                client.DeleteMessage(i);

                                email = null;
                            }
                            catch (Exception ex)
                            {
                                Servicos.Log.TratarErro(ex, "ReceberEmail");
                            }
                        }

                        client.Disconnect(true);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro("Não pode se conectar ao POP3 para leitura: (" + ex.Message + ")", "ReceberEmail");
                    }
                    finally { System.GC.Collect(); }
                }
            }
        }

        public string textarConexaoPOP3(String user, String password, String servidorPOP3, bool possuiSSL = false, int porta = 995)
        {
            using (var client = new MailKit.Net.Pop3.Pop3Client())
            {
                try
                {
                    if (porta == 0)
                        porta = 995;

                    client.SslProtocols = System.Security.Authentication.SslProtocols.Ssl3 |
                                          System.Security.Authentication.SslProtocols.Tls |
                                          System.Security.Authentication.SslProtocols.Tls11 |
                                          System.Security.Authentication.SslProtocols.Tls12;

                    if (user.Contains("@multicte.com.br"))
                        client.Connect(servidorPOP3, porta, SecureSocketOptions.None);
                    else
                        client.Connect(servidorPOP3, porta, possuiSSL);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    client.Authenticate(user, password);

                    client.Disconnect(true);

                    return "Sucesso";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        #endregion

        public string TestarConexaoSMTP(String from, String user, String password, String servidorSMTP, String recepient, TipoServicoMultisoftware tipoServicoMultisoftware, bool possuiSSL = false, int porta = 587, String razaoEmpresa = "")
        {
            try
            {
                //Verifica se os campos obrigatórios estão preenchidos
                if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(servidorSMTP))
                    return "Informe os campos obrigatorios para testar a conexão";

                // Cria uma instância do objeto MailMessage
                MailMessage mMailMessage = new MailMessage();

                // Define o endereço do remetente
                mMailMessage.From = new MailAddress(from);
                // Define o assunto
                mMailMessage.Subject = "Teste Configuracao Email";
                // Define o corpo da mensagem
                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiNFe)
                    mMailMessage.Body = "Teste de Configuracao de Email do Multi NF-e da Empresa: " + razaoEmpresa + "<br/>" + "<br/>" + "<br/>" + "E-mail enviado automaticamente, favor ignorar.";
                else
                    mMailMessage.Body = "Teste Configuracao Email";

                //' Define o formato do email como HTML
                mMailMessage.IsBodyHtml = true;
                // Define a prioridade da mensagem como normal
                mMailMessage.Priority = MailPriority.Normal;

                if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiNFe)
                {
                    if (!string.IsNullOrWhiteSpace(recepient))
                        mMailMessage.To.Add(new MailAddress(recepient));
                    else
                        mMailMessage.To.Add(new MailAddress("suporte@commerce.inf.br"));
                }
                else
                    mMailMessage.To.Add(new MailAddress("rodrigo@multisoftware.com.br"));

                // Cria uma instância de SmtpClient - Nota - Define qual o host a ser usado para envio
                // de mensagens, no local de smtp.server.com use o nome do SEU servidor
                SmtpClient mSmtpClient = new SmtpClient(servidorSMTP);

                //Porta

                if (porta == 0)
                    porta = 587;
                mSmtpClient.Port = porta;

                // Seta as credenciais do e-mail (usuário e senha) para autenticar no servidor de envio
                mSmtpClient.Credentials = new System.Net.NetworkCredential(user, password);

                // Define se a conexão utiliza SSL
                mSmtpClient.EnableSsl = possuiSSL;


                // Envia o email
                mSmtpClient.Send(mMailMessage);

                // Email enviado com sucesso!
                return "Sucesso";
            }
            catch (SmtpException spex)
            {
                if (spex.InnerException == null)
                    return spex.Message;
                else
                    return spex.InnerException.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        #endregion

        #region Métodos Privados

        private async Task ReceberEmailGmailAsync(Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, String user, String password, String servidorPOP3, bool possuiSSL = false, int porta = 995, Repositorio.UnitOfWork unitOfWork = null)
        {
            if (porta == 0)
                porta = 993;

            string GMailAccount = configEmail.Email;//"username@gmail.com";

            var clientSecrets = new ClientSecrets
            {
                ClientId = configEmail.ClientId,//"XXX.apps.googleusercontent.com",
                ClientSecret = configEmail.ClientSecret//"XXX"
            };

            var codeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                DataStore = new FileDataStore("CredentialCacheFolder", false),
                Scopes = new[] { "https://mail.google.com/" },
                ClientSecrets = clientSecrets
            });

            // Note: For a web app, you'll want to use AuthorizationCodeWebApp instead.
            var codeReceiver = new LocalServerCodeReceiver();
            var authCode = new AuthorizationCodeInstalledApp(codeFlow, codeReceiver);

            //var credential = await authCode.AuthorizeAsync(GMailAccount, CancellationToken.None);
            var credential = authCode.AuthorizeAsync(GMailAccount, CancellationToken.None);

            if (credential.Result.Token.IsExpired(SystemClock.Default))
                credential.Result.RefreshTokenAsync(CancellationToken.None);

            var oauth2 = new SaslMechanismOAuth2(credential.Result.UserId, credential.Result.Token.AccessToken);

            //GoogleAuthProvider _auth = null;            

            //GoogleCredential googleCred = await _auth.GetCredentialAsync();
            //string token = await googleCred.UnderlyingCredential.GetAccessTokenForRequestAsync();

            //var oauth2 = new SaslMechanismOAuth2(GMailAccount, token);

            using (var client = new ImapClient())
            {
                try
                {
                    client.ConnectAsync("imap.gmail.com", porta, SecureSocketOptions.SslOnConnect);
                    client.AuthenticateAsync(oauth2);

                    var qtde = client.Inbox.Count;

                    for (int i = 0; i < qtde; i++)
                    {
                        try
                        {
                            var email = client.Inbox.GetMessage(i);

                            ProcessarEmailRecebido(configEmail, tipoServicoMultisoftware, email, unitOfWork);

                            // marca a mensagem para exclusão
                            //client.Inbox.AddFlags(i, MailKit.MessageFlags.Deleted, true);

                            email = null;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "ReceberEmail");
                        }
                    }


                    client.DisconnectAsync(true);

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro("Não pode se conectar ao POP3 para leitura: (" + ex.Message + ")", "ReceberEmail");
                }
                finally { System.GC.Collect(); }
            }
        }

        private void ReceberEmailExchange(Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, bool TesteLeitura)
        {
            Servicos.TokenSettings Tokens = null;
            try
            {
                Tokens = new Servicos.TokenSettings(configEmail, new TokenSettingsService());
                if (Tokens == null || Tokens.msgErro != "")
                    throw new Dominio.Excecoes.Embarcador.ServicoException(Tokens.msgErro);

                using (var client = new ImapClient())
                {
                    client.Connect(configEmail.Smtp, configEmail.PortaSmtp, true);
                    var oauth2 = new SaslMechanismOAuth2(configEmail.Email, Tokens.GetAccessToken());
                    client.Authenticate(oauth2);
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);
                    for (int i = 0; i < inbox.Count; i++)
                    {
                        try
                        {
                            var message = inbox.GetMessage(i);
                            if (!TesteLeitura)
                            {// NÃO PROCESSA O EMAIL POIS O REGISTRO DE CONFIGURAÇÃO PODE NÃO EXISTIR CASO VENHA APENAS DA TELA 
                                ProcessarEmailRecebido(configEmail, tipoServicoMultisoftware, message, unitOfWork);
                                inbox.AddFlags(i, MessageFlags.Deleted, true);
                            }
                            message = null;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex, "ReceberEmail");
                            throw;
                        }
                    }
                    if (!TesteLeitura)
                        inbox.Expunge(); // Expurga os emails marcados para exclusão
                    inbox.Close();
                    client.Disconnect(true);
                }
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException ex)
            {
                Servicos.Log.TratarErro("Erro ao tentar ler email(" + configEmail.Email + ") Exchange: (" + ex.Message + ")", "ReceberEmail");
                throw new Dominio.Excecoes.Embarcador.ServicoException("Erro ao tentar ler email(" + configEmail.Email + ") Exchange.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Erro ao tentar ler email(" + configEmail.Email + ") Exchange: (" + ex.Message + ")", "ReceberEmail");
                throw new Exception("Erro ao tentar ler email(" + configEmail.Email + ") Exchange.");
            }

        }

        private void ProcessarEmailRecebido(Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configEmail, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, MimeKit.MimeMessage email, Repositorio.UnitOfWork unitOfWork = null)
        {
            Repositorio.Embarcador.Email.EmailCaixaEntrada repCaixaEntrada = new Repositorio.Embarcador.Email.EmailCaixaEntrada(unitOfWork);

            string remetente = "";
            try
            {
                //remetente = ((MimeKit.MailboxAddress)(MimeKit.GroupAddress)email.From[0]).Members[0]).Address; se necessário testar assim para os GMAILS.
                remetente = ((MimeKit.MailboxAddress)email.From[0]).Address;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex, "RemetenteEmail");
                remetente = string.IsNullOrWhiteSpace(email.Subject) ? "" : Utilidades.String.Left(email.Subject, 149);
            }

            Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada emailCaixaEntrada = new Dominio.Entidades.Embarcador.Email.EmailCaixaEntrada
            {
                ConfigEmail = configEmail,
                Remetente = remetente,
                Assunto = remetente,
                DataRecebimento = DateTime.Now,
                TipoServico = tipoServicoMultisoftware
            };

            List<MimeKit.MimeEntity> anexos = email.Attachments.ToList();

            bool body = false;
            if (anexos.Count <= 0)
            {
                anexos = email.BodyParts.ToList();
                body = true;
            }

            foreach (var attachment in anexos)
            {
                if (emailCaixaEntrada.Anexos == null) emailCaixaEntrada.Anexos = new List<Dominio.Entidades.Embarcador.Email.EmailAnexos>();

                string fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name ?? "";
                if (string.IsNullOrEmpty(fileName))
                {
                    if (!body)
                        fileName = "ANEXO SEM NOME";
                    else
                        continue;
                }

                Dominio.Entidades.Embarcador.Email.EmailAnexos anexo = new Dominio.Entidades.Embarcador.Email.EmailAnexos
                {
                    NomeArquivo = fileName,
                    GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "")
                };

                string extensao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();

                if (extensao == ".rar" || extensao == ".iso" || extensao == ".gzip" || extensao == ".tar" || extensao == ".7zip" || extensao == ".zip")
                    anexo.ArquivoZipado = true;

                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "Entrada");

                string fileLocation = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidNomeArquivo + extensao);

                // also literally copied and pasted from the FAQ:
                using (var stream = Utilidades.IO.FileStorageService.Storage.Create(fileLocation))
                {
                    if (attachment is MimeKit.MessagePart)
                    {
                        var rfc822 = (MimeKit.MessagePart)attachment;
                        rfc822.Message.WriteTo(stream);
                    }
                    else
                    {
                        var part = (MimeKit.MimePart)attachment;
                        part.Content.DecodeTo(stream);
                    }
                }

                if (anexo.ArquivoZipado)
                {
                    //Vamos extrair o arquivo e gerar um anexo para cada arquivo..
                    string pathCompactados = Utilidades.IO.FileStorageService.Storage.Combine(caminho, "COMPACTADOS");
                    if (!System.IO.Directory.Exists(pathCompactados))
                        System.IO.Directory.CreateDirectory(pathCompactados);

                    // Problema ao descompactar emails .rar
                    try
                    {
                        ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                        zip.ExtractZip(fileLocation, pathCompactados, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, "", "", false);
                    }
                    catch
                    {
                        //Caso der problema ao tentar descompactar, vamos ver se o arquivo compactado descompacta usando a compactação .rar
                        try
                        {
                            NUnrar.Archive.RarArchive.WriteToDirectory(fileLocation, pathCompactados);
                        }
                        catch (Exception exRar)
                        {
                            Servicos.Log.TratarErro(exRar, "ReceberEmail");
                        }
                    }

                    var files = new System.IO.DirectoryInfo(pathCompactados).GetFiles();
                    for (int f = 0; f < files.Length; f++)
                    {
                        string arquivo = files[f].Name;

                        Dominio.Entidades.Embarcador.Email.EmailAnexos anexoInZip = new Dominio.Entidades.Embarcador.Email.EmailAnexos
                        {
                            NomeArquivo = arquivo,
                            GuidNomeArquivo = Guid.NewGuid().ToString().Replace("-", "")
                        };

                        string extensaoInZip = System.IO.Path.GetExtension(anexoInZip.NomeArquivo).ToLower();

                        if (extensaoInZip == ".rar" || extensaoInZip == ".iso" || extensaoInZip == ".gzip" ||
                            extensaoInZip == ".tar" || extensaoInZip == ".7zip" || extensaoInZip == ".zip")
                            anexoInZip.ArquivoZipado = true;

                        string mover = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexoInZip.GuidNomeArquivo + extensaoInZip);

                        Utilidades.IO.FileStorageService.Storage.Move(files[f].FullName, mover);

                        emailCaixaEntrada.Anexos.Add(anexoInZip);
                    }

                    //Apos salvar os arquivos que estavam compactados, vamos deletar o principal .Zip
                    try
                    {
                        Utilidades.IO.FileStorageService.Storage.Delete(fileLocation);
                    }
                    catch (Exception ex) 
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao deletar arquivo ZIP de email - continuando processamento: {ex.ToString()}", "CatchNoAction");
                    } // Vai pra cova.. senão conseguir deletar o zip..
                }
                else
                    emailCaixaEntrada.Anexos.Add(anexo);
            }

            repCaixaEntrada.Inserir(emailCaixaEntrada);
        }

        private static async Task<SaslMechanismOAuth2> getAccessToken(string[] scopes, string username, string password, string appID, string appSecret, string tenantID)
        {
            if (scopes == null || scopes.Length == 0) throw new ArgumentException("At least one scope is required", nameof(scopes));

            var scopesStr = String.Join(" ", scopes.Select(x => x?.Trim()).Where(x => !String.IsNullOrEmpty(x)));
            var content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("client_id", appID),
                new KeyValuePair<string, string>("client_secret", appSecret),
                new KeyValuePair<string, string>("scope", scopesStr),
            });
            var client = HttpClientFactoryWrapper.GetClient(nameof(Email));
            var response = await client.PostAsync($"https://login.microsoftonline.com/{tenantID}/oauth2/v2.0/token", content).ConfigureAwait(continueOnCapturedContext: false);
            var responseString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseString);
            var token = json["access_token"];
            return token != null
                ? new SaslMechanismOAuth2(username, token.ToString())
                : null;
        }

        #endregion
    }
}
