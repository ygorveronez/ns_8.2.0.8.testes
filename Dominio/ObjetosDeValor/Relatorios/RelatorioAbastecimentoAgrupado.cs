namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioAbastecimentoAgrupado
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public decimal QuilometrosRodados { get; set; }
        public decimal LitrosGastos { get; set; }
        public decimal ValorTotalReceita { get; set; }
        public decimal ValorTotalDespesa { get; set; }
        public decimal ValorMedioPagoPorLitro { get; set; }
    }
}
