using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class CTeIntegracaoRetornoLog : RepositorioBase<Dominio.Entidades.CTeIntegracaoRetornoLog>, Dominio.Interfaces.Repositorios.CTeIntegracaoRetornoLog
    {
        public CTeIntegracaoRetornoLog(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CTeIntegracaoRetornoLog BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetornoLog>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.CTeIntegracaoRetornoLog> BuscarPorRetornoIntegracao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeIntegracaoRetornoLog>();
            var result = from obj in query where obj.CTeIntegracaoRetorno.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
