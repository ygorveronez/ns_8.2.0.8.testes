using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe.CTes
{
    public class PosicaoCTe
    {
        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public int SerieCTe { get; set; }
        public string StatusCTe { get; set; }
        public string NumeroCarga { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataAutorizacao { get; set; }
        public DateTime DataCancelamento { get; set; }
        public DateTime DataAnulacao { get; set; }
        public DateTime DataImportacao { get; set; }
        public DateTime DataVinculoCarga { get; set; }        
        public string CPFCNPJRemetente { get; set; }
        public string Remetente { get; set; }
        public string CPFCNPJDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string CPFCNPJTomador { get; set; }
        public string Tomador { get; set; }        
        public string InicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }
        public string Transportador { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorMercadoria { get; set; }        
        public string ChaveCTe { get; set; }
        public DateTime DataFatura { get; set; }
    }
}
