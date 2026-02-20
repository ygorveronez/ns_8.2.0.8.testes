using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public class ModalFerroviario
    {
        public Enumeradores.TipoTrafego TipoTrafego { get; set; }
        public string NumeroFluxoFerroviario { get; set; }
        public string ChaveCTeFerroviaOrigem { get; set; }
        public decimal ValorFrete { get; set; }
        public virtual Enumeradores.FerroviaResponsavel? ResponsavelFaturamento { get; set; }
        public virtual Enumeradores.FerroviaResponsavel? FerroviaEmitente { get; set; }
        public List<ModalFerroviarioFerrovia> Ferrovias { get; set; }
    }
}
