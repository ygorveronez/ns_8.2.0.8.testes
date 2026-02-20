using System;

namespace Dominio.ObjetosDeValor.Embarcador.GestaoPatio
{
    public sealed class FluxoGestaoPatioEtapaAdicionar
    {
        public Entidades.Embarcador.Cargas.CargaJanelaCarregamento CargaJanelaCarregamento { get; set; }

        public DateTime DataPrevisaoInicio { get; set; }

        public bool EtapaLiberada { get; set; }

        public Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio FluxoGestaoPatio { get; set; }
    }
}
