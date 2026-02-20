using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaDadosTipoOperacaoSemFilialEmissora : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaDadosTipoOperacaoSemFilialEmissora>
    {
        #region Construtores

        public CargaDadosTipoOperacaoSemFilialEmissora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaDadosTipoOperacaoSemFilialEmissora BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTipoOperacaoSemFilialEmissora>();
            query = query.Where(o => o.Carga.Codigo == codigoCarga);
            return query.FirstOrDefault();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDadosTipoOperacaoSemFilialEmissora>();
            query = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return query.Any();
        }

        #endregion
    }
}
