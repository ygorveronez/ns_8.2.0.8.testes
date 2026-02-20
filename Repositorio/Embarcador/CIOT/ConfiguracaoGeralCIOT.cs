using System.Linq;

namespace Repositorio.Embarcador.CIOT
{
    public class ConfiguracaoGeralCIOT : RepositorioBase<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT>
    {
        public ConfiguracaoGeralCIOT(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CIOT.ConfiguracaoGeralCIOT>();

            return query.FirstOrDefault();
        }
    }
}