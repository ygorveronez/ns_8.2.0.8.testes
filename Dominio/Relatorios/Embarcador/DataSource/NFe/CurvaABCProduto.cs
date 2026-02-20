namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class CurvaABCProduto
    {
        public int Posicao { get; set; }
        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public decimal Estoque { get; set; }
        public int Vezes { get; set; }
        public decimal Quantidade { get; set; }
        public decimal Valor { get; set; }
        public decimal Contribuicao { get; set; }
        public decimal Acumulado { get; set; }
        public string Empresa { get; set; }
        public string GrupoProduto { get; set; }

        public string EstoqueFormatado
        {
            get { return Estoque.ToString("n4"); }
        }
    }
}
