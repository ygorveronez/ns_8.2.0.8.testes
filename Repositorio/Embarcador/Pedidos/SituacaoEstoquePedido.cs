using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class SituacaoEstoquePedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido>
    {
        #region Construtores

        public SituacaoEstoquePedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region MétodosPrivados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoEstoquePedido filtroPesquisa)
        {
            var consultaSituacaoEstoquePedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido>();

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.Descricao))
                consultaSituacaoEstoquePedido = consultaSituacaoEstoquePedido.Where(o => o.Descricao.Contains(filtroPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoIntegracao))
                consultaSituacaoEstoquePedido = consultaSituacaoEstoquePedido.Where(o => o.CodigoIntegracao.Equals(filtroPesquisa.CodigoIntegracao));

            if (filtroPesquisa.BloqueiaPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos)
            {
                bool valorBloqueiaPedido = filtroPesquisa.BloqueiaPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim;
                consultaSituacaoEstoquePedido = consultaSituacaoEstoquePedido.Where(o => o.BloqueiaPedido == valorBloqueiaPedido);
            }

            return consultaSituacaoEstoquePedido;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido BuscarPorCodigo(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> consultaSituacaoEstoquePedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido>()
                .Where(o => o.Codigo == codigo);

            return consultaSituacaoEstoquePedido.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoEstoquePedido filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> consultaSituacaoEstoquePedido = Consultar(filtroPesquisa);

            return ObterLista(consultaSituacaoEstoquePedido, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoEstoquePedido filtroPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> consultaSituacaoEstoquePedido = Consultar(filtroPesquisa);

            return consultaSituacaoEstoquePedido.Count();
        }

        public bool ExisteSituacaoEstoquePedido()
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido> consultaSituacaoEstoquePedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoEstoquePedido>();

            return consultaSituacaoEstoquePedido.Any();
        }

        #endregion
    }
}
