using System.Threading.Tasks;

namespace Dominio.Interfaces.Autenticacao
{
    public interface ISSoConfigService
    {
        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoSSO ObterConfiguracaoSSo(string stringConexao, Dominio.Enumeradores.TipoSso tipoSso);
    }
}
