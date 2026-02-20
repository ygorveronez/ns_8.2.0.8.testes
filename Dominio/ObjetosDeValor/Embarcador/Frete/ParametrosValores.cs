namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParametrosValores
    {
        public dynamic Valores { get; set; }
        public dynamic Observacoes { get; set; }
        public dynamic ValoresMinimosGarantidos { get; set; }
        public dynamic ValoresMaximos { get; set; }
        public dynamic ValoresBases { get; set; }
        public dynamic ValoresExcedentes { get; set; }
        public dynamic PercentuaisPagamentoAgregados { get; set; }
        public Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaFreteCliente { get; set; }
    }
}
