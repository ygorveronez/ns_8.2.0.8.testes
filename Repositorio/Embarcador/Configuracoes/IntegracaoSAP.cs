using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoSAP : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP>
    {
        public IntegracaoSAP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSAP>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}