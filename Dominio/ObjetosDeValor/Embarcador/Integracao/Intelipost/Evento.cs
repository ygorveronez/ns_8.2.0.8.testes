using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost
{
    public class Evento
    {
        public string event_date { get; set; }
        public string original_code { get; set; }
        public string original_message { get; set; }

        public List<EventoAnexo> attachments { get; set; }
    }
}
