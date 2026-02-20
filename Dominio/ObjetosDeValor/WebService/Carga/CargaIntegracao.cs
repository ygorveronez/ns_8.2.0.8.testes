using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    [DataContract]
    public class CargaIntegracao
    {
        [DataMember]
        public int ProtocoloCarga { get; set; }

        [DataMember]
        public int ProtocoloPedido { get; set; }

        [DataMember]
        public string NumeroControlePedido { get; set; }

        [DataMember]
        public string NumeroCarga { get; set; }

        [DataMember]
        public string NumeroPreCarga { get; set; }

        [DataMember]
        public string IdentificacaoAdicional { get; set; }

        [DataMember]
        public string NumeroPedidoEmbarcador { get; set; }

        public string NumeroPedidoEmbarcadorSemRegra { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoCarregamento TipoCarregamento { get; set; }

        public string RegraMontarNumeroPedidoEmbarcadorWebService { get; set; }

        [DataMember]
        public int CFOP { get; set; }

        /// <summary>
        /// Flag utilizada para indicar se o valor do frete já foi calculado no sistema.
        /// </summary>
        [DataMember]
        public bool ValorFreteCalculado { get; set; }

        [DataMember]
        public Embarcador.Filial.Filial Filial { get; set; }

        [DataMember]
        public Embarcador.Filial.Filial FilialVenda { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Empresa TransportadoraEmitente { get; set; }

        [DataMember]
        public bool UsarOutroEnderecoOrigem { get; set; }

        [DataMember]
        public Embarcador.Localidade.Endereco Origem { get; set; }

        [DataMember]
        public bool UsarOutroEnderecoDestino { get; set; }

        [DataMember]
        public Embarcador.Localidade.Endereco Destino { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Regiao RegiaoDestino { get; set; }

        [DataMember]
        public Embarcador.Logistica.Fronteira Fronteira { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa PontoPartida { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa Remetente { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa Destinatario { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa Tomador { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa Recebedor { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa Expedidor { get; set; }

        [DataMember]
        public Embarcador.Frete.FreteRota FreteRota { get; set; }

        [DataMember]
        public Embarcador.Pedido.CanalEntrega CanalEntrega { get; set; }

        [DataMember]
        public Embarcador.Pedido.CanalVenda CanalVenda { get; set; }

        [DataMember]
        public string CustoFrete { get; set; }

        [DataMember]
        public Embarcador.Pedido.Deposito Deposito { get; set; }

        [DataMember]
        public string DataInicioCarregamento { get; set; }

        [DataMember]
        public string DataFinalCarregamento { get; set; }

        [DataMember]
        public string DataTerminoCarregamento { get; set; }

        [DataMember]
        public string DataValidade { get; set; }

        [DataMember]
        public string DataAgendamento { get; set; }

        [DataMember]
        public string NumeroAgendamento { get; set; }

        [DataMember]
        public string DataColeta { get; set; }

        [DataMember]
        public string DataPrevisaoEntrega { get; set; }

        [DataMember]
        public string DataPrevisao { get; set; }

        [DataMember]
        public string HoraPrevisaoEntrega { get; set; }

        [DataMember]
        public string PrevisaoEntregaTransportador { get; set; }

        [DataMember]
        public bool EntregaAgendada { get; set; }

        [DataMember]
        public string SenhaAgendamentoEntrega { get; set; }

        [DataMember]
        public string DataCriacaoCarga { get; set; }

        [DataMember]
        public string DataCancelamentoCarga { get; set; }

        [DataMember]
        public string DataAnulacaoCarga { get; set; }

        [DataMember]
        public string DataUltimaLiberacao { get; set; }

        [DataMember]
        public string UsuarioCriacaoRemessa { get; set; }

        [DataMember]
        public string NumeroOrdem { get; set; }

        [DataMember]
        public int NumeroPaletes { get; set; }

        [DataMember]
        public decimal NumeroPaletesPagos { get; set; }

        [DataMember]
        public decimal NumeroSemiPaletes { get; set; }

        [DataMember]
        public decimal NumeroSemiPaletesPagos { get; set; }

        [DataMember]
        public decimal NumeroPaletesFracionado { get; set; }

        [DataMember]
        public decimal NumeroCombis { get; set; }

        [DataMember]
        public decimal NumeroCombisPagas { get; set; }

        [DataMember]
        public decimal PesoTotalPaletes { get; set; }

        [DataMember]
        public decimal ValorTotalPaletes { get; set; }

        [DataMember]
        public decimal PesoBruto { get; set; }

        [DataMember]
        public decimal PesoLiquido { get; set; }

        [DataMember]
        public decimal CubagemTotal { get; set; }

        [DataMember]
        public int QuantidadeVolumes { get; set; }

        [DataMember]
        public decimal Distancia { get; set; }

        private List<Embarcador.Pedido.Produto> _produtos = null;

        [DataMember]
        public List<Embarcador.Pedido.Produto> Produtos
        {
            get => _produtos;
            set
            {
                if (value != null && value.Count > 0)
                    value.ForEach(p => p.DescricaoProduto = !string.IsNullOrEmpty(p.DescricaoProduto) ? System.Security.SecurityElement.Escape(p.DescricaoProduto) : p.DescricaoProduto);

                _produtos = value;
            }
        }

        [DataMember]
        public Embarcador.Pedido.Produto ProdutoPredominante { get; set; }

        [DataMember]
        public bool UtilizarTipoTomadorInformado { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Enumeradores.TipoTomador TipoTomador { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoTomadorCabotagem TipoTomadorCabotagem { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Enumeradores.TipoPagamento TipoPagamento { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoModalPropostaCabotagem TipoModalPropostaCabotagem { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaCabotagem TipoPropostaCabotagem { get; set; }

        [DataMember]
        public List<Embarcador.Carga.Motorista> Motoristas { get; set; }

        [DataMember]
        public Embarcador.Frota.Veiculo Veiculo { get; set; }

        [DataMember]
        public Embarcador.Frota.Veiculo VeiculoDaNota { get; set; }

        [DataMember]
        public Embarcador.Carga.ModeloVeicular ModeloVeicular { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoOperacao TipoOperacaoPedido { get; set; }

        [DataMember]
        public string KMOrigemXDestino { get; set; }

        [DataMember]
        public Embarcador.Carga.Funcionario FuncionarioVendedor { get; set; }

        [DataMember]
        public Embarcador.Carga.Funcionario FuncionarioSupervisor { get; set; }

        [DataMember]
        public Embarcador.Carga.Funcionario FuncionarioGerente { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoCargaEmbarcador TipoCargaEmbarcador { get; set; }

        [DataMember]
        public Embarcador.Frete.FreteValor ValorFrete { get; set; }

        [DataMember]
        public Embarcador.Frete.FreteValor ValorFreteFilialEmissora { get; set; }

        [DataMember]
        public bool FecharCargaAutomaticamente { get; set; }

        [DataMember]
        public bool ViagemJaFoiFinalizada { get; set; }

        [DataMember]
        public bool PedidoPallet { get; set; }

        [DataMember]
        public string Observacao { get; set; }

        [DataMember]
        public string ObservacaoTransportador { get; set; }

        [DataMember]
        public string ObservacaoInterna { get; set; }

        [DataMember]
        public string ObservacaoCTe { get; set; }

        [DataMember]
        public string CodigoAgrupamento { get; set; }

        [DataMember]
        public string ObservacaoLocalEntrega { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Enumeradores.TipoRateioProdutos TipoRateioProdutos { get; set; }

        [DataMember]
        public string ImpressoraNumero { get; set; }

        [DataMember]
        public List<string> Lacres { get; set; }

        [DataMember]
        public string NumeroTransferencia { get; set; }

        [DataMember]
        public string Alocacao { get; set; }

        [DataMember]
        public decimal ValorDescarga { get; set; }

        [DataMember]
        public decimal ValorPedagio { get; set; }

        [DataMember]
        public List<Embarcador.NFe.NotaFiscal> NotasFiscais { get; set; }

        [DataMember]
        public List<Embarcador.CTe.CTe> CTes { get; set; }

        [DataMember]
        public List<Embarcador.Carga.BlocoCarregamento> BlocosCarregamento { get; set; }

        [DataMember]
        public List<Embarcador.Carga.Distribuicao> Distribuicoes { get; set; }

        [DataMember]
        public Embarcador.Carga.Doca Doca { get; set; }

        [DataMember]
        public Embarcador.Carga.Averbacao Averbacao { get; set; }

        [DataMember]
        public string Temperatura { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Carga.FaixaTemperatura FaixaTemperatura { get; set; }

        [DataMember]
        public string Vendedor { get; set; }

        [DataMember]
        public string Ordem { get; set; }

        [DataMember]
        public int OrdemColetaProgramada { get; set; }

        [DataMember]
        public string PortoSaida { get; set; }

        [DataMember]
        public string PortoChegada { get; set; }

        [DataMember]
        public string Companhia { get; set; }

        [DataMember]
        public string Navio { get; set; }

        [DataMember]
        public string Reserva { get; set; }

        [DataMember]
        public string Resumo { get; set; }

        [DataMember]
        public string ETA { get; set; }

        [DataMember]
        public string TipoEmbarque { get; set; }

        [DataMember]
        public decimal ValorFreteCobradoCliente { get; set; }

        [DataMember]
        public decimal ValorCustoFrete { get; set; }

        [DataMember]
        public decimal ValorFreteInformativo { get; set; }

        [DataMember]
        public string CodigoIntegracaoRota { get; set; }

        [DataMember]
        public string DescricaoRota { get; set; }

        [DataMember]
        public string DataInclusaoPCP { get; set; }

        [DataMember]
        public string DataInclusaoBooking { get; set; }

        [DataMember]
        public string DeliveryTerm { get; set; }

        [DataMember]
        public string IdAutorizacao { get; set; }

        [DataMember]
        public string IdentificadorRota { get; set; }

        [DataMember]
        public bool PossuiGenset { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPedido? TipoPedido { get; set; }

        [DataMember]
        public int OrdemEntrega { get; set; }

        [DataMember]
        public int OrdemColeta { get; set; }

        [DataMember]
        public Embarcador.Carga.FreteNegociado FreteNegociado { get; set; }

        [DataMember]
        public bool PedidoTrocaNota { get; set; }

        [DataMember]
        public string NumeroPedidoTrocaNota { get; set; }

        [DataMember]
        public string NumeroCIOT { get; set; }

        [DataMember]
        public bool NaoGlobalizarPedido { get; set; }

        [DataMember]
        public string Adicional1 { get; set; }

        [DataMember]
        public string Adicional2 { get; set; }

        [DataMember]
        public string Adicional3 { get; set; }

        [DataMember]
        public string Adicional4 { get; set; }

        [DataMember]
        public string Adicional5 { get; set; }

        [DataMember]
        public string Adicional6 { get; set; }

        [DataMember]
        public string Adicional7 { get; set; }

        [DataMember]
        public bool PermiteQuebraPedidoMultiplosCarregamentos { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa ClienteAdicional { get; set; }

        [DataMember]
        public Despachante Despachante { get; set; }

        [DataMember]
        public ViaTransporte ViaTransporte { get; set; }

        [DataMember]
        public PortoViagem PortoViagemOrigem { get; set; }

        [DataMember]
        public PortoViagem PortoViagemDestino { get; set; }

        [DataMember]
        public InLand InLand { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.PagamentoMaritimo? PagamentoMaritimo { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa ClienteDonoContainer { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoProbe? TipoProbe { get; set; }

        [DataMember]
        public bool CargaPaletizada { get; set; }

        [DataMember]
        public NavioViagem NavioViagem { get; set; }

        [DataMember]
        public string ETS { get; set; }

        [DataMember]
        public int FreeDeten { get; set; }

        [DataMember]
        public string NumeroEXP { get; set; }

        [DataMember]
        public string RefEXPTransferencia { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.StatusEXP? StatusEXP { get; set; }

        [DataMember]
        public string NumeroPedidoProvisorio { get; set; }

        [DataMember]
        public Especie Especie { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.StatusPedidoEmbarcador? StatusPedidoEmbarcador { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.AcondicionamentoCarga? AcondicionamentoCarga { get; set; }

        [DataMember]
        public string DataEstufagem { get; set; }

        [DataMember]
        public string Onda { get; set; }

        [DataMember]
        public string ClusterRota { get; set; }

        [DataMember]
        public string DataPrevisaoInicioViagem { get; set; }

        [DataMember]
        public string DataPrevisaoChegadaDestinatario { get; set; }

        [DataMember]
        public string NumeroPager { get; set; }

        [DataMember]
        public string DataInicioViagem { get; set; }

        [DataMember]
        public string DataSeparacaoMercadoria { get; set; }

        [DataMember]
        public string RotaEmbarcador { get; set; }

        #region Aquaviario para Aliança

        [DataMember]
        public string NumeroBooking { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa Embarcador { get; set; }

        [DataMember]
        public Embarcador.Carga.Viagem Viagem { get; set; }

        [DataMember]
        public Embarcador.Carga.Viagem ViagemLongoCurso { get; set; }

        [DataMember]
        public Embarcador.Carga.Porto PortoOrigem { get; set; }

        [DataMember]
        public Embarcador.Carga.Porto PortoDestino { get; set; }

        [DataMember]
        public Embarcador.Carga.TerminalPorto TerminalPortoOrigem { get; set; }

        [DataMember]
        public Embarcador.Carga.TerminalPorto TerminalPortoDestino { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoContainer TipoContainerReserva { get; set; }

        [DataMember]
        public bool ContemCargaPerigosa { get; set; }

        [DataMember]
        public bool ContemCargaRefrigerada { get; set; }

        [DataMember]
        public string TemperaturaObservacao { get; set; }

        [DataMember]
        public bool ValidarNumeroContainer { get; set; }

        [DataMember]
        public Embarcador.Carga.PropostaComercial PropostaComercial { get; set; }

        [DataMember]
        public List<Embarcador.Carga.Transbordo> Transbordo { get; set; }

        [DataMember]
        public int CodigoOrdemServico { get; set; }

        [DataMember]
        public string NumeroOrdemServico { get; set; }

        [DataMember]
        public string Embarque { get; set; }

        [DataMember]
        public string MasterBL { get; set; }

        [DataMember]
        public string NumeroDIEmbarque { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa ProvedorOS { get; set; }

        [DataMember]
        public Embarcador.Carga.Container Container { get; set; }

        [DataMember]
        public int TaraContainer { get; set; }

        [DataMember]
        public string NumeroLacre1 { get; set; }

        [DataMember]
        public string NumeroLacre2 { get; set; }

        [DataMember]
        public string NumeroLacre3 { get; set; }

        [DataMember]
        public int CodigoBooking { get; set; }

        [DataMember]
        public string NumeroBL { get; set; }

        [DataMember]
        public bool NecessitaAverbacao { get; set; }

        [DataMember]
        public bool CargaRefrigeradaPrecisaEnergia { get; set; }

        [DataMember]
        public int QuantidadeTipoContainerReserva { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Enumeradores.FormaAverbacaoCTE? FormaAverbacaoCTE { get; set; }

        [DataMember]
        public decimal PercentualADValorem { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Enumeradores.TipoDocumentoAverbacao? TipoDocumentoAverbacao { get; set; }

        [DataMember]
        public string ObservacaoProposta { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Enumeradores.TipoPropostaFeeder? TipoPropostaFeeder { get; set; }

        [DataMember]
        public string DescricaoTipoPropostaFeeder { get; set; }

        [DataMember]
        public string DescricaoCarrierNavioViagem { get; set; }

        [DataMember]
        public bool RealizarCobrancaTaxaDocumentacao { get; set; }

        [DataMember]
        public int QuantidadeConhecimentosTaxaDocumentacao { get; set; }

        [DataMember]
        public decimal ValorTaxaDocumento { get; set; }

        [DataMember]
        public bool ContainerADefinir { get; set; }

        [DataMember]
        public decimal ValorCusteioSVM { get; set; }

        [DataMember]
        public int QuantidadeContainerBooking { get; set; }

        [DataMember]
        public Embarcador.Carga.EmpresaResponsavel EmpresaResponsavel { get; set; }

        [DataMember]
        public Embarcador.Carga.CentroCusto CentroCusto { get; set; }

        [DataMember]
        public List<string> CNPJsDestinatariosNaoAutorizados { get; set; }

        [DataMember]
        public string ParametroIdentificacaoCliente { get; set; }

        [DataMember]
        public bool PedidoDeSVMTerceiro { get; set; }

        [DataMember]
        public bool CargaSVMProprio { get; set; }

        [DataMember]
        public string NumeroCE { get; set; }

        [DataMember]
        public decimal ValorTaxaFeeder { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCalculoCargaFracionada? TipoCalculoCargaFracionada { get; set; }

        [DataMember]
        public bool CargaDePreCarga { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga SituacaoCarga { get; set; }

        [DataMember]
        public string OperadorCargaNome { get; set; }

        [DataMember]
        public string OperadorCargaEmail { get; set; }

        [DataMember]
        public string OperadorCargaCPF { get; set; }

        [DataMember]
        public bool NaoAtualizarDadosDoPedido { get; set; }

        [DataMember]
        public string IMOUnidade { get; set; }

        [DataMember]
        public string IMOClasse { get; set; }

        [DataMember]
        public string IMOSequencia { get; set; }

        #endregion

        [DataMember]
        public bool NecessarioAjudante { get; set; }

        [DataMember]
        public bool AntecipacaoICMS { get; set; }

        [DataMember]
        public int DiasItinerario { get; set; }

        [DataMember]
        public int DiasUteisPrazoTransportador { get; set; }

        public string NumeroDI { get; set; }

        [DataMember]
        public int NumeroEntregasFinais { get; set; }

        [DataMember]
        public bool PossuiPendenciaRoteirizacao { get; set; }

        [DataMember]
        public string NumeroPedidoCliente { get; set; }

        [DataMember]
        public int IDPropostaTrizy { get; set; }

        [DataMember]
        public virtual int IDLoteTrizy { get; set; }

        [DataMember]
        public DadosTransporteMaritimo DadosTransporteMaritimo { get; set; }

        [DataMember]
        public decimal ValorTotalPedido { get; set; }

        [DataMember]
        public bool PedidoDeDevolucao { get; set; }

        [DataMember]
        public string NumeroPedidoDevolucao { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoDetalhe ProcessamentoEspecial { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoDetalhe HorarioEntrega { get; set; }

        [JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
        [DataMember]
        public List<Embarcador.Enumeradores.DiaSemana> DiasRestricaoEntrega { get; set; }

        [DataMember]
        public string NumeroPedidoICT { get; set; }

        [DataMember]
        public string CondicaoExpedicao { get; set; }

        [DataMember]
        public string GrupoFreteMaterial { get; set; }

        [DataMember]
        public string RestricaoEntrega { get; set; }

        [DataMember]
        public string DataCriacaoRemessa { get; set; }

        [DataMember]
        public string DataCriacaoVenda { get; set; }

        [DataMember]
        public string PrevisaoTerminoViagem { get; set; }

        [DataMember]
        public string PrevisaoSaidaDestino { get; set; }

        [DataMember]
        public string PrevisaoStopTracking { get; set; }

        [DataMember]
        public string IndicadorPOF { get; set; }

        [DataMember]
        public string NumeroRastreioCorreios { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoDetalhe ZonaTransporte { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoDetalhe PeriodoEntrega { get; set; }

        [DataMember]
        public Embarcador.Carga.TipoDetalhe DetalheEntrega { get; set; }

        [DataMember]
        public bool ProdutoVolumoso { get; set; }

        [DataMember]
        public long ProtocoloCotacao { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public ObjetosDeValor.Embarcador.Enumeradores.IndicativoColetaEntrega IndicativoColetaEntrega { get; set; }

        [DataMember]
        public string TipoServico { get; set; }

        [DataMember]
        public string NumeroAutorizacaoColetaEntrega { get; set; }

        [DataMember]
        public Embarcador.Pessoas.Pessoa ClientePropostaComercial { get; set; }

        [DataMember]
        public string NumeroContratoFreteCliente { get; set; }

        [DataMember]
        public string TipoSeguro { get; set; }

        [DataMember]
        public string NumeroOSMae { get; set; }

        [DataMember]
        public string CodigoPedidoCliente { get; set; }

        [DataMember]
        public List<string> ChavesCTes { get; set; }

        [DataMember]
        public decimal KMAsfaltoAteDestino { get; set; }

        [DataMember]
        public decimal KMChaoAteDestino { get; set; }

        [DataMember]
        public string CodigoAgrupamentoCarregamento { get; set; }

        [DataMember]
        public int NumeroAcerto { get; set; }

        [DataMember]
        public string DataInicialAcerto { get; set; }

        [DataMember]
        public string DataFinalAcerto { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoServicoCarga? TipoServicoCarga { get; set; }

        [DataMember]
        public bool? ExecaoCab { get; set; }

        [DataMember]
        public string CategoriaCargaEmbarcador { get; set; }

        [DataMember]
        public string SetPointVeiculo { get; set; }

        [DataMember]
        public List<Dominio.ObjetosDeValor.PercursoMDFe> PercursosMDFe { get; set; }

        [DataMember]
        public bool PedidoComRestricaoData { get; set; }

        [DataMember]
        public string NumeroOT { get; set; }
        [DataMember]

        public bool PedidoBloqueado { get; set; }

        [DataMember]
        public string DataCriacaoPedidoERP { get; set; }

        [DataMember]
        public string ObsCarregamento { get; set; }

        [DataMember]
        public Embarcador.Pedido.SituacaoComercial SituacaoComercial { get; set; }

        [DataMember]
        public Embarcador.Pedido.SituacaoEstoquePedido SituacaoEstoque { get; set; }

        [DataMember]
        public bool EssePedidopossuiPedidoBonificacao { get; set; }

        [DataMember]
        public bool EssePedidopossuiPedidoVenda { get; set; }

        [DataMember]
        public string NumeroPedidoVinculado { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoPaleteCliente? TipoPaleteCliente { get; set; }

        [DataMember]
        public Dominio.ObjetosDeValor.Monitoramento.Posicao PrimeiraPosicaoMonitoramento { get; set; }

        [DataMember]
        public string SituacaoMonitoramento { get; set; }

        [DataMember]
        public Embarcador.Pedido.ConsultaValePedagio ConsultaValePedagio { get; set; }

        [DataMember]
        public bool? ConversaoCargaPaletizada { get; set; }

        [DataMember]
        public bool NaoComprarValePedagio { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.CategoriaOS? CategoriaOS { get; set; }

        [DataMember]
        public string NecessariaAverbacao { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.DocumentoProvedor? DocumentoProvedor { get; set; }

        [DataMember]
        public decimal ValorTotalProvedor { get; set; }

        [DataMember]
        public string LiberarPagamento { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoOS? TipoOS { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoOSConvertido? TipoOSConvertido { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.DirecionamentoOS? DirecionamentoOS { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoServicoXML? TipoServicoXML { get; set; }

        [DataMember]
        public bool IndicLiberacaoOk { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.TipoDirecionamentoCustoExtra TipoDirecionamentoCustoExtra { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [DataMember]
        public Embarcador.Enumeradores.StatusCustoExtra StatusCustoExtra { get; set; }

        [DataMember]
        public bool ImprimirObservacaoCTe { get; set; }

        [DataMember]
        public bool CargaCritica { get; set; }

        [DataMember]
        public bool Mesclar { get; set; }

        [DataMember]
        public bool PedidoAlterado { get; set; }

        [DataMember]
        public List<string> PlacasCarregamento { get; set; }

        [DataMember]
        public string EscritorioVenda { get; set; }

        [DataMember]
        public string EquipeVendas { get; set; }

        [DataMember]
        public string TipoMercadoria { get; set; }

        [DataMember]
        public bool Parqueada { get; set; }

        [DataMember]
        public string RastreamentoPedido { get; set; }

        public string RastreamentoMonitoramento  { get; set; }

        [DataMember]
        public WebService.Pedido.SeparacaoPedido InformarSeparacaoPedido { get; set; }
    }
}
