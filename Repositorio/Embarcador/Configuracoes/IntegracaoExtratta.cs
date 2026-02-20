using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoExtratta : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta>
    {
        public IntegracaoExtratta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta Buscar()
        {
            var consultaIntegracaoExtratta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoExtratta>();

            return consultaIntegracaoExtratta.FirstOrDefault();
        }

        #endregion
    }
}
