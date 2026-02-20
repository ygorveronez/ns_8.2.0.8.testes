using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dominio.Relatorios.Embarcador.DataSource.CTe.CTes
{
    public class CTe
    {
        #region Propriedades

        public int Codigo { get; set; }
        public int NumeroCTe { get; set; }
        public int CFOP { get; set; }
        public int SerieCTe { get; set; }
        public int RPS { get; set; }
        public string StatusCTe { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string ContaContabil { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroCargaAgrupamento { get; set; }
        public string PreCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroPedidoInterno { get; set; }
        public long NumeroMinuta { get; set; }
        public string NumeroNotaFiscal { get; set; }

        private string _chaveNotaFiscal;
        public string ChaveNotaFiscal
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._chaveNotaFiscal) || this._chaveNotaFiscal?.Trim().Length < 44)
                    return this._chaveNotaFiscal;

                return FormatacaoContext.ExportarCamposFormatado ? "'" + this._chaveNotaFiscal.Trim() : this._chaveNotaFiscal.Trim();
            }

            set { _chaveNotaFiscal = value; }
        }

        private string _cpfMotorista;
        public string CpfMotorista
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfMotorista.ObterCpfOuCnpjFormatado() : _cpfMotorista;
            set => _cpfMotorista = value;

        }
        public string DataCriacaoCarga { get; set; }
        public StatusTitulo StatusTitulo { get; set; }
        public string NumeroFatura { get; set; }
        public string NumeroPreFatura { get; set; }
        public string DataNFEmissao { get; set; }
        public string NumeroDocumentoAnterior { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataAutorizacao { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime DataOperacaoNavio { get; set; }
        public string DataImportacao { get; set; }
        public string DataVinculoCarga { get; set; }
        public string DataCancelamento { get; set; }
        public string DataAnulacao { get; set; }
        public string DataEntrega { get; set; }
        public string AbreviacaoModeloDocumentoFiscal { get; set; }
        public string DataFatura { get; set; }
        public string DataVencimento { get; set; }

        public string CodigoRemetente { get; set; }

        private string _cpfCnpjRemetente;
        public string CPFCNPJRemetente
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjRemetente.ObterCpfOuCnpjFormatado() : _cpfCnpjRemetente;
            set => _cpfCnpjRemetente = value;
        }
        public string IERemetente { get; set; }
        public string Remetente { get; set; }
        public string LocalidadeRemetente { get; set; }
        public string UFRemetente { get; set; }
        public string CodigoEnderecoRemetente { get; set; }
        public string EnderecoRemetente { get; set; }
        public string GrupoRemetente { get; set; }
        public string CategoriaRemetente { get; set; }
        public string CodigoDocumentoRemetente { get; set; }

        public string CodigoExpedidor { get; set; }

        private string _cpfCnpjExpedidor;
        public string CPFCNPJExpedidor
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjExpedidor.ObterCpfOuCnpjFormatado() : _cpfCnpjExpedidor;
            set => _cpfCnpjExpedidor = value;
        }
        public string IEExpedidor { get; set; }
        public string Expedidor { get; set; }
        public string LocalidadeExpedidor { get; set; }
        public string UFExpedidor { get; set; }
        public string CodigoDocumentoExpedidor { get; set; }

        public string CodigoRecebedor { get; set; }

        private string _cpfCnpjRecebedor;
        public string CPFCNPJRecebedor
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjRecebedor.ObterCpfOuCnpjFormatado() : _cpfCnpjRecebedor;
            set => _cpfCnpjRecebedor = value;
        }
        public string IERecebedor { get; set; }
        public string Recebedor { get; set; }
        public string LocalidadeRecebedor { get; set; }
        public string UFRecebedor { get; set; }
        public string CodigoDocumentoRecebedor { get; set; }

        public string CodigoDestinatario { get; set; }

        private string _cpfCnpjDestinatario;
        public string CPFCNPJDestinatario
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjDestinatario.ObterCpfOuCnpjFormatado() : _cpfCnpjDestinatario;
            set => _cpfCnpjDestinatario = value;
        }
        public string IEDestinatario { get; set; }
        public string Destinatario { get; set; }
        public string LocalidadeDestinatario { get; set; }
        public string UFDestinatario { get; set; }
        public string GrupoDestinatario { get; set; }
        public string CategoriaDestinatario { get; set; }
        public string CodigoDocumentoDestinatario { get; set; }

        public string CodigoIntegracaoTomador { get; set; }

        private string _cpfCnpjTomador;
        public string CPFCNPJTomador
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjTomador.ObterCpfOuCnpjFormatado() : _cpfCnpjTomador;
            set => _cpfCnpjTomador = value;
        }
        public string IETomador { get; set; }
        public string Tomador { get; set; }
        public string UFTomador { get; set; }
        public string CodigoDocumentoTomador { get; set; }

        public string TipoDeCarga { get; set; }
        public int IBGEInicioPrestacao { get; set; }
        public string InicioPrestacao { get; set; }
        public string UFInicioPrestacao { get; set; }
        public int IBGEFimPrestacao { get; set; }
        public string FimPrestacao { get; set; }
        public string UFFimPrestacao { get; set; }

        private string _cpfCnpjTransportador;
        private string CNPJTransportador
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjTransportador.ObterCpfOuCnpjFormatado() : _cpfCnpjTransportador;
            set => _cpfCnpjTransportador = value;
        }
        public string RazaoSocialTransportador { get; set; }
        public string NomeFantasiaTransportador { get; set; }
        public string UFTransportador { get; set; }

        public decimal AliquotaICMS { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public decimal ValorISSRetido { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSemImposto { get; set; }
        public decimal ValorReceber { get; set; }
        public decimal ValorMercadoria { get; set; }
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
        public string Veiculo { get; set; }
        public string Observacao { get; set; }
        public string GrupoTomador { get; set; }
        public string Rotas { get; set; }
        public string Ocorrencia { get; set; }
        public string TipoOcorrencia { get; set; }
        public string ContratoFrete { get; set; }
        public string TipoOperacao { get; set; }

        #region Colunas Dinâmicas
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
        public string LayoutArquivo1 { get; set; }
        public string LayoutArquivo2 { get; set; }
        public string LayoutArquivo3 { get; set; }
        public string LayoutArquivo4 { get; set; }
        public string LayoutArquivo5 { get; set; }
        public string LayoutArquivo6 { get; set; }
        #endregion

        public string NumeroOCADocumentoOriginario { get; set; }
        public string ProtocoloAutorizacao { get; set; }
        public string ProtocoloInutilizacaoCancelamento { get; set; }
        public string Motorista { get; set; }
        public string ProcImportacao { get; set; }
        public decimal PesoKg { get; set; }
        public decimal PesoLiquidoKg { get; set; }
        public int Volumes { get; set; }
        public decimal MetrosCubicos { get; set; }
        public decimal Pallets { get; set; }
        public string Pago { get; set; }
        public string RetornoSefaz { get; set; }
        public string NomeProprietarioVeiculo { get; set; }
        public string Frota { get; set; }
        public string DataOcorrenciaFinal { get; set; }
        public string VeiculoUltimoMDFe { get; set; }
        public string NumeroUltimoMDFe { get; set; }
        public string DescricaoUltimaOcorrencia { get; set; }
        private string CST { get; set; }
        public string CodigoEnderecoDestinatario { get; set; }
        public string EnderecoDestinatario { get; set; }
        public string BairroDestinatario { get; set; }
        public string CEPDestinatario { get; set; }
        public decimal ValorPrestacao { get; set; }
        public string Operador { get; set; }
        public string DataColeta { get; set; }
        public string ModeloVeicular { get; set; }
        public string ModeloVeiculoCarga { get; set; }
        public string Log { get; set; }
        public string DataPrevistaEntrega { get; set; }
        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }
        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }
        public string TipoServicoMultimodal { get; set; }
        public bool Afretamento { get; set; }
        public string NumeroProtocoloANTAQ { get; set; }
        public string NumeroManifesto { get; set; }
        public string NumeroManifestoFeeder { get; set; }
        public string NumeroCEMercante { get; set; }
        public string NumeroCEANTAQ { get; set; }
        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }
        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }
        public string NumeroDTNatura { get; set; }
        public decimal KmRodado { get; set; }
        public decimal ValorKMContrato { get; set; }
        public decimal ValorKMExcedenteContrato { get; set; }
        public decimal ValorFreteFranquiaKM { get; set; }
        public decimal ValorFreteFranquiaKMExcedido { get; set; }
        public decimal KmConsumido { get; set; }
        public decimal KmConsumidoExcedente { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal AliquotaPIS { get; set; }

        private string _cnpjFilial;
        public string CNPJFilial
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cnpjFilial.ObterCpfOuCnpjFormatado() : _cnpjFilial;
            set => _cnpjFilial = value;
        }
        public string Filial { get; set; }
        public string FilialVenda { get; set; }
        public string NumeroDI { get; set; }
        public string NumeroDTA { get; set; }
        public string NumeroPedidoNotaFiscal { get; set; }
        public string CodigoReferencia { get; set; }
        public string CodigoImportacao { get; set; }
        public string SegmentoVeiculo { get; set; }
        public string NumeroValePedagio { get; set; }
        public decimal ValorValePedagio { get; set; }
        public string TabelaFrete { get; set; }
        public string TabelaFreteCliente { get; set; }
        public string SituacaoCarga { get; set; }
        public string NumeroFechamentoFrete { get; set; }
        public string NumeroEscrituracao { get; set; }
        public string NumeroPagamento { get; set; }
        public string NumeroContabilizacao { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public string TipoProposta { get; set; }
        public string NumeroProposta { get; set; }
        public string TipoCarga { get; set; }
        public int QuantidadeNF { get; set; }
        public string Viagem { get; set; }
        public string NumeroLacre { get; set; }
        public string Tara { get; set; }
        public string Container { get; set; }
        public string TipoContainer { get; set; }
        public string NumeroBoleto { get; set; }
        public string DataBoleto { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string PortoTransbordo { get; set; }
        public string NavioTransbordo { get; set; }
        public string PossuiCartaCorrecao { get; set; }
        public string FoiAnulado { get; set; }
        public string PossuiCTeComplementar { get; set; }
        public string FoiSubstituido { get; set; }
        public string ETS { get; set; }
        public string ETA { get; set; }
        public string MotivoCancelamento { get; set; }
        public string NumeroLoteCancelamento { get; set; }
        public decimal AliquotaICMSInterna { get; set; }
        public decimal PercentualICMSPartilha { get; set; }
        public decimal ValorICMSUFOrigem { get; set; }
        public decimal ValorICMSUFDestino { get; set; }
        public decimal ValorICMSFCPFim { get; set; }
        public string CaracteristicaTransporteCTe { get; set; }
        public string ProdutoPredominante { get; set; }
        public string CentroResultado { get; set; }
        public string RotaFrete { get; set; }
        public decimal ValorSemTributo { get; set; }
        public string NumeroCTeSubstituto { get; set; }
        public string NumeroCTeAnulacao { get; set; }
        public string NumeroCTeComplementar { get; set; }
        public string NumeroCTeDuplicado { get; set; }
        public string NumeroCTeOriginal { get; set; }
        public string NumeroControleCTeSubstituto { get; set; }
        public string NumeroControleCTeAnulacao { get; set; }
        public string NumeroControleCTeComplementar { get; set; }
        public string NumeroControleCTeDuplicado { get; set; }
        public string NumeroControleCTeOriginal { get; set; }
        public string NumeroCIOT { get; set; }
        public string NumeroDocumentoOriginario { get; set; }
        public string DataInicioViagem { get; set; }
        public string DataFimViagem { get; set; }
        public int QuantidadeContainer { get; set; }
        public decimal Taxa { get; set; }
        public string NumeroCTeTerceiroOcorrencia { get; set; }
        public string NumeroDocumentoRecebedor { get; set; }
        public string NumeroPedidoCliente { get; set; }
        public decimal QuantidadeTotalProduto { get; set; }
        public string ModeloVeiculo { get; set; }
        public string TipoCarroceria { get; set; }
        public decimal DistanciaCargaAgrupada { get; set; }
        public string OperadorResponsavelCancelamento { get; set; }
        public string VeiculoTracao { get; set; }
        public string ChassiTracao { get; set; }
        public string VeiculoReboque { get; set; }
        public decimal KMRota { get; set; }
        public string DataConfirmacaoDocumento { get; set; }
        public string RuaRecebedor { get; set; }
        public string NumeroRecebedor { get; set; }
        public string BairroRecebedor { get; set; }
        public string LacresCargaLacre { get; set; }
        public decimal PalletsPedido { get; set; }
        public string RegraICMS { get; set; }
        public string DescricaoRegraICMS { get; set; }

        private string _cpfCnpjTerceiro;
        public string CPFCNPJTerceiro
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cpfCnpjTerceiro.ObterCpfOuCnpjFormatado() : _cpfCnpjTerceiro;
            set => _cpfCnpjTerceiro = value;
        }
        public string NomeTerceiro { get; set; }
        public string NumeroContainer { get; set; }
        public string NumeroEXP { get; set; }
        public string JustificativaNaoEnviarParaMercante { get; set; }
        private string SituacaoCanhoto { get; set; }
        private SituacaoPagamento SituacaoPagamento { get; set; }
        private DateTime DataOcorrencia { get; set; }
        private DateTime DataAprovacaoPagamento { get; set; }
        public string Anexos { get; set; }
        public string DescricaoAnexos { get; set; }
        public string DataVigenciaTabelaFrete { get; set; }
        public string CodigoTabelaFreteCliente { get; set; }
        public DateTime DataVencimentoTitulo { get; set; }
        public string TipoProprietario { get; set; }
        public string JustificativaMotivoMercante { get; set; }
        private TipoModal TipoModal { get; set; }
        public string NumeroContratoFreteTerceiro { get; set; }
        private SituacaoFatura SituacaoFatura { get; set; }
        public string NumeroDocumentoOriginal { get; set; }
        public string ChaveCTeOriginal { get; set; }
        public decimal ValorReceberCTeOriginal { get; set; }
        public string Terceiro { get; set; }
        public string Redespacho { get; set; }
        private DateTime CargaDataCarregamento { get; set; }
        public string FuncionarioResponsavel { get; set; }
        private bool ExcecaoCab { get; set; }
        public string ChaveCTEAnterior { get; set; }
        public string NumeroCarregamento { get; set; }
        public int NumeroCTEAnterior { get; set; }
        public string CodigoDocumentacaoNavio { get; set; }
        public string CodigoEscrituracao { get; set; }
        public string NumeroFolha { get; set; }
        public string DataFolha { get; set; }
        public string DataFolhaFormatada
        {
            get { return this.DataFolha; }
        }
        public string FolhaCalculada { get; set; }
        public string FolhaAtribuida { get; set; }
        public string FolhaTransferida { get; set; }
        public string FolhaCancelada { get; set; }
        public string FolhaInconsistente { get; set; }
        public string FolhaCanceladaFormatada { get { return this.FolhaCancelada; } }
        public string FolhaInconsistenteFormatada { get { return this.FolhaInconsistente; } }
        public string InconsistenciaFolha { get; set; }
        private bool Substituicao { get; set; }
        public string Vendedor { get; set; }
        public decimal ValorFreteTerceiro { get; set; }
        public string NumeroCompleto { get; set; }
        public string Etapa { get; set; }
        public string Ordem { get; set; }
        public string NumeroMiro { get; set; }
        public string NumeroEstorno { get; set; }
        public string Bloqueio { get; set; }
        public string DataMiro { get; set; }
        public string Vencimento { get; set; }
        public string TermoPagamento { get; set; }
        public decimal BCCTeSubstituido { get; set; }
        public TipoFreteEscolhido TipoFreteEscolhido { get; set; }
        public decimal PesoPedido { get; set; }
        public string BookingReferente { get; set; }
        public string NumeroValePedagioManual { get; set; }
        public decimal ValorValePedagioManual { get; set; }
        public string SerieNota { get; set; }
        public string DocFaturaSap { get; set; }
        public string CodigoCentroDeCustoEmissor { get; set; }
        public string TipoOSConvertido { get; set; }
        public string TipoOS { get; set; }
        public string ProvedorOS { get; set; }
        public int CentroDeCustoViagemCodigo { get; set; }
        public string CentroDeCustoViagemDescricao { get; set; }

        private string _cnpjMdfe;
        private string CnpjMdfe
        {
            get => FormatacaoContext.ExportarCamposFormatado ? _cnpjMdfe.ObterCpfOuCnpjFormatado() : _cnpjFilial;
            set => _cnpjFilial = value;
        }
        public TipoEmissao TipoEmissao { get; set; }
        public string NumeroCRT { get; set; }
        public string MicDTA { get; set; }
        public bool ExportarCamposFormatado { get; set; }

        private bool FormatarCampos { get; set; }

        #endregion

        #region Propriedades com Regras

        public decimal ICMSGNRE
        {
            get { return ValorICMS; }
        }


        public string CNPJTransportadorFormatada
        {
            get { return CNPJTransportador != null ? CNPJTransportador.ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        public string DataPagamentoFormatada
        {
            get { return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy HH:mm") : ""; }
        }

        public string DescricaoStatusTitulo
        {
            get { return StatusTitulo.ObterDescricao(); }
        }

        public string DescricaoAfretamento
        {
            get { return Afretamento ? "Sim" : "Não"; }
        }

        public string DescricaoTipoCTe
        {
            get { return TipoCTe.ObterDescricao(); }
        }

        public string DescricaoTipoPagamento
        {
            get { return TipoPagamento.ObterDescricao(); }
        }

        public string DescricaoTipoServico
        {
            get { return TipoServico.ObterDescricao(); }
        }

        public string DescricaoTipoTomador
        {
            get
            {
                switch (TipoTomador)
                {
                    case Dominio.Enumeradores.TipoTomador.Remetente:
                        return "Remetente";
                    case Dominio.Enumeradores.TipoTomador.Expedidor:
                        return "Expedidor";
                    case Dominio.Enumeradores.TipoTomador.Recebedor:
                        return "Recebedor";
                    case Dominio.Enumeradores.TipoTomador.Destinatario:
                        return "Destinatário";
                    case Dominio.Enumeradores.TipoTomador.Outros:
                        return "Outro";
                    default:
                        return "";
                }
            }
        }

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

        public decimal ValorCOFINS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaCOFINS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public decimal ValorPIS
        {
            get { return Math.Round(BaseCalculoICMS * (AliquotaPIS / 100), 2, MidpointRounding.AwayFromZero); }
        }

        public int AnoEmissao
        {
            get { return DataEmissao.Year; }
        }

        public int MesEmissao
        {
            get { return DataEmissao.Month; }
        }

        public string SituacaoCargaFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SituacaoCarga))
                    return SituacaoCarga;

                string[] listaSituacoes = SituacaoCarga.Split(',');

                return string.Join(", ", (from situacao in listaSituacoes select SituacaoCargaHelper.ObterDescricao((SituacaoCarga)situacao.ToInt())));
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAutorizacaoFormatada
        {
            get { return DataAutorizacao != DateTime.MinValue ? DataAutorizacao.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CSTFormatada
        {
            get { return TipoICMSHelper.ObterTipoDescricao(CST); }
        }

        public string SituacaoCanhotoFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SituacaoCanhoto))
                    return SituacaoCanhoto;

                string[] listaSituacoes = SituacaoCanhoto.Split(',');

                return string.Join(", ", (from situacao in listaSituacoes select SituacaoCanhotoHelper.ObterDescricao((SituacaoCanhoto)situacao.ToInt())));
            }
        }

        public string SituacaoPagamentoFormatada
        {
            get { return SituacaoPagamento.ObterDescricao(); }
        }

        public string DataOcorrenciaFormatada
        {
            get { return DataOcorrencia != DateTime.MinValue ? DataOcorrencia.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAprovacaoPagamentoFormatada
        {
            get { return DataAprovacaoPagamento != DateTime.MinValue ? DataAprovacaoPagamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataOperacaoNavioFormatada
        {
            get { return DataOperacaoNavio != DateTime.MinValue ? DataOperacaoNavio.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataVencimentoTituloFormatada
        {
            get { return DataVencimentoTitulo != DateTime.MinValue ? DataVencimentoTitulo.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public virtual string DescricaoTipoModal
        {
            get { return TipoModal.ObterDescricao(); }
        }

        public string SituacaoFaturaDescricao
        {
            get { return SituacaoFatura.ObterDescricao(); }
        }

        public string CargaDataCarregamentoFormatada
        {
            get { return CargaDataCarregamento != DateTime.MinValue ? CargaDataCarregamento.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DescricaoExcecaoCab
        {
            get { return ExcecaoCab ? "Sim" : "Não"; }
        }

        public string ExisteEscrituracao
        {
            get { return !string.IsNullOrWhiteSpace(this.CodigoEscrituracao) ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao; }
        }

        public string SubstituicaoDescricao
        {
            get { return Substituicao ? "Sim" : "Não"; }
        }

        public string FreteInformadoManualmente
        {
            get
            {
                if (TipoFreteEscolhido.Operador == TipoFreteEscolhido)
                    return "Sim";
                return "Não";
            }
        }

        public string DocFaturaSapFormatada
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocFaturaSap))
                    return DocFaturaSap;

                string faturaFormatada = DocFaturaSap.ToUpper().Replace('D', ' ').Replace('O', ' ').Replace('F', ' ').Replace('A', ' ').Replace('T', ' ');

                return faturaFormatada ?? " ";
            }
        }

        public string CnpjMdfeFormatada
        {
            get { return !string.IsNullOrEmpty(CnpjMdfe) ? CnpjMdfe.ObterCpfOuCnpjFormatado() : string.Empty; }
        }

        private string _chaveCTe;

        public string ChaveCTe
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this._chaveCTe) || this._chaveCTe?.Trim().Length < 44)
                    return this._chaveCTe;

                return FormatacaoContext.ExportarCamposFormatado ? "'" + this._chaveCTe.Trim() : this._chaveCTe.Trim();
            }

            set { _chaveCTe = value; }
        }
    }
    #endregion

    #region Formatacao
    public static class FormatacaoContext
    {
        private static readonly AsyncLocal<bool> _exportarCamposFormatadoStore = new();

        public static bool ExportarCamposFormatado
        {
            get => _exportarCamposFormatadoStore.Value;
            set => _exportarCamposFormatadoStore.Value = value;
        }
    }

    #endregion
}