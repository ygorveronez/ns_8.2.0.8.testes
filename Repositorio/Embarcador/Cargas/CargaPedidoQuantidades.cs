using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoQuantidades : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>
    {
        public CargaPedidoQuantidades(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> BuscarPorCargaPedido(int codCargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();
            var result = query.Where(obj => obj.CargaPedido.Codigo == codCargaPedido);
            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> BuscarPorCargaPedidosQuantidades(List<int> codigosCargaPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();
            var result = query.Where(obj => codigosCargaPedidos.Contains(obj.CargaPedido.Codigo));
            return result.Distinct().ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>> BuscarPorCargaPedidosQuantidadesAsync(List<int> codigosCargaPedidos, int limitePorExecucao = 2000)
        {
            if (codigosCargaPedidos == null || codigosCargaPedidos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            var resultados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            var totalPaginas = (int)Math.Ceiling(codigosCargaPedidos.Count / (double)limitePorExecucao);

            for (int paginaAtual = 0; paginaAtual < totalPaginas; paginaAtual++)
            {
                var codigosPagina = codigosCargaPedidos
                    .Skip(paginaAtual * limitePorExecucao)
                    .Take(limitePorExecucao)
                    .ToList();

                var resultadosPagina = await this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>()
                    .Where(obj => codigosPagina.Contains(obj.CargaPedido.Codigo))
                    .Distinct()
                    .ToListAsync();

                resultados.AddRange(resultadosPagina);
            }

            return resultados.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> BuscarPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades>();

            query = query.Where(o => o.CargaPedido.Carga.Codigo == carga);

            return query.Fetch(o => o.CargaPedido).ToList();
        }

    }
}
