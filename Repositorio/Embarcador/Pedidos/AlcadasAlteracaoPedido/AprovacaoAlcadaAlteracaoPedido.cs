using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pedidos.AlcadasAlteracaoPedido
{
    public sealed class AprovacaoAlcadaAlteracaoPedido : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido,
        Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.RegraAutorizacaoAlteracaoPedido,
        Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido
    >
    {
        #region Construtores

        public AprovacaoAlcadaAlteracaoPedido(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao filtrosPesquisa)
        {
            var consultaAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>();
            var consultaAlcadaAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlcadasAlteracaoPedido.AprovacaoAlcadaAlteracaoPedido>()
                .Where(o => !o.Bloqueada);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoAlteracaoPedido.HasValue)
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.Situacao == filtrosPesquisa.SituacaoAlteracaoPedido.Value);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaAlteracaoPedido = consultaAlcadaAlteracaoPedido.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoAlteracaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoPedido.AguardandoAprovacao)
                consultaAlcadaAlteracaoPedido = consultaAlcadaAlteracaoPedido.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaAlteracaoPedido.Where(o => consultaAlcadaAlteracaoPedido.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAlteracaoPedido = Consultar(filtrosPesquisa);

            return ObterLista(consultaAlteracaoPedido, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacao filtrosPesquisa)
        {
            var consultaAlteracaoPedido = Consultar(filtrosPesquisa);

            return consultaAlteracaoPedido.Count();
        }

        #endregion
    }
}
