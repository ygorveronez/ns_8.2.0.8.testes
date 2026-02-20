using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaConsultaMonitorIntegracoesSuperApp
    {
        public int CodigoCargaEmbarcador { get; set; }
        public TipoEventoApp? TipoEventoApp { get; set; }
        public SituacaoProcessamentoIntegracao? SituacaoIntegracao { get; set; }
        public DateTime? DataInicioRecebimento { get; set; }
        public DateTime? DataFimRecebimento { get; set; }
        public int CodigoTransportador { get; set; }
    }
}
