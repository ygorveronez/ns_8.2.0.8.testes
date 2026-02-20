using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class ValePedagio
    {
        public int Codigo { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataCarga { get; set; }
        public string DataCargaFormatada
        {
            get
            {
                return DataCarga > DateTime.MinValue ? DataCarga.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string Filial { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public string TipoCarga { get; set; }
        public string ModeloVeicular { get; set; }
        public string TipoOperacao { get; set; }
        public string Transportador { get; set; }
        public string Motoristas { get; set; }
        public string NumeroValePedagio { get; set; }
        public SituacaoValePedagio SituacaoValePedagio { get; set; }
        public string SituacaoValePedagioDescricao
        {
            get
            {
                return SituacaoValePedagio.ObterDescricao();
            }
        }
        public decimal ValorValePedagio { get; set; }

        public SituacaoIntegracao SituacaoIntegracaoValePedagio { set; get; }
        public string SituacaoIntegracaoValePedagioDescricao
        {
            get
            {
                return SituacaoIntegracaoValePedagio.ObterDescricao();
            }
        }
        public DateTime DataRetornoValePedagio { get; set; }
        public string DataRetornoValePedagioFormatada
        {
            get
            {
                return DataRetornoValePedagio > DateTime.MinValue ? DataRetornoValePedagio.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public string RetornoIntegracao { get; set; }
    }
}
