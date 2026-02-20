namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFreteDadosFrete
    {
        public string tipoCarga { get; set; }        
        public ContratoFreteDadosFreteTarifa tarifa { get; set; }
        public string codigoTipoCargaANTT { get; set; }
        public int distanciaPercorrida { get; set; }
        public decimal valorPedagio { get; set; }
        public decimal INSS { get; set; }
        public decimal IR { get; set; }
        public decimal sestSenat { get; set; }
        public decimal seguro { get; set; }
        public decimal outrosDebitos { get; set; }
        public decimal tarifaAssociacaoCartao { get; set; }
        public decimal valorUnidadeFrete { get; set; }
        public decimal valorKgMercadoria { get; set; }
        public decimal valorPesoSaida { get; set; }
        public decimal valorTotalFrete { get; set; }
    }
}