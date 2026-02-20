using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class AgendamentoColetaAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo>
    {
        #region Construtores

        public AgendamentoColetaAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public AgendamentoColetaAnexo(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> BuscarPorAgendamentoColeta(int codigoAgendamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo>();
            query = from obj in query where obj.EntidadeAnexo.Codigo == codigoAgendamento select obj;

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo>();
            query = from obj in query where obj.EntidadeAnexo.Pedido.Codigo == codigoPedido select obj;

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo>> BuscarPorPedidoAsync(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo>();
            query = from obj in query where obj.EntidadeAnexo.Pedido.Codigo == codigoPedido select obj;

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo> BuscarPorPedidosECarga(List<int> codigosPedidos, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaAnexo>();
            query = from obj in query where (obj.EntidadeAnexo.Pedidos.Any(x => codigosPedidos.Contains(x.Pedido.Codigo)) || obj.EntidadeAnexo.Carga.Codigo == codigoCarga) select obj;

            return query.ToList();
        }

        #endregion
    }
}
