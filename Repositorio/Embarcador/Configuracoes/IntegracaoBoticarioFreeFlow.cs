using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoBoticarioFreeFlow : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow>
    {
        public IntegracaoBoticarioFreeFlow(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBoticarioFreeFlow>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
