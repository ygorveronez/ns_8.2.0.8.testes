using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escalas
{
    public class ExpedicaoEscala : RepositorioBase<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala>
    {
        #region Construtores

        public ExpedicaoEscala(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala BuscarPorCodigo(int codigo)
        {
            var consultaExpedicaoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala>()
                .Where(o => o.Codigo == codigo);

            return consultaExpedicaoEscala.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala> BuscarPorGeracaoEscala(int codigoGeracaoEscala)
        {
            var consultaExpedicaoEscala = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escalas.ExpedicaoEscala>()
                .Where(o => o.GeracaoEscala.Codigo == codigoGeracaoEscala);

            consultaExpedicaoEscala = consultaExpedicaoEscala
                .Fetch(o => o.CentroCarregamento)
                .Fetch(o => o.ProdutoEmbarcador).ThenFetch(o => o.Unidade);

            return consultaExpedicaoEscala.ToList();
        }

        #endregion
    }
}
