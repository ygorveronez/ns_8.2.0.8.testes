using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public sealed class CargaJanelaCarregamentoTransportadorChecklist : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist>
    {
        #region Construtores

        public CargaJanelaCarregamentoTransportadorChecklist(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist> BuscarPorCargaJanelaCarregamentoEVeiculo(int codigoCargaJanelaCarregamento, int codigoVeiculo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorChecklist>();
            query = from obj in query where obj.CargaJanelaCarregamentoTransportador.Codigo == codigoCargaJanelaCarregamento && obj.Veiculo.Codigo == codigoVeiculo select obj;

            return query.ToList();
        }

        #endregion
    }
}
