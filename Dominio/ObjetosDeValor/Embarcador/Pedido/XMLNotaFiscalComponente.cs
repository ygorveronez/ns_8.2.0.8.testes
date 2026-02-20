namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class XMLNotaFiscalComponente
    {
        public int CodigoComponenteFrete { get; set; }
        public int CodigoCargaPedido { get; set; }
        public decimal Valor { get; set; }

        public bool? IncluirICMS { get; set; }
        public bool? IncluirIntegralmenteContratoFreteTerceiro { get; set; }
    }
}
