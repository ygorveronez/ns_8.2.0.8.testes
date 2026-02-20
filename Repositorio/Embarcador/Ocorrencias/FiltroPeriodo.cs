using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class FiltroPeriodo : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo>
    {
        public FiltroPeriodo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo> BuscarPorTipoOcorrencia(int tipoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo>();

            var result = from o in query where o.TipoOcorrencia.Codigo == tipoOcorrencia select o;

            return result
                .Fetch(obj => obj.Remetente)
                .Fetch(obj => obj.Destinatario)
                .ToList();
        }
    }
}
