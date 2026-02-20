using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoNotificacaoApp : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp>
    {
        #region Construtores

        public TipoOperacaoNotificacaoApp(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp> BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoNotificacaoApp>();
            query = query.Where(obj => obj.TipoOperacao.Codigo == codigoTipoOperacao);
            return query.ToList();
        }
        #endregion

        #region Métodos Privados
        #endregion
    }
}
