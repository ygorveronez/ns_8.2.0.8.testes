using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportadorHistoricoOferta : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorHistoricoOferta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta> BuscarPorCargaJanelaCarregamentoTransportadorHistorico(int codigo)
        {
            var consultaHistoricoOferta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorHistoricoOferta>()
                .Where(obj => obj.CargaJanelaCarregamentoTransportadorHistorico.Codigo == codigo);

            return consultaHistoricoOferta
                .Fetch(o => o.Empresa)
                .ToList();
        }

        #endregion
    }
}
