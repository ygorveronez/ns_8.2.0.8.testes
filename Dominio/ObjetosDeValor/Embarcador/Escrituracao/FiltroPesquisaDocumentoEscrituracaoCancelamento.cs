using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroPesquisaDocumentoEscrituracaoCancelamento
    {
        public int CodigoFilial { get; set; }

        public int CodigoLoteEscrituracaoCancelamento { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoModeloDocumento { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public bool SomentePagamentoLiberado { get; set; }

        public bool SelecionarTodos { get; set; }
    }
}
