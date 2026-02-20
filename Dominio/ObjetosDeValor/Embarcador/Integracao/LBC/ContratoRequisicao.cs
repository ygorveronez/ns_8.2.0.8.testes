using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.LBC
{
    public class ContratoRequisicao
    {
        [JsonProperty("data")]
        public List<ContratoRequisicaoItem> Itens { get; set; }
    }
}
