using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class CentroCarregamentoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador>
    {
        #region Construtores

        public CentroCarregamentoTransportador(UnitOfWork unitOfWork) : base (unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador BuscarPorCodigo(int codigo)
        {
            var consultaCentroCarregamentoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoTransportador>()
                .Where(o => o.Codigo == codigo);

            return consultaCentroCarregamentoTransportador.FirstOrDefault();
        }

        #endregion
    }
}
