using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoAcessoViaToken : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken>
    {
        #region Construtores

        public ConfiguracaoAcessoViaToken(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public ConfiguracaoAcessoViaToken(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken Buscar()
        {
            IQueryable<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken> consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAcessoViaToken>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
