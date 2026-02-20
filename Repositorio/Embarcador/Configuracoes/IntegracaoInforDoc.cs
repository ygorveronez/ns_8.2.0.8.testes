using System.Linq;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoInforDoc : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc>
    {
        public IntegracaoInforDoc(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoInforDoc>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
