using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pedidos.AlteracaoPedido
{
    public sealed class MotivoRejeicaoAlteracaoPedido : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido>
    {
        #region Construtores

        public MotivoRejeicaoAlteracaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaMotivoRejeicaoAlteracaoPedido filtrosPesquisa)
        {
            var consultaMotivoRejeicaoAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMotivoRejeicaoAlteracaoPedido = consultaMotivoRejeicaoAlteracaoPedido.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Tipo != TipoMotivoRejeicaoAlteracaoPedido.Todos)
                consultaMotivoRejeicaoAlteracaoPedido = consultaMotivoRejeicaoAlteracaoPedido.Where(o => o.Tipo == TipoMotivoRejeicaoAlteracaoPedido.Todos || o.Tipo == filtrosPesquisa.Tipo);

            if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaMotivoRejeicaoAlteracaoPedido = consultaMotivoRejeicaoAlteracaoPedido.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaMotivoRejeicaoAlteracaoPedido = consultaMotivoRejeicaoAlteracaoPedido.Where(o => !o.Ativo);

            return consultaMotivoRejeicaoAlteracaoPedido;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.MotivoRejeicaoAlteracaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaMotivoRejeicaoAlteracaoPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoRejeicaoAlteracaoPedido = Consultar(filtrosPesquisa);

            return ObterLista(consultaMotivoRejeicaoAlteracaoPedido, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaMotivoRejeicaoAlteracaoPedido filtrosPesquisa)
        {
            var consultaMotivoRejeicaoAlteracaoPedido = Consultar(filtrosPesquisa);

            return consultaMotivoRejeicaoAlteracaoPedido.Count();
        }

        #endregion
    }
}
