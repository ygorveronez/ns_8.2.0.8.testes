using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class RegrasAvariaOrigem : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>
    {
        public RegrasAvariaOrigem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.RegrasOrigem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasOrigem>();
            var result = from obj in query where obj.RegrasAutorizacaoAvaria.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}