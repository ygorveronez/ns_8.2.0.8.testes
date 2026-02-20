using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Transportadores
{
    public class ConfiguracaoIntegracaoEmpresa : RepositorioBase<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa>
    {
        public ConfiguracaoIntegracaoEmpresa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa BuscarPorEmpresa(int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Transportadores.ConfiguracaoIntegracaoEmpresa>();
            var result = from obj in query where obj.Empresa.Codigo == empresa select obj;
            return result.FirstOrDefault();
        }
    }
}
