using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class RegrasOcorrenciaValorOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>
    {
        public RegrasOcorrenciaValorOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia>();
            var result = from obj in query where obj.RegrasAutorizacaoOcorrencia.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
