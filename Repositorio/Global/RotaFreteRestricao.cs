using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RotaFreteRestricao : RepositorioBase<Dominio.Entidades.RotaFreteRestricao>
    {
        public RotaFreteRestricao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.RotaFreteRestricao BuscarPorCodigo(int codigo)
        {
            var restricao = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteRestricao>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return restricao;
        }

        public List<Dominio.Entidades.RotaFreteRestricao> BuscarPorRotaFrete(int codigo)
        {
            var listaRestricao = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteRestricao>()
                .Where(o => o.RotaFrete.Codigo == codigo)
                .ToList();

            return listaRestricao;
        }
    }
}
