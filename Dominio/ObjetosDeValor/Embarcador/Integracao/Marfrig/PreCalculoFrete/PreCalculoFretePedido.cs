namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete
{
    public class PreCalculoFretePedido
    {
        public int protocoloPedido { get; set; }
        public decimal valorFrete { get; set; }
        public decimal baseCalculo { get; set; }
        public decimal valorIcms { get; set; }
        public decimal aliquotaIcms { get; set; }
        public string tipoCarga { get; set; }
        public string tipoOperacao { get; set; }
        public string CST { get; set; }
    }
}
