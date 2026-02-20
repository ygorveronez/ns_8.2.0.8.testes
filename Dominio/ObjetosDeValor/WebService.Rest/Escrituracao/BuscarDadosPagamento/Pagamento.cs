using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento
{
    public class Pagamento
    {
        [JsonIgnore]
        public int CodigoCarga { get; set; }
        [JsonIgnore]
        public int CodigoPagamento { get; set; }
        public int? NumeroPagamento { get; set; }
        public decimal? ValorPagamento { get; set; }
        [JsonIgnore]
        public SituacaoPagamento SituacaoPagamentoInt { get; set; }
        public string SituacaoPagamento
        {
            get
            {
                return this.SituacaoPagamentoInt.ObterDescricao();
            }
        }
        public List<DocumentoPagamento> Documentos { get; set; }
        public List<IntegracaoPagamento> Integracoes { get; set; }
    }
}
