using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class AFRMMControlMercante
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string CodigoNavio { get; set; }
        public string Navio { get; set; }
        public string Viagem { get; set; }
        public string Direcao { get; set; }
        public string CodigoInicioPrestacao { get; set; }
        public string InicioPrestacao { get; set; }
        public string CodigoPortoOrigem { get; set; }
        public string PortoOrigem { get; set; }
        public string CodigoPortoDestino { get; set; }
        public string PortoDestino { get; set; }
        public string CodigoFimPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string NumeroControle { get; set; }
        public string UBLI { get; set; }
        public int Carrier { get; set; }
        private DateTime ETA { get; set; }
        private DateTime DataOperacaoETA { get; set; }
        public string ShipperCode { get; set; }
        public string Shipper { get; set; }
        public string Notify1Code { get; set; }
        public string Notify1 { get; set; }
        public string FreightCode { get; set; }
        public string Freight { get; set; }

        public int Quantidade20 { get; set; }
        public int Quantidade40 { get; set; }
        public int Tariff { get; set; }
        public int CommodityCode { get; set; }
        public string CommodityText { get; set; }
        public string Service { get; set; }
        public string NumeroBooking { get; set; }
        public string Copysgn { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroCEMercante { get; set; }

        private decimal ValorPrestacaoServico { get; set; }
        public string FrtCurr { get; set; }
        public string FrtPaymode { get; set; }
        public int Amount1 { get; set; }
        public string Curr1 { get; set; }
        public string PayM1 { get; set; }
        public int Amount2 { get; set; }
        public string Curr2 { get; set; }
        public string PayM2 { get; set; }
        public int Amount3 { get; set; }
        public string Curr3 { get; set; }
        public string PayM3 { get; set; }
        public int Amount4 { get; set; }
        public string Curr4 { get; set; }
        public string PayM4 { get; set; }

        public decimal ValorICMS { get; set; }
        public string Curr5 { get; set; }
        public string PayM5 { get; set; }
        public decimal ValorICMSST { get; set; }
        public string Curr6 { get; set; }
        public string PayM6 { get; set; }

        public int Amount7 { get; set; }
        public string Curr7 { get; set; }
        public string PayM7 { get; set; }
        public int Amount8 { get; set; }
        public string Curr8 { get; set; }
        public string PayM8 { get; set; }

        public decimal Amount9
        {
            get { return TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder ? ValorPrestacaoServico : 0; }
        }
        public string Curr9
        {
            get { return TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder ? "BRL" : string.Empty; }
        }
        public string PayM9
        {
            get { return TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder ? "P" : string.Empty; }
        }

        public int Amount10 { get; set; }
        public string Curr10 { get; set; }
        public string PayM10 { get; set; }
        public int Amount11 { get; set; }
        public string Curr11 { get; set; }
        public string PayM11 { get; set; }
        public int Amount12 { get; set; }
        public string Curr12 { get; set; }
        public string PayM12 { get; set; }
        public int Amount13 { get; set; }
        public string Curr13 { get; set; }
        public string PayM13 { get; set; }
        public int Amount14 { get; set; }
        public string Curr14 { get; set; }
        public string PayM14 { get; set; }
        public int Amount15 { get; set; }
        public string Curr15 { get; set; }
        public string PayM15 { get; set; }
        public int Amount16 { get; set; }
        public string Curr16 { get; set; }
        public string PayM16 { get; set; }
        public int Amount17 { get; set; }
        public string Curr17 { get; set; }
        public string PayM17 { get; set; }
        public int Amount18 { get; set; }
        public string Curr18 { get; set; }
        public string PayM18 { get; set; }
        public int Amount19 { get; set; }
        public string Curr19 { get; set; }
        public string PayM19 { get; set; }
        public int Amount20 { get; set; }
        public string Curr20 { get; set; }
        public string PayM20 { get; set; }

        public string Irin { get; set; }
        public string CTeSign { get; set; }
        public int Station { get; set; }
        private TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        #endregion

        #region Propriedades com Regras

        public string DataETAFormatada
        {
            get { return ETA != DateTime.MinValue ? ETA.ToString("dd.MM.yyyy") : string.Empty; }
        }

        public string DataOperacaoETAFormatada
        {
            get { return DataOperacaoETA != DateTime.MinValue ? DataOperacaoETA.ToString("dd.MM.yyyy") : string.Empty; }
        }

        public decimal ValorPrestacao
        {
            get { return TipoPropostaMultimodal == TipoPropostaMultimodal.Feeder ? 0 : ValorPrestacaoServico; }
        }

        public string ConsigCode
        {
            get { return this.FreightCode; }
        }
        public string Consig
        {
            get { return this.Freight; }
        }

        #endregion
    }
}
