using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CTeProduto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CTeProduto>
    {
        #region Construtores

        public CTeProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        public List<Dominio.Entidades.Embarcador.Cargas.CTeProduto> ObterProdutosPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CTeProduto>();
            query = query.Where(o => o.CTe.Codigo == codigoCTe);
            return query
                .Fetch(o => o.Produto)
                .ToList();
        }

    }
}
