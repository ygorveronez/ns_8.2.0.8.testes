using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pallets
{
    public class DevolucaoPalletSituacao : RepositorioBase<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao>
    {
        public DevolucaoPalletSituacao(UnitOfWork unitOfWork) : base(unitOfWork)        {        }

        public Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao BuscarPorCodigo (int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao> BuscarPorDevolucao(int devolucao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pallets.DevolucaoPalletSituacao>();

            var result = from obj in query where obj.Devolucao.Codigo == devolucao select obj;

            return result.ToList();
        }
    }
}
