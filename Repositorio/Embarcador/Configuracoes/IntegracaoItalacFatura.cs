using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoItalacFatura : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura>
    {
        #region Construtores

        public IntegracaoItalacFatura(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoItalacFatura(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalacFatura>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
