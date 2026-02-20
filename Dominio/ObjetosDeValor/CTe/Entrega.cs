using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.CTe
{
    public class Entrega
    {
        public int codigoLocalidadeOrigem { get; set; }

        public int codigoLocalidadeDestino { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorPrestacaoServico { get; set; }

        public decimal ValorAReceber { get; set; }

        public List<EntregaComponentePrestacao> ComponentesPrestacao { get; set; }

        public List<Documento> Documentos { get; set; }

        public List<DocumentoTransporteAnterior> DocumentosTransporteAnteriores { get; set; }
    }
}