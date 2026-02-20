using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class UnidadeDeMedida : RepositorioBase<Dominio.Entidades.UnidadeDeMedida>, Dominio.Interfaces.Repositorios.UnidadeDeMedida
    {
        public UnidadeDeMedida(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.UnidadeDeMedida BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.UnidadeDeMedida BuscarPorSigla(string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();

            var result = from obj in query where obj.Sigla.Equals(sigla) select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.UnidadeDeMedida BuscarPorCodigoUnidade(string codigoUnidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();

            var result = from obj in query where obj.CodigoDaUnidade.Equals(codigoUnidade) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.UnidadeDeMedida> BuscarTodos(string status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();
            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            return result.OrderBy(o => o.Sigla).ToList();
        }

        public List<Dominio.Entidades.UnidadeDeMedida> Consultar(string descricao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();
            var result = from obj in query where obj.Descricao.Contains(descricao) && obj.Status.Equals("A") select obj;
            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();
            var result = from obj in query where obj.Descricao.Contains(descricao) && obj.Status.Equals("A") select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.UnidadeDeMedida> Consultar(string descricao, string sigla, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(obj => obj.Sigla.Contains(sigla));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.UnidadeDeMedida>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(obj => obj.Sigla.Contains(sigla));

            return result.Count();
        }

    }
}
