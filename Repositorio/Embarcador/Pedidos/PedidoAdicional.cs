using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoAdicional : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>
    {
        public PedidoAdicional(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PedidoAdicional(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>();

            var result = from obj in query where obj.Pedido.Codigo == codigoPedido select obj;

            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosCrtMicPedido>> BuscarDadosCrtMicPorPedidosAsync(List<int> codigosPedidos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosCrtMicPedido> resultados = new List<Dominio.ObjetosDeValor.Embarcador.Carga.DadosCrtMicPedido>();

            for (int i = 0; i < codigosPedidos.Count; i += 1000)
            {
                List<int> lote = codigosPedidos.Skip(i).Take(1000).ToList();

                var parciais = await this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                    .Where(obj => lote.Contains(obj.Pedido.Codigo))
                    .Select(obj => new Dominio.ObjetosDeValor.Embarcador.Carga.DadosCrtMicPedido
                    {
                        CodigoPedido = obj.Pedido.Codigo,
                        Incoterm = obj.Incoterm,
                        TransitoAduaneiro = obj.TransitoAduaneiro,
                        NotificacaoCRT = obj.NotificacaoCRT,
                        DtaRotaPrazoTransporte = obj.DtaRotaPrazoTransporte,
                        TipoEmbalagem = obj.TipoEmbalagem,
                        DetalheMercadoria = obj.DetalheMercadoria,
                    })
                    .ToListAsync(CancellationToken);

                resultados.AddRange(parciais);
            }

            return resultados;
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> BuscarPorPedidos(List<int> codigosPedidos)
        {
            if (codigosPedidos.Count < 2000)
            {
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
             .Where(pa => codigosPedidos.Contains(pa.Pedido.Codigo));

                return query
                    .ToList();
            }

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> listaRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>();

            List<int> listaOriginal = codigosPedidos;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                      .Where(pa => lote.Contains(pa.Pedido.Codigo));

                listaRetornar.AddRange(query.ToList());

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>> BuscarPorPedidosAsync(List<int> codigosPedidos)
        {
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> listaRetornar = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>();

            List<int> listaOriginal = codigosPedidos;
            int tamanhoLote = 2000;
            int indiceInicial = 0;

            while (indiceInicial < listaOriginal.Count)
            {
                List<int> lote = listaOriginal.GetRange(indiceInicial, Math.Min(tamanhoLote, listaOriginal.Count - indiceInicial));

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                      .Where(pa => lote.Contains(pa.Pedido.Codigo));

                listaRetornar.AddRange(await query.ToListAsync(CancellationToken));

                indiceInicial += tamanhoLote;
            }

            return listaRetornar;
        }

        public List<string> BuscarNumerosOSMae(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>()
                .Where(ped => codigosPedidos.Contains(ped.Pedido.Codigo));

            return query.Select(ped => ped.NumeroOSMae).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional> BuscarPorCarga(int codigoCarga)
        {
            var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            var resultCargaPedido = queryCargaPedido.Where(c => c.Carga.Codigo == codigoCarga).Select(o => o.Pedido.Codigo);

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoAdicional>();
            query = query.Where(p => resultCargaPedido.Contains(p.Pedido.Codigo));
            return query.ToList();
        }

    }
}
