using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoPiracanjuba : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba>
    {
        public IntegracaoPiracanjuba(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPiracanjuba>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
