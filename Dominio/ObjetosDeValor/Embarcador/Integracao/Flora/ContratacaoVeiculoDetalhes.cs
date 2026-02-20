using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Flora
{
    public class ContratacaoVeiculoDetalhes
    {
        #region Propriedades

        [JsonProperty("ProtocoloCarregamento")]
        public string ProtocoloCarregamento { get; set; }

        [JsonProperty("CdMidiaOrigem")]
        public int MidiaOrigem { get; set; }

        [JsonProperty("Veiculos")]
        public ListaVeiculo Veiculos { get; set; }

        #endregion Propriedades
                
    }

}
