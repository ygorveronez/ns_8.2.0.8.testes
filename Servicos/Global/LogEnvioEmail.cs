using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Servicos.Global
{
    public class LogEnvioEmail : ServicoBase
    {
        public LogEnvioEmail() : base() { }

        public static void SalvarLogEnvioEmail(string from, string recepient, string[] bcc, string[] cc, string subject, string body, List<Attachment> attachments = null, string replyTo = "", int codigoEmpresa = 0, Repositorio.UnitOfWork unitOfWork = null)
        {
            try
            {
                if (unitOfWork == null)
                    return;

                Repositorio.Embarcador.Email.LogEnvioEmail repLogEnvioEmail = new Repositorio.Embarcador.Email.LogEnvioEmail(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                Dominio.Entidades.Embarcador.Email.LogEnvioEmail logEnvioEmail = new Dominio.Entidades.Embarcador.Email.LogEnvioEmail();

                logEnvioEmail.Data = DateTime.Now;
                logEnvioEmail.EmailRemetente = from;
                logEnvioEmail.EmailDestinatario = recepient;
                if (!string.IsNullOrEmpty(replyTo))
                    logEnvioEmail.EmailResposta = replyTo;
                if (cc != null)
                    logEnvioEmail.EmailCopia = String.Join("; ", cc);
                if (bcc != null)
                    logEnvioEmail.EmailCopiaOculta = String.Join("; ", bcc);
                if (attachments != null)
                {
                    List<string> listAttachment = new List<string>();
                    for (int i = 0; i < attachments.Count; i++)
                        listAttachment.Add(attachments[i].Name);
                    logEnvioEmail.DescricaoAnexo = String.Join("; ", listAttachment);
                }
                if (!string.IsNullOrEmpty(subject))
                    logEnvioEmail.Assunto = subject;
                if (!string.IsNullOrEmpty(body))
                {
                    string bodyFormated = "";
                    Regex regex = new Regex(@"(<br />|<br/>|</ br>|</br>)");
                    bodyFormated = regex.Replace(body, "\r\n");
                    bodyFormated = Regex.Replace(bodyFormated, "<.*?>", String.Empty);

                    logEnvioEmail.Mensagem = bodyFormated;
                }
                if (codigoEmpresa > 0)
                    logEnvioEmail.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                repLogEnvioEmail.Inserir(logEnvioEmail);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "LogEnvioEmail");
            }
        }
    }
}
