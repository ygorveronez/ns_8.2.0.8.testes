using System.Linq;


namespace Repositorio
{
    public class ConfiguracaoEmail : RepositorioBase<Dominio.Entidades.ConfiguracaoEmail>, Dominio.Interfaces.Repositorios.ConfiguracaoEmail
    {
        public ConfiguracaoEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.ConfiguracaoEmail BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConfiguracaoEmail>();
            var result = from obj in query select obj;
            return result.FirstOrDefault();
        }       

    }
}
