using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Usuarios
{
    public class CaixaFuncionario
    {
        public string Operador { get; set; }
        public string PlanoConta { get; set; }
        public DateTime DataAbertura { get; set; }
        public decimal SaldoInicial { get; set; }
        public int Codigo { get; set; }
        public int CodigoMovimento { get; set; }
        public DateTime DataMovimento { get; set; }
        public string NumeroDocumento { get; set; }
        public string ObservacaoMovimento { get; set; }
        public decimal ValorMovimento { get; set; }
        public int DebitoCreditoMovimento { get; set; }
        public decimal Entradas { get; set; }
        public decimal Saidas { get; set; }
        public decimal SaldoFinal { get; set; }
        public decimal ValoresNoCaixa { get; set; }
    }
}
