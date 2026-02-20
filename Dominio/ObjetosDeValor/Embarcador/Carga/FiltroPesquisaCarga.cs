using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public sealed class FiltroPesquisaCarga
    {
        #region Construtores 

        public FiltroPesquisaCarga()
        {
            PossuiValePedagio = OpcaoSimNao.Todos;
        }

        #endregion Construtores 

        #region Atributos Privados

        private bool _codigoFilialInformado;
        private bool _codigoTipoCargaInformado;
        private bool _codigoTipoOperacaoInformado;
        private bool _codigoTransportadorInformado;
        private bool _codigoModeloVeicularCargaInformado;
        private bool _cpfCnpjRemetenteInformado;
        private bool _cpfCnpjDestinatarioInformado;
        private bool _cpfCnpjExpedidorInformado;
        private bool _cpfCnpjRecebedorInformado;
        private bool _codigoVeiculoInformado;
        private bool _codigoRotaInformado;

        #endregion Atributos Privados

        #region Propriedades

        public int CodigoCarga { get; set; }

        public int CodigoCargaAtual { get; set; }

        public bool ApenasEmpresaPermiteEncaixe { get; set; }

        public int Serie { get; set; }

        public bool BuscarCargasRedespacho { get; set; }

        public bool RetornarCargaDocumentoEmitido { get; set; }

        public bool VisualizarValorNFSeDescontandoISSRetido { get; set; }

        public double CnpjClienteUsuario { get; set; }

        public int Codigo { get; set; }

        public int CodigoFuncionarioVendedor { get; set; }

        public int CodigoCanalEntrega { get; set; }

        public int CodigoCanalVenda { get; set; }

        public int CodigoCarregamento { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoCIOT { get; set; }

        public List<int> CodigosCentroCarregamento { get; set; }

        public int CodigoDestino { get; set; }
        public List<int> CodigosDestino { get; set; }

        public int CodigoEmpresa { get; set; }

        public List<int> CodigoGrupoPessoas { get; set; }

        public List<int> CodigosModeloVeicularCarga { get; set; }

        public int CodigoMotorista { get; set; }
        public List<int> CodigosMotorista { get; set; }
        public List<int> CodigosRota { get; set; }
        public List<int> CodigosCentroResultado { get; set; }

        public int CodigoOrigem { get; set; }
        public List<int> CodigosOrigem { get; set; }

        public int CodigoOperador { get; set; }

        public int CodigoPaisDestino { get; set; }

        public string codigoPedidoEmbarcador { get; set; }

        public string CodigoPedidoCliente { get; set; }

        public int CodigoPedidoViagemNavio { get; set; }

        public List<int> CodigosFilial { get; set; }
        public List<int> CodigosFilialVenda { get; set; }

        public List<int> CodigosTabelasFrete { get; set; }

        public List<int> CodigosTipoCarga { get; set; }

        public List<int> CodigosTipoOperacao { get; set; }

        public List<int> CodigosTransportador { get; set; }

        public List<int> CodigosVeiculos { get; set; }

        public List<int> CodigosGrupoProduto { get; set; }
        public List<TipoPropostaMultimodal> TiposPropostasMultimodal { get; set; }

        public List<double> CpfCnpjExpedidores { get; set; }

        public List<double> CpfCnpjRecebedores { get; set; }

        public List<double> CpfCnpjRecebedoresOuSemRecebedores { get; set; }

        public InformacoesRelatorioCarga InformacoesRelatorioCargas { get; set; }

        public ProblemasCarga Problemas { get; set; }

        public List<double> CpfCnpjDestinatarios { get; set; }

        public List<double> CpfCnpjRemetentes { get; set; }

        public List<double> CpfCnpjRemetentesOuDestinatarios { get; set; }

        public DateTime? DataInicialFimEmissaoDocumentos { get; set; }

        public DateTime? DataInicialInicioEmissaoDocumentos { get; set; }

        public DateTime? DataFinalFimEmissaoDocumentos { get; set; }

        public DateTime? DataFinalInicioEmissaoDocumentos { get; set; }

        public DateTime? DataAnulacaoFinal { get; set; }

        public DateTime? DataAnulacaoInicial { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataFinal { get; set; }

        public DateTime? DataCarregamentoFinal { get; set; }

        public DateTime? DataCarregamentoInicial { get; set; }

        public bool ExibirCargasAgrupadas { get; set; }

        public bool FiltrarCargasPorParteDoNumero { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroControle { get; set; }

        public int NumeroCTe { get; set; }

        public int NumeroCTeSubcontratacao { get; set; }

        public int NumeroMDFe { get; set; }

        public int NumeroNF { get; set; }

        public string NumeroOS { get; set; }

        public List<string> ListaNumeroOSMae { get; set; }
        public List<string> ListaNumeroOS { get; set; }

        public string NumeroOe { get; set; }

        public int NumeroPedido { get; set; }

        public string NumeroPedidoTrocado { get; set; }

        public string NumeroTransporte { get; set; }

        public Entidades.Embarcador.Operacional.OperadorLogistica OperadorLogistica { get; set; }

        public string Ordem { get; set; }

        public string Pedido { get; set; }

        public int PedidoCentroCusto { get; set; }

        public int Container { get; set; }

        public int PedidoEmpresaResponsavel { get; set; }

        public string PlacaAgrupamento { get; set; }

        public string PortoSaida { get; set; }

        public string PreCarga { get; set; }

        public string Reserva { get; set; }

        public string SiglaEstadoDestino { get; set; }

        public string SiglaEstadoOrigem { get; set; }

        public Dominio.Enumeradores.OpcaoSimNao CargasAguardandoImportacaoCTe { get; set; }

        public List<SituacaoCarga> Situacoes { get; set; }

        public SituacaoCarga SituacaoCarga { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }

        public bool SomenteAgrupadas { get; set; }

        public bool SomenteCargasReentrega { get; set; }

        public bool SomenteComReserva { get; set; }

        public bool SomenteDescontoOperador { get; set; }

        public bool SomenteSituacao { get; set; }

        public bool SomentePermiteAgrupamento { get; set; }

        public bool SomentePermiteMDFeManual { get; set; }

        public bool? SomenteTerceiros { get; set; }

        public bool CargaRelacionadas { get; set; }

        public TipoContratacaoCarga? TipoContratacaoCarga { get; set; }

        public string TipoEmbarque { get; set; }

        public TipoLocalPrestacao TipoLocalPrestacao { get; set; }

        public TipoOperacaoCargaCTeManual TipoOperacaoCargaCTeManual { get; set; }

        public List<TipoCTE> TipoCTe { get; set; }

        public List<TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }

        public List<TipoServicoMultimodal> TipoServicoMultimodal { get; set; }

        public List<TipoCobrancaMultimodal> TipoCobrancaMultimodal { get; set; }

        public OpcaoSimNaoPesquisa VeioPorImportacao { get; set; }

        public bool SomenteCTeSubstituido { get; set; }

        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServicoMultisoftware { get; set; }

        public bool? Transbordo { get; set; }

        public DateTime? DataInicialEmissao { get; set; }

        public DateTime? DataFinalEmissao { get; set; }

        public int PortoOrigem { get; set; }

        public int PortoDestino { get; set; }

        public bool? CargaPerigosa { get; set; }

        public bool? CargaTrocaDeNota { get; set; }

        public DateTime? DataInicioAverbacao { get; set; }

        public DateTime? DataFimAverbacao { get; set; }

        public string RaizCNPJ { get; set; }

        public string DeliveryTerm { get; set; }

        public string IdAutorizacao { get; set; }

        public DateTime? DataInclusaoPCPInicial { get; set; }

        public DateTime? DataInclusaoPCPLimite { get; set; }

        public DateTime? DataInclusaoBookingInicial { get; set; }

        public DateTime? DataInclusaoBookingLimite { get; set; }

        public string SituacaoCTe { get; set; }

        public List<int> CodigosModeloDocumentoFiscal { get; set; }

        public bool ConsultaParaGeracaoDocumentos { get; set; }

        public int CodigoTipoSeparacao { get; set; }

        public bool HabilitarHoraFiltroDataInicialFinalRelatorioCargas { get; set; }

        public string NumeroEXP { get; set; }

        public string ObservacaoCTe { get; set; }

        public double CpfCnpjTransportadorTerceiro { get; set; }

        public string RotaEmbarcador { get; set; }

        public string NumeroPedidoCliente { get; set; }

        public DateTime? DataEncerramentoInicial { get; set; }

        public DateTime? DataEncerramentoFinal { get; set; }

        public DateTime? DataConfirmacaoDocumentosInicial { get; set; }

        public DateTime? DataConfirmacaoDocumentosFinal { get; set; }

        public List<int> CodigosRotas { get; set; }

        public int CodigoOperadorInsercao { get; set; }

        public bool SomenteCargasComValePedagio { get; set; }

        public bool SomenteCargasComFaturaFake { get; set; }

        public OpcaoSimNaoPesquisa NaoComparecimento { get; set; }

        public bool? PossuiPendencia { get; set; }

        public List<int> CodigosEmpresa { get; set; }

        public List<int> CodigosEmpresas { get; set; }

        public int CodigoZonaTransporte { get; set; }

        public List<double> CpfCnpjEmpresasConsulta { get; set; }

        public CargaTrechos CargaTrechos { get; set; }

        public int CanalEntrega { get; set; }

        public bool ApenasMDFeEncerrados { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.CargaTrechoSumarizada? CargaTrechoSumarizada { get; set; }

        public DateTime? DataFaturamentoInicial { get; set; }

        public DateTime? DataFaturamentoFinal { get; set; }

        public bool SomenteCargasComDocumentoOriginarioVinculado { get; set; }
        public bool SomenteCargasCriticas { get; set; }
        public bool SomenteCargasSemCIOT { get; set; }
        public bool SetarCargaComoBloqueadaEnquantoNaoReceberDesbloqueioViaIntegracaoOuManual { get; set; }

        public DateTime? DataCarregamentoInicio { get; set; }

        public DateTime? DataCarregamentoFim { get; set; }

        public string NumeroOT { get; set; }

        public bool ExibirCargasNaoFechadas { get; set; }

        public string NumeroPedidoNFe { get; set; }

        public bool CargasSemPacote { get; set; }

        public FormaIntegracao FormaIntegracaoNotas { get; set; }

        public List<CategoriaOS> CategoriaOS { get; set; }

        public List<TipoOSConvertido> TipoOSConvertido { get; set; }

        public List<TipoOS> TipoOS { get; set; }

        public List<TipoDirecionamentoCustoExtra> DirecionamentoCustoExtra { get; set; }

        public List<StatusCustoExtra> StatusCustoExtra { get; set; }

        public bool UsuarioUtilizaSegregacaoPorProvedor { get; set; }

        public List<double> CodigosProvedores { get; set; }

        public TipoOSConvertido TipoOSConvertidoPropriedade { get; set; }

        public TipoOS TipoOSPropriedade { get; set; }

        public TipoDirecionamentoCustoExtra? DirecionamentoCustoExtraPropriedade { get; set; }

        public StatusCustoExtra? StatusCustoExtraPropriedade { get; set; }

        public double CodigoProvedor { get; set; }

        public int CentroDeCustoViagemCodigo { get; set; }
        public bool SomenteCargasNaoValidadasNaGR { get; set; }
        public bool FlagCargaPercentualExecucao { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao PossuiValePedagio { get; set; }
        public List<int> Regiao { get; set; }
        public List<int> Mesorregiao { get; set; }
        public DateTime? DataInclusaoDadosTransporte { get; set; }
        public string NumeroPreCarga { get; set; }
        public int NumeroDtNatura { get; set; }
        public string NumeroContainerVeiculo { get; set; }

        public bool PermitirFiltrarCargasNaEmissaoManualCteSemTerCtesImportados { get; set; }
        public string CMDID { get; set; }
        public string CodigoNavio { get; set; }
        #endregion

        #region Propriedades com Regras

        public int CodigoVeiculo
        {
            get
            {
                if (_codigoVeiculoInformado)
                    return CodigosVeiculos.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoVeiculoInformado = true;
                    CodigosVeiculos = new List<int>() { value };
                }
            }
        }

        public int CodigoTransportador
        {
            get
            {
                if (_codigoTransportadorInformado)
                    return CodigosTransportador.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTransportadorInformado = true;
                    CodigosTransportador = new List<int>() { value };
                }
            }
        }

        public int CodigoFilial
        {
            get
            {
                if (_codigoFilialInformado)
                    return CodigosFilial.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoFilialInformado = true;
                    CodigosFilial = new List<int>() { value };
                }
            }
        }

        public int CodigoTipoCarga
        {
            get
            {
                if (_codigoTipoCargaInformado)
                    return CodigosTipoCarga.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTipoCargaInformado = true;
                    CodigosTipoCarga = new List<int>() { value };
                }
            }
        }

        public int CodigoTipoOperacao
        {
            get
            {
                if (_codigoTipoOperacaoInformado)
                    return CodigosTipoOperacao.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoTipoOperacaoInformado = true;
                    CodigosTipoOperacao = new List<int>() { value };
                }
            }
        }

        public int CodigoModeloVeicularCarga
        {
            get
            {
                if (_codigoModeloVeicularCargaInformado)
                    return CodigosModeloVeicularCarga.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoModeloVeicularCargaInformado = true;
                    CodigosModeloVeicularCarga = new List<int>() { value };
                }
            }
        }

        public double CpfCnpjDestinatario
        {
            get
            {
                if (_cpfCnpjDestinatarioInformado)
                    return CpfCnpjDestinatarios.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _cpfCnpjDestinatarioInformado = true;
                    CpfCnpjDestinatarios = new List<double>() { value };
                }
            }
        }

        public double CpfCnpjRemetente
        {
            get
            {
                if (_cpfCnpjRemetenteInformado)
                    return CpfCnpjRemetentes.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _cpfCnpjRemetenteInformado = true;
                    CpfCnpjRemetentes = new List<double>() { value };
                }
            }
        }

        public double CpfCnpjExpedidor
        {
            get
            {
                if (_cpfCnpjExpedidorInformado)
                    return CpfCnpjExpedidores.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _cpfCnpjExpedidorInformado = true;
                    CpfCnpjExpedidores = new List<double>() { value };
                }
            }
        }

        public double CpfCnpjRecebedor
        {
            get
            {
                if (_cpfCnpjRecebedorInformado)
                    return CpfCnpjRecebedores.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _cpfCnpjRecebedorInformado = true;
                    CpfCnpjRecebedores = new List<double>() { value };
                }
            }
        }
        public bool CargasParaEncerramento { get; set; }
        public int CodigoRota
        {
            get
            {
                if (_codigoRotaInformado)
                    return CodigosRotas.FirstOrDefault();

                return 0;
            }
            set
            {
                if (value > 0)
                {
                    _codigoRotaInformado = true;
                    CodigosTipoOperacao = new List<int> { value };
                }
            }
        }
        #endregion Propriedades com Regras
    }
}