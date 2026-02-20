using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente>
    {
        public CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente BuscarPorPedidoCTeParaSubcontratacao(int codigoPedidoCTeParaSubcontratacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoCTeParaSubcontratacaoTabelaFreteCliente>();

            query = query.Where(o => o.PedidoCTeParaSubContratacao.Codigo == codigoPedidoCTeParaSubcontratacao);

            return query.FirstOrDefault();
        }
    }
}
