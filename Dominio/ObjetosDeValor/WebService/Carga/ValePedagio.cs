using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class ValePedagio
    {
        public int ProtocoloCarga { get; set; }

        public string NumeroValePedagio { get; set; }

        public decimal ValorTotalValePedagio { get; set; }

        public int QuantidadeEixos { get; set; }

        public bool CompraComEixosSuspensos { get; set; }

        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao TipoIntegradora { get; set; }

        public string Observacao { get; set; }

        public List<PracaValePedagio> PracasValePedagio { get; set; }

    }
}
