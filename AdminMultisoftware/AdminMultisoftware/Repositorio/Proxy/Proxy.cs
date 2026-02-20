using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace AdminMultisoftware.Repositorio.Proxy
{
    public class Proxy : RepositorioBase<Dominio.Entidades.Proxy.Proxy>
    {
        public Proxy(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Proxy.Proxy BuscarPorIP(string IP)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proxy.Proxy>();
            var result = from obj in query where obj.IP == IP select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Proxy.Proxy BuscarPorChave(string Chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proxy.Proxy>();
            var result = from obj in query where obj.ChaveEmConsulta == Chave select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Proxy.Proxy BuscarProximo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proxy.Proxy>();
            var result = from obj in query where !obj.EmBloqueio select obj;
            return result.OrderBy(obj => obj.DataUltimaRequisicaoValida).FirstOrDefault();
        }

        public Dominio.Entidades.Proxy.Proxy BuscarProximoBloqueado(int tempoBloqueioHrs)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Proxy.Proxy>();
            var result = from obj in query where obj.EmBloqueio && obj.Databloqueio >= System.DateTime.Now.AddHours(-tempoBloqueioHrs) && obj.Uso > 0 select obj;
            return result.OrderBy(obj => obj.Databloqueio).FirstOrDefault();
        }

    }
}
