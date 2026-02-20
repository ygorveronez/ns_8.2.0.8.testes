using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class SeparacaoMercadoriaResponsavel : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel>
    {
        #region Construtores

        public SeparacaoMercadoriaResponsavel(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados


        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel> BuscarPorSeparacaoMercadoria(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.SeparacaoMercadoriaResponsavel>()
                .Where(obj => obj.SeparacaoMercadoria.Codigo == codigo);

            return query
                .Fetch(obj => obj.Responsavel)
                .ToList();
        }

        #endregion
    }
}
