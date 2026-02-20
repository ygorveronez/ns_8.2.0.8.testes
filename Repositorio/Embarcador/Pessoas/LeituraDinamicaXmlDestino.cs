using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class LeituraDinamicaXmlDestino : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino>
    {
        public LeituraDinamicaXmlDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta()
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino>();
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino> Consultar(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino>();

            var result = from obj in query select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
    }
}