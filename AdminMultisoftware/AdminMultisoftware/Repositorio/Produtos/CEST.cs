using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Produtos
{
    public class CEST : RepositorioBase<Dominio.Entidades.Produtos.CEST>
    {
        public CEST(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Produtos.CEST BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.CEST>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produtos.CEST BuscarPorCEST(string CEST)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.CEST>();
            var result = from obj in query where obj.Numero.StartsWith(CEST) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produtos.CEST BuscarPorNCM(string NCM)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.CEST>();
            var result = from obj in query where obj.CodigoNCM.StartsWith(NCM) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Produtos.CEST> ConsultarCEST(string codigoCEST, string NCM, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.CEST>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(NCM))
                result = result.Where(o => o.CodigoNCM.Contains(NCM));

            if (!string.IsNullOrWhiteSpace(codigoCEST))
                result = result.Where(o => o.Numero.StartsWith(codigoCEST));

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaCEST(string codigoCEST, string NCM, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produtos.CEST>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(NCM))
                result = result.Where(o => o.CodigoNCM.Contains(NCM));

            if (!string.IsNullOrWhiteSpace(codigoCEST))
                result = result.Where(o => o.Numero.StartsWith(codigoCEST));

            return result.Count();
        }

    }
}
