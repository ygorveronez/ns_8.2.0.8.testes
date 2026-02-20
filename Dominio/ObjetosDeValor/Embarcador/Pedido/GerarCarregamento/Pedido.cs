using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.GerarCarregamento
{
    public class Pedido
    {
        #region Campos usados para consulta
        public int CodigoPedido { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public Dominio.ObjetosDeValor.Localidade Origem { get; set; }
        public Dominio.ObjetosDeValor.Localidade Destino { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public DateTime? DataCarregamentoPedido { get; set; }
        public Dominio.ObjetosDeValor.Cliente Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Cliente Recebedor { get; set; }
        public Dominio.ObjetosDeValor.Cliente Expedidor { get; set; }
        public Dominio.ObjetosDeValor.Cliente Remetente { get; set; }
        public Dominio.ObjetosDeValor.Cliente Tomador { get; set; }
        public Dominio.ObjetosDeValor.Cliente PontoPartida { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo> Veiculos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo VeiculoTracao { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Carga.Motorista> Motoristas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoFronteira> Fronteiras { get; set; }
        public int NumeroPaletes { get; set; }
        public decimal TotalPallets { get; set; }
        public decimal SaldoVolumesRestante { get; set; }
        public decimal ValorTotalPaletes { get; set; }
        public int Numero { get; set; }
        public TipoCarga TipoCarga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoDeCarga TipoDeCarga { get; set; }
        public TipoOperacao TipoOperacao { get; set; }
        public string Observacao { get; set; }
        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }
        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal CubagemTotal { get; set; }
        public decimal PesoLiquidoTotal { get; set; }
        public decimal ValorTotalNotasFiscais { get; set; }
        public decimal ValorFreteNegociado { get; set; }
        public decimal ValorFreteAReceber { get; set; }
        public decimal PercentualAliquota { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal PesoTotalPaletes { get; set; }
        public decimal ValorFreteFilialEmissora { get; set; }
        public DateTime? DataInicialColeta { get; set; }
        public DateTime? DataFinalColeta { get; set; }
        public int OrdemColetaProgramada { get; set; }
        public int OrdemEntregaProgramada { get; set; }
        public decimal PercentualInclusaoBC { get; set; }
        public bool ColetaEmProdutorRural { get; set; }
        public bool PedidoDeDevolucao { get; set; }
        public bool ImpostoNegociado { get; set; }
        public bool UsarOutroEnderecoOrigem { get; set; }
        public bool IncluirBaseCalculoICMS { get; set; }
        public bool PedidoTakeOrPay { get; set; }
        public bool PedidoDemurrage { get; set; }
        public bool PedidoDetention { get; set; }
        public bool PedidoDeSVMTerceiro { get; set; }
        public bool PedidoSVM { get; set; }
        public ObjetosDeValor.Embarcador.Pedido.CanalEntrega CanalEntrega { get; set; }
        public int CodigoCanalVenda { get; set; }
        public int CodigoFilial { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoRotaFrete { get; set; }
        public int CodigoCentroResultadoEmbarcador { get; set; }
        public int CodigoContaContabil { get; set; }
        public int QuantidadeNotasFiscais { get; set; }
        public string ElementoPEP { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.FormaEmissaoSVM PortoDestinoFormaEmissaoSVM { get; set; }

        #endregion

        #region Campos utilizados para atualizar dados

        public DateTime? PrevisaoEntrega { get; set; }
        public int QtVolumes { get; set; }
        public bool ReentregaSolicitada { get; set; }
        public bool UsarTipoPagamentoNF { get; set; }
        public bool UsarTipoTomadorPedido { get; set; }
        public bool PedidoIntegradoEmbarcador { get; set; }
        public bool PedidoTotalmenteCarregado { get; set; }
        public bool PedidoRedespachoTotalmenteCarregado { get; set; }
        public bool PedidoDePreCarga { get; set; }
        public decimal PesoSaldoRestante { get; set; }
        public decimal PalletSaldoRestante { get; set; }

        #endregion

        public virtual Dominio.ObjetosDeValor.Cliente ObterTomador()
        {
            if (TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                return Remetente;
            else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                return Destinatario;
            else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Outros || TipoTomador == Dominio.Enumeradores.TipoTomador.Tomador)
                return Tomador;
            else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                return Recebedor;
            else if (TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                return Expedidor;
            else
                return null;
        }

    }
}
