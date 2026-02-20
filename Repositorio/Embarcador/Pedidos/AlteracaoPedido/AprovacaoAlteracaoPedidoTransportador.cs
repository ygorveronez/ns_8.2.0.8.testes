using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;

namespace Repositorio.Embarcador.Pedidos.AlteracaoPedido
{
    public sealed class AprovacaoAlteracaoPedidoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>
    {
        #region Construtores

        public AprovacaoAlteracaoPedidoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador filtrosPesquisa)
        {
            var consultaAlteracaoPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido>();
            var consultaAprovacaoAlteracaoPedidoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>()
                .Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoEmbarcador))
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.Pedido.NumeroPedidoEmbarcador == filtrosPesquisa.NumeroPedidoEmbarcador);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.DataCriacao.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.DataCriacao.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.SituacaoAprovacao.HasValue)
                consultaAprovacaoAlteracaoPedidoTransportador = consultaAprovacaoAlteracaoPedidoTransportador.Where(o => o.Situacao == filtrosPesquisa.SituacaoAprovacao.Value);

            if (filtrosPesquisa.SituacaoAprovacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                consultaAlteracaoPedido = consultaAlteracaoPedido.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlteracaoPedido.AguardandoAprovacaoTransportador);

            return consultaAlteracaoPedido.Where(o => consultaAprovacaoAlteracaoPedidoTransportador.Where(a => a.AlteracaoPedido.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador> BuscarPendentes(List<int> codigosAlteracoesPedidos, int codigoTransportador)
        {
            var consultaAprovacaoAlteracaoPedidoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>()
                .Where(aprovacao =>
                    (codigosAlteracoesPedidos.Contains(aprovacao.AlteracaoPedido.Codigo)) &&
                    (aprovacao.Transportador.Codigo == codigoTransportador) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                );

            return consultaAprovacaoAlteracaoPedidoTransportador.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador> BuscarPorAlteracaoPedido(int codigoAlteracaoPedido)
        {
            var consultaAprovacaoAlteracaoPedidoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>()
                .Where(aprovacao =>
                    (aprovacao.AlteracaoPedido.Codigo == codigoAlteracaoPedido)
                );

            return consultaAprovacaoAlteracaoPedidoTransportador.ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador BuscarPorAlteracaoPedidoETransportador(int codigoAlteracaoPedido, int codigoTransportador)
        {
            var consultaAprovacaoAlteracaoPedidoTransportador = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>()
                .Where(aprovacao =>
                    (aprovacao.AlteracaoPedido.Codigo == codigoAlteracaoPedido) &&
                    (aprovacao.Transportador.Codigo == codigoTransportador)
                );

            return consultaAprovacaoAlteracaoPedidoTransportador.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AlteracaoPedido> Consultar(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAlteracaoPedido = Consultar(filtrosPesquisa);

            return ObterLista(consultaAlteracaoPedido, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaAlteracaoPedidoAprovacaoTransportador filtrosPesquisa)
        {
            var consultaAlteracaoPedido = Consultar(filtrosPesquisa);

            return consultaAlteracaoPedido.Count();
        }

        public int ContarPendentes(int codigoAlteracaoPedido)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>()
                .Where(aprovacao =>
                    (aprovacao.AlteracaoPedido.Codigo == codigoAlteracaoPedido) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente)
                );

            return aprovacoes.Count();
        }

        public int ContarReprovacoes(int codigoAlteracaoPedido)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.AlteracaoPedido.AprovacaoAlteracaoPedidoTransportador>()
                .Where(aprovacao =>
                    (aprovacao.AlteracaoPedido.Codigo == codigoAlteracaoPedido) &&
                    (aprovacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Rejeitada)
                );

            return aprovacoes.Count();
        }

        #endregion
    }
}
