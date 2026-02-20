using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.SuperApp
{
    public class NotificacaoApp : RepositorioBase<Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp>
    {
        public NotificacaoApp(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp>()
                .Where(n => n.Codigo == codigo);

            return query.FirstOrDefault();
        }
        public List<Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.SuperApp.NotificacaoApp>()
                .Where(n => codigos.Contains(n.Codigo));

            return query.ToList();
        }
        #endregion
    }
}
