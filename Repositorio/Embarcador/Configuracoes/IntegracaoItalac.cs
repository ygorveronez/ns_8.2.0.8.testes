using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoItalac : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac>
    {
        #region Construtores

        public IntegracaoItalac(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoItalac(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoItalac>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }


}
