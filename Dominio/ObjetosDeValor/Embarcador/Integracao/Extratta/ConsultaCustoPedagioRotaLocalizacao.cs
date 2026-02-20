using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Extratta
{
    public class ConsultaCustoPedagioRotaLocalizacao
    {
        [JsonProperty(PropertyName = "IbgeCidade", Required = Required.Default)]
        public int IbgeCidade { get; set; }
    }
}
