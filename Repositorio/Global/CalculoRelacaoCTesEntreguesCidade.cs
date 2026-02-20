using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class CalculoRelacaoCTesEntreguesCidade : RepositorioBase<Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade>
    {

        public CalculoRelacaoCTesEntreguesCidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade BuscarPorCidade(int cidade, int calculoRelacaoCTesEntregues)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CalculoRelacaoCTesEntreguesCidade>();
            var result = from obj in query where obj.Cidade.Codigo == cidade && obj.CalculoRelacaoCTesEntregues.Codigo == calculoRelacaoCTesEntregues select obj;
            return result.FirstOrDefault();
        }
    }
}
