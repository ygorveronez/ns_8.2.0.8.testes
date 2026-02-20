using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.Rastreamento
{

    public class retornoPosicoes
    {
        public List<Posicoes> body { get; set; }
        public Headers headers { get; set; }
        public string identificador { get; set; }
    }

    public class Posicoes
    {
        public Int64? numero { get; set; }
        public int? numeroSM { get; set; }
        public string numeroTerminalEquipamento { get; set; }
        public string nomeTecnologia { get; set; }
        public string placaCavalo { get; set; }
        public DateTime? dataLocalizacao { get; set; }
        public string latitudeLocalizacao { get; set; }
        public string longitudeLocalizacao { get; set; }
        public int? velocidadeVeiculo { get; set; }
        public DateTime dataEnvio { get; set; }
        public decimal? temperatura { get; set; }

    }

    public class Headers
    {
        [JsonProperty("Keep-Alive")]
        public string KeepAlive { get; set; }
        public int responseStatusCode { get; set; }

        [JsonProperty("Access-Control-Allow-Origin")]
        public string AccessControlAllowOrigin { get; set; }
        public string NEXT_STEPS_SIZE { get; set; }

        [JsonProperty("Access-Control-Allow-Credentials")]
        public string AccessControlAllowCredentials { get; set; }

        [JsonProperty("Access-Control-Allow-Methods")]
        public string AccessControlAllowMethods { get; set; }

        [JsonProperty("Access-Control-Max-Age")]
        public string AccessControlMaxAge { get; set; }

        [JsonProperty("Access-Control-Allow-Headers")]
        public string AccessControlAllowHeaders { get; set; }
    }

    public class Errormsg
    {
        public string id { get; set; }
        public string msg { get; set; }
        public string reason { get; set; }
    }

}
