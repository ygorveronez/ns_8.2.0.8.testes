using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.RegraAutorizacao
{
    public class Alcada<TAlcada, TRegra, TPropriedade> : RepositorioBase<TAlcada>
        where TAlcada : Dominio.Entidades.Embarcador.RegraAutorizacao.Alcada<TRegra, TPropriedade>
        where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
    {
        #region Construtores

        public Alcada(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public TAlcada BuscarPorCodigo(int codigo)
        {
            var alcada = this.SessionNHiBernate.Query<TAlcada>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return alcada;
        }

        public IList<TAlcada> BuscarPorRegra(int codigoRegra)
        {
            var listaAlcadas = this.SessionNHiBernate.Query<TAlcada>()
                .Where(o => o.RegrasAutorizacao.Codigo == codigoRegra)
                .ToList();

            return listaAlcadas;
        }

        #endregion
    }
}
