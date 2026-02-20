using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCorreios : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios>
    {
        public IntegracaoCorreios(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCorreios>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}