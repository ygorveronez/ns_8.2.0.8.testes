using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class FimCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento>
    {
        #region Construtores

        public FimCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        
        public Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento BuscarPorCodigo(int codigo)
        {
            var consultaFimCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaFimCarregamento.FirstOrDefault();
        }
        
        public Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento BuscarPorFluxoGestaoPatio(int codigoFluxoGestaoPatio)
        {
            var consultaFimCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.FimCarregamento>()
                .Where(o => o.FluxoGestaoPatio.Codigo == codigoFluxoGestaoPatio);

            return consultaFimCarregamento.FirstOrDefault();
        }

        #endregion
    }
}
