using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.YMS
{
    public class Retorno
    {
        public string NumeroAgendamento { get; set; }
        [JsonProperty("Notifications")]
        public List<Notificacao> Notificacoes { get; set; }

        [JsonProperty("hashId")]
        public string HashId { get; set; }

        public class Notificacao
        {
            [JsonProperty("Title")]
            public string Titulo { get; set; }
            [JsonProperty("Message")]
            public string Messagem { get; set; }
            [JsonProperty("Level")]
            public string Nivel { get; set; }
        }
    }
}
