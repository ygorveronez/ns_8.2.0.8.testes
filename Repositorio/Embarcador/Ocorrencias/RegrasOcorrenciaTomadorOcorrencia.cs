using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class RegrasOcorrenciaTomadorOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>
    {
        public RegrasOcorrenciaTomadorOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTomadorOcorrencia>();
            var result = from obj in query where obj.RegrasAutorizacaoOcorrencia.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}

