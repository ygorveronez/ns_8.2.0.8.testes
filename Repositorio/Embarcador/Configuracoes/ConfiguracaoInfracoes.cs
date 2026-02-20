using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoInfracoes : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes>
    {
        public ConfiguracaoInfracoes(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoInfracoes>();

            return query.FirstOrDefault();
        }
    }
}
