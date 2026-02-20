namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFretePagamentoFreteFreteQuitacaoRegras
    {
        public string tipoTolerancia { get; set; }
        public string freteTipoPeso { get; set; }
        public decimal freteLimiteSuperior { get; set; }
        public string quebraTipoCobranca { get; set; }
        public decimal quebraTolerancia { get; set; }
        public string avariaTipoCobranca { get; set; }
    }
}
