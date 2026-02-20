using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class LeituraDinamicaXmlOrigem : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem>
    {
        public LeituraDinamicaXmlOrigem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem>();
            criteria.Add(NHibernate.Criterion.Expression.Eq("TipoDocumento", tipoDocumento));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem>().Where(obj => obj.Codigo == codigo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento tipoDocumento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.TipoDocumento == tipoDocumento);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }
    }
}