using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Repom
{
    public class ConsultaValorPedagio
    {
        [JsonProperty(PropertyName = "codigoRoteiro")]
        public string CodigoRoteiro { get; set; }

        [JsonProperty(PropertyName = "codigoPercurso")]
        public string CodigoPercurso { get; set; }

        [JsonProperty(PropertyName = "numeroEixos")]
        public string NumeroEixos { get; set; }

        [JsonProperty(PropertyName = "numeroEixosSuspensosIda")]
        public string NumeroEixosSuspensosIda { get; set; }

        [JsonProperty(PropertyName = "numeroEixosSuspensosVolta")]
        public string NumeroEixosSuspensosVolta { get; set; }

    }
}
