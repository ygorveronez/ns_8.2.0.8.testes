using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.KMM
{
    public class Documento
    {
        [JsonProperty(PropertyName = "pis_pasep_nit", Required = Required.Default)]
        public string PisPasepNit { get; set; }

        [JsonProperty(PropertyName = "carteira_identidade", Required = Required.Default)]
        public CarteiraIdentidade CarteiraIdentidade { get; set; }

        [JsonProperty(PropertyName = "cnh", Required = Required.Default)]
        public CNH CNH { get; set; }

        [JsonProperty(PropertyName = "carteira_reservista", Required = Required.Default)]
        public string CarteiraReservista { get; set; }

        [JsonProperty(PropertyName = "titulo_eleitor", Required = Required.Default)]
        public TituloEleitor TituloEleitor { get; set; }

        [JsonProperty(PropertyName = "carteira_trabalho", Required = Required.Default)]
        public CarteiraTrabalho CarteiraTrabalho { get; set; }
    }
}
