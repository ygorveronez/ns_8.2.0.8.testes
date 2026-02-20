using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoVLI : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI>
    {
        public IntegracaoVLI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoVLI>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}