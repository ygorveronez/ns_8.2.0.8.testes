using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoObramax : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax>
    {
        #region Construtores

        public IntegracaoObramax(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoObramax(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoObramax>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}

