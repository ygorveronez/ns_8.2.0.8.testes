namespace Dominio.ObjetosDeValor.Relatorios
{
    public class RelatorioMovimentoDoFinanceiroAgrupado
    {
        public int CodigoPlano { get; set; }
        public string DescricaoPlano { get; set; }
        public string Conta { get; set; }
        public string TipoPlano { get; set; }
        public string TipoConta { get; set; }
        public decimal ValorTotal { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public int ObterValor(int mes, int ano, string tipo)
        {
            return 0;            
        }
    }
}
