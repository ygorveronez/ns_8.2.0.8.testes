using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoRavex : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex>
    {
        public IntegracaoRavex(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex Buscar()
        {
            var consultaIntegracaoMicDta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRavex>();

            return consultaIntegracaoMicDta.FirstOrDefault();
        }

        #endregion
    }
}
