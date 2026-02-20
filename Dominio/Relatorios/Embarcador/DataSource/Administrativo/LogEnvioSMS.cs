using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Administrativo
{
    public class LogEnvioSMS
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string Pessoa { get; set; }
        public int Nota { get; set; }
        public string Celular { get; set; }
        public string Link { get; set; }
        public string Status { get; set; }
        public string Mensagem { get; set; }

        public string DataFormatada
        {
            get { return Data != DateTime.MinValue ? Data.ToString("dd/MM/yyyy HH:mm") : ""; }
        }
    }
}
