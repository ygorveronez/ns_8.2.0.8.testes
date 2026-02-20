using System.Collections.Generic;
using System.Net.Mail;

namespace Dominio.ObjetosDeValor.Email
{
    public class Mensagem
    {
        public List<string> Destinatarios { get; set; }
        public List<string> Copias { get; set; }
        public List<string> CopiasOcultas { get; set; }
        public List<string> ResponderPara { get; set; }
        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public List<Attachment> Anexos { get; set; }
    }
}
