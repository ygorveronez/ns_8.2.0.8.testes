using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class TipoOperacaoLayoutEDI : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>
    {
        public TipoOperacaoLayoutEDI(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI> BuscarPorTipoOperacao(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();

            var result = from obj in query where obj.TipoOperacao.Codigo == codigo select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoLayoutEDI>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
    }
}
