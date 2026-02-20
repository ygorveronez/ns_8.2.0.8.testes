using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frotas
{
    public class TipoDestinoOleo : RepositorioBase<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo>
    {
        public TipoDestinoOleo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo> Consultar(string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo> result = Consultar(descricao);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo> Consultar(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result;
        }

        public int ContarConsulta(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo> result = Consultar(descricao);

            return result.Count();
        }
    }
}