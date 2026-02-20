namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class RetornoGridAgendamentoColetaAlterarProdutos
    {
        public int Codigo { get; set; }
        public int CodigoProduto { get; set; }
        public string CodigoEmbarcador { get; set; }
        public string Descricao { get; set; }
        public string Setor { get; set; }
        public decimal Quantidade { get; set; }
        public decimal QuantidadeOriginal { get; set; }
        public bool Removido { get; set; }
        public bool DT_Enable { get; set; }
        public string DT_RowColor { get; set; }
        public int DT_RowId { get; set; }
        public int QuantidadeCaixas { get; set; }
    }
}
