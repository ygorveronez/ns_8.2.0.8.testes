namespace Dominio.Interfaces.Embarcador.GestaoPatio
{
    public interface IFluxoGestaoPatioEtapaLiberarAutomaticamente
    {
        void LiberarProximaEtapaAutomaticamente(Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio);
    }
}
