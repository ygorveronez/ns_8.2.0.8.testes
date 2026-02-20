using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class FiltroPesquisaRelatorioProvisaoVolumetria
    {
        public List<int> CodigosTransportador { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosFilial { get; set; }
        public DateTime? DataEmissaoNFInicio { get; set; }
        public DateTime? DataEmissaoNFFim { get; set; }
        public DateTime? DataIntegracaoPagamentoInicio { get; set; }
        public DateTime? DataIntegracaoPagamentoFim { get; set; }
        public DateTime? DataVencimentoInicio { get; set; }
        public DateTime? DataVencimentoFim { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }
    }
}
