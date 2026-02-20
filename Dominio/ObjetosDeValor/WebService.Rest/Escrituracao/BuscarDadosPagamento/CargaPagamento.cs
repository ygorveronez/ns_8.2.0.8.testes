using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento
{
    public class CargaPagamento
    {
        [JsonIgnore]
        public int CodigoCarga { get; set; }
        [JsonIgnore]
        public int? CodigoPagamento { get; set; }
        [JsonIgnore]
        public DateTime? DataHoraEmissao { get; set; }
        public string NumeroCarga { get; set; }
        [JsonIgnore]
        public SituacaoCarga SituacaoCargaInt { get; set; }
        public string SituacaoCarga
        {
            get
            {
                return SituacaoCargaHelper.ObterDescricao(SituacaoCargaInt);
            }
        }
        public string Transportador { get; set; }
        public string CnpjTransportador { get; set; }
        public string Filial { get; set; }
        public string TipoOperacao { get; set; }
        public List<Pagamento> Pagamentos { get; set; }
    }
}