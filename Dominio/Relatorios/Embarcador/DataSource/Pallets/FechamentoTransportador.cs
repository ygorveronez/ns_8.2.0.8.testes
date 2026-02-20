using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pallets
{
    public class FechamentoTransportador
    {
        public string CNPJTransportador { get; set; }
        public string Transportador { get; set; }
        public string TransportadorCodigoIntegracao { get; set; }
        public int TotalEntradas { get; set; }
        public int TotalSaidas { get; set; }
        public int SaldoTotal { get; set; }
        public int DiasDeRotatividade { get; set; }

        public int TotalEmRotatividade { get; set; }

        public int SaldoPendente
        {
            get { return SaldoTotal - TotalEmRotatividade; }
        }

        public decimal ValorTotal
        {
            get { return SaldoPendente * 26; } //todo: valor fixo de 26 para tirol 
        }

        public decimal Acrescimo
        {
            get { return ValorTotal * 0.30m; } //todo: valor fixo de 30 % para tirol
        }

        public string CNPJFormatado
        {
            get { return this.CNPJTransportador.ObterCnpjFormatado(); }
        }

        public decimal TotalFinal
        {
            get { return ValorTotal + Acrescimo; }
        }
    }
}
