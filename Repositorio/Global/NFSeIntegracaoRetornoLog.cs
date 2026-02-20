using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class NFSeIntegracaoRetornoLog : RepositorioBase<Dominio.Entidades.NFSeIntegracaoRetornoLog>, Dominio.Interfaces.Repositorios.NFSeIntegracaoRetornoLog
    {
        public NFSeIntegracaoRetornoLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.NFSeIntegracaoRetornoLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetornoLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NFSeIntegracaoRetornoLog> BuscarPorRetornoIntegracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSeIntegracaoRetornoLog>();
            var result = from obj in query where obj.NFSeIntegracaoRetorno.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
