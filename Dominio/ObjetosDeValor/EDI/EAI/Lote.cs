using System;

namespace Dominio.ObjetosDeValor.EDI.EAI
{
    public class Lote
    {
        public string Estabelecimento { get; set; }
        public string Serie { get; set; }
        public string NumeroDocumento { get; set; }
        public string CodigoEmitente { get; set; }
        public string ContaContabil { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataTransacao { get; set; }
        public decimal ValorDesconto { get; set; }
        public string ObservacaoDocumento { get; set; }
    }
}
