using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Globus
{
    public class ContaBancaria
    {
        [JsonProperty(PropertyName = "tipo", Required = Required.Default)]
        public string Tipo { get; set; }

        [JsonProperty(PropertyName = "banco", Required = Required.Default)]
        public int Banco { get; set; }

        [JsonProperty(PropertyName = "agencia", Required = Required.Default)]
        public int Agencia { get; set; }

        [JsonProperty(PropertyName = "digitoAgencia", Required = Required.Default)]
        public int DigitoAgencia { get; set; }

        [JsonProperty(PropertyName = "conta", Required = Required.Default)]
        public int Conta { get; set; }

        [JsonProperty(PropertyName = "digitoConta", Required = Required.Default)]
        public int DigitoConta { get; set; }
    }
}
