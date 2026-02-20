namespace Dominio.ObjetosDeValor.WebService.Rest.Frete
{
    public class DadosInformarValorFreteOperador
    {
        public int CodigoCarga { get; set; }
        public int CodigoMotivo { get; set; }
        public decimal ValorFrete { get; set; }
        public string Observacao { get; set; }
        public bool FreteFilialEmissoraOperador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }
        public decimal ValorCotacaoMoeda { get; set; }
        public decimal ValorTotalMoeda { get; set; }
        public bool AvancarCarga { get; set; }
    }
}