using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class ValorParametroEscoltaOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia>
    {

        public ValorParametroEscoltaOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.ValorParametroEscoltaOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
