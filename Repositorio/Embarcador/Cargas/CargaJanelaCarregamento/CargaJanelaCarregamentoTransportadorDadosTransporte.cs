using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportadorDadosTransporte : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorDadosTransporte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte BuscarPorCargaJanelaCarregamentoTransportador(int codigo)
        {
            var consultaDadosTransporte = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorDadosTransporte>()
                .Where(obj => obj.CargaJanelaCarregamentoTransportador.Codigo == codigo);

            return consultaDadosTransporte.FirstOrDefault();
        }

        #endregion
    }
}
