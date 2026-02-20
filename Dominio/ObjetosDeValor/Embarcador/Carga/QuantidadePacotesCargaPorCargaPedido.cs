namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public struct QuantidadePacotesCargaPorCargaPedido
    {
        public int CPE_CODIGO { get; set; }
        public int QTD_PACOTES { get; set; }
        public int QTD_CTES_ANTERIORES { get; set; }
        public decimal PERCENTUAL { get; set; }
    }
}
