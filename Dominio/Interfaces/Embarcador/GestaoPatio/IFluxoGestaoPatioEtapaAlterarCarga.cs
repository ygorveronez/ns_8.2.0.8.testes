namespace Dominio.Interfaces.Embarcador.GestaoPatio
{
    public interface IFluxoGestaoPatioEtapaAlterarCarga
    {
        void DefinirCarga(Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Entidades.Embarcador.Cargas.Carga carga, bool etapaLiberada);

        void TrocarCarga(Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, Entidades.Embarcador.Cargas.Carga cargaNova);
    }
}
