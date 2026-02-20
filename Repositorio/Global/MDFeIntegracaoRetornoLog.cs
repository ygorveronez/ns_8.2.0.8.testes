using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class MDFeIntegracaoRetornoLog : RepositorioBase<Dominio.Entidades.MDFeIntegracaoRetornoLog>, Dominio.Interfaces.Repositorios.MDFeIntegracaoRetornoLog
    {
        public MDFeIntegracaoRetornoLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.MDFeIntegracaoRetornoLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeIntegracaoRetornoLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MDFeIntegracaoRetornoLog> BuscarPorRetornoIntegracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MDFeIntegracaoRetornoLog>();
            var result = from obj in query where obj.MDFeIntegracaoRetorno.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
