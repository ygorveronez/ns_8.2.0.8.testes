namespace Dominio.ObjetosDeValor.Embarcador.Avarias
{
    public class ProdutosAvariados
    {
        public int Codigo { get; set; }
		public string CodigoProduto { get; set; }
		public string ProdutoEmbarcador { get; set; }
        public string GrupoProduto { get; set; }
        public decimal ValorAvaria { get; set; }
        public decimal UnidadesAvariadas { get; set; }
        public decimal CaixasAvariadas { get; set; } 
        public bool RemovidoLote { get; set; }
    }
}
