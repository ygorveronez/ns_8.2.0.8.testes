using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Transportadores
{
    public class Mercante
    {

        public static void NotificarDespachanteMercante(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            Repositorio.Embarcador.Documentos.ArquivoMercante repArquivoMercante = new Repositorio.Embarcador.Documentos.ArquivoMercante(unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(0);
            IList<Dominio.ObjetosDeValor.Embarcador.Documentos.ConsultaMercante> consultaMercante = repArquivoMercante.BuscarCNPJDespachanteEnvioEmail();

            if (consultaMercante == null || consultaMercante.Count == 0)
                return;

            foreach (var consulta in consultaMercante)
            {
                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.Mercante> mercanteEnvio = repArquivoMercante.BuscarDadosMercanteDespachanteEnvioEmail(consulta.CNPJDespachante, consulta.CodigoViagem, consulta.CodigoPorto, consulta.EmailDespachante);
                if (mercanteEnvio == null || mercanteEnvio.Count == 0)
                    continue;

                try
                {
                    string assunto = "Liberação Mercante Aliança - " + mercanteEnvio.FirstOrDefault().Viagem + " – " + mercanteEnvio.FirstOrDefault().PortoDestino;
                    string corpoEmail = "Liberação Mercante Aliança: <br/>";
                    corpoEmail += "Viagem: " + (mercanteEnvio.FirstOrDefault().Viagem) + " <br/>";
                    corpoEmail += "Porto: " + (mercanteEnvio.FirstOrDefault().PortoDestino) + " <br/><br/><br/>";

                    MemoryStream arquivoINPUT = new MemoryStream();
                    StreamWriter x = new StreamWriter(arquivoINPUT, Encoding.UTF8);
                    x.WriteLine($"Tomador;CNPJ Tomador;Número do Container;Booking;Número de Controle;Número da chave do SVM/AAK;Número do CE;Número do Manifesto;Viagem/Navio/Direção;Porto de origem;Porto de Destino;");
                    //string aspas = gerarFormatoTXT ? "\u0022" : string.Empty;
                    string aspas = "'";

                    foreach (var dadosMercante in mercanteEnvio)
                    {
                        x.WriteLine(aspas + dadosMercante.Tomador + aspas + ";" +//Tomador
                            aspas + dadosMercante.CNPJTomador + aspas + ";" +//CNPJ Tomador
                            aspas + dadosMercante.Container + aspas + ";" +//Número do Container
                            aspas + dadosMercante.Booking + aspas + ";" +//Booking
                            aspas + dadosMercante.NumeroControle + aspas + ";" +//Número de Controle
                            aspas + dadosMercante.ChaveSVMAAK + aspas + ";" +//Número da chave do SVM/AAK
                            aspas + dadosMercante.NumeroCE + aspas + ";" +//Número do CE
                            aspas + dadosMercante.NumeroManifesto + aspas + ";" +//Número do Manifesto
                            aspas + dadosMercante.Viagem + aspas + ";" +//Viagem/Navio/Direção
                            aspas + dadosMercante.PortoOrigem + aspas + ";" +//Porto de origem
                            aspas + dadosMercante.PortoDestino + aspas + ";"//Porto de Destino
                        );
                    }
                    x.Flush();

                    List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                    Stream stream = new MemoryStream(arquivoINPUT.ToArray());
                    attachments.Add(new System.Net.Mail.Attachment(stream, assunto + ".csv"));

                    List<string> emailsEnvio = new List<string>();
                    emailsEnvio.AddRange(!string.IsNullOrWhiteSpace(consulta.EmailDespachante) ? consulta.EmailDespachante.Split(';').ToList() : mercanteEnvio.FirstOrDefault().EmailDespachante?.Split(';').ToList());
                    if (emailsEnvio != null && emailsEnvio.Count > 0 && email != null)
                        Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsEnvio.ToArray(), null, assunto, corpoEmail, email.Smtp, out string mensagemErro, email.DisplayEmail, attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex, "NotificarDespachanteMercante");
                }

            }

        }
    }
}
