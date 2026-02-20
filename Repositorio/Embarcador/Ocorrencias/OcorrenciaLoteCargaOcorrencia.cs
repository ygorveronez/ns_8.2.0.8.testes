using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaLoteCargaOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia>
    {
        public OcorrenciaLoteCargaOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia> BuscarPorOcorrenciaLote(int codigoOcorrenciaLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia>()
                .Where(obj => obj.OcorrenciaLote.Codigo == codigoOcorrenciaLote);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia> BuscarNaoGeradasPorOcorrenciaLote(int codigoOcorrenciaLote)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaLoteCargaOcorrencia>()
                .Where(obj => obj.OcorrenciaLote.Codigo == codigoOcorrenciaLote && obj.CargaOcorrencia == null);

            return query.ToList();
        }
    }
}
