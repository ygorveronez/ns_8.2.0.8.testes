using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public sealed class FiltroPesquisaDocumentoProvisao
    {
        public int CodigoCancelamentoProvisao { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoOcorrencia { get; set; }

        public int CodigoProvisao { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public int CodigoTransportador { get; set; }

        public int CodigoRegraEscrituracao { get; set; }

        public bool ConcatenarComDocumentosSemPrevisao { get; set; }

        public double CpfCnpjTomador { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public List<int> ListaCodigoTransportador { get; set; }

        public Enumeradores.SituacaoProvisaoDocumento SituacaoProvisaoDocumento { get; set; }

        public bool SomenteSemProvisao { get; set; }

        public bool CancelamentoProvisaoContraPartida { get; set; }
        
        public Enumeradores.TipoLocalPrestacao TipoLocalPrestacao { get; set; }

        public bool TipoEtapasDocumentoProvisao { get; set; }

        public bool NaoPermitirProvisionarSemCalculoFrete { get; set; }
    }
}
