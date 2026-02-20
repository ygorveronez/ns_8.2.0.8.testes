using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoIsis : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis>
    {
        public IntegracaoIsis(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIsis>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
