using System.Linq;

namespace Repositorio
{
    public class ConfiguracaoIntegracaoLsTranslog : RepositorioBase<Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog>, Dominio.Interfaces.Repositorios.ConfiguracaoIntegracaoLsTranslog
    {
        public ConfiguracaoIntegracaoLsTranslog(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog BuscaPorEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoIntegracaoLsTranslog>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            return result.FirstOrDefault();
        }

    }
}

