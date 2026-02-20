namespace Dominio.ObjetosDeValor.MDFe
{
    public class ValePedagio
    {
        public string CNPJFornecedor { get; set; }

        public string CNPJResponsavel { get; set; }

        public string NumeroComprovante { get; set; }

        public string CodigoAgendamentoPorto { get; set; }

        public decimal ValorValePedagio { get; set; }

        public Dominio.Enumeradores.TipoCompraValePedagio? TipoCompra { get; set; }

        public int QuantidadeEixos { get; set; }
    }
}
