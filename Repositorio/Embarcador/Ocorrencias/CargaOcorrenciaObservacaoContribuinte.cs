using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrenciaObservacaoContribuinte : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte>
    {
        public CargaOcorrenciaObservacaoContribuinte(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte> BuscarPorOcorrencia(int ocorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaObservacaoContribuinte>();
            var result = from obj in query where obj.Ocorrencia.Codigo == ocorrencia select obj;
            return result.ToList();
        }
    }
}
