using System;

namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class DocumentosAcertoViagem
    {
        public int Codigo { get; set; }
        public int CodigoAcerto { get; set; }
        public string DataEmissao { get; set; }
        public string Chave { get; set; }
        public int Numero { get; set; }
        public int Serie { get; set; }
        public string Modelo { get; set; }
        public string Cliente { get; set; }
        public string LocalColeta { get; set; }
        public string LocalEntrega { get; set; }
        public decimal ValorBase { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal ValorComissao { get; set; }
        public decimal ValorRecebido { get; set; }
        public DateTime DataEmissaoSemFormato { get; set; }
    }
}
