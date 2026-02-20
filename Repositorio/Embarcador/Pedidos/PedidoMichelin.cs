using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoMichelin : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoMichelin>
    {
        public PedidoMichelin(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoMichelin BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoMichelin>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public bool ContemPorNomeArquivo(string nomeArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoMichelin>();

            var result = from obj in query where obj.NomeArquivo == nomeArquivo select obj;

            return result.Any();
        }
    }
}
