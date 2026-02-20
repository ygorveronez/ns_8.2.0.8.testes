using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pessoas
{
    public class FuncionarioComissao
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public string Funcionario { get; set; }
        public string Operador { get; set; }
        public DateTime DataGeracao { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public decimal ValorTotalFinal { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal PercentualComissaoAcrescimo { get; set; }
        public decimal PercentualComissaoTotal { get; set; }
        public decimal ValorComissao { get; set; }
        public int QuantidadeTitulos { get; set; }
    }
}
