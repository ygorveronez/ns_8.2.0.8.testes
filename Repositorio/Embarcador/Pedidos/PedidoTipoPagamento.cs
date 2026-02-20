using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoTipoPagamento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento>
    {
        public PedidoTipoPagamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento> query = ObterQueryConsulta(descricao, status);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento> query = ObterQueryConsulta(descricao, status);

            return query.Count();
        }

        #endregion

        #region Métodos Privados

        public IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento> ObterQueryConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoTipoPagamento>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }

        #endregion
    }
}
