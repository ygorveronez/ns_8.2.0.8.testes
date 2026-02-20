using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoShopee : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee>
    {
        #region Construtores

        public IntegracaoShopee(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoShopee(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoShopee>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }


}
