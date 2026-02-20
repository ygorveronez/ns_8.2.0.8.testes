using Repositorio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Pedido
{
    public sealed class RegraNumeroPedidoEmbarcador : ServicoBase
    {

        #region Construtores

        public RegraNumeroPedidoEmbarcador(UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        #endregion



        #region Métodos Públicos

        public async Task DefinirNumeroPedidoEmbarcadorComRegraAsync(Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao cargaIntegracao)
        {
            if (cargaIntegracao.NaoAtualizarDadosDoPedido || string.IsNullOrWhiteSpace(cargaIntegracao.NumeroPedidoEmbarcador))
                return;
            
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadraoAsync();

            if (string.IsNullOrWhiteSpace(configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService))
                return;

            DateTime dataAtual = DateTime.Now;
            cargaIntegracao.RegraMontarNumeroPedidoEmbarcadorWebService = configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService;
            cargaIntegracao.NumeroPedidoEmbarcadorSemRegra = cargaIntegracao.NumeroPedidoEmbarcador;
            cargaIntegracao.NumeroPedidoEmbarcador = configuracaoEmbarcador.RegraMontarNumeroPedidoEmbarcadorWebService
                .Replace("#Ano#", dataAtual.ToString("yyyy"))
                .Replace("#Dia#", dataAtual.ToString("dd"))
                .Replace("#Horas#", dataAtual.ToString("HH"))
                .Replace("#Mes#", dataAtual.ToString("MM"))
                .Replace("#Minutos#", dataAtual.ToString("mm"))
                .Replace("#NumeroPedido#", cargaIntegracao.NumeroPedidoEmbarcadorSemRegra)
                .Replace("#Segundos#", dataAtual.ToString("ss"))
                .Trim();
        }

        public string ObterNumeroPedidoEmbarcadorSemRegra(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            return string.IsNullOrWhiteSpace(pedido.NumeroPedidoEmbarcadorSemRegra) ? pedido.NumeroPedidoEmbarcador : pedido.NumeroPedidoEmbarcadorSemRegra;
        }

        #endregion
    }
}
