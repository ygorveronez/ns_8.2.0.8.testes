using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RelatorioMovimentosMotorista
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string Banco { get; set; }
        public string NumeroAgencia { get; set; }
        public string DigitoAgencia { get; set; }
        public string NumeroConta { get; set; }
        public string TipoConta { get; set; }
        public decimal Valor { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Observacao { get; set; }
        public DateTime DataMovimentacao { get; set; }
    }
}
