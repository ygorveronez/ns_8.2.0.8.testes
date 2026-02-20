using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoLBC : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC>
    {
        public IntegracaoLBC(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLBC>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }

}