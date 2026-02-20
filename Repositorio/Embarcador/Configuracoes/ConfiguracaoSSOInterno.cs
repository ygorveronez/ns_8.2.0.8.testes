using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoSSOInterno : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno>
    {
        public ConfiguracaoSSOInterno(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno BuscarConfiguracaoPadrao()
        {
            var consultaConfiguracaoSSOInterno = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSOInterno>();

            return consultaConfiguracaoSSOInterno.FirstOrDefault();
        }
    }
}
