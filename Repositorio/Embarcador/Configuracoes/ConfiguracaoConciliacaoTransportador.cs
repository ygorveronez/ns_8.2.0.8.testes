using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoConciliacaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador>
    {
        public ConfiguracaoConciliacaoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador>();
            return query.FirstOrDefault();
        }

        #endregion
    }
}
