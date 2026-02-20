using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento
{
    public class DocumentoPagamento
    {
        [JsonIgnore]
        public int CodigoPagamento { get; set; }
        [JsonIgnore]
        public int CodigoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string SerieDocumento { get; set; }
        public string Chave { get; set; }
        public DateTime? DataEmissaoDocumento { get; set; }
        [JsonIgnore]
        public TipoDocumento TipoDocumentoInt { get; set; }
        public string TipoDocumento
        {
            get
            {
                return TipoDocumentoInt.ObterDescricao();
            }
        }
        public string NotaFiscal { get; set; }
        public int? ProtocoloCTe { get; set; }
        public decimal? ValorFrete { get; set; }
        public int? NumeroCancelamento { get; set; }
        [JsonIgnore]
        public SituacaoCancelamentoPagamento SituacaoCancelamentoInt { get; set; }
        public string SituacaoCancelamento
        {
            get
            {
                return SituacaoCancelamentoInt.ObterDescricao();
            }
        }
        public string MotivoCancelamento { get; set; }
        public List<IntegracaoPagamento> Integracoes { get; set; }
    }
}
