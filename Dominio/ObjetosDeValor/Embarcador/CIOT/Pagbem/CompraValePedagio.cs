namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class CompraValePedagio
    {
        public bool isViagemLiberada { get; set; }
        public ContratoFreteMotorista motorista { get; set; }
        public ContratoFreteOrigem origem { get; set; }
        public ContratoFreteDestino destino { get; set; }
        public ContratoFretePeriodoViagem periodoViagem { get; set; }
        public ContratoFreteRota rota { get; set; }
        public ContratoFreteVeiculos veiculos { get; set; }
        public ContratoFreteDocumentosViagem[] documentosViagem { get; set; }
        public string numeroViagemCliente { get; set; }
        public string operador { get; set; }
        public string numeroCartaoPagbem { get; set; }
    }
}
