using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PlanejamentoPedidoDisponibilidade : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade>
    {
        public PlanejamentoPedidoDisponibilidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade BuscarPorDataeFrota(DateTime data, string frota)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Data == data && ent.NumeroFrota == frota);

            return result.FirstOrDefault();
        }

        public int ContarPorFrotaEntreData(string frota, DateTime dataIni, DateTime dataFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            var result = from obj in query where obj.Veiculos.Any(o => o.NumeroFrota == frota) && obj.DataCarregamentoPedido >= dataIni && obj.DataCarregamentoPedido < dataFim && obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto select obj;
            return result.Count();
        }

        public int ContarFrotaEntreData(DateTime dataIni, DateTime dataFim)
        {
            var queryVeiculos = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>()
                .Where(o => o.NumeroFrota != "");

            var queryPedidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                         .Where(obj => obj.DataCarregamentoPedido >= dataIni &&
                                       obj.DataCarregamentoPedido < dataFim &&
                                       obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);

            var query = queryVeiculos.Where(o => queryPedidos.Any(p => p.Veiculos.Any(v => v.Codigo == o.Codigo)));


            return query.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade> BuscarDisponivelPorData(DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidade>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Data == data && ent.Disponivel == true);

            return result.ToList();
        }

        #endregion
    }
}
