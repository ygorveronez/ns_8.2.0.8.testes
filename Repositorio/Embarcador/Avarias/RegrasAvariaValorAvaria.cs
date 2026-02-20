using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class RegrasAvariaValorAvaria : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>
    {
        public RegrasAvariaValorAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasValorAvaria>();
            var result = from obj in query where obj.RegrasAutorizacaoAvaria.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}