using System.Linq;

namespace Repositorio.Embarcador.BI
{
    public class ConfigracaoBI : RepositorioBase<Dominio.Entidades.Embarcador.BI.ConfiguracaoBI>
    {
        public ConfigracaoBI(UnitOfWork unitOfWork) : base(unitOfWork)
        {

        }


        public Dominio.Entidades.Embarcador.BI.ConfiguracaoBI BuscarPadrao()
        {
            var query = SessionNHiBernate.Query< Dominio.Entidades.Embarcador.BI.ConfiguracaoBI>();
            
            return query.FirstOrDefault();
        }
    }
}
