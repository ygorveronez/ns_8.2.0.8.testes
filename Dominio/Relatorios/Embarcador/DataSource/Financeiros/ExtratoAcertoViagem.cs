using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class ExtratoAcertoViagem
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal ValorSaida { get; set; }
        public decimal ValorEntrada { get; set; }
        public string NumeroFrota { get; set; }
        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string Observacao { get; set; }
        public string Veiculos { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal Saldo { get; set; }
        public string Justificativa { get; set; }
        public int NumeroAcerto { get; set; }
    }
}