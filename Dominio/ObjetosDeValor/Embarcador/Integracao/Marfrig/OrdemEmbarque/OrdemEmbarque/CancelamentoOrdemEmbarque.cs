using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.OrdemEmbarque
{
    public sealed class CancelamentoOrdemEmbarque
    {
        [JsonProperty(PropertyName = "cabecalho", Required = Required.Always)]
        public CancelamentoOrdemEmbarqueCabecalho Cabecalho { get; set; }
    }
}
