namespace Dominio.ObjetosDeValor.Embarcador.Imposto
{
    public class ParametroCalculoICMS
    {
        public int IDProposta { get; set; }
        public string UFOrigem { get; set; }
        public string UFDestino { get; set; }
        public string UFEmissor { get; set; }
        public string UFTomador { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorServico { get; set; }
        public int CodigoAtividade { get; set; }
    }
}
