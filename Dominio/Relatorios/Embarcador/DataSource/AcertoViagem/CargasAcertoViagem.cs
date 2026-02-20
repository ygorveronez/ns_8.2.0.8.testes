using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class CargasAcertoViagem
    {
        public int CodigoAcerto { get; set; }
        public int Codigo { get; set; }        
        public decimal PercentualCarga { get; set; }
        public decimal PedagioCarga { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataEmissaoCTe { get; set; }
        public string Placa { get; set; }
        public string NumeroCarga { get; set; }
        public string Emitente { get; set; }
        public string CNPJEmitente { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal Peso { get; set; }
        public int CodigoCarga { get; set; }
        public decimal ValorComponenteFrete { get; set; }
        public decimal BonificacaoCliente { get; set; }
        public decimal ValorBrutoCarga { get; set; }
        public decimal ValorICMSCarga { get; set; }
        public decimal ValorPedagioCredito { get; set; }

        public string DataEmissaoCTeFormatada
        {
            get { return DataEmissaoCTe != DateTime.MinValue ? DataEmissaoCTe.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
