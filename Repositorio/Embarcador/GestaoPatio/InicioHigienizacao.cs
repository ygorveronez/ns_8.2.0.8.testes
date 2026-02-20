using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class InicioHigienizacao : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao>
    {
        #region Construtores

        public InicioHigienizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao BuscarPorCodigo(int codigo)
        {
            var consultaInicioHigienizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao>()
                .Where(o => o.Codigo == codigo);

            return consultaInicioHigienizacao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaInicioHigienizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.InicioHigienizacao>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaInicioHigienizacao.FirstOrDefault();
        }

        #endregion
    }
}
