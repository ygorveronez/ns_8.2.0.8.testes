using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Trizy
{
    public class ContratoDetalhes
    {
        [JsonProperty(PropertyName = "atributos", Required = Required.Default)]
        public List<Atributos> Atributos { get; set; }

        [JsonProperty(PropertyName = "descricao", Required = Required.Default)]
        public string Descricao { get; set; }

        [JsonProperty(PropertyName = "incoterm", Required = Required.Default)]
        public string Incoterm { get; set; }

        [JsonProperty(PropertyName = "identificador_externo", Required = Required.Default)]
        public string IdentificadorExterno { get; set; }

        [JsonProperty(PropertyName = "terminal", Required = Required.Default)]
        public Terminal Terminal { get; set; }

        [JsonProperty(PropertyName = "cliente", Required = Required.Default)]
        public ClienteContrato Cliente { get; set; }

        [JsonProperty(PropertyName = "operacao", Required = Required.Default)]
        public Operacao Operacao { get; set; }

        [JsonProperty(PropertyName = "produto", Required = Required.Default)]
        public ProdutoContrato Produto { get; set; }

        [JsonProperty(PropertyName = "vigencia", Required = Required.Default)]
        public Vigencia Vigencia { get; set; }

        [JsonProperty(PropertyName = "recursos", Required = Required.Default)]
        public Recursos Recursos { get; set; }

        [JsonProperty(PropertyName = "flags", Required = Required.Default)]
        public Flags Flags { get; set; }

        [JsonProperty(PropertyName = "emissor", Required = Required.Default)]
        public Emissor Emissor { get; set; }

        [JsonProperty(PropertyName = "tomador", Required = Required.Default)]
        public Tomador Tomador { get; set; }

        [JsonProperty(PropertyName = "destino", Required = Required.Default)]
        public Destino Destino { get; set; }

        [JsonProperty(PropertyName = "cotas", Required = Required.Default)]
        public List<Cotas> Cotas { get; set; }

        [JsonProperty(PropertyName = "fracoes_cota", NullValueHandling = NullValueHandling.Ignore)]
        public List<FracoesCota> FracoesCota { get; set; }
    }
}
