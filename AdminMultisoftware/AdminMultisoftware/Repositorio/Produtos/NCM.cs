using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Produtos
{
    public class NCM : RepositorioBase<Dominio.Entidades.Produtos.NCM>
    {
        public NCM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Produtos.NCM BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.NCM>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produtos.NCM BuscarPorNCM(string ncm)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.NCM>();
            var result = from obj in query where obj.Numero.StartsWith(ncm) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Produtos.NCM> ConsultarNCM(string codigoNCM, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.NCM>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoNCM))
                result = result.Where(o => o.Numero.StartsWith(codigoNCM));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaNCM(string codigoNCM, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.NCM>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoNCM))
                result = result.Where(o => o.Numero.StartsWith(codigoNCM));

            return result.Count();
        }

    }
}
