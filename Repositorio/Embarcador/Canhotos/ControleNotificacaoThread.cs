using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Canhotos
{
    public class ControleNotificacaoThread : RepositorioBase<Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread>
    {
        public ControleNotificacaoThread(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread BuscarPadrao()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Canhotos.ControleNotificacaoThread>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }
    }
}
