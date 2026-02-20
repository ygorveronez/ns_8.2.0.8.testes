using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Produtos
{
    public class ONU : RepositorioBase<Dominio.Entidades.Produtos.ONU>
    {
        public ONU(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Produtos.ONU BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.ONU>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produtos.ONU BuscarPorONU(string onu)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.ONU>();
            var result = from obj in query where obj.Numero.StartsWith(onu) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Produtos.ONU> ConsultarONU(string codigoONU, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.ONU>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoONU))
                result = result.Where(o => o.Numero.StartsWith(codigoONU));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaONU(string codigoONU, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.ONU>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoONU))
                result = result.Where(o => o.Numero.StartsWith(codigoONU));

            return result.Count();
        }

    }
}
