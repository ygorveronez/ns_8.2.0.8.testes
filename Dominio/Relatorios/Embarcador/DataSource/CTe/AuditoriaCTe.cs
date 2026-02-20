using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe
{
    public class AuditoriaCTe
    {
        public int Codigo { get; set; }
        public string MunicipioCobranca { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public string NumeroRomaneio { get; set; }
        public decimal KilometragemRota { get; set; }
        public int NumeroCTe { get; set; }
        public decimal TotalReceberCTe { get; set; }
        public decimal ValorTotalMercadoria { get; set; }
        public string NotasFiscais { get; set; }
        public decimal FreteUnitario { get; set; }
        public decimal PesoCarregado { get; set; }
        public decimal PesoCobrado { get; set; }
        public decimal PesoFrete { get; set; }
        public string TabelaFreteCarga { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorISS { get; set; }
        public string SecCat { get; set; }
        public string DespachoCATITR { get; set; }
        public decimal TotalImpostos { get; set; }
        public decimal Peso { get; set; }
        public string Mercadoria { get; set; }
        public string MercadoriaGrupo { get; set; }
        public string EspecieDescricaoFrete { get; set; }
        public string PlacaTracao { get; set; }
        public string ModalidadePlacaTracao { get; set; }
        public string TipoFrete { get; set; }
        public string Operacao { get; set; }
        public string RazaoSocialRemetente { get; set; }
        public string MunicipioRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string RazaoSocialColeta { get; set; }
        public string UFColeta { get; set; }
        public string RazaoSocialRedespacho { get; set; }
        public string MunicipioRedespacho { get; set; }
        public string UFRedespacho { get; set; }
        public string RazaoSocialDestinatario { get; set; }
        public string MunicipioDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string DataEntrega { get; set; }
        public string Observacao { get; set; }
        public string TipoConhecimento { get; set; }
        public string InseridoPor { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public string ChaveCTe { get; set; }

        public string DataEmissaoCTeFormatada
        {
            get { return DataEmissaoCTe != DateTime.MinValue ? DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public int AnoEmissaoCTe
        {
            get { return DataEmissaoCTe.Year; }
        }

        public int MesEmissaoCTe
        {
            get { return DataEmissaoCTe.Month; }
        }

        public decimal ValorPIS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaPIS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public decimal ValorCOFINS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero); }
        }
    }
}
