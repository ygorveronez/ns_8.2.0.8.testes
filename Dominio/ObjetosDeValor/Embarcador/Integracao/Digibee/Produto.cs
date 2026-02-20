namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class Produto
    {
        public string pedidoERP { get; set; }
        public int numeroItem { get; set; }
        public string codigoProduto { get; set; }
        public int quantidade { get; set; }
        public string unidade { get; set; }
        public string statusDoEstoque { get; set; }
        public string motivoDoBloqueioDeQualidade { get; set; }
    }
}
