using System;

namespace Dominio.ObjetosDeValor.WebService.ValePedagio
{
    public class ConsultaValePedagio
    {
        public virtual int ProtocoloCarga { get; set; }
        public virtual string NumeroValePedagio { get; set; }
        public virtual decimal ValorValePedagio { get; set; }
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRotaSemParar TipoRota { get; set; }
        public virtual int QuantidadeEixos { get; set; }
        public virtual string SituacaoIntegracao { get; set; }
        public virtual string ProblemaIntegracao { get; set; }
        public virtual DateTime DataIntegracao { get; set; }
    }
}