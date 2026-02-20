using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedido
{
    public class Pedido
    {
        #region Propriedades

        public int Codigo { get; set; }

        public bool Ajudante { get; set; }

        public string CodigoAgrupamentoCarregamento { get; set; }

        public string CodigoPedidoCliente { get; set; }

        public string Companhia { get; set; }

        public decimal CubagemTotal { get; set; }

        public DateTime? DataAgendamento { get; set; }

        public DateTime? DataCarregamentoCarga { get; set; }

        public DateTime? DataCarregamentoPedido { get; set; }

        public DateTime? DataCriacao { get; set; }

        public DateTime? DataETA { get; set; }

        public DateTime? DataInclusaoBooking { get; set; }

        public DateTime? DataInclusaoPCP { get; set; }

        public DateTime? DataInicialColeta { get; set; }

        public DateTime? DataPrevisaoSaida { get; set; }

        public DateTime? DataTerminoCarregamento { get; set; }

        public string DeliveryTerm { get; set; }

        public int DiasUteisPrazoTransportador { get; set; }

        public bool DisponibilizarPedidoParaColeta { get; set; }

        public decimal Distancia { get; set; }

        public string IdAutorizacao { get; set; }

        public int Numero { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroContainer { get; set; }

        public string NumeroNavio { get; set; }

        public string NumeroOrdem { get; set; }

        public int NumeroPaletes { get; set; }

        public decimal NumeroPaletesFracionado { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public string Observacao { get; set; }

        public string ObservacaoCTe { get; set; }

        public string ObservacaoInterna { get; set; }

        public string Ordem { get; set; }

        public decimal PalletSaldoRestante { get; set; }

        public bool PedidoBloqueado { get; set; }

        public bool PedidoLiberadoMontagemCarga { get; set; }

        public bool PedidoRestricaoData { get; set; }

        public bool PedidoTotalmenteCarregado { get; set; }

        public decimal PercentualSeparacaoPedido { get; set; }

        public decimal PesoLiquidoTotal { get; set; }

        public decimal PesoSaldoRestante { get; set; }

        public decimal PesoTotal { get; set; }

        public decimal PesoTotalPaletes { get; set; }

        public string PortoChegada { get; set; }

        public string PortoSaida { get; set; }

        public bool PossuiCarga { get; set; }

        public bool PossuiDescarga { get; set; }

        public DateTime? PrevisaoEntrega { get; set; }

        public string ProdutoPredominante { get; set; }

        public int QtVolumes { get; set; }

        public int QtdAjudantes { get; set; }

        public bool ReentregaSolicitada { get; set; }

        public string Reserva { get; set; }

        public string Resumo { get; set; }

        public int SaldoVolumesRestante { get; set; }

        public string SenhaAgendamento { get; set; }

        public string SenhaAgendamentoCliente { get; set; }

        public SituacaoPedido Situacao { get; set; }

        public string Temperatura { get; set; }

        public string TipoEmbarque { get; set; }

        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        public TipoPaleteCliente? TipoPaleteCliente { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public bool UsarOutroEnderecoDestino { get; set; }

        public decimal? ValorCarga { get; set; }

        public decimal? ValorCobrancaFreteCombinado { get; set; }

        public decimal? ValorDescarga { get; set; }

        public decimal ValorFreteNegociado { get; set; }

        public decimal ValorTotalNotasFiscais { get; set; }

        public string Vendedor { get; set; }

        public PedidoAdicional PedidoAdicional { get; set; }

        public Filial Filial { get; set; }

        public Empresa Empresa { get; set; }

        public TipoCarga TipoCarga { get; set; }

        public TipoOperacao TipoOperacao { get; set; }

        public Localidade Origem { get; set; }

        public Localidade Destino { get; set; }

        public Endereco EnderecoOrigem { get; set; }

        public Endereco EnderecoDestino { get; set; }

        public Cliente Remetente { get; set; }

        public Cliente Destinatario { get; set; }

        public Cliente Expedidor { get; set; }

        public Cliente Recebedor { get; set; }

        public Cliente Tomador { get; set; }

        public Cliente RecebedorColeta { get; set; }

        public Cliente LocalPaletizacao { get; set; }

        public Cliente Fronteira { get; set; }

        public GrupoPessoas GrupoPessoas { get; set; }

        public ModeloVeicularCarga ModeloVeicularCarga { get; set; }

        public CanalEntrega CanalEntrega { get; set; }

        public CanalVenda CanalVenda { get; set; }

        public RotaFrete RotaFrete { get; set; }

        public Usuario FuncionarioGerente { get; set; }

        public Usuario FuncionarioSupervisor { get; set; }

        public Usuario FuncionarioVendedor { get; set; }

        public Usuario Usuario { get; set; }

        public Usuario Autor { get; set; }

        public SituacaoComercialPedido SituacaoComercialPedido { get; set; }

        public Container Container { get; set; }

        public PedidoViagemNavio PedidoViagemNavio { get; set; }

        public Porto Porto { get; set; }

        public Porto PortoDestino { get; set; }

        public List<PedidoProduto> Produtos { get; set; }

        public List<PedidoAnexo> Anexos { get; set; }

        public List<Cliente> Fronteiras { get; set; }

        #endregion Propriedades

        #region Propriedades com Regras

        public bool LatitudelongitudeNaoInformada
        {
            get
            {
                if ((Remetente != null) && !Remetente.Endereco.LatitudelongitudeInformada)
                    return true;

                if ((Destinatario != null) && !UsarOutroEnderecoDestino && !Destinatario.Endereco.LatitudelongitudeInformada)
                    return true;

                if ((Destinatario != null) && UsarOutroEnderecoDestino && !(EnderecoDestino?.LatitudelongitudeInformada ?? false))
                    return true;

                if ((Recebedor != null) && !Recebedor.Endereco.LatitudelongitudeInformada)
                    return true;

                if ((Expedidor != null) && !Expedidor.Endereco.LatitudelongitudeInformada)
                    return true;

                return false;
            }
        }

        public Cliente ObterTomador()
        {
            if (TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                return Remetente;

            if (TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                return Destinatario;

            if ((TipoTomador == Dominio.Enumeradores.TipoTomador.Outros) || (TipoTomador == Dominio.Enumeradores.TipoTomador.Tomador))
                return Tomador;

            if (TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                return Recebedor;

            if (TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                return Expedidor;

            return null;
        }

        #endregion Propriedades com Regras
    }
}
