using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Bidding
{
    public class TipoBiddingAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo>
    {
        public TipoBiddingAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo> BuscarPorTipoBidding(int codigoTipoBidding)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBiddingAnexo>();

            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigoTipoBidding select obj;

            return result.ToList();
        }

        #endregion Métodos Públicos

    }
}
