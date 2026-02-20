using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class CadastroRoteiro
    {
        [JsonProperty(PropertyName = "codigoRoteiroCliente", Required = Required.AllowNull)]
        public string CodigoRoteiroCliente { get; set; }

        [JsonProperty(PropertyName = "idaVolta", Required = Required.Always)]
        public bool IdaVolta { get; set; }

        [JsonProperty(PropertyName = "voltarMesmoCaminhoIda", Required = Required.Always)]
        public bool VoltarPeloMesmoCaminhoIda { get; set; }

        [JsonProperty(PropertyName = "origem", Required = Required.Always)]
        public CadastroRoteiroCidade Origem { get; set; }

        [JsonProperty(PropertyName = "paradas", Required = Required.AllowNull)]
        public List<CadastroRoteiroCidade> Paradas { get; set; }

        [JsonProperty(PropertyName = "destino", Required = Required.Always)]
        public CadastroRoteiroCidade Destino { get; set; }
    }
}
