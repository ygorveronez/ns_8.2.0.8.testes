using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Rest.Financeiro
{
    public class RetornoMiro
    {
        public string ChaveDocumento { get; set; }
        public string DocumentoContabil { get; set; }
        public string NumeroDocumento { get; set; }
        public string NumeroMiro { get; set; }
        public DateTime? DataMiro { get; set; }
        public DateTime? Vencimento { get; set; }
        public string CondicaoPagamento { get; set; }
        public string Bloqueio { get; set; }
        public string NumeroEstorno { get; set; }
        public List<PocessamentoSAP> processamentoSAP { get; set; }
    }

    public class PocessamentoSAP
    {
        public string CodigoMensagem { get; set; }
        public string DescricaoMensagem { get; set; }

    }
}
