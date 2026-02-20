using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga
{
    public class CargaPedidoHistorico : ServicoBase
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public CargaPedidoHistorico(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public async Task CriarCargaPedidoHistoricoAsync(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaPedidoHistoricoTipoAcao tipoAcao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaPedidoHistoricoSituacaoIntegracao situacaoIntegracao, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico cargaPedidoHistorico = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoHistorico
            {
                TipoAcao = tipoAcao,
                SituacaoIntegracao = situacaoIntegracao,
                Pedido = pedido,
                Carga = carga,
                DataCriacao = DateTime.Now
            };

            Repositorio.Embarcador.Cargas.CargaPedidoHistorico repcargaPedidoHistorico = new Repositorio.Embarcador.Cargas.CargaPedidoHistorico(_unitOfWork);
            await repcargaPedidoHistorico.InserirAsync(cargaPedidoHistorico);
        }

        #endregion

    }
}
