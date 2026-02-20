namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Troca
{
    public class Produto
    {
        public int Pai { get; set; }

        public string Item { get; set; }

        public string Descricao { get; set; }

        public decimal Quantidade { get; set; }

        public string Devolucao { get; set; }

        public decimal QuantidadeTotal { get; set; }

        public decimal TotalizadorQuantidade { get; set; }

    }
}
