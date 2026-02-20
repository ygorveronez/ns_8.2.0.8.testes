using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Swagger
{
    public class ConfiguracaoSwagger : RepositorioBase<Dominio.Entidades.Embarcador.Swagger.ConfiguracaoSwagger>
    {
        public ConfiguracaoSwagger(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public async Task<bool> ValidarCredenciaisAsync(string usuario, string senha)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Swagger.ConfiguracaoSwagger>();

            return await query.AnyAsync(a => a.Usuario == usuario && a.Senha == senha);
        }

        #endregion Métodos Públicos
    }
}
