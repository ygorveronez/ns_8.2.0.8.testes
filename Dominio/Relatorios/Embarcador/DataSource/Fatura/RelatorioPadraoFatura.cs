using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fatura
{
    public class RelatorioPadraoFatura
    {
        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public string ChaveCTe { get; set; }
        public string Notas { get; set; }
        public string Produtos { get; set; }
        public string Quantidades { get; set; }
        public string CteOriginario { get; set; }
        public decimal Peso { get; set; }
        public int SerieCTE { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal ValorSemICMSBase { get; set; }

        public decimal ValorSemICMS
        {
            get { return TipoTomadorExterior ? (ValorSemICMSBase / ValorCotacaoMoedaCTe) : ValorSemICMSBase; }
        }

        public decimal TotalCobrado
        {
            get { return ValorSemICMS + ICMS; }
        }
        public decimal ICMS { get; set; }
        public decimal Total { get; set; }
        public string PedidoNavioDirecao { get; set; }
        public string NumeroControleCTe { get; set; }
        public string Trecho { get; set; }
        public decimal ValorTotalMoeda { get; set; }
        public MoedaCotacaoBancoCentral Moeda { get; set; }
        public int TipoProposta { get; set; }

        public string SiglaMoeda
        {
            get { return Moeda.ObterSigla(); }
        }

        public int TipoMoeda
        {
            get { return (int)Moeda; }
        }

        public string Destinatario { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorMercadoria { get; set; }
        public bool PossuiProdutos { get; set; }
        public bool TipoTomadorExterior { get; set; }

        private decimal _valorCotacaoMoedaCTe;
        public decimal ValorCotacaoMoedaCTe
        {
            get => _valorCotacaoMoedaCTe;
            set => _valorCotacaoMoedaCTe = value > 0 ? value : 1;
        }

    }
}
