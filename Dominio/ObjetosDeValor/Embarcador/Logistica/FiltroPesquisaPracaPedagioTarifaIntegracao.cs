using System;

namespace Dominio.ObjetosDeValor.Embarcador.Logistica
{
    public sealed class FiltroPesquisaPracaPedagioTarifaIntegracao
    {
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }
}
