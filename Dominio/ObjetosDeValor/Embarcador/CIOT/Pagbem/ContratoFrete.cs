using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CIOT.Pagbem
{
    public class ContratoFrete
    {
        public bool isViagemLiberada { get; set; }
        public ContratoFreteDadosFrete dadosFrete { get; set; }
        public ContratoFreteDocumentosViagem[] documentosViagem { get; set; }
        public ContratoFreteContratado contratado { get; set; }
        //public ContratoFreteSubontratado subcontratado { get; set; }
        public ContratoFreteMotorista motorista { get; set; }
        public ContratoFreteOrigem origem { get; set; }
        public ContratoFreteDestino destino { get; set; }
        public ContratoFretePeriodoViagem periodoViagem { get; set; }
        //public ContratoFreteRota rota { get; set; }
        public ContratoFreteDadosCarga dadosCarga { get; set; }
        public ContratoFreteVeiculos veiculos { get; set; }
        public ContratoFretePagamentoFrete pagamentoFrete { get; set; }
        public string numeroViagemCliente { get; set; }
        public List<string> produtos { get; set; }
        public string operador { get; set; }
        public Subcontratado subcontratado { get; set; }
        public Retorno retorno { get; set; }
    }
}