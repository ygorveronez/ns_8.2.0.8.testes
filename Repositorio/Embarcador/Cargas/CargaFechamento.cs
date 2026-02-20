using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaFechamento : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaFechamento>
    {
        public CargaFechamento(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public Dominio.Entidades.Embarcador.Cargas.CargaFechamento BuscarPorCarga(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFechamento>();
            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga);
            return result.FirstOrDefault();
        }


        public int ContarConsulta(string numeroCarga, string numeroPedidoEmbarcador, SituacaoCargaFechamento? situacao)
        {
            var query = MontarConsulta(numeroCarga, numeroPedidoEmbarcador, situacao);

            return query.Count();
        }

        public bool CargaEstaEmFechamentoAgRateio(int codigoCarga)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFechamento>();
            var result = query.Where(obj => obj.Carga.Codigo == codigoCarga && obj.SituacaoFechamento == SituacaoCargaFechamento.AgRateio);
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> Consultar(string numeroCarga, string numeroPedidoEmbarcador, SituacaoCargaFechamento? situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = MontarConsulta(numeroCarga, numeroPedidoEmbarcador, situacao);

            query = query.Fetch(obj => obj.Carga)
                         .ThenFetch(obj => obj.DadosSumarizados)
                         .Fetch(obj => obj.Carga);

            if (limite > 0)
                return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
            else
                return query.OrderBy(propOrdenar + " " + dirOrdena).ToList();
        }


        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> MontarConsulta(string numeroCarga, string numeroPedidoEmbarcador, SituacaoCargaFechamento? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFechamento>();

            if (!string.IsNullOrWhiteSpace(numeroCarga))
                query = query.Where(o => o.Carga.CodigoCargaEmbarcador == numeroCarga);

            if (situacao.HasValue)
                query = query.Where(o => o.SituacaoFechamento == situacao);

            if (!string.IsNullOrWhiteSpace(numeroPedidoEmbarcador))
                query = query.Where(o => o.Carga.Pedidos.Any(cargaPedido => cargaPedido.Pedido.NumeroPedidoEmbarcador == numeroPedidoEmbarcador));

            return query;
        }


        public List<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> BuscarAguardandoRateio()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFechamento>();
            query = query.Where(o => o.SituacaoFechamento == SituacaoCargaFechamento.AgRateio);

            return query.Fetch(obj => obj.Carga).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> BuscarAguardandoCalculoFrete()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaFechamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFechamento>();
            query = query.Where(o => o.SituacaoFechamento == SituacaoCargaFechamento.AgCalculoFrete);

            return query.Fetch(obj => obj.Carga).ToList();
        }
    }
}
