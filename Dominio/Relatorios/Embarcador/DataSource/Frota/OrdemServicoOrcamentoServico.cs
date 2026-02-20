namespace Dominio.Relatorios.Embarcador.DataSource.Frota
{
    public class OrdemServicoOrcamentoServico
    {
        public int CodigoServico { get; set; }
        public string NomeServico { get; set; }
        public int OrdemServicoFrota { get; set; }
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorTotalMaoObra { get; set; }
        public decimal ValorTotalOrcado { get; set; }
        public decimal ValorTotalPreAprovado { get; set; }
        public int Parcelas { get; set; }
        public string Observacao { get; set; }
        public int CodigoOrdemServicoOrcamentoServico { get; set; }
        public int OrdemServicoFrotaOrcamento { get; set; }
        public int Manutencao { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorMaoObra { get; set; }
        public string OrcadoPor { get; set; }
        public string ObservacaoServico { get; set; }

    }
}
