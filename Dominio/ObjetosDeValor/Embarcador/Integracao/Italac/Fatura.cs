using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Italac
{
    public class Fatura
    {
        [JsonProperty(PropertyName = "nrofatura")]
        public int NumeroFatura { get; set; }

        [JsonProperty(PropertyName = "filial")]
        public string Filial { get; set; }

        [JsonProperty(PropertyName = "dataHoraFatura")]
        public string DataFatura { get; set; }

        [JsonProperty(PropertyName = "valorFatura")]
        public Decimal ValorFatura { get; set; }

        [JsonProperty(PropertyName = "valorDesconto")]
        public Decimal ValorDesconto { get; set; }

        [JsonProperty(PropertyName = "valorAcrescimo")]
        public Decimal ValorAcrescimo { get; set; }

        [JsonProperty(PropertyName = "obsFatura")]
        public string Observacao { get; set; }

        [JsonProperty(PropertyName = "transportador")]
        public Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.Transportador Transportador { get; set; }

        [JsonProperty(PropertyName = "documentos")]
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Italac.DocumentoFatura> Documentos { get; set; }
    }
}
