using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoHavan : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan>
    {
        public IntegracaoHavan(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
