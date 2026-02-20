using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class ProvisaoManual : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual>
    {
        public ProvisaoManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual> Consultar(int centroResultado, int filial, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual> query = ObterQueryConsulta(centroResultado, filial);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query
                .Fetch(obj => obj.Filial)
                .Fetch(obj => obj.CentroResultado)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ContarConsulta(int centroResultado, int filial)
        {
            return ObterQueryConsulta(centroResultado, filial).Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual> ObterQueryConsulta(int centroResultado, int filial)
        {
            IQueryable<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ProvisaoManual>();

            if (centroResultado > 0)
                query = query.Where(o => o.CentroResultado.Codigo == centroResultado);

            if (filial > 0)
                query = query.Where(o => o.Filial.Codigo == filial);

            return query;
        }

        #endregion
    }
}
