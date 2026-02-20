using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CentroCarregamentoTransportadorTerceiro : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro>
    {
        #region Construtores

        public CentroCarregamentoTransportadorTerceiro(UnitOfWork unitOfWork) : base (unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro BuscarPorCodigo(int codigo)
        {
            var consultaCentroCarregamentoTransportadorTerceiro = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportadorTerceiro>()
                .Where(o => o.Codigo == codigo);

            return consultaCentroCarregamentoTransportadorTerceiro.FirstOrDefault();
        }

        #endregion
    }
}
