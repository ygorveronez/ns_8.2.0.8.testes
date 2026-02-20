using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Omnicomm
{
    public class RequisicaoConsultaPosicoesRastreSat
    {
        [JsonPropertyName("ids")]
        public List<int> Ids { get; set; }

        public RequisicaoConsultaPosicoesRastreSat(List<int> ids)
        {
            Ids = ids;
        }
    }

}
