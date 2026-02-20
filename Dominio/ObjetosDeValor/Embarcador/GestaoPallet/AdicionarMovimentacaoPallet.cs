namespace Dominio.ObjetosDeValor.Embarcador.GestaoPallet
{
    public sealed class AdicionarMovimentacaoPallet
    {
        public int QuantidadePallets { get; set; }
        public Entidades.Cliente Cliente { get; set; }
        public Entidades.Empresa Transportador { get; set; }
        public Entidades.Embarcador.Filiais.Filial Filial { get; set; }
        public Entidades.Embarcador.Filiais.Filial FilialDestino { get; set; }
        public Entidades.Embarcador.Cargas.Carga Carga { get; set; }
        public Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }
        public Entidades.Embarcador.Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
        public Enumeradores.RegraPallet RegraPallet { get; set; }
        public Enumeradores.ResponsavelPallet ResponsavelPallet { get; set; }
        public Enumeradores.TipoEntradaSaida TipoMovimentacao { get; set; }
        public Enumeradores.TipoLancamento TipoLancamento { get; set; }
        public Enumeradores.SituacaoGestaoPallet Situacao { get; set; }
        public string Observacao { get; set; }
    }
}
