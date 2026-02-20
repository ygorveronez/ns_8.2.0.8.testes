using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar
{
    public class Fatura
    {
        [JsonProperty("param")]
        public string Parametro { get; set; }

        [JsonProperty("codEmp")]
        public int CodigoEmpresa { get; set; }

        [JsonProperty("codFil")]
        public string CodigoFilial { get; set; }

        [JsonProperty("codFor")]
        public string CodigoTransportador { get; set; }

        [JsonProperty("codFpg")]
        public int CodigoFormaPagamento { get; set; }

        [JsonProperty("codCpg")]
        public string CodigoCondicaoPagamentoFatura { get; set; }

        [JsonProperty("vlrFat")]
        public string Valor { get; set; }

        [JsonProperty("codUsu")]
        public string CodigoUsuarioGerador { get; set; }

        [JsonProperty("CTE")]
        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.Olfar.CTe> CTes { get; set; }
    }
}
