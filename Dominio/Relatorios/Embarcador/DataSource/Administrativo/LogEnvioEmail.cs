using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Administrativo
{
    public class LogEnvioEmail
    {
        public DateTime Data { get; set; }
        public string EmailRemetente { get; set; }
        public string EmailDestinatario { get; set; }
        public string EmailResposta { get; set; }
        public string EmailCopia { get; set; }
        public string EmailCopiaOculta { get; set; }
        public string DescricaoAnexo { get; set; }
        public string Assunto { get; set; }
        public string Mensagem { get; set; }

    }
}
