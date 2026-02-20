using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoNFSeManual : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual>
    {
        public ConfiguracaoNFSeManual(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoNFSeManual>();

            return query.FirstOrDefault();
        }
    }
}
