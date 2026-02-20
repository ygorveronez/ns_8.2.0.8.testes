using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes
    {
        public DateTime DataInicialCarga { get; set; }
        public DateTime DataFinalCarga { get; set; }
        public DateTime DataInicioIntegracao { get; set; }
        public DateTime DataFinalIntegracao { get; set; }
        public DateTime DataInicioEncerramento { get; set; }
        public DateTime DataFinalEncerramento { get; set; }
        public SituacaoIntegracao? Situacao { get; set; }
        public TipoIntegracao? TipoIntegracao { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoFilial { get; set; }
    }
}
