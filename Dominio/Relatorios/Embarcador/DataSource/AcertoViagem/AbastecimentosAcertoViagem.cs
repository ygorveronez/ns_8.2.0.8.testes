using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class AbastecimentosAcertoViagem
    {     
        public int Codigo { get; set; }
        public int CodigoAcerto { get; set; }
        public DateTime Data { get; set; }
        public decimal KMAnterior { get; set; }
        public decimal Kilometragem { get; set; }
        public decimal Litros { get; set; }
        public decimal ValorUnitario { get; set; }
        public double CNPJPosto { get; set; }
        public string NomePosto { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Placa { get; set; }
        public string Documento { get; set; }
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public int TipoAbastecimento { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorDigitado { get; set; }
        public decimal KmInicial { get; set; }
        public int KmTotal { get; set; }
        public int KmTotalAjustado { get; set; }
        public decimal PercentualAjusteKM { get; set; }
        public decimal MediaKM { get; set; }
    }
}
