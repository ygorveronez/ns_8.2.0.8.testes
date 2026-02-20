using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class TabelaFreteClienteItemParametroBaseCalculo
    {
        public int Codigo { get; set; }
        public int CodigoItem { get; set; }
        public TipoCampoValorTabelaFrete TipoValor { get; set; }
        public TipoParametroBaseTabelaFrete TipoObjeto { get; set; }
        public decimal Valor { get; set; }
    }
}