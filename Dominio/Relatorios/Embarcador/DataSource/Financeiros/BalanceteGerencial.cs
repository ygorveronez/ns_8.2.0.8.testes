using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class BalanceteGerencial
    {
        public int Codigo { get; set; }
        public string Plano { get; set; }
        public string Descricao { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal Entrada { get; set; }
        public decimal Saida { get; set; }
        public decimal SaldoFinal { get; set; }
        public string CentroResultado { get; set; }
        public string PlanoCentroResultado { get; set; }
        public int ReceitaDespesa { get; set; }
        public int CodigoPlanoConta { get; set; }
        private AnaliticoSintetico TipoConta { get; set; }

        public string TipoContaDescricao
        {
            get { return TipoConta.ObterDescricao(); }
        }
    }
}
