using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class OcorrenciaParametroOcorrenciaGerado : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado>
    {

        public OcorrenciaParametroOcorrenciaGerado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado> Consultar(int ocorrenciaParametroOcorrencia, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado>();
            var result = from obj in query where obj.OcorrenciaParametroOcorrencia.Codigo == ocorrenciaParametroOcorrencia select obj;

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int ocorrenciaParametroOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaParametroOcorrenciaGerado>();
            var result = from obj in query where obj.OcorrenciaParametroOcorrencia.Codigo == ocorrenciaParametroOcorrencia select obj;

            return result.Count();
        }
    }
}
