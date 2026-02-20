using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Bidding.AlcadasBidding
{
    public class RegraAutorizacaoBidding : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>
    {
        public RegraAutorizacaoBidding(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public bool BuscarRegistroAtivo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>().Where(o => o.Ativo);

            return query.Count() > 0;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        #endregion Métodos Privados
    }
}
