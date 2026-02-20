using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGSW : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW>
    {
        public IntegracaoGSW(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
