using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class AlteracaoOrdemEmbarque
    {
        [JsonProperty(PropertyName = "cabecalho", Required = Required.Always)]
        public AlteracaoOrdemEmbarqueCabecalho Cabecalho { get; set; }

        [JsonProperty(PropertyName = "frete", Required = Required.Always)]
        public AlteracaoOrdemEmbarqueFrete Frete { get; set; }
    }
}
