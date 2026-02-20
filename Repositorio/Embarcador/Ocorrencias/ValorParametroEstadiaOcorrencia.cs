using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroEstadiaOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia>
    {

        public ValorParametroEstadiaOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEstadiaOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
