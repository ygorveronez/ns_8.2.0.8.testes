using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class ComissaoAcertoViagem
    {
        public int CodigoMotorista { get; set; }
        public int NumeroAcerto { get; set; }
        public string Motorista { get; set; }
        public string Cavalo { get; set; }
        public string Reboques { get; set; }
        public string ModeloVeiculo { get; set; }
        public string Segmento { get; set; }
        public DateTime DataInicialAcerto { get; set; }
        public DateTime DataFinalAcerto { get; set; }
        public decimal Media { get; set; }
        public decimal ConsumoCombustivel { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal Bonificacoes { get; set; }
        public decimal Descontos { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal ValorComissao { get; set; }

        public string DataInicialAcertoFormatada
        {
            get { return DataInicialAcerto != DateTime.MinValue ? DataInicialAcerto.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataFinalAcertoFormatada
        {
            get { return DataFinalAcerto != DateTime.MinValue ? DataFinalAcerto.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}
