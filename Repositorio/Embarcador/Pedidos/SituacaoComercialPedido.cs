using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class SituacaoComercialPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>
    {
        #region Construtores

        public SituacaoComercialPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region MétodosPrivados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoComercialPedido filtroPesquisa)
        {
            var consultaSituacaoComercialPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>();

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.Descricao))
                consultaSituacaoComercialPedido = consultaSituacaoComercialPedido.Where(o => o.Descricao.Contains(filtroPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.CodigoIntegracao))
                consultaSituacaoComercialPedido = consultaSituacaoComercialPedido.Where(o => o.CodigoIntegracao.Equals(filtroPesquisa.CodigoIntegracao));

            if (filtroPesquisa.BloqueiaPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos)
            {
                bool valorBloqueiaPedido = filtroPesquisa.BloqueiaPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Sim;
                consultaSituacaoComercialPedido = consultaSituacaoComercialPedido.Where(o => o.BloqueiaPedido == valorBloqueiaPedido);
            }


            return consultaSituacaoComercialPedido;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido BuscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido> lstSituacaoComercialPedido = null)
        {
            if (lstSituacaoComercialPedido != null)
                return lstSituacaoComercialPedido.Find(o => o.CodigoIntegracao == codigoIntegracao);
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>().Where(o => o.CodigoIntegracao == codigoIntegracao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido BuscarPorCodigo(int codigo)
        {
            var consultaSituacaoComercialPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>()
                .Where(o => o.Codigo == codigo);

            return consultaSituacaoComercialPedido.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoComercialPedido filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaSituacaoComercialPedido = Consultar(filtroPesquisa);

            return ObterLista(consultaSituacaoComercialPedido, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaSituacaoComercialPedido filtroPesquisa)
        {
            var consultaSituacaoComercialPedido = Consultar(filtroPesquisa);

            return consultaSituacaoComercialPedido.Count();
        }

        public bool ExisteSituacaoComercialPedido()
        {
            var consultaSituacaoComercialPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.SituacaoComercialPedido>();

            return consultaSituacaoComercialPedido.Any();
        }

        #endregion
    }
}
