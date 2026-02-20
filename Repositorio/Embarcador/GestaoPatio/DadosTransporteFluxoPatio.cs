using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class DadosTransporteFluxoPatio : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio>
    {
        #region Construtores

        public DadosTransporteFluxoPatio(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados


        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio>()
                .Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio BuscarPorSolicitacaoVeiculo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio>()
                .Where(obj => obj.SolicitacaoVeiculo.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio BuscarPorCarga(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.DadosTransporteFluxoPatio>()
                .Where(obj => obj.SolicitacaoVeiculo.Carga.Codigo == codigo);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
