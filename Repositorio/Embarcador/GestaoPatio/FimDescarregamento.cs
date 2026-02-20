using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class FimDescarregamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento>
    {
        #region Construtores

        public FimDescarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento BuscarPorCodigo(int codigo)
        {
            var consultaFimDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaFimDescarregamento.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaFimDescarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FimDescarregamento>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaFimDescarregamento.FirstOrDefault();
        }

        #endregion
    }
}
