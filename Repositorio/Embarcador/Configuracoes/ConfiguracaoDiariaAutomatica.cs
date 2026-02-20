using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class ConfiguracaoDiariaAutomatica : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica>
    {
        public ConfiguracaoDiariaAutomatica(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica BuscarConfiguracaoPadrao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoDiariaAutomatica>();
            return query.FirstOrDefault();
        }


        #region Métodos Privados

        #endregion
    }
}
