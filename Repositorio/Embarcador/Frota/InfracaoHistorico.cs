using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public sealed class InfracaoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Frota.InfracaoHistorico>
    {
        #region Construtores

        public InfracaoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Frota.InfracaoHistorico BuscarPorCodigo(int codigo)
        {
            var infracaoHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.InfracaoHistorico>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return infracaoHistorico;
        }

        #endregion
    }
}
