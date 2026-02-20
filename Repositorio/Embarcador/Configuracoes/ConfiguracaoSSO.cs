using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoSSO : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO>
    {
        public ConfiguracaoSSO(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO BuscarConfiguracaoPadrao()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO>();

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO BuscarConfiguracao(Dominio.Enumeradores.TipoSso tipoSso)
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO>();

            return query.Where(x => x.TipoSSo == tipoSso && x.Ativo).FirstOrDefault();
        }

        public bool PossuiSSO()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO>();

            return query.Where(x => x.Ativo).Any();
        }
    }
}
