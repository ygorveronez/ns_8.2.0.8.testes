using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoDocumentacaoAFRMM : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM>
    {
        public ConfiguracaoDocumentacaoAFRMM(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDocumentacaoAFRMM>();
            return query.FirstOrDefault();
        }
    }
}
