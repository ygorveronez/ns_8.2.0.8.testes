using System.Threading.Tasks;

namespace Servicos.Autenticacao
{
    public class SSoConfigService : Dominio.Interfaces.Autenticacao.ISSoConfigService
    {
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO ObterConfiguracaoSSo(string stringConexao, Dominio.Enumeradores.TipoSso tipoSso)
        {
            if (string.IsNullOrWhiteSpace(stringConexao)) return null;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO repositorioConfiguracaoSSo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoSSO(new Repositorio.UnitOfWork(stringConexao));

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO configuracaoSSo = repositorioConfiguracaoSSo.BuscarConfiguracao(tipoSso);

            return configuracaoSSo;
        }
    }
}
