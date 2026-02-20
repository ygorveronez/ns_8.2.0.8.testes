using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoTargetGeral : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral>
    {
        public IntegracaoTargetGeral(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral Buscar()
        {
            var consultaIntegracaoTargetGeral = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoTargetGeral>();

            return consultaIntegracaoTargetGeral.FirstOrDefault();
        }

        #endregion
    }
}
