using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class OcorrenciaColetaEntregaNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal>
    {
        public OcorrenciaColetaEntregaNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal> BuscarPorOcorrenciaColetaEntrega(int codigoOcorrenciaColetaEntrega)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal>();
            var result = query.Where(obj => obj.OcorrenciaColetaEntrega.Codigo == codigoOcorrenciaColetaEntrega);
            return result
                .ToList();
        }
    }
}
