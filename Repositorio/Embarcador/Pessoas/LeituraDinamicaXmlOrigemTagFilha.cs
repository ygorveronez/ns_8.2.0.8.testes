using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class LeituraDinamicaXmlOrigemTagFilha : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha>
    {
        public LeituraDinamicaXmlOrigemTagFilha(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta(int codigoLeituraDinamicaXmlOrigem)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("LeituraDinamicaXmlOrigem.Codigo", codigoLeituraDinamicaXmlOrigem));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha> Consultar(int codigoLeituraDinamicaXmlOrigem, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.LeituraDinamicaXmlOrigem.Codigo == codigoLeituraDinamicaXmlOrigem);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
    }
}