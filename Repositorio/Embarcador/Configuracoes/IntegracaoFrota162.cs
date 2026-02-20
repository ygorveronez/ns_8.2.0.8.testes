using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoFrota162 : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162>
    {
        public IntegracaoFrota162(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162 Buscar()
        {
            var consultaIntegracaoTargetGeral = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrota162>();

            return consultaIntegracaoTargetGeral.FirstOrDefault();
        }

        #endregion
    }
}
