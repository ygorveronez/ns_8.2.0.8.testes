using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoFronteira : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>
    {
        public PedidoFronteira(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoFronteira(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<List<double>> BuscarCPFCNPJFronteirasPorPedidoAsync(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();

            var result = from obj in query where obj.Pedido.Codigo == pedido select obj.Fronteira.CPF_CNPJ;

            return result.ToListAsync();
        }

        public List<double> BuscarCPFCNPJFronteirasPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();

            var result = from obj in query where obj.Pedido.Codigo == pedido select obj.Fronteira.CPF_CNPJ;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>> BuscarFronteirasPorPedidoCPFCNPJAsync(List<double> codigosFronteiras, int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();

            return query
                .Where(pf => codigosFronteiras.Contains(pf.Fronteira.CPF_CNPJ) && pf.Pedido.Codigo == pedido)
                .ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> BuscarFronteirasPorPedidoCPFCNPJ(List<double> codigosFronteiras, int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();

            return query
                .Where(pf => codigosFronteiras.Contains(pf.Fronteira.CPF_CNPJ) && pf.Pedido.Codigo == pedido)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Cliente>> BuscarFronteirasPorPedidoAsync(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();

            var result = from obj in query where obj.Pedido.Codigo == pedido select obj.Fronteira;

            return result.ToListAsync();
        }

        public List<Dominio.Entidades.Cliente> BuscarFronteirasPorPedido(int pedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira>();

            var result = from obj in query where obj.Pedido.Codigo == pedido select obj.Fronteira;

            return result.ToList();
        }
    }
}
