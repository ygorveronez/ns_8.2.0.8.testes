using Newtonsoft.Json;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class ContratacaoVeiculo
    {
        [JsonProperty("ContratacaoVeiculo")]
        public ContratacaoVeiculoDetalhes ContratacaoVeiculoDetalhes { get; set; }
    }
}
