using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Naturalidade
    {
        [JsonProperty(PropertyName = "naturalidade", Required = Required.Default)]
        public string NaturalidadeDesc { get; set; }

        [JsonProperty(PropertyName = "naturalidade_uf", Required = Required.Default)]
        public string NaturalidadeUF { get; set; }

        [JsonProperty(PropertyName = "nat_municipio_id", Required = Required.Default)]
        public string NatMunicipioId { get; set; }

        [JsonProperty(PropertyName = "nat_municipio_ibge", Required = Required.Default)]
        public string NatMunicipioIBGE { get; set; }

        [JsonProperty(PropertyName = "nat_pais_id", Required = Required.Default)]
        public int NatPaisId { get; set; }

    }
}
