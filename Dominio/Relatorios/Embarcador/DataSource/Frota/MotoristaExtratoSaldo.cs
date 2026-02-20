using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class MotoristaExtratoSaldo
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public DateTime DataLancamento { get; set; }
        public decimal Entrada { get; set; }
        public decimal Saida { get; set; }
        public string TipoPagamento { get; set; }
        public string Operador { get; set; }
        public string Motorista { get; set; }
        public int NumeroAcerto { get; set; }
        public int NumeroPagamento { get; set; }
    }
}
