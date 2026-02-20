namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class PedidoPendente
    {
        public int Codigo { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string TipoOperacao { get; set; }
        public string DescricaoFilial { get; set; }
        public bool UsarLayoutAgendamentoPorCaixaItem { get; set; }
        public long CodigoDestinatario { get; set; }
        public int CodigoFilial { get; set; }
        public string TipoCarga { get; set; }
        public int TipoCargaCodigo { get; set; }
        public int QtVolumes { get; set; }
        public int Saldo { get; set; }
        public string GrupoProduto { get; set; }
        public string Modalidade { get; set; }
        public int VolumesEnviar { get; set; }
        public int SKU { get; set; }
        public int QtProdutos { get; set; }
        public string DataFimJanelaDescarga { get; set; }
        public string DataCriacao { get; set; }
        public string DataInicioJanelaDescarga { get; set; }
        public string Categoria { get; set; }
        public double CNPJRemetente { get; set; }
        public string CodigoIntegracaoProduto { get; set; }
        public string DescricaoProduto { get; set; }
    }
}
