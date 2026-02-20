using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace AdminMultisoftware.Repositorio.Mobile
{
    public class NotificacaoOneSignal : RepositorioBase<Dominio.Entidades.Mobile.NotificacaoOneSignal>
    {
        public NotificacaoOneSignal(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public List<Dominio.Entidades.Mobile.NotificacaoOneSignal> BuscarPorUsuarioMobile(int codigoUsuarioMobile, int ultimoCodigoRecebido = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.NotificacaoOneSignal>();
            var result = from obj in query where obj.Motorista.Codigo == codigoUsuarioMobile select obj;

            if (ultimoCodigoRecebido > 0)
                result = result.Where(o => o.Codigo > ultimoCodigoRecebido);

            return result.ToList();
        }

        public List<Dominio.Entidades.Mobile.NotificacaoOneSignal> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Mobile.NotificacaoOneSignal>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

    }
}
