using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoPedidoNotaFiscal : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>
    {
        public CarregamentoPedidoNotaFiscal(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoPedidoNotaFiscal(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal BuscarPorPedidoECarregamento(int codigoPedido, int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.CarregamentoPedido.Pedido.Codigo == codigoPedido);
            query = query.Where(obj => obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorPedidoECarregamentoAsync(int codigoPedido, int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.CarregamentoPedido.Pedido.Codigo == codigoPedido
                                    && obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorPedidoDoCarregamento(int codigoCarregamento)
        {
            IQueryable<int> queryCarregamentoPedidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedido>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento)
                .Select(a => a.Pedido.Codigo);

            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>()
               .Where(o => queryCarregamentoPedidos.Contains(o.CarregamentoPedido.Pedido.Codigo)).ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>> BuscarPorPedidoAsync(List<int> codigosPedido)
        {
            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var partialResult = await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>()
                    .Where(obj => tmp.Contains(obj.CarregamentoPedido.Pedido.Codigo))
                    .ToListAsync(CancellationToken);

                result.AddRange(partialResult);

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarJaUtilizadasPorCarregamentoEPedidos(List<int> codigosPedido, int codigoCarregamento)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
            //query = query.Where(obj => codigosPedido.Contains(obj.CarregamentoPedido.Pedido.Codigo) && obj.CarregamentoPedido.Carregamento.Codigo != codigoCarregamento);
            //return query.ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedido?.Count)
            {
                List<int> tmp = codigosPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                query = query.Where(obj => tmp.Contains(obj.CarregamentoPedido.Pedido.Codigo) && obj.CarregamentoPedido.Carregamento.Codigo != codigoCarregamento);

                result.AddRange(query.ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorPedidoECarregamento(int codigoCarregamento, List<int> codigosPedidos)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            //query = query.Where(obj => codigosPedidos.Contains(obj.CarregamentoPedido.Pedido.Codigo));
            //query = query.Where(obj => obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);

            //return query.ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            int take = 1000;
            int start = 0;
            while (start < codigosPedidos?.Count)
            {
                List<int> tmp = codigosPedidos.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                query = query.Where(obj => tmp.Contains(obj.CarregamentoPedido.Pedido.Codigo) && obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);

                result.AddRange(query.ToList());

                start += take;
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorCarregamento(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);

            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>> BuscarPorCarregamentoAsync(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.CarregamentoPedido.Carregamento.Codigo == codigoCarregamento);

            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorChavesNotas(List<string> chavesNotas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.NotasFiscais.Any(x => chavesNotas.Contains(x.Chave)));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorCarregamentoPedido(int codigoCarregamentoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.CarregamentoPedido.Codigo == codigoCarregamentoPedido);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> BuscarPorCarregamentoPedido(List<int> codigosCarregamentoPedido)
        {
            //var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
            //query = query.Where(obj => codigosCarregamentoPedido.Contains(obj.CarregamentoPedido.Codigo));
            //return query.ToList();

            List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal> result = new List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            int take = 1000;
            int start = 0;
            while (start < codigosCarregamentoPedido?.Count)
            {
                List<int> tmp = codigosCarregamentoPedido.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();
                query = query.Where(obj => tmp.Contains(obj.CarregamentoPedido.Codigo));

                result.AddRange(query.ToList());

                start += take;
            }

            return result;

        }

        public bool BuscarExistenciaPorNota(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal, int codigoCarregamentoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoPedidoNotaFiscal>();

            query = query.Where(obj => obj.CarregamentoPedido.Codigo != codigoCarregamentoPedido &&
                                obj.NotasFiscais.Contains(notaFiscal));

            return query.Count() > 0;
        }
    }
}
