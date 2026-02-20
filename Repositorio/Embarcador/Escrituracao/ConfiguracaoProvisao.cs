using System.Linq;

namespace Repositorio.Embarcador.Escrituracao
{
    public sealed class ConfiguracaoProvisao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao>
    {
        #region Construtores

        public ConfiguracaoProvisao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao BuscarConfiguracao()
        {
            var configuracaoProvisao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.ConfiguracaoProvisao>();

            return configuracaoProvisao.FirstOrDefault();
        }

        #endregion
    }
}
