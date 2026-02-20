using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class RegrasAvariaMotivoAvaria : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>
    {
        public RegrasAvariaMotivoAvaria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.RegrasMotivoAvaria>();
            var result = from obj in query where obj.RegrasAutorizacaoAvaria.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}