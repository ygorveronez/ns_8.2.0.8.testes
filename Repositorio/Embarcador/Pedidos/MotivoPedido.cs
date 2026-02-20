using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using DominioPedidos = Dominio.Entidades.Embarcador.Pedidos;

namespace Repositorio.Embarcador.Pedidos
{
    public class MotivoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.MotivoPedido>
    {
        #region Construtores

        public MotivoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.MotivoPedido> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var listaMotivoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.MotivoPedido>();

            if (status != SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == SituacaoAtivoPesquisa.Ativo;
                listaMotivoPedido = listaMotivoPedido.Where(motivoPedido => motivoPedido.Ativo == ativo);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                listaMotivoPedido = listaMotivoPedido.Where(motivoPedido => motivoPedido.Descricao.Contains(descricao));

            return listaMotivoPedido;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.MotivoPedido BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.MotivoPedido>()
                .Where(motivoPedido => motivoPedido.Codigo == codigo)
                .FirstOrDefault();
        }

        public List<DominioPedidos.MotivoPedido> Consultar(string descricao, SituacaoAtivoPesquisa status, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoPedido = Consultar(descricao, status);

            return ObterLista(consultaMotivoPedido, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public List<DominioPedidos.MotivoPedido> BuscarPorTipo(EnumTipoMotivoPedido? tipoMotivo, string descricao)
        {
            return SessionNHiBernate.Query<DominioPedidos.MotivoPedido>()
                .Where(motivoPedido => motivoPedido.Ativo
                    && (tipoMotivo == null || motivoPedido.TipoMotivo == tipoMotivo)
                    && (string.IsNullOrEmpty(descricao) || motivoPedido.Descricao.Contains(descricao)))
                .ToList();

        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa status)
        {
            var listaMotivoPedido = Consultar(descricao, status);

            return listaMotivoPedido.Count();
        }

        #endregion
    }
}
