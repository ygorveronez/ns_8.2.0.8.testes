using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class FluxoCaixa
    {
        public string Documento { get; set; }
        public string Pessoa { get; set; }
        public double CNPJCPFPessoa { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorPagar { get; set; }
        public decimal ValorReceber { get; set; }
    }
}