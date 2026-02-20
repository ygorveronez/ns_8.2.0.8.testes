using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using DominioPedidos = Dominio.Entidades.Embarcador.Pedidos;

namespace Repositorio.Embarcador.Pedidos
{
    public class MotivoCancelamentoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido>
    {
        #region Construtores

        public MotivoCancelamentoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var listaMotivoCancelamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido>();

            if (status != SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == SituacaoAtivoPesquisa.Ativo;
                listaMotivoCancelamento = listaMotivoCancelamento.Where(q => q.Ativo == ativo);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                listaMotivoCancelamento = listaMotivoCancelamento.Where(q => q.Descricao.Contains(descricao));

            return listaMotivoCancelamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido BuscarPorCodigo(int codigo)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.MotivoCancelamentoPedido>()
                .Where(q => q.Codigo == codigo)
                .FirstOrDefault();
        }

        public List<DominioPedidos.MotivoCancelamentoPedido> Consultar(string descricao, SituacaoAtivoPesquisa status, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoCancelamento = Consultar(descricao, status);

            return ObterLista(consultaMotivoCancelamento, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, SituacaoAtivoPesquisa status)
        {
            var listaMotivoCancelamento = Consultar(descricao, status);

            return listaMotivoCancelamento.Count();
        }

        #endregion
    }
}
