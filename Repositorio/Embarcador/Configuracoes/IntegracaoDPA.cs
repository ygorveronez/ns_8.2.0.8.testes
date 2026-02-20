using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoDPA : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA>
    {
        public IntegracaoDPA(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDPA>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
