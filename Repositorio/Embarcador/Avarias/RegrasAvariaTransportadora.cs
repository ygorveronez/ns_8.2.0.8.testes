using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class RegrasAvariaTransportadora : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>
    {
        public RegrasAvariaTransportadora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasTransportadora>();
            var result = from obj in query where obj.RegrasAutorizacaoAvaria.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}