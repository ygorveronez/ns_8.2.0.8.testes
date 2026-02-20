using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.Recebimento
{
    public sealed class RecebimentoCabecalho
    {
        [JsonProperty(PropertyName = "numeroEXP", Order = 1, Required = Required.Always)]
        public string NumeroEXP { get; set; }

        [JsonProperty(PropertyName = "protocoloReferencia", Order = 2, Required = Required.Always)]
        public string ProtocoloReferencia { get; set; }
    }
}
