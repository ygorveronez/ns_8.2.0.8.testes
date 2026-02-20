using Dominio.ObjetosDeValor.Embarcador.Pessoas;
using System.Runtime.Serialization;

namespace Dominio.ObjetosDeValor.Embarcador.Produtos
{
    public class OrdemProdutoParceiro
    {
        [DataMember(Name = "partnerFunction")]
        public string partnerFunction { get; set; }

        //[JsonProperty("conditions")]
        //[DataMember(Name = "partnerFunction")]
        public Pessoa OutroFornecedor { get; set; }
    }
}
