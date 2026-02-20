using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Pedidos
{
    public sealed class GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega>
    {
        #region Construtores

        public GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            var consultaGatilhoGeracaoAutomatica = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega>()
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao);

            return consultaGatilhoGeracaoAutomatica
                .Fetch(o => o.TipoOcorrencia)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega> BuscarPorTipoOperacaoEGatilho(int codigoTipoOperacao, TipoGatilhoPedidoOcorrenciaColetaEntrega gatilho)
        {
            var consultaGatilhoGeracaoAutomatica = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.GatilhoGeracaoAutomaticaPedidoOcorrenciaColetaEntrega>()
                .Where(o => o.TipoOperacao.Codigo == codigoTipoOperacao && o.Gatilho == gatilho);

            return consultaGatilhoGeracaoAutomatica
                .Fetch(o => o.TipoOcorrencia)
                .ToList();
        }

        #endregion
    }
}
