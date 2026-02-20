using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PlanejamentoPedidoDisponibilidadeMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista>
    {
        public PlanejamentoPedidoDisponibilidadeMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista BuscarPorDataeNomeMotorista(DateTime data, string nomeMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Data == data && ent.Nome == nomeMotorista);

            return result.FirstOrDefault();
        }

        public int ContarMotoristaEntreData(DateTime dataIni, DateTime dataFim)
        {
            var queryUsuarios = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>()
                .Where(o => o.Nome != "");

            var queryPedidos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Pedido>()
                         .Where(obj => obj.DataCarregamentoPedido >= dataIni &&
                                       obj.DataCarregamentoPedido < dataFim &&
                                       obj.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto);

            var query = queryUsuarios.Where(o => queryPedidos.Any(p => p.Motoristas.Any(v => v.Codigo == o.Codigo)));

            return query.Distinct().Count();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista> BuscarDisponivelPorData(DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoDisponibilidadeMotorista>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Data == data && ent.Disponivel);

            return result.ToList();
        }

        #endregion
    }
}
