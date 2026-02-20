using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Tecnorisk
{
    public class Veiculos
    {
        [JsonProperty("veiculo_placa")]
        public string Placa { get; set; }

        [JsonProperty("veiculo_tipo")]
        public string VeiculoTipo { get; set; }
    }
}
