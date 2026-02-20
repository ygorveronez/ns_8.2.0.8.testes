using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Global
{
    public class Dica : RepositorioBase<Dominio.Entidades.Dica>
    {

        public Dica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Dica> Consultar(int issue, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(issue);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(int issue)
        {
            var result = Consultar(issue);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Dica> Consultar(int issue)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Dica>()
                .Where(o => o.CodigoAjuda == issue);

            return query;
        }

        #endregion

    }
}
