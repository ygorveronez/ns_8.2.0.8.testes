using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public sealed class RelatorioCarga
    {
        #region Propriedades
        public decimal CapacidadePesoVeiculo { get; set; }
        public string CustoFrete { get; set; }
        public string Carregamento { get; set; }
        public string CategoriaDestinatario { get; set; }
        public string CategoriaRemetente { get; set; }
        public string CNPJFilial { get; set; }
        public string CNPJTransportador { get; set; }
        public string TipoTerceiro { get; set; }
        public int Codigo { get; set; }
        public int IDCarga { get; set; }
        public string CodigoIntegracaoDestinatarios { get; set; }
        public string CodigoIntegracaoRemetentes { get; set; }
        public string Companhia { get; set; }
        public string DataAnulacao { get; set; }
        public string DataCancelamento { get; set; }
        public DateTime DataCarga { get; set; }
        public DateTime DataCarregamento { get; set; }
        public string DataColeta { get; set; }
        public DateTime DataEmbarque { get; set; }
        public DateTime DataDescarregamento { get; set; }
        public string DataETA { get; set; }
        public string DataInclusaoBooking { get; set; }
        public string DataInclusaoPCP { get; set; }
        public string DeliveryTerm { get; set; }
        public string SituacaoAverbacao { get; set; }
        public string DataAverbacao { get; set; }
        public string NumeroAverbacao { get; set; }
        public string DataInicioEmissaoDocumentos { get; set; }
        public string DataFimEmissaoDocumentos { get; set; }
        public string DataRetiradaCtrn { get; set; }
        public string Destinatario { get; set; }
        public string Destino { get; set; }
        public string Filial { get; set; }
        public string FreteTerceiro { get; set; }
        public string GrupoDestinatario { get; set; }
        public string GrupoPessoas { get; set; }
        public string GrupoRemetente { get; set; }
        public string HoraFinal { get; set; }
        public string HoraInicial { get; set; }
        public string IdAutorizacao { get; set; }
        public string JustificativaAnulacao { get; set; }
        public string JustificativaCancelamento { get; set; }
        public int KMFinal { get; set; }
        public int KMInicial { get; set; }
        public decimal KmRodado { get; set; }
        public string Mdfes { get; set; }
        public string ModeloVeiculo { get; set; }
        public string MotivoCancelamento { get; set; }
        public string Motoristas { get; set; }
        public string NotasFiscais { get; set; }
        public string NotasParciais { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroCarga { get; set; }
        public int NumeroColetas { get; set; }
        public int NumeroEntregas { get; set; }
        public string NumeroNavio { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroPedidoInterno { get; set; }
        public string NumeroPedidoNotaFiscal { get; set; }
        public string NumeroValePedagio { get; set; }
        public string OperadorAnulacao { get; set; }
        public string OperadorCarga { get; set; }
        public string OperadorCancelamento { get; set; }
        public string Ordem { get; set; }
        public string Genset { get; set; }
        public string Origem { get; set; }
        public string PaisDestino { get; set; }
        public string PaisOrigem { get; set; }
        public decimal Pallets { get; set; }
        public decimal PesoCarga { get; set; }
        public decimal PesoLiquidoCarga { get; set; }
        public string PortoChegada { get; set; }
        public string PortoSaida { get; set; }
        public string ObservacaoInterna { get; set; }
        public string PreCarga { get; set; }
        public decimal QuantidadeHorasExcedentes { get; set; }
        public decimal QuantidadeHorasNormais { get; set; }
        public string Remetente { get; set; }
        public string Reserva { get; set; }
        public string Resumo { get; set; }
        public string Rota { get; set; }
        public SituacaoCarga Situacao { get; set; }
        public string TabelaFrete { get; set; }
        public string DataVigenciaTabelaFrete { get; set; }
        public decimal TaxaIncidenciaFrete { get; set; }
        public decimal TaxaOcupacaoVeiculo { get; set; }
        public string Temperatura { get; set; }
        public string CentroResultado { get; set; }
        public string TipoCarga { get; set; }
        public string TipoEmbarque { get; set; }
        public TipoFreteEscolhido TipoFreteEscolhido { get; set; }
        public string TipoOperacao { get; set; }
        public string Tomador { get; set; }
        public string Transbordo { get; set; }
        public string Transportador { get; set; }
        public string CargaLacre { get; set; }
        public string CentroDeCustoViagemDescricao { get; set; }
        public string GrupoProduto { get; set; }
        private SituacaoIntegracao SituacaoBRK { get; set; }
        public string MensagemBRK { get; set; }
        public DateTime DataInclusaoDadosTransporte { get; set; }

        #region Propriedades Dinâmicas

        public decimal ValorComponente1 { get; set; }
        public decimal ValorComponente2 { get; set; }
        public decimal ValorComponente3 { get; set; }
        public decimal ValorComponente4 { get; set; }
        public decimal ValorComponente5 { get; set; }
        public decimal ValorComponente6 { get; set; }
        public decimal ValorComponente7 { get; set; }
        public decimal ValorComponente8 { get; set; }
        public decimal ValorComponente9 { get; set; }
        public decimal ValorComponente10 { get; set; }
        public decimal ValorComponente11 { get; set; }
        public decimal ValorComponente12 { get; set; }
        public decimal ValorComponente13 { get; set; }
        public decimal ValorComponente14 { get; set; }
        public decimal ValorComponente15 { get; set; }
        public decimal ValorComponente16 { get; set; }
        public decimal ValorComponente17 { get; set; }
        public decimal ValorComponente18 { get; set; }
        public decimal ValorComponente19 { get; set; }
        public decimal ValorComponente20 { get; set; }
        public decimal ValorComponente21 { get; set; }
        public decimal ValorComponente22 { get; set; }
        public decimal ValorComponente23 { get; set; }
        public decimal ValorComponente24 { get; set; }
        public decimal ValorComponente25 { get; set; }
        public decimal ValorComponente26 { get; set; }
        public decimal ValorComponente27 { get; set; }
        public decimal ValorComponente28 { get; set; }
        public decimal ValorComponente29 { get; set; }
        public decimal ValorComponente30 { get; set; }
        public decimal ValorComponente31 { get; set; }
        public decimal ValorComponente32 { get; set; }
        public decimal ValorComponente33 { get; set; }
        public decimal ValorComponente34 { get; set; }
        public decimal ValorComponente35 { get; set; }
        public decimal ValorComponente36 { get; set; }
        public decimal ValorComponente37 { get; set; }
        public decimal ValorComponente38 { get; set; }
        public decimal ValorComponente39 { get; set; }
        public decimal ValorComponente40 { get; set; }
        public decimal ValorComponente41 { get; set; }
        public decimal ValorComponente42 { get; set; }
        public decimal ValorComponente43 { get; set; }
        public decimal ValorComponente44 { get; set; }
        public decimal ValorComponente45 { get; set; }
        public decimal ValorComponente46 { get; set; }
        public decimal ValorComponente47 { get; set; }
        public decimal ValorComponente48 { get; set; }
        public decimal ValorComponente49 { get; set; }
        public decimal ValorComponente50 { get; set; }
        public decimal ValorComponente51 { get; set; }
        public decimal ValorComponente52 { get; set; }
        public decimal ValorComponente53 { get; set; }
        public decimal ValorComponente54 { get; set; }
        public decimal ValorComponente55 { get; set; }
        public decimal ValorComponente56 { get; set; }
        public decimal ValorComponente57 { get; set; }
        public decimal ValorComponente58 { get; set; }
        public decimal ValorComponente59 { get; set; }
        public decimal ValorComponente60 { get; set; }

        #endregion

        public decimal ValorFrete { get; set; }
        public decimal ValorFreteInformadoPeloOperador { get; set; }
        public decimal ValorFreteLiquido { get; set; }
        public decimal ValorFreteResidual { get; set; }
        public decimal ValorViagem { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorISS { get; set; }
        public string CSTIBSCBS { get; set; }
        public string ClassificacaoTributariaIBSCBS { get; set; }
        public decimal BaseCalculoIBSCBS { get; set; }
        public decimal AliquotaIBSEstadual { get; set; }
        public decimal PercentualReducaoIBSEstadual { get; set; }
        public decimal ValorReducaoIBSEstadual { get; set; }
        public decimal ValorIBSEstadual { get; set; }
        public decimal AliquotaIBSMunicipal { get; set; }
        public decimal PercentualReducaoIBSMunicipal { get; set; }
        public decimal ValorReducaoIBSMunicipal { get; set; }
        public decimal ValorIBSMunicipal { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal PercentualReducaoCBS { get; set; }
        public decimal ValorReducaoCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public decimal ValorTabelaFrete { get; set; }
        public decimal ValorTotalNotaFiscal { get; set; }
        public decimal ValorValePedagio { get; set; }
        public string Veiculos { get; set; }
        public string GuaritaEntrada { get; set; }
        public string GuaritaSaida { get; set; }
        public string ETS { get; set; }
        public string ETA { get; set; }
        public string Navio { get; set; }
        public string NumeroDeControle { get; set; }
        public string Containeres { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroProposta { get; set; }
        public string TipoProposta { get; set; }
        public string CargaIMO { get; set; }
        public int QuantidadeNF { get; set; }
        public string DataFaturaDocumento { get; set; }
        public string NomeProprietarioVeiculo { get; set; }
        public string ContratoFrete { get; set; }
        public string NumeroContratoFrete { get; set; }
        public string DescricaoContratoFrete { get; set; }
        public DateTime DataInicialContratoFrete { get; set; }
        public DateTime DataFinalContratoFrete { get; set; }
        public string TipoContratoFrete { get; set; }
        public string SerieCte { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string PortoTransbordo { get; set; }
        public string NumeroCargaAgrupada { get; set; }
        public int QtdVolumesCarga { get; set; }
        public int VolumesCTe { get; set; }
        public string PrevisaoEntregaPrimeiroPedido { get; set; }
        public string PrevisaoEntregaUltimoPedido { get; set; }
        public int HorasEmTransito { get; set; }
        public string DataRealizadaUltimaEntrega { get; set; }
        public int TotalEntregasRealizadas { get; set; }
        public int TotalEntregasDevolvidas { get; set; }
        public int TotalEntregasPendentes { get; set; }
        public decimal DistanciaRota { get; set; }
        public int TotalEntregasBaixaManual { get; set; }
        public string NumeroFatura { get; set; }
        public string NumeroBoleto { get; set; }
        public string DataBoleto { get; set; }
        public string SituacaoFatura { get; set; }
        public string SituacaoBoleto { get; set; }
        public string Lacres { get; set; }
        public string Tara { get; set; }
        public string TipoContainers { get; set; }
        public string TipoSeparacao { get; set; }
        private DateTime DataInicioViagem { get; set; }
        private decimal AliquotaPIS { get; set; }
        private decimal AliquotaCOFINS { get; set; }
        public string ObservacaoCarga { get; set; }
        public string OperadorPedido { get; set; }
        public string TipoServico { get; set; }
        private DateTime DataInicioGeracaoCTes { get; set; }
        private DateTime DataFinalizacaoProcessamentoDocumentosFiscais { get; set; }
        private DateTime DataFimViagem { get; set; }
        public string DataPedido { get; set; }
        public DateTime DataRoteirizacaoCarga { get; set; }
        public string DataProgramacaoCarga { get; set; }
        public string DataExpedicao { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public string PrevisaoEntregaTransportador { get; set; }
        public decimal ValorCustoFrete { get; set; }
        public decimal QuantidadeTotalProduto { get; set; }
        public int NumeroNfProdutor { get; set; }
        public decimal PorcentagemPerda { get; set; }
        public int PesagemQuantidadeCaixas { get; set; }
        public decimal PesagemInicial { get; set; }
        public decimal PesagemFinal { get; set; }
        public string NumeroLacrePesagem { get; set; }
        public string LoteInternoPesagem { get; set; }
        private DateTime DataConfirmacaoDocumento { get; set; }
        public string ObservacaoCTe { get; set; }
        public string Expedidor { get; set; }
        public string Recebedor { get; set; }
        public string LocRecebedor { get; set; }
        public string DataUltimaSaidaRaio { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroEXP { get; set; }
        public decimal ValorReceberCTe { get; set; }
        public string DataEnvioUltimaNfe { get; set; }
        public string Frotas { get; set; }
        public string UsuarioAceiteTransportador { get; set; }
        private DateTime DataDocaInformada { get; set; }
        private DateTime DataChegadaVeiculo { get; set; }
        private DateTime DataFaturamento { get; set; }
        private DateTime DataAceiteTransportador { get; set; }
        private TipoNaoComparecimento NaoComparecimento { get; set; }
        public string TipoPropriedade { get; set; }
        public string ModeloCarroceria { get; set; }
        public string NumeroContainerVeiculo { get; set; }
        public decimal PercentualBonificsacaoTransportador { get; set; }
        public decimal ValorFreteTabelaFrete { get; set; }
        public string LocalRetiradaContainer { get; set; }
        private bool CargaDePreCarga { get; set; }
        private bool FreteDeTerceiro { get; set; }
        private int CodigoRedespacho { get; set; }
        private TipoContratacaoCarga TipoContratacaoCarga { get; set; }
        private bool HorarioEncaixado { get; set; }
        public decimal KmRota { get; set; }
        public string ClienteFinal { get; set; }
        public int TaraContainer { get; set; }
        public int MaxGross { get; set; }
        public int Protocolo { get; set; }
        public string NumeroDocumentoOriginario { get; set; }
        public string DataNumeroDocumentoOriginario { get; set; }
        public string NaturezaOP { get; set; }
        public decimal KMExecutado { get; set; }
        public int QuantidadeAnexos { get; set; }
        public string TipoAnexo { get; set; }
        public double CNPJLocalRetiradaContainer { get; set; }
        public string TipoClienteLocalRetiradaContiner { get; set; }
        public decimal ValorNFSemPallet { get; set; }
        private int EncerramentoManual { get; set; }
        private decimal DistanciaPrevista { get; set; }
        private decimal DistanciaRealizada { get; set; }
        public string CanalEntrega { get; set; }
        public string CargaRelacionada { get; set; }
        public string JustificativaCargaRelacionada { get; set; }
        public decimal Cubagem { get; set; }
        public decimal PedagioPagoTerceiro { get; set; }
        public decimal OutrosDescontosTerceiro { get; set; }
        public decimal IRPFTerceiro { get; set; }
        public decimal INSSTerceiro { get; set; }
        public decimal SESTSENATTerceiro { get; set; }
        public decimal OutrasTaxasTerceiro { get; set; }
        public decimal ValorTotalTerceiro { get; set; }
        public int ContratoTerceiro { get; set; }
        public decimal PalletsCarregadosNestaCarga { get; set; }
        public string AgrupamentoCargas { get; set; }
        public string FilialVenda { get; set; }
        private bool RotaRecorrente { get; set; }
        public string ValorToneladaSimulado { get; set; }
        public string ValorFreteSimulacao { get; set; }
        public string SerieNfe { get; set; }
        public int ComponenteDuplicado { get; set; }
        public int NumeroPagamento { get; set; }
        public DateTime DataEnvioPagamento { get; set; }
        public SituacaoPagamento SituacaoPagamento { get; set; }
        public TipoParametroBaseTabelaFrete ParametroBaseCalculoFrete { get; set; }
        public string ResponsavelValePedagio { get; set; }
        public string AprovadorDocumento { get; set; }
        public DateTime DataAprovacaoDocumento { get; set; }
        public string SituacaoAprovacaoDocumento { get; set; }
        public string TipoOSConvertido { get; set; }
        public string TipoOS { get; set; }
        public string ProvedorOS { get; set; }
        public TipoDirecionamentoCustoExtra DirecionamentoCustoExtra { get; set; }
        public StatusCustoExtra StatusCustoExtra { get; set; }
        public decimal ValorFreteIntegracao { get; set; }
        public string UsuarioAlteracaoFrete { get; set; }
        public string JustificativaCustoExtra { get; set; }
        public string SetorResponsavel { get; set; }

        public string PercentualExecucao { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TAGPedagio TAGPedagio { get; set; }

        public string CargaBloqueada { get; set; }
        public decimal ValorTotalMercadoriaDosPedidos { get; set; }
        public string OutrosNumerosDaCarga { get; set; }
        public string CargaReentrega { get; set; }
        public string NumeroOrdemServico { get; set; }
        public int NumeroDtNatura { get; set; }
        public string MotivoSolicitacaoFrete { get; set; }
        public string ObservacaoSolicitacaoFrete { get; set; }
        public string NumeroCTes { get; set; }
        public DateTime DataVinculoTracao { get; set; }
        public DateTime DataVinculoReboque { get; set; }
        public DateTime DataVinculoMotorista { get; set; }
        public LocalVinculo LocalVinculo { get; set; }
        public string NumeroDoca { get; set; }
        public string Observacao { get; set; }
        public decimal PesoReentrega { get; set; }
        public decimal PesoTotal { get; set; }
        public string CMDID { get; set; }
        public string CodigoNavio { get; set; }
        public string ObservacaoTransportadorJanela { get; set; }
        public string ObservacaoTransportadorCarga { get; set; }
        public string NumeroTransferencia { get; set; }
        public string Alocacao { get; set; }
        #endregion







        #region Propriedades com Regras       

        public string ObservacaoTransportador
        {
            get
            {
                string janela = ObservacaoTransportadorJanela?.Trim();
                string carga = ObservacaoTransportadorCarga?.Trim();

                if (string.IsNullOrEmpty(janela) && string.IsNullOrEmpty(carga))
                    return string.Empty;

                if (string.IsNullOrEmpty(janela))
                    return carga;

                if (string.IsNullOrEmpty(carga))
                    return janela;

                return $"{janela} / {carga}";
            }
        }

        public string PossuiComponenteDuplicado
        {
            get
            {
                return ComponenteDuplicado > 0 ? "Sim" : "Não";
            }
        }

        public string DataEnvioPagamentoFormatada
        {
            get { return DataEnvioPagamento == DateTime.MinValue ? "" : DataEnvioPagamento.ToString("dd/MM/yyyy HH:mm"); }
        }
        public string DataAprovacaoDocumentoFormatada
        {
            get { return DataAprovacaoDocumento == DateTime.MinValue ? "" : DataAprovacaoDocumento.ToString("dd/MM/yyyy HH:mm"); }
        }

        public decimal ICMSGNRE
        {
            get { return ValorICMS; }
        }

        public string DescricaoTAGPedagio
        {
            get { return TAGPedagio.ObterDescricao(); }
        }

        public string SituacaoPagamentoDescricao
        {
            get { return SituacaoPagamento.ObterDescricao(); }
        }
        public string ParametroBaseCalculoFreteDescricao
        {
            get { return ParametroBaseCalculoFrete.ObterDescricao(); }
        }

        public string DataConfirmacaoDocumentoFormatada
        {
            get { return DataConfirmacaoDocumento == DateTime.MinValue ? "" : DataConfirmacaoDocumento.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataAceiteTransportadorFormatada
        {
            get { return DataAceiteTransportador == DateTime.MinValue ? "" : DataAceiteTransportador.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataInicialContratoFreteFormatada
        {
            get { return DataInicialContratoFrete == DateTime.MinValue ? "" : DataInicialContratoFrete.ToString("dd/MM/yyyy"); }
        }

        public string DataFinalContratoFreteFormatada
        {
            get { return DataFinalContratoFrete == DateTime.MinValue ? "" : DataFinalContratoFrete.ToString("dd/MM/yyyy"); }
        }

        public string CNPJFormatado
        {
            get { return this.CNPJFilial.ObterCnpjFormatado(); }
        }

        public string CNPJTransportadorFormatado
        {
            get { return this.CNPJTransportador.ObterCnpjFormatado(); }
        }

        public string DataEmbarqueFormatada
        {
            get { return DataEmbarque == DateTime.MinValue ? "" : DataEmbarque.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataDescarregamentoFormatada
        {
            get { return DataDescarregamento == DateTime.MinValue ? "" : DataDescarregamento.ToString("dd/MM/yyyy HH:mm"); }
        }

        public string DataInicioViagemFormatada
        {
            get { return DataInicioViagem != DateTime.MinValue ? DataInicioViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFimViagemFormatada
        {
            get { return DataFimViagem != DateTime.MinValue ? DataFimViagem.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataInicioGeracaoCTesFormatada
        {
            get { return DataInicioGeracaoCTes != DateTime.MinValue ? DataInicioGeracaoCTes.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFinalizacaoDocumentosFiscaisFormatada
        {
            get { return DataFinalizacaoProcessamentoDocumentosFiscais != DateTime.MinValue ? DataFinalizacaoProcessamentoDocumentosFiscais.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public decimal DescontoOperador
        {
            get
            {
                if (TipoFreteEscolhido == TipoFreteEscolhido.Operador)
                {
                    decimal diferenca = ValorTabelaFrete - ValorFrete;

                    return (diferenca > 0) ? diferenca : 0m;
                }
                else
                    return 0;
            }
        }

        public decimal ValorKmRota
        {
            get
            {
                if (KmRota != 0 && ValorFreteLiquido != 0)
                {
                    decimal res = (ValorFreteLiquido / KmRota);

                    return (res > 0) ? res : 0m;
                }
                else
                    return 0;
            }
        }

        public string DescricaoDataCarregamento
        {
            get { return (DataCarga != DateTime.MinValue) ? DataCarga.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string DataCarregamentoFormatada
        {
            get { return (DataCarregamento != DateTime.MinValue) ? DataCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }



        public string DescricaoSituacao
        {
            get { return this.Situacao.ObterDescricao(); }
        }

        public string DiaSemana
        {
            get { return DataEmbarque == DateTime.MinValue ? "" : DiaSemanaHelper.ObterDiaSemana(DataEmbarque).ObterDescricao(); }
        }

        public int KMTotal
        {
            get { return KMFinal - KMInicial; }
        }

        public decimal QuantidadeHorasTotais
        {
            get { return QuantidadeHorasNormais + QuantidadeHorasExcedentes; }
        }

        public string UFDestino
        {
            get { return ListaUFs(Destino); }
        }

        public string UFOrigem
        {
            get { return ListaUFs(Origem); }
        }

        public decimal ValorFreteSemImposto
        {
            get { return (ValorFrete - ValorISS - ValorICMS); }
        }

        public decimal ValorKm
        {
            get { return KmRodado > 0 ? ValorFrete / KmRodado : 0; }
        }

        public decimal ValorLiquidoKm
        {
            get { return KmRodado > 0 ? ValorFreteLiquido / KmRodado : 0; }
        }

        public decimal ValorPIS
        {
            get { return AliquotaPIS > 0 ? (ValorFreteLiquido + ValorICMS) * (AliquotaPIS / 100) : 0; }
        }

        public decimal ValorCOFINS
        {
            get { return AliquotaCOFINS > 0 ? (ValorFreteLiquido + ValorICMS) * (AliquotaCOFINS / 100) : 0; }
        }

        public decimal PesoLiquidoPesagem
        {
            get { return PesagemFinal - PesagemInicial; }
        }

        private decimal PesoLiquidoCaixas
        {
            get { return (PesagemInicial - PesagemFinal) / (decimal)40.80; }
        }

        public decimal PesoLiquidoPosPerdas
        {
            get { return (((decimal)1.0 - (PorcentagemPerda / 100)) * PesoLiquidoCaixas); }
        }

        public decimal ResultadoFinalProcessoCaixas
        {
            get { return (PesoLiquidoPosPerdas - PesagemQuantidadeCaixas); }
        }

        public string DataDocaInformadaFormatada
        {
            get { return DataDocaInformada != DateTime.MinValue ? DataDocaInformada.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataChegadaVeiculoFormatada
        {
            get { return DataChegadaVeiculo != DateTime.MinValue ? DataChegadaVeiculo.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataFaturamentoFormatada
        {
            get { return DataFaturamento != DateTime.MinValue ? DataFaturamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string NaoComparecimentoDescricao
        {
            get { return NaoComparecimento.ObterDescricao(); }
        }

        public string CargaPreCarga
        {
            get { return CargaDePreCarga ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public string Terceiro
        {
            get { return FreteDeTerceiro ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public string Redespacho
        {
            get { return this.TipoContratacaoCarga == TipoContratacaoCarga.Redespacho || this.TipoContratacaoCarga == TipoContratacaoCarga.RedespachoIntermediario || CodigoRedespacho > 0 ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public string HorarioEncaixadoDescricao
        {
            get { return HorarioEncaixado ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }
        private string DocumentoTransporte { get; set; }
        public string ExternalDT1
        {
            get
            {
                if (string.IsNullOrEmpty(this.DocumentoTransporte))
                    return string.Empty;

                Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(this.DocumentoTransporte);
                return documentoTransporte.ExternalID1;
            }
        }
        public string ExternalDT2
        {
            get
            {
                if (string.IsNullOrEmpty(this.DocumentoTransporte))
                    return string.Empty;

                Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte documentoTransporte = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.WebService.Rest.DocumentoTransporte>(this.DocumentoTransporte);
                return documentoTransporte.ExternalID2;
            }
        }

        public string JustificativaEncerramento { get; set; }
        public string ObservacaoEncerramento { get; set; }

        public string CNPJLocalRetiradaContainerFormatado
        {
            get
            {
                if (this.CNPJLocalRetiradaContainer == 0 || string.IsNullOrWhiteSpace(this.TipoClienteLocalRetiradaContiner)) return "";
                if (this.TipoClienteLocalRetiradaContiner.Equals("E")) return "00.000.000/0000-00";
                return this.TipoClienteLocalRetiradaContiner.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CNPJLocalRetiradaContainer) : String.Format(@"{0:000\.000\.000\-00}", this.CNPJLocalRetiradaContainer);
            }
        }
        public string PossuiEncerramentoManual
        {
            get { return EncerramentoManual > 0 ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public string KMPrevisto
        {
            get { return String.Format("{0:n1} Km", DistanciaPrevista); }
        }

        public string KMRealizado
        {
            get { return String.Format("{0:n1} Km", DistanciaRealizada); }
        }

        public string Aprovador { get; set; }
        public int QuantidadePacotes { get; set; }
        public int QuantidadePacotesColetados { get; set; }
        public int QuantidadeCTeAnterior { get; set; }
        public string DataAutorizacaoFormatada { get { return DataAutorizacao != null ? DataAutorizacao?.ToString("d") : string.Empty; } }
        public string SituacaoAprovacaoFormatada { get { return SituacaoAprovacao.ObterDescricao(); } }

        public string RotaRecorrenteDescricao
        {
            get { return RotaRecorrente ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public string CodigoIntegracaoRota { get; set; }
        public string DiferencaValores { get; set; }

        public string TipoOSConvertidoDescricao
        {
            get
            {
                if (string.IsNullOrEmpty(TipoOSConvertido))
                    return string.Empty;

                int[] valoresEnum = TipoOSConvertido.Split(',').Select(n => int.Parse(n)).ToArray();

                List<string> descricoes = (from numero in valoresEnum
                                           where Enum.IsDefined(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido), numero)
                                           let tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido)numero
                                           select tipo.ObterDescricao()).ToList();

                return string.Join(", ", descricoes);
            }
        }

        public string TipoOSDescricao
        {
            get
            {
                if (string.IsNullOrEmpty(TipoOS))
                    return string.Empty;

                int[] valoresEnum = TipoOS.Split(',').Select(n => int.Parse(n)).ToArray();

                List<string> descricoes = (from numero in valoresEnum
                                           where Enum.IsDefined(typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOS), numero)
                                           let tipo = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOS)numero
                                           select tipo.ObterDescricao()).ToList();

                return string.Join(", ", descricoes);
            }
        }
        public string DirecionamentoCustoExtraDescricao { get { return DirecionamentoCustoExtra.ObterDescricao(); } }
        public string StatusCustoExtraDescricao { get { return StatusCustoExtra.ObterDescricao(); } }
        public string SituacaoBRKFormatada { get { return SituacaoBRK.ObterDescricao(); } }

        public string DataInclusaoDadosTransporteFormatada
        {
            get { return DataInclusaoDadosTransporte != DateTime.MinValue ? DataInclusaoDadosTransporte.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DescricaoLocalVinculo
        {
            get { return this.LocalVinculo.ObterDescricao() ?? ""; }
        }

        public string DataVinculoTracaoFormatada
        {
            get { return DataVinculoTracao != DateTime.MinValue ? DataVinculoTracao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataVinculoReboqueFormatada
        {
            get { return DataVinculoReboque != DateTime.MinValue ? DataVinculoReboque.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataVinculoMotoristaFormatada
        {
            get { return DataVinculoMotorista != DateTime.MinValue ? DataVinculoMotorista.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public decimal DiferencaFreteValorOperadorValorTabelaFrete
        {
            get
            {
                return ValorFreteInformadoPeloOperador > 0 && ValorTabelaFrete > 0 ? ValorFreteInformadoPeloOperador - ValorTabelaFrete : 0;
            }
        }

        #endregion

        #region Métodos Privados

        private string ListaUFs(string cidadesEstados)
        {
            if (string.IsNullOrWhiteSpace(cidadesEstados))
                return string.Empty;

            Regex expressaoRegular = new Regex(@"-\s*([A-Z]){2}((\s*\/)|(\s*-)|(\s*$))");
            HashSet<string> listaEstados = new HashSet<string>();

            foreach (Match estadoEncontrado in expressaoRegular.Matches(cidadesEstados))
                listaEstados.Add(estadoEncontrado.ToString().Replace("-", "").Replace("/", "").Trim());

            return string.Join(", ", listaEstados);
        }

        private SituacaoAlcadaRegra SituacaoAprovacao { get; set; }

        private DateTime? DataAutorizacao { get; set; }
        #endregion

    }
}
