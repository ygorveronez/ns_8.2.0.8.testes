using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class PneuSucata : RepositorioBase<Dominio.Entidades.Embarcador.Frota.PneuSucata>
    {
        #region Construtores

        public PneuSucata(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion


        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Frota.PneuSucata BuscarPorPneu(int codigoPneu)
        {
            var pneuSucata = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.PneuSucata>()
                .Where(o => o.Pneu.Codigo == codigoPneu)
                .FirstOrDefault();

            return pneuSucata;
        }

        #endregion

    }
}
