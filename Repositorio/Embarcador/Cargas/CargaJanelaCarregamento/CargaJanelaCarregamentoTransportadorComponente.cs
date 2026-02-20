using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoTransportadorComponente : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente>
    {
        public CargaJanelaCarregamentoTransportadorComponente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente> BuscarPorCargaJanelaCarregamentoTransportador(int cargaJanelaCarregamentoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoTransportadorComponente>();

            var result = from obj in query where obj.CargaJanelaCarregamentoTransportador.Codigo == cargaJanelaCarregamentoTransportador select obj;

            return result.ToList();
        }
    }
}