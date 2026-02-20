using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class Sefaz : RepositorioBase<Dominio.Entidades.Sefaz>
    {
        public Sefaz(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Sefaz BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Sefaz>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Sefaz BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Sefaz>();
            var result = from obj in query where obj.Descricao.Equals(descricao) && obj.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Sefaz> Consulta(string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Sefaz>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Sefaz>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));


            return result.Count();
        }
    }
}
