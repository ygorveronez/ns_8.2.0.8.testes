using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ConfiguracaoCSRT : RepositorioBase<Dominio.Entidades.ConfiguracaoCSRT>
    {
        public ConfiguracaoCSRT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoCSRT BuscarPorEstado(string uf)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoCSRT>();
            var result = from obj in query where obj.Estado.Sigla == uf select obj;
            return result.FirstOrDefault();
        }
    }
}
