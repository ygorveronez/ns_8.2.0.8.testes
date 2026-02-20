using System;

namespace Dominio.ObjetosDeValor.EDI.Pedido
{
    public class CabecalhoDocumento
    {
        public string IdentificacaoRegitro { get; set; }
        public string CodigoIdentificadorMensagem { get; set; }
        public string CNPJTransportador { get; set; }
        public string CodigoParceiro { get; set; }
        public string CNPJEmissora { get; set; }
        public string CodigoRelatorio { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime HotaEmissao { get; set; }
    }
}
