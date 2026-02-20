using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class InicioDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento>
    {
        #region Construtores

        public InicioDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento BuscarPorCodigo(int codigo)
        {
            var consultaInicioDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaInicioDescarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaInicioDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.InicioDescarregamento>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaInicioDescarregamento.FirstOrDefault();
        }

        #endregion
    }
}
