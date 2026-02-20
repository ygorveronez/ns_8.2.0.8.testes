using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pedido
{
    public sealed class TipoOperacao
    {
        #region Atributos

        public readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public TipoOperacao(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public TipoUltimoPontoRoteirizacao? ObterTipoUltimoPontoRoteirizacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Empresa transportador)
        {
            if (tipoOperacao == null)
                return null;

            if (transportador == null)
                return tipoOperacao.TipoUltimoPontoRoteirizacao;

            Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador repositorioTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador tipoOperacaoTransportador = repositorioTipoOperacaoTransportador.BuscarPorTipoOperacaoETransportador(tipoOperacao.Codigo, transportador.Codigo);

            return tipoOperacaoTransportador?.TipoUltimoPontoRoteirizacao ?? tipoOperacao.TipoUltimoPontoRoteirizacao;
        }


        public async Task<TipoUltimoPontoRoteirizacao?> ObterTipoUltimoPontoRoteirizacaoAsync(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao, Dominio.Entidades.Empresa transportador)
        {
            if (tipoOperacao == null)
                return null;

            if (transportador == null)
                return tipoOperacao.TipoUltimoPontoRoteirizacao;

            Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador repositorioTipoOperacaoTransportador = new Repositorio.Embarcador.Pedidos.TipoOperacaoTransportador(_unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoTransportador tipoOperacaoTransportador = await repositorioTipoOperacaoTransportador.BuscarPorTipoOperacaoETransportadorAsync(tipoOperacao.Codigo, transportador.Codigo);

            return tipoOperacaoTransportador?.TipoUltimoPontoRoteirizacao ?? tipoOperacao.TipoUltimoPontoRoteirizacao;
        }

        #endregion
    }
}
