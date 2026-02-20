using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class MotivoParadaCentroTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao>
    {
        #region Construtores

        public MotivoParadaCentroTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoParadaCentroTipoOperacao>()
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return query.FirstOrDefault();
        }

        #endregion
    }
}
