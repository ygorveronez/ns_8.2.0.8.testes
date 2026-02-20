using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class FimHigienizacao : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao>
    {
        #region Construtores

        public FimHigienizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao BuscarPorCodigo(int codigo)
        {
            var consultaFimHigienizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao>()
                .Where(o => o.Codigo == codigo);

            return consultaFimHigienizacao.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaFimHigienizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FimHigienizacao>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaFimHigienizacao.FirstOrDefault();
        }

        #endregion
    }
}
