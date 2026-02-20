using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaDocumentoEscrituracao
    {
        public int CodigoFilial { get; set; }

        public int CodigoLoteEscrituracao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoModeloDocumento { get; set; }
        public int CodigoTipoOperacao { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public bool SomentePagamentoLiberado { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? TipoServicoMultisoftware { get; set; }

        public int IntervaloParaEscrituracaoDocumento { get; set; }
    }
}
