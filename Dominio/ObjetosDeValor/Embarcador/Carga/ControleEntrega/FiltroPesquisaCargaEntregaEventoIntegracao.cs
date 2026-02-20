using System;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega
{
    public class FiltroPesquisaCargaEntregaEventoIntegracao
    {
        public string CodigoCargaEmbarcador { get; set; }
        public int CodigoTipoDeOcorrencia { get; set; }
        public int CodigoTipoIntegracao { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }
}
