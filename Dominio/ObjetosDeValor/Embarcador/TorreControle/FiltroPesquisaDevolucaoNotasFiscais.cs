using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaDevolucaoNotasFiscais
    {
        public DateTime? DataInicialEmissaoNFD { get; set; }
        public DateTime? DataFinalEmissaoNFD { get; set; }
        public DateTime? DataInicialChamado { get; set; }
        public DateTime? DataFinalChamado { get; set; }
        public string CodigosNotaFiscalDevolucao { get; set; }
        public List<int> CodigosNotaFiscalOrigem { get; set; }
        public List<int> CodigosTipoOperacao { get; set; }
        public List<int> CodigosGrupoTipoOperacao { get; set; }
        public List<int> CodigosCargas { get; set; }
        public List<int> CodigosChamados { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoCliente { get; set; }
        public string PedidoEmbarcador { get; set; }
        public string PedidoCliente { get; set; }
    }
}
