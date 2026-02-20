namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public sealed class FiltroPesquisaCondicoesPagamentoTransportador
    {
        public string CodigoIntegracao { get; set; }
        public string Estado { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoTipoCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
    }
}
