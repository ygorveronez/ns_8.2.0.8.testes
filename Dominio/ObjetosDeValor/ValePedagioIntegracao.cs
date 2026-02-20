namespace Dominio.ObjetosDeValor
{
    public class ValePedagioIntegracao
    {
        public string CNPJFornecedor { get; set; }

        public string CNPJResponsavel { get; set; }

        public string NumeroComprovante { get; set; }

        public decimal ValorValePedagio { get; set; }

        public Dominio.Enumeradores.TipoCompraValePedagio? TipoCompra { get; set; }

        public int QuantidadeEixos { get; set; }
    }
}
