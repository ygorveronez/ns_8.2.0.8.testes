using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class AjusteTabelaFreteProcessamentoValores : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores>
    {
        public AjusteTabelaFreteProcessamentoValores(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores BuscarPorAjuste(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteProcessamentoValores>();
            var result = from obj in query where obj.AjusteTabelaFrete.Codigo == codigo orderby obj.Data descending select obj;
            return result.FirstOrDefault();
        }
    }
}
