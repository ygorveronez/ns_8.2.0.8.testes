using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class InicioCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento>
    {
        #region Construtores

        public InicioCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento BuscarPorCodigo(int codigo)
        {
            var consultaInicioCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaInicioCarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaInicioCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.InicioCarregamento>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaInicioCarregamento.FirstOrDefault();
        }

        #endregion
    }
}
