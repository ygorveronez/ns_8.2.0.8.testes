using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Canhotos
{
    public class ControleNotificacao : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao>
    {
        public ControleNotificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public void SetarNotificacoesFinalizadas()
        {
            string hql = "UPDATE ControleNotificacao notificacao SET notificacao.Finalizado = :Finalizado";
            var query = this.SessionNHiBernate.CreateQuery(hql);
            query.SetBoolean("Finalizado", false);
            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao> BuscarPorControles()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleNotificacao>();
            var result = from obj in query select obj;
            return result.ToList();
        }
    }
}
