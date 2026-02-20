using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost
{
    public class ResponseEventoRastreamento
    {
        public string status { get; set; }
        public List<Mensagem> messages { get; set; }
        public string time { get; set; }
        public int client_id { get; set; }
        public int logistics_provider { get; set; }
        public string logistics_provider_name { get; set; }
        public string timezone { get; set; }
        public string locale { get; set; }
        public string hash { get; set; }
    }
}
