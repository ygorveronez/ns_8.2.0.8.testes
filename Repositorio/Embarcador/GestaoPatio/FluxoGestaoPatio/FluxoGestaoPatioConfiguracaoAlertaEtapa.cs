using System.Linq;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class FluxoGestaoPatioConfiguracaoAlertaEtapa : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa>
    {
        public FluxoGestaoPatioConfiguracaoAlertaEtapa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa BuscarPorConfiguracaoEtapa(int codigoConfiguracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxoGestaoPatio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoAlertaEtapa>();
            var result = from obj in query where obj.ConfiguracaoAlerta.Codigo == codigoConfiguracao && obj.EtapaFluxoGestaoPatio == etapaFluxoGestaoPatio select obj;
            return result.FirstOrDefault();
        }

        #endregion
    }
}
