using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Frete;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class Pedido
    {
        public int Protocolo { get; set; }

        public string NumeroPedido { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoCTeDocumentos TipoRateio { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Remetente { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Recebedor { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }

        public List<Dominio.ObjetosDeValor.WebService.Carga.NotaFiscal> NotasFiscais { get; set; }

        public bool PossuiCTe { get; set; }

        public bool PossuiNFS { get; set; }

        public bool PossuiNFSManual { get; set; }

        public string PalletAgrupamento { get; set; }

        public bool PedidoCancelado { get; set; }

        public List<string> CNPJsDestinatariosNaoAutorizados { get; set; }

        public bool CargaRefrigeradaPrecisaEnergia { get; set; }

        public Embarcador.Carga.CentroCusto CentroCusto { get; set; }
        public int CodigoBooking { get; set; }
        public int CodigoOrdemServico { get; set; }
        public Embarcador.Carga.Container Container { get; set; }
        public bool ContainerADefinir { get; set; }
        public bool ContemCargaPerigosa { get; set; }
        public bool ContemCargaRefrigerada { get; set; }
        public decimal CubagemTotal { get; set; }
        public string DataFinalCarregamento { get; set; }
        public string DataInicioCarregamento { get; set; }
        public string DataPrevisaoEntrega { get; set; }
        public string DataPrevisaoSaida { get; set; }
        public string DataCarregamentoPedido { get; set; }
        public string DataColeta { get; set; }
        public string DescricaoCarrierNavioViagem { get; set; }
        public string DescricaoTipoPropostaFeeder { get; set; }
        public Embarcador.Localidade.Endereco Destino { get; set; }
        public Empresa EmpresaResponsavel { get; set; }
        public Embarcador.Pessoas.Pessoa Embarcador { get; set; }
        public FormaAverbacaoCTE FormaAverbacaoCTE { get; set; }
        public bool NecessitaAverbacao { get; set; }
        public string NumeroBL { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroLacre1 { get; set; }
        public string NumeroLacre2 { get; set; }
        public string NumeroLacre3 { get; set; }
        public string NumeroOrdemServico { get; set; }
        public string NumeroPedidoEmbarcador { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoLocalEntrega { get; set; }
        public string ObservacaoProposta { get; set; }
        public string Ordem { get; set; }
        public Embarcador.Localidade.Endereco Origem { get; set; }
        public decimal PercentualADValorem { get; set; }
        public decimal PesoBruto { get; set; }
        public Porto PortoDestino { get; set; }
        public Porto PortoOrigem { get; set; }
        public List<CargaPedidoProduto> Produtos { get; set; }
        public PropostaComercial PropostaComercial { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa ProvedorOS { get; set; }
        public int QuantidadeConhecimentosTaxaDocumentacao { get; set; }
        public int QuantidadeContainerBooking { get; set; }
        public int QuantidadeTipoContainerReserva { get; set; }
        public int QuantidadeVolumes { get; set; }
        public bool RealizarCobrancaTaxaDocumentacao { get; set; }
        public int TaraContainer { get; set; }
        public string Temperatura { get; set; }
        public string TemperaturaObservacao { get; set; }
        public TerminalPorto TerminalPortoDestino { get; set; }
        public TerminalPorto TerminalPortoOrigem { get; set; }
        public TipoCargaEmbarcador TipoCargaEmbarcador { get; set; }
        public TipoContainer TipoContainerReserva { get; set; }
        public TipoDocumentoAverbacao TipoDocumentoAverbacao { get; set; }
        public TipoOperacao TipoOperacao { get; set; }
        public TipoPropostaFeeder TipoPropostaFeeder { get; set; }
        public List<Transbordo> Transbordo { get; set; }
        public Empresa TransportadoraEmitente { get; set; }
        public bool UsarOutroEnderecoDestino { get; set; }
        public bool UsarOutroEnderecoOrigem { get; set; }
        public bool ValidarNumeroContainer { get; set; }
        public decimal ValorDescarga { get; set; }
        public FreteValor ValorFrete { get; set; }
        public bool ValorFreteCalculado { get; set; }
        public decimal ValorPedagio { get; set; }
        public decimal ValorTaxaDocumento { get; set; }
        public decimal ValorTotalPaletes { get; set; }
        public bool ViagemJaFoiFinalizada { get; set; }
        public Embarcador.Carga.Viagem Viagem { get; set; }
        public Embarcador.Carga.Viagem ViagemLongoCurso { get; set; }
        public bool? ExecaoCap { get; set; }
        public List<Embarcador.Pedido.Produto> ProdutosPedido { get; set; }        
        public Embarcador.Pessoas.Empresa TransportadoraEmitentePedido { get; set; }
        public Embarcador.Enumeradores.TipoServicoCarga? TipoServicoCarga { get; set; }
        public Dominio.ObjetosDeValor.WebService.Carga.CanalEntrega CanalEntrega { get; set; }
        public bool EssePedidopossuiPedidoBonificacao { get; set; }
        public bool EssePedidopossuiPedidoVenda { get; set; }
        public string NumeroPedidoVinculado { get; set; }

    }
}
