using System;

namespace Servicos
{
    public class LogEnvioSMS : ServicoBase
    {
        public LogEnvioSMS() : base() { }

        public static void SalvarLogEnvioSMS(string link, bool sucesso, string message, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Repositorio.Embarcador.NotaFiscal.LogEnvioSMS repLogEnvioSMS = new Repositorio.Embarcador.NotaFiscal.LogEnvioSMS(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.LogEnvioSMS logEnvioSMS = new Dominio.Entidades.Embarcador.NotaFiscal.LogEnvioSMS();

                logEnvioSMS.Data = DateTime.Now;
                logEnvioSMS.Link = link;
                logEnvioSMS.StatusEnvio = sucesso;
                logEnvioSMS.MensagemEnvio = message;

                logEnvioSMS.NotaFiscal = notaFiscal;
                logEnvioSMS.Pessoa = notaFiscal.Cliente;
                logEnvioSMS.Celular = notaFiscal.Cliente.Celular;
                logEnvioSMS.Empresa = notaFiscal.Empresa;

                repLogEnvioSMS.Inserir(logEnvioSMS);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
            }
        }
    }
}
