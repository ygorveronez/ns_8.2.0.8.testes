using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaPedidoOcorrenciaColetaEntregaIntegracao
    {
        public string CodigoCargaEmbarcador { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public int CodigoTipoDeOcorrencia { get; set; }
        public int CodigoTransportador { get; set; }
        public double CodigoTomador { get; set; }
        public int CodigoTipoIntegracao { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }   
}
