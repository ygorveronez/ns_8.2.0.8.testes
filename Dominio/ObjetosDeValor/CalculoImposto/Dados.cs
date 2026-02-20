namespace Dominio.ObjetosDeValor.CalculoImposto
{
    public class Dados
    {
        public Dominio.ObjetosDeValor.CTe.Empresa Transportador { get; set; }
        public int IBGEOrigemPrestacao { get; set; }
        public int IBGETerminoPrestacao { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Remetente { get; set; }
        public Dominio.ObjetosDeValor.CTe.Cliente Destinatario { get; set; }
        public decimal ValorFrete { get; set; }
        public string CodigoProdutoEmbarcador { get; set; }
    }
}
