namespace Dominio.Relatorios.Embarcador.DataSource.AcertoViagem
{
    public class ReceitaAcertoViagem
    {
        public int CodigoAcerto { get; set; }
        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string Periodo { get; set; }
        public int NumeroViagens { get; set; }
        public int NumeroViagensCompartilhada { get; set; }
        public decimal ValorViagensCompartilhada { get; set; }
        public string PlacasVeiculos { get; set; }
        public string PlacasReboques { get; set; }
        public int DiasViagem { get; set; }

        public decimal DespesaCombustivel { get; set; }
        public decimal DespesaArla { get; set; }        
        public decimal PedagioPago { get; set; }
        public decimal DespesaMotorista { get; set; }
        public decimal TotalDespesa { get; set; }

        public decimal ReceitaFrete { get; set; }
        public decimal PedagioRecebido { get; set; }
        public decimal OutrosRecebimentos { get; set; }
        public decimal BonificacaoCliente { get; set; }
        public decimal Ocorrencias { get; set; }
        public decimal TotalReceita { get; set; }
        
        public decimal TotalSaldo { get; set; }

        public decimal FaturamentoLiquido { get; set; }
        public decimal FaturamentoBruto { get; set; }
        public decimal TotalImposto { get; set; }
        public decimal ComissaoMotorista { get; set; }
        public decimal ValorBonificacao { get; set; }
        public string MotivoBonificacao { get; set; }

        public decimal PremioComissaoMotorista { get; set; }
        public decimal PercentualPremioComissao { get; set; }

        public decimal ValorTotalBonificacao { get; set; }
        public decimal ValorTotalDesconto { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal AdiantamentoXDespesas { get; set; }
        public decimal TotalPagarMotorista { get;  set;}

        public decimal ValorLiquidoMes { get; set; }

        public decimal ValorKMDiesel { get; set; }
        public decimal ValorKMLiquido { get; set; }
        public decimal MediaKM { get; set; }
        public decimal Parametro { get; set; }

        public decimal AbastecimentoMotorista { get; set; }
        public decimal PedagioMotorista { get; set; }
        public decimal OutraDespesaMotorista { get; set; }
        public decimal AdiantamentoMotorista { get; set; }
        public decimal RetornoAdiantamento { get; set; }
        public decimal DiariaMotorista { get; set; }
        public decimal TotalDespesaMotorista { get; set; }
        public decimal TotalReceitaMotorista { get; set; }
        public decimal SaldoMotorista { get; set; }
        public decimal DescontosMotorista { get; set; }
        public decimal DevolucoesMotorista { get; set; }
        public decimal BonificacoesMotorista { get; set; }
        public decimal VariacaoCambial { get; set; }
        public decimal VariacaoCambialReceita { get; set; }
    }
}
