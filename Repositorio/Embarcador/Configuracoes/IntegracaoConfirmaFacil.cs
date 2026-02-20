using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoConfirmaFacil : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil>
    {
        #region Construtores

        public IntegracaoConfirmaFacil(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoConfirmaFacil(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
