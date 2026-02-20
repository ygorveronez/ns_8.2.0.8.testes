using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class SessaoRoteirizadorPedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>
    {
        #region Construtores

        public SessaoRoteirizadorPedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public SessaoRoteirizadorPedidoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        public List<int> BuscarSessoesPorPedidoProduto(int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>().Where(x => x.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao);

            query = query.Where(o => o.PedidoProduto.Codigo == codigoPedidoProduto);

            return query.Select(o => o.SessaoRoteirizadorPedido.SessaoRoteirizador.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> BuscarPorSessaoRoteirizador(int codigoSessao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();
            var result = from obj in query
                         where obj.SessaoRoteirizadorPedido.SessaoRoteirizador.Codigo == codigoSessao &&
                               obj.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;
            return result.ToList();
        }

        public List<int> BuscarCodigosPedidoProdutoPorSessaoRoteirizador(int codigoSessao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();
            var result = from obj in query
                         where obj.SessaoRoteirizadorPedido.SessaoRoteirizador.Codigo == codigoSessao &&
                               obj.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;
            return result.Select(x => x.PedidoProduto.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> BuscarSessaoRoteirizadorPorPedidos(List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();
                query = (from obj in query
                         where
                            tmp.Contains(obj.SessaoRoteirizadorPedido.Pedido.Codigo) && obj.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                            && obj.SessaoRoteirizadorPedido.SessaoRoteirizador.SituacaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Cancelada
                         select obj);

                result.AddRange(query.ToList());
                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> BuscarSessaoRoteirizadorPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();
            query = (from obj in query
                     where
                        obj.SessaoRoteirizadorPedido.Pedido.Codigo == codigoPedido && obj.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                        && obj.SessaoRoteirizadorPedido.SessaoRoteirizador.SituacaoSessaoRoteirizador != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador.Cancelada
                     select obj);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto BuscarPorSessaoRoteirizadorEPedidoProduto(int codigoSessao, int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();
            var result = from obj in query
                         where obj.SessaoRoteirizadorPedido.SessaoRoteirizador.Codigo == codigoSessao
                               && obj.PedidoProduto.Codigo == codigoPedidoProduto
                               && obj.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;
            return result.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto> BuscarPorSessaoRoteirizadorEPedidoProdutoAsync(int codigoSessao, int codigoPedidoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizadorPedidoProduto>();
            var result = from obj in query
                         where obj.SessaoRoteirizadorPedido.SessaoRoteirizador.Codigo == codigoSessao
                               && obj.PedidoProduto.Codigo == codigoPedidoProduto
                               && obj.SessaoRoteirizadorPedido.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizadorPedido.RemovidoPedidoSessao
                         select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }
    }
}
