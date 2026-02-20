using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class OrdemCompraRequisicao : RepositorioBase<Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao>
    {

        public OrdemCompraRequisicao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}
