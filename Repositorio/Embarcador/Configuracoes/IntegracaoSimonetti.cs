using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSimonetti : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti>
    {
        public IntegracaoSimonetti(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti BuscarDadosIntegracao()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSimonetti>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}