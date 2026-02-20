using System;

namespace Dominio.Relatorios.Embarcador.DataSource.WMS
{
    public class Armazenagem
    {
        public string NumeroNota { get; set; }
        public string Serie { get; set; }
        public DateTime DataSaida { get; set; }
        public string Destinatario { get; set; }
        public string QuantidadeItens { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal ValorTotal { get; set; }
        public string TipoNF { get; set; }
        public int StatusNF { get; set; }
    }
}
