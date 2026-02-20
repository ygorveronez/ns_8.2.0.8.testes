using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.TorreControle
{
    public class FiltroPesquisaConsultaMonitorNotificacaoApp
    {
        public int CodigoCarga { get; set; }
        public int CodigoMotorista { get; set; }
        public TipoNotificacaoApp? TipoNotificacaoApp { get; set; }
        public SituacaoIntegracao? SituacaoIntegracao { get; set; }
        public DateTime? DataInicioEnvio { get; set; }
        public DateTime? DataFimEnvio { get; set; }
        public int CodigoTransportador { get; set; }
        public int CodigoChamado { get; set; }
    }
}
