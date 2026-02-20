using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoFrimesa : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa>
    {
        public IntegracaoFrimesa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoFrimesa>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
