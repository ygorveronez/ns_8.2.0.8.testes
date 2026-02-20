using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoData : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoData>
    {
        public ChamadoData(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoData> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoData>();

            var result = from obj in query where obj.Chamado.CargaEntrega.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }
    }
}
