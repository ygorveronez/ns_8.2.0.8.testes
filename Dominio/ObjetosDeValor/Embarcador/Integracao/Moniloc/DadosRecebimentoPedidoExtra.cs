namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Moniloc
{
    public class DadosRecebimentoPedidoExtraProduto
    {
        public string CodigoProduto { get; set; }
        public decimal QuantidadeSolicitada { get; set; }
        public string Embalagem { get; set; }
        public decimal AlturaEmbalagem { get; set; }
        public decimal LarguraEmbalagem { get; set; }
        public decimal ComprimentoEmbalagem { get; set; }
        public decimal PesoEmbalagem { get; set; }
        public decimal QtdProdutoEmbalagem { get; set; }
        public decimal PesoProduto { get; set; }
        public int EmpilhamentoMaximo { get; set; }
        public string Palet { get; set; }
        public decimal AlturaPalet { get; set; }
        public decimal LarguraPalet { get; set; }
        public decimal ComprimentoPalet { get; set; }
        public decimal PesoPalet { get; set; }
        public decimal QtdEmbalagemPorCamada { get; set; }
        public string CodigoOrigemDestino { get; set; }
        public string HoraColetaEntrega { get; set; }
        public string Reservado { get; set; }
    }

    public class DadosRecebimentoPedidoExtraEmbalagem
    {
        public string CodigoEmbalagem { get; set; }
        public decimal QuantidadeSolicitada { get; set; }
        public decimal AlturaEmbalagem { get; set; }
        public decimal LarguraEmbalagem { get; set; }
        public decimal ComprimentoEmbalagem { get; set; }
        public decimal PesoEmbalagem { get; set; }
        public string Reservado { get; set; }
    }
}
