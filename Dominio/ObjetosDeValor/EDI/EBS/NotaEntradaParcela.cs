using System;

namespace Dominio.ObjetosDeValor.EDI.EBS
{
    public class NotaEntradaParcela
    {
        public string TipoRegistro { get; set; }
        public string TipoParcela { get; set; }
        public string NumeroFatura { get; set; }
        public string TipoTitulo { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorParcela { get; set; }
        public decimal ValorTarifa { get; set; }
        public string Brancos { get; set; }
        public string UsoEBS { get; set; }
        public string Sequencia { get; set; }
    }
}
