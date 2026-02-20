using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.CotacaoPedido
{
    public class CotacaoPedidoCubagem : RepositorioBase<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem>
    {
        public CotacaoPedidoCubagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem> BuscarPorCotacao(long codigoCotacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.CotacaoPedido.CotacaoPedidoCubagem>();
            var result = from obj in query where obj.CotacaoPedido.Codigo == codigoCotacao select obj;
            return result.ToList();
        }
    }
}
