using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Configuracoes
{
    public class IntegracaoCTePagamentoLoggi : RepositorioBase<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi>
    {
        #region Construtores

        public IntegracaoCTePagamentoLoggi(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IntegracaoCTePagamentoLoggi(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi Buscar()
        {
            var consultaIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Configuracoes.IntegracaoCTePagamentoLoggi>();

            return consultaIntegracao.FirstOrDefault();
        }

        #endregion
    }
}
