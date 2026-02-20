using Newtonsoft.Json;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Calisto
{
    public class Produto
    {
        [JsonProperty("codigoAlternativo")]
        public string CodigoAlternativo { get; set; }

        [JsonProperty("unidade")]
        public string Unidade { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("dataAlteracao")]
        public string DataAlteracao { get; set; }

        [JsonProperty("codigoClasse")]
        public string CodigoClasse { get; set; }

        [JsonProperty("codigoGrupo")]
        public string CodigoGrupo { get; set; }

        [JsonProperty("codigoSubGrupo")]
        public string CodigoSubGrupo { get; set; }
    }
}
