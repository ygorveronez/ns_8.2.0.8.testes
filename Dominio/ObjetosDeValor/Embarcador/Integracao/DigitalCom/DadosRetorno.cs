using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.DigitalCom
{
    public class DadosRetorno
    {
        [JsonProperty("idDocumentoTransporte")]
        public long DocumentoTransporte { get; set; }

        [JsonProperty("cnpjMeioPagamento")]
        public string CnpjMeioPagamento { get; set; }

        [JsonProperty("flagpedagio")]
        public string FlagPedagio { get; set; }

        [JsonProperty("erro")]
        public string Erro { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("viagem")]
        public List<Viagem> Viagem { get; set; }
    }
}
