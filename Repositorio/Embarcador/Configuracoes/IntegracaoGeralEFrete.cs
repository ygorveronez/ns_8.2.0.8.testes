using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoGeralEFrete : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete>
    {
        #region Construtores

        public IntegracaoGeralEFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoGeralEFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGeralEFrete>();

            return consultaIntegracao.FirstOrDefault();
        }
    }
}