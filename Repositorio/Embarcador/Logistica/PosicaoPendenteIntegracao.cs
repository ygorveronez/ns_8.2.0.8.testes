using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoPendenteIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao>
    {
        #region Métodos públicos

        public PosicaoPendenteIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao> BuscarComLimite(int quantidadePosicoes = 100)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao>();
            consulta = consulta.OrderBy(obj => obj.DataVeiculo);
            return consulta.Fetch(obj => obj.Veiculo).Take(quantidadePosicoes).ToList();
        }

        #endregion
    }
}
