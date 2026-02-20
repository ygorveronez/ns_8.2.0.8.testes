/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Canhoto.js" />
/// <reference path="../../Enumeradores/EnumSituacaoEntrega.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOnTime.js" />
/// <reference path="../ControleEntregaDevolucao/ControleEntregaDevolucao.js" />
/// <reference path="Fluxo.js" />
/// <reference path="ControleEntrega.js" />
/// <reference path="Produtos.js" />
/// <reference path="EntregaAreaRedex.js" />
/// <reference path="Assinatura.js" />
/// <reference path="Devolucao.js" />
/// <reference path="Atendimento.js" />
/// <reference path="../../Atendimentos/Chamado/Chamado.js" />
/// <reference path="../../Chamados/Chamado/Ocorrencias.js" />
/// <reference path="../../Chamados/Chamado/Analise.js" />
/// <reference path="../../Chamados/Chamado/Chamado.js" />

// #region Objetos Globais do Arquivo

var _entrega;
var _mapEntrega = null;
var _mapImagem = null;
var _mapOcorrencia = null;
var _dadosMapa = null;
var _draggImagem = null;
var _gridOcorrencia;
var callbackCanhotosCarregados = null;
var _pedidosCarregados = null;
var _comprovanteEntregaCarregado = null
var _GTACarregado = null;
var $tabGPA = null;
var $tabBoletos = null;
var $tabPedidos = null;
var $tabDocumentos = null;
var $tabOcorrencia = null;
var $tabComprovanteEntrega = null;
var $tabVisualizacaoCanhotos = null;
var $tabNFTransferenciaDevolucaoPallet = null;
var _ocorrenciasMobileCarregado = null;
var _boletosCarregados = null;
var _chavesNFeCarregados = null;
var _nfTransferenciaPalletsCarregadas = null;
var _draggCanhoto = null;
var _gridGTA = null;
var _gridPedidos = null;
var _gridChavesNFe = null;
var _gridAdicionarAnexoOcorrencia = null;
var _gridChamadosEntrega = null;
var _dadosDetalhesEntrega = null;
var _buscaCanhotoParaVinculo;
var _canhotoParaVinculo;
var _camera;
var _gridConferenciaProduto;
var _gridBoletos;
var _gridSobras;
var _ocorrencia;
var _chamado;
var _chamadosEntrega;
var _gridNFTransferenciaDevolucaoPallet;
var _mapaPosicao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var EntregaPedido = function () {
    //FAVOR COLOCAR A PROPRIEDADE VISIBLE EM TODOS OS NOVOS CAMPOS E ADICIONAR NA CONFIGURAÇÃO WIDGET.
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.CodigoChamado = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.Carga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.EnumSituacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    // Dados do Cliente
    this.NomeRecebedor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Cliente.getFieldDescription()), visible: ko.observable(true) });
    this.TelefoneCliente = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Telefone.getFieldDescription()), visible: ko.observable(true) });
    this.DocumentoRecebedor = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CPFCNPJ.getFieldDescription(), visible: ko.observable(true) });
    this.LocalidadeCliente = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.LocalidadeCliente.getFieldDescription()), visible: ko.observable(true) });
    this.Mesoregiao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Mesoregiao.getFieldDescription()), visible: ko.observable(true) });
    this.Regiao = PropertyEntity({ text: ko.observable(Localization.Resources.Pessoas.Pessoa.Regiao.getFieldDescription()), visible: ko.observable(true) });
    this.EnderecoCliente = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Endereco.getFieldDescription()), visible: ko.observable(true) });
    this.JanelaDescarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.JanelaDeDescarga.getFieldDescription(), visible: ko.observable(true) });
    this.InicioViagemPrevista = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoInicioViagem.getFieldDescription(), visible: ko.observable(true) });
    this.Localizacao = PropertyEntity({ text: `${Localization.Resources.Cargas.ControleEntrega.Localizacao} (Lat/Long):`, visible: ko.observable(true) });
    this.Fronteira = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true) });
    this.Parqueamento = PropertyEntity({ type: types.bool, val: ko.observable(false), visible: ko.observable(true) });

    this.CodigoSap = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoCliente.getFieldDescription(), visible: ko.observable(false) });
    this.Vendedor = PropertyEntity({ type: types.local, visible: ko.observable(false), visibilidadeEmail: ko.observable(false), visibilidadeTelefone: ko.observable(false) });
    this.Supervisor = PropertyEntity({ type: types.local, visible: ko.observable(false), visibilidadeEmail: ko.observable(false), visibilidadeTelefone: ko.observable(false) });
    this.Gerente = PropertyEntity({ type: types.local, visible: ko.observable(false), visibilidadeEmail: ko.observable(false), visibilidadeTelefone: ko.observable(false) });
    this.GerenteRegional = PropertyEntity({ type: types.local, visible: ko.observable(false), visibilidadeEmail: ko.observable(false), visibilidadeTelefone: ko.observable(false) });
    this.GerenteNacional = PropertyEntity({ type: types.local, visible: ko.observable(false), visibilidadeEmail: ko.observable(false), visibilidadeTelefone: ko.observable(false) });
    this.VendedorDados = PropertyEntity({ type: types.local, Nome: ko.observable(''), Email: ko.observable(''), Telefone: ko.observable(''), clickTelefone: exibeTelefoneVendedor, clickEmail: exibeEmailVendedor, clickWhats: abrirWhatsVendedor });
    this.SupervisorDados = PropertyEntity({ type: types.local, Nome: ko.observable(''), Email: ko.observable(''), Telefone: ko.observable(''), text: Localization.Resources.Cargas.ControleEntrega.Supervisor.getFieldDescription(), clickTelefone: exibeTelefoneSupervisor, clickEmail: exibeEmailSupervisor, clickWhats: abrirWhatsSupervisor });
    this.GerenteDados = PropertyEntity({ type: types.local, Nome: ko.observable(''), Email: ko.observable(''), Telefone: ko.observable(''), clickTelefone: exibeTelefoneGerente, clickEmail: exibeEmailGerente, clickWhats: abrirWhatsGerente });
    this.GerenteRegionalDados = PropertyEntity({ type: types.local, Nome: ko.observable(''), Email: ko.observable(''), Telefone: ko.observable(''), clickTelefone: exibeTelefoneGerenteRegional, clickEmail: exibeEmailGerenteRegional, clickWhats: abrirWhatsGerenteRegional });
    this.GerenteNacionalDados = PropertyEntity({ type: types.local, Nome: ko.observable(''), Email: ko.observable(''), Telefone: ko.observable(''), clickTelefone: exibeTelefoneGerenteNacional, clickEmail: exibeEmailGerenteNacional, clickWhats: abrirWhatsGerenteNacional });
    this.MostrarNomeCadeiaAjuda = PropertyEntity({ val: ko.observable(false) });
    this.MostrarEmailCadeiaAjuda = PropertyEntity({ val: ko.observable(false) });
    this.MostrarTelefoneCadeiaAjuda = PropertyEntity({ val: ko.observable(false) });
    this.MostrarWhatsAppCadeiaAjuda = PropertyEntity({ val: ko.observable(false) });


    this.AbrirWhatsAppSupervisor = PropertyEntity({ type: types.event, href: ko.computed(abrirWhatsSupervisor.bind(this)), visible: ko.observable(false) });
    this.AbrirWhatsAppVendedor = PropertyEntity({ type: types.event, href: ko.computed(abrirWhatsVendedor.bind(this)), visible: ko.observable(false) });
    this.AbrirWhatsAppGerenteNacional = PropertyEntity({ type: types.event, href: ko.computed(abrirWhatsGerenteNacional.bind(this)), visible: ko.observable(false) });
    this.AbrirWhatsAppGerenteRegional = PropertyEntity({ type: types.event, href: ko.computed(abrirWhatsGerenteRegional.bind(this)), visible: ko.observable(false) });
    this.AbrirWhatsAppGerente = PropertyEntity({ type: types.event, href: ko.computed(abrirWhatsGerente.bind(this)), visible: ko.observable(false) });

    // Dados da Entrega
    this.Numero = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Numero.getFieldDescription(), visible: ko.observable(false) });
    this.NumeroCTe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Ctes.getFieldDescription(), visible: ko.observable(false) });
    this.NumeroChamado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Atendimento.getFieldDescription(), visible: ko.observable(false) });
    this.Atendimentos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Atendimentos.getFieldDescription(), visible: ko.observable(false) });
    this.TempoRestanteChamado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TempoRestanteChamado.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.Email = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Email.getFieldDescription(), visible: ko.observable(false) });
    this.OrdemEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.SequenciaPrevistaSequenciaRealizada.getFieldDescription(), visible: ko.observable(false) });
    this.EntregaNoPrazo = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.EntregaNoPrazo.getFieldDescription(), visible: ko.observable(false) })
    this.OnTime = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OnTime.getFieldDescription(), type: typesKnockout.options, val: ko.observable(EnumSituacaoOnTime.NaoAjustado), def: EnumSituacaoOnTime.NaoAjustado, options: EnumSituacaoOnTime.obterOpcoes(), visible: ko.observable(false) });
    this.JustificativaOnTime = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Observacao.getFieldDescription()), visible: ko.observable(false), required: false });
    this.LocalidadeEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.LocalidadeEntrega.getFieldDescription()), visible: ko.observable(false) });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription(), visible: ko.observable(true) });
    this.DistanciaOrigemXEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DistanciaOrigemXEntrega.getFieldDescription(), val: ko.observable(""), visible: ko.observable(true) });
    this.DataPrevisaoEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaEntrega.getFieldDescription()), visible: ko.observable(false) });
    this.DataPrevisaoSaida = PropertyEntity({ text: ko.observable("Previsão de saída:"), visible: ko.observable(false) });
    this.DataAgendada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataAgendada.getFieldDescription()), visible: ko.observable(false) });
    this.ObservacoesAgendamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.ObservacoesAgendamento.getFieldDescription()), visible: ko.observable(false) });
    this.DataEntregaReprogramada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoReprogramadaDaEntrega.getFieldDescription()), visible: ko.observable(false) });
    this.DataProgramadaColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataProgramadaDaColeta.getFieldDescription(), visible: ko.observable(false) });
    this.TempoProgramadaColeta = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TempoProgramadaDaEntrega.getFieldDescription(), visible: ko.observable(false) });
    this.InicioViagemRealizada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.InicioViagemRealizada.getFieldDescription(), visible: ko.observable(false) });
    this.EtapaStage = PropertyEntity({ text: "Etapa:", visible: ko.observable(false) });
    this.FilialVenda = PropertyEntity({ text: "Filial da Venda:", visible: ko.observable(false) });
    this.DataConfirmacaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataConfirmacaoEntregaUsuario.getFieldDescription(), visible: ko.observable(false) });


    this.TipoMercadoria = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria.getFieldDescription(), visible: ko.observable(true) });

    this.EquipeVendas = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EquipeVendas.getFieldDescription(), visible: ko.observable(true) });

    this.EscritorioVenda = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EscritorioVenda.getFieldDescription(), visible: ko.observable(true) });

    this.CanalVenda = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CanalVenda.getFieldDescription(), visible: ko.observable(true) });

    this.Matriz = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Matriz.getFieldDescription(), visible: ko.observable(true) });

    this.Parqueada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Parqueada.getFieldDescription(), visible: ko.observable(true) });

    // Datas
    this.DataProgramadaDescarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataProgramadaDaDescarga.getFieldDescription(), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataInicioEntrega.getFieldDescription()), visible: ko.observable(false) });
    this.DataFim = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataTerminoDaEntrega.getFieldDescription()), visible: ko.observable(false) });
    this.DataConfirmacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataDeConfirmacao.getFieldDescription()), visible: ko.observable(true) });
    this.DataConfirmacaoApp = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataDeConfirmacaoApp.getFieldDescription()), visible: ko.observable(true) });
    this.DataRejeitado = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataDeRejeicao.getFieldDescription(), visible: ko.observable(false) });
    this.DataEntradaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Chegada.getFieldDescription(), visible: ko.observable(false), verLocalClick: abrirModalLocalizacaoEntradaRaio, verLocalVisible: ko.observable(false) });
    this.DataSaidaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Saida.getFieldDescription(), visible: ko.observable(false) });
    this.DataAgendamentoDeEntrega = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataAgendamentoDeEntrega.getFieldDescription()), visible: ko.observable(true), val: ko.observable("") });
    this.DataAgendamentoEntregaTransportador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataAgendamentoEntregaTransportador.getFieldDescription()), visible: ko.observable(true), val: ko.observable("") });
    this.OrigemSituacaoDataAgendamentoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OrigemDataDeAgendamentoDaEntrega.getFieldDescription(), visible: ko.observable(true), val: ko.observable("") });
    this.DataPrevisaoEntregaAjustada = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaAjustada.getFieldDescription()), visible: ko.observable(true), val: ko.observable("") });

    this.StatusTendenciaEntrega = PropertyEntity({ text: ko.observable(''), visible: ko.observable(true) });


    this.QuantidadePlanejada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.QuantidadePlanejada.getFieldDescription(), visible: ko.observable(false) });
    this.QuantidadeTotal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.QuantidadeColetada.getFieldDescription(), visible: ko.observable(false) });
    this.ObservacaoEntrega = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Observacao.getFieldDescription() });
    this.ResponsavelFinalizacaoManual = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.OperadorBaixaManual.getFieldDescription(), visible: ko.observable(false) });
    this.Pedidos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Pedidos.getFieldDescription(), visible: ko.observable(true) });
    this.NumeroPedidoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NumeroPedidoCliente.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.NotasFiscais = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.NotasFiscais.getFieldDescription(), visible: ko.observable(true) });
    this.QuantidadePacotesColetados = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PacotesColetados.getFieldDescription(), getType: typesKnockout.int, val: ko.observable(0), def: 0, visible: ko.observable(false) });
    this.ExigirInformarNumeroPacotesNaColetaTrizy = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ExibirDataPrevisaoEntregaTransportador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.DataPrevisaoEntregaTransportador = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaTransportador.getFieldDescription()), visible: ko.observable(false) });
    this.DetalhesProcessamentoFinalizacaoAssincrona = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.ControleEntrega.DetalhesProcessamentoFinalizacaoAssincrona.getFieldDescription()), visible: ko.observable(false) });
    this.InfoMotivoFalhaNotaFiscal = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoFalhaNotaFiscal.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });
    this.InfoMotivoRejeicao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoRejeicao.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });
    this.InfoPermiteEntregarMaisTarde = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.PermiteEntregarMaisTarde, visible: ko.observable(false) });
    this.InfoMotivoRetificacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoDaRetificacao.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });
    this.InfoMotivoFalhaGTA = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoDaFalhaDaGta.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Peso.getFieldDescription(), visible: ko.observable(false) });
    this.LeadTimeTransportador = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.LeadTimeTransportador.getFieldDescription(), visible: ko.observable(false) });
    this.ObservacoesPedidos = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ObservacoesPedidos.getFieldDescription(), visible: ko.observable(false) });
    this.LocalEntrega = PropertyEntity({ type: types.local });
    this.JustificativaEntregaForaRaio = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.JustificativaParaEntregaForaDoRaio.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(false) });
    this.LocalCliente = PropertyEntity({ type: types.local });
    this.Legenda = PropertyEntity({ type: types.local, LocalCliente: ko.observable(''), LocalFinalizacao: ko.observable(''), LocalDescarga: ko.observable('') });
    this.Imagens = PropertyEntity({ val: ko.observableArray([]) });
    this.Ocorrencias = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
    this.AnexosOcorrenciaAdicionar = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
    this.AnexosOcorrencia = PropertyEntity({ type: types.dynamic, list: new Array(), idGrid: guid() });
    this.Upload = PropertyEntity({ type: types.event, eventChange: UploadChange, idFile: guid(), accept: ".jpg,.tif,.pdf", text: "Upload", icon: "fal fa-file", visible: ko.observable(true) });
    this.Arquivo = PropertyEntity({ type: types.file, codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Foto.getFieldDescription(), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.NomeArquivo = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.Coleta = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.PossuiNotaCobertura = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ListaPedidoProduto = ko.observableArray();
    this.DataInicioEntregaInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Inicio.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(false) });
    this.DataEntregaInformada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Fim.getFieldDescription(), getType: typesKnockout.dateTime, val: ko.observable(Global.DataHoraAtual()), required: true, visible: ko.observable(false) });
    this.MotivoRejeicao = _controleEntregaDevolucaoDados.MotivoRejeicao;
    this.PermitirEntregarMaisTarde = _controleEntregaDevolucaoDados.PermitirEntregarMaisTarde;
    this.ExibirDadosDevolucao = _controleEntregaDevolucaoDados.ExibirDadosDevolucao;
    this.TipoDevolucao = _controleEntregaDevolucaoDados.TipoDevolucao;
    this.MotivoRetificacao = _motivoRetificacaoColetaEntrega != undefined ? _motivoRetificacaoColetaEntrega.MotivoRetificacao : "";
    this.ClientePossuiAreaRedex = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ColetaDeContainer = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.Armador = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ArmadorContainer = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });
    this.AreasRedex = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
    this.ColetasContainer = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
    this.Ocorrencia = PropertyEntity({ val: ko.observable(0), options: ko.observable(new Array()), def: 0, text: Localization.Resources.Cargas.ControleEntrega.Ocorrencia.getFieldDescription(), visible: ko.observable(true) });
    this.DataOcorrencia = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Data.getFieldDescription(), getType: typesKnockout.dateTime, def: Global.DataHoraAtual(), val: ko.observable(Global.DataHoraAtual()) });
    this.ObservacaoOcorrencia = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacoes.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, maxlength: 2000 });
    this.PermitirTransportadorConfirmarRejeitarEntrega = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.TipoContainerCarga = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.DescricaoTipoContainerCarga = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.CodigoContainer = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0) });
    this.DescricaoContainer = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    this.LocalRetiradaContainer = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
    // Cronômetro de carregamento ou descarregamento (hoje disponível paraLocalization.Resources.Cargas.ControleEntrega. Aves)
    this.DataInicioCarregamentoOuDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Inicio.getFieldDescription(), visible: ko.observable(true) });
    this.DataTerminoCarregamentoOuDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Fim.getFieldDescription(), visible: ko.observable(true) });
    this.TempoCarregamentoOuDescarregamento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Tempo.getFieldDescription(), visible: ko.observable(true) });
    this.ControlarTempo = PropertyEntity({ type: types.local, val: ko.observable(false) });
    this.AtivarConfirmacaoEntrega = PropertyEntity({ type: types.bool, val: ko.observable(false) });
    this.DataReentregaMesmaCarga = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataReentregaMesmaCarga.getFieldDescription(), visible: ko.observable(false) });
    this.DataEntregaNota = PropertyEntity({ val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.DataLimiteParaEntrega.getFieldDescription(), visible: ko.observable(_CONFIGURACAO_TMS.ExibirDataEntregaNotaControleEntrega) });
    this.StatusEntregaNota = PropertyEntity({ val: ko.observable(""), def: "", text: ko.observable(Localization.Resources.Cargas.ControleEntrega.StatusEntregaNota.getFieldDescription()), visible: ko.observable(true) });
    this.CodigoIntegracaoCliente = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoIntegracaoCliente.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.CodigoIntegracaoFilial = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CodigoIntegracaoFilial.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DataEmissaoNota = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEmissaoNota.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.DataRejeicaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataRejeicaoEntrega.getFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });

    this.Arquivo.val.subscribe(function (nomeArquivoSelecionado) {
        _entrega.NomeArquivo.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
        enviarImagem();
    });

    this.AbrirCamera = PropertyEntity({ eventClick: AbrirCamaraClick, type: types.event, text: "Abrir Camara", visible: ko.observable(true) });
    this.TiraFoto = PropertyEntity({ eventClick: TirarFotoClick, type: types.event, text: "Tirar Foto", visible: ko.observable(false) });

    this.expandirImagem = PropertyEntity({ eventClick: expandirImagem, type: types.event });
    this.DownloadImagem = PropertyEntity({ eventClick: downloadImagemEntregaClick, type: types.event });
    this.RemoverImagem = PropertyEntity({ eventClick: removerAnexoClick, type: types.event });
    this.VisualizarNoMapa = PropertyEntity({ eventClick: VisualizarNoMapaImagemEntregaClick, type: types.event });

    //Aba Canhotos
    this.VincularCanhotoEsperandoVinculo = PropertyEntity({ eventClick: vincularCanhotoEsperandoVinculoClick, type: types.event });
    this.ImagensConferencia = PropertyEntity({ val: ko.observableArray([]), });
    this.DownloadCanhoto = PropertyEntity({ eventClick: downloadCanhotoClick, type: types.event });
    this.AuditarCanhoto = PropertyEntity({ eventClick: auditarCanhotoClick, type: types.event, text: "Auditar", visible: ko.observable(true) });
    this.DownloadCanhotoEmMassa = PropertyEntity({ eventClick: DownloadMassaClick, type: types.event });
    this.EnviarImagem = PropertyEntity({ type: types.event, eventChange: enviarImagemCanhotoClick, idFile: guid(), accept: ".jpg,.tif,.pdf, .png", text: "Enviar Imagem Canhoto", visible: ko.observable(_CONFIGURACAO_TMS.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) });
    this.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(_CONFIGURACAO_TMS.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas == true && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe), def: (_CONFIGURACAO_TMS.PermitirEnvioCanhotosPeloPortalTransportadorControleEntregas == true && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) });

    // Aba Fotos Notas Fiscais
    this.ImagensNotasFiscais = PropertyEntity({ val: ko.observableArray([]), });
    this.DownloadFotoNF = PropertyEntity({ eventClick: downloadImagemNotaFiscalClick, type: types.event });
    this.MostrarAbaNotasFiscais = PropertyEntity({ type: types.local, val: ko.observable(false) });

    // Aba GTA
    this.GridGTA = PropertyEntity({});
    this.MostrarAbaGta = PropertyEntity({ type: types.local, val: ko.observable(false) });
    this.ImagensFotoGTA = PropertyEntity({ val: ko.observableArray([]), });

    // Aba Pedidos
    this.GridPedidos = PropertyEntity({});

    // Aba Assinatura
    this.Assinatura = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Asssinatura.getFieldDescription(), visible: ko.observable(false) });
    this.Assinatura.val.subscribe(function (novoValor) {
        if (novoValor != "")
            _entrega.AssinarManualmente.text("Refazer Assinatura");
        else
            _entrega.AssinarManualmente.text("Assinar Manualmente");
    });

    // Aba Pesquisa
    this.CodigoCheckList = PropertyEntity({ val: ko.observable(0) });
    this.GrupoPerguntas = PropertyEntity({ val: ko.observableArray([]) });
    this.SalvarCheckList = PropertyEntity({ eventClick: SalvarCheckListClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.SalvarPesquisa, visible: VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteEditarPesquisa, _PermissoesPersonalizadasControleEntrega) });
    this.ExibirCheckList = PropertyEntity({ type: types.map, val: ko.observable(false) });

    // Aba Pesquisa Desembarque
    this.CodigoCheckListDesembarque = PropertyEntity({ val: ko.observable(0) });
    this.GrupoPerguntasDesembarque = PropertyEntity({ val: ko.observableArray([]) });
    this.SalvarCheckListDesembarque = PropertyEntity({ eventClick: SalvarCheckListDesembarqueClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.SalvarPesquisa, visible: VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteEditarPesquisa, _PermissoesPersonalizadasControleEntrega) });
    this.ExibirCheckListDesembarque = PropertyEntity({ type: types.map, val: ko.observable(false) });

    // Aba Avaliação
    this.DataAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataAvaliacao.getFieldDescription() });
    this.TipoAvaliacaoGeral = PropertyEntity({ val: ko.observable(true) });
    this.Questionario = PropertyEntity({ perguntas: ko.observable([]) });
    this.Avaliacao = PropertyEntity({ estrelas: [5, 4, 3, 2, 1], val: ko.observable(0), def: 0 });
    this.MotivoAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.MotivoAvaliacao.getFieldDescription() });
    this.ObservacaoAvaliacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ObservacaoAvaliacao.getFieldDescription() });

    // Aba Dados Recebedor
    this.DadosRecebedorNome = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Nome.getFieldDescription(), required: false });
    this.DadosRecebedorCPF = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.CPF.getFieldDescription(), maxlength: 14, getType: typesKnockout.cpf, required: false });
    this.DadosRecebedorDataEntrega = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataEntrega.getFieldDescription(), visible: ko.observable(true), getType: typesKnockout.date, required: false });
    this.DadosRecebedorAssinatura = PropertyEntity({ visible: ko.observable(false) });
    this.DadosRecebedorFoto = PropertyEntity({ visible: ko.observable(true), getType: typesKnockout.string });
    this.DadosRecebedorPercentualCompatibilidadeFoto = PropertyEntity({ getType: typesKnockout.string, visible: ko.observable(true) });
    this.AtualizarDadosRecebedor = PropertyEntity({ eventClick: atualizarDadosRecebedorClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Salvar) });
    this.AtualizarAssinatura = PropertyEntity({ eventClick: enviarImagemAssinatura, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.EnviarImagem) });
    this.DadosRecebedorNomeArquivoAssinatura = PropertyEntity({ val: ko.observable(Localization.Resources.Gerais.Geral.Selecionar), def: Localization.Resources.Gerais.Geral.Selecionar, getType: typesKnockout.string });
    this.DadosRecebedorArquivoAssinatura = PropertyEntity({ type: types.file, codEntity: ko.observable(0), val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(true) });
    this.ParametrosDeCalculo = PropertyEntity({ type: types.dinamyc, val: ko.observable({}) });
    this.DadosRecebedorArquivoAssinatura.val.subscribe(function (nomeArquivoSelecionado) {
        if (nomeArquivoSelecionado != "") {
            _entrega.DadosRecebedorNomeArquivoAssinatura.val(nomeArquivoSelecionado.replace('C:\\fakepath\\', ''));
            carregarImagemNaTela();
        }
        else
            _entrega.DadosRecebedorNomeArquivoAssinatura.val(_entrega.DadosRecebedorNomeArquivoAssinatura.def);
    });
    this.AssinarManualmente = PropertyEntity({ type: types.event, eventClick: assinarManualmenteClick, text: ko.observable("Assinar"), visible: ko.observable(true) });

    // Aba Comprovante de Entrega
    this.GridComprovanteEntrega = PropertyEntity({});

    // Aba Confirmação de chegada
    this.LocalConfirmacaoChegada = PropertyEntity({ type: types.local });

    // Aba Observacoes
    this.Observacao = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacoes.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.string, maxlength: 2000, visible: ko.observable(true) });
    this.SalvarObservacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Salvar, eventClick: salvarObservacaoClick, type: types.event, visible: ko.observable(true) });

    // Aba Documentos
    this.GridChavesNFe = PropertyEntity({});

    this.AdicionarChaveNFe = PropertyEntity({ eventClick: adicionarChaveNFeClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(false) });
    this.FinalizarEnvioChavesColeta = PropertyEntity({ eventClick: FinalizarEnvioChavesColetaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.FinalizarEnvioChaveNotaFiscal, visible: ko.observable(false) });

    this.ChaveNFe = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.ChaveNotaFiscal, def: "", required: true, getType: typesKnockout.string, visible: ko.observable(false) });

    // Aba Conferencia Produtos
    this.ExigeConferenciaProdutos = PropertyEntity({ getTypes: typesKnockout.bool, val: ko.observable(false), def: false });
    this.ConferenciaProdutos = ko.observableArray([]);
    this.ConfirmarConferenciaProdutos = PropertyEntity({ type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, eventClick: confirmarConferenciaProdutos, visible: ko.observable(false) });

    this.Sobras = PropertyEntity({ val: ko.observableArray([]), idGrid: guid(), visible: ko.observable(false) });

    // Aba Boletos
    this.GridBoletos = PropertyEntity({});

    //Aba Devolução Pallet
    this.GridNFTransferenciaDevolucaoPallet = PropertyEntity({});
    this.ChaveNFTransferenciaPallet = PropertyEntity({ getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, text: ko.observable("Chave"), val: ko.observable(""), enable: ko.observable(false) });
    this.NFeTransferencia = PropertyEntity({ text: ko.observable("Adicionar NFes de Transferência de Palletes?"), getType: typesKnockout.bool, val: ko.observable(false), def: false });
    this.SalvarNotaTransferenciaDevolucaoPallet = PropertyEntity({ eventClick: salvarNotaTransferenciaDevolucaoPalletClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true), enable: ko.observable(false) });

    // Botões e relacionados
    this.Fechar = PropertyEntity({ eventClick: fecharEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Fechar), visible: ko.observable(false) });
    this.Retificar = PropertyEntity({ eventClick: retificarClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Retificar, visible: ko.observable(false) });
    this.RemoverReentrega = PropertyEntity({ eventClick: removerReentregaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.RemoverReentrega, visible: ko.observable(false) });
    this.RemoverEntrega = PropertyEntity({ eventClick: removerEntregaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.RemoverEntrega, visible: ko.observable(false) });
    this.AlterarDataEntrega = PropertyEntity({ eventClick: alterarDataEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.AlterarDataEntrega), visible: ko.observable(false) });
    this.ConfirmarEntrega = PropertyEntity({ eventClick: confirmarEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Confirmar), visible: ko.observable(false) });
    this.RejeitarEntrega = PropertyEntity({ eventClick: rejeitarEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Rejeitar), visible: ko.observable(false) });
    this.ConfirmarRejeicaoEntrega = PropertyEntity({ eventClick: confirmarRejeicaoColetaEntregaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.Rejeitar) });
    this.AtualizarCoordenadasCliente = PropertyEntity({ eventClick: atualizarCoordenadasClienteClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AtualizarCoordenadasDoCliente, visible: ko.observable(true) });
    this.AtualizarProdutos = PropertyEntity({ eventClick: atualizarProdutosClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AtualizarProdutos });
    this.VerDetalhesChamado = PropertyEntity({ eventClick: verDetalhesChamadoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.VerDetalhes, visible: ko.observable(true) });
    this.VerMultiplosChamadosCarga = PropertyEntity({ eventClick: verChamadosCargaEntregaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.VerDetalhes, visible: ko.observable(false) });
    this.AbrirNovoAtendimento = PropertyEntity({ eventClick: NovoAtendimentoClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.NovoAtendimento, visible: ko.observable(_CONFIGURACAO_TMS.PermitirAbrirAtendimento) });
    this.AdicionarOcorrencia = PropertyEntity({ eventClick: adicionarOcorrenciaEntregaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.AlterarDestinatario = PropertyEntity({ eventClick: alterarDestinatarioClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Alterar, visible: ko.observable(false) });
    this.BaixarComprovanteColeta = PropertyEntity({ eventClick: baixarComprovanteColetaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.BaixarComprovanteColeta, visible: ko.observable(true) });
    this.AlterarOnTime = PropertyEntity({ eventClick: alterarOnTimeClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AlterarOnTime, visible: this.OnTime.visible });
    this.AlterarDataReagendamento = PropertyEntity({ eventClick: alterarDataReagendamentoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.ControleEntrega.AlterarDataReagendamento), visible: ko.observable(false) });
    this.Imprimir = PropertyEntity({ eventClick: imprimirClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Imprimir, visible: ko.observable(true) });
    this.AlterarDataAgendamentoDeEntrega = PropertyEntity({ eventClick: alterarDataAgendamentoEntregaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Alterar, visible: ko.observable(false) });
    this.AlterarDataPrevisaoEntregaAjustada = PropertyEntity({ val: ko.observable(false), eventClick: AlterarDataPrevisaoEntregaAjustadaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Alterar, visible: ko.observable(true) });
    this.AlterarDataAgendamentoEntregaTransportador = PropertyEntity({ val: ko.observable(false), eventClick: alterarDataAgendamentoEntregaTransportadorClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Alterar, visible: ko.observable(false) });
    this.VerParametroCalculo = PropertyEntity({ eventClick: AbriModalParametrosCalculo, type: types.event, text: "Ver Parâmetros de cálculo", visible: ko.observable(true) });

    this.AtualizarCoordenadasClienteDrag = PropertyEntity({ eventClick: atualizarCoordenadasClienteDragClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.AtualizarCoordenadasDoCliente, visible: ko.observable(true), Latitude: null, Longitude: null });
    this.CpfCnpjCliente = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0 });

    this.AuditarCheckList = PropertyEntity({ eventClick: exibirAuditoriaCheckListClick, type: types.event, visible: ko.observable(false) });
    this.AuditarCheckListDesembarque = PropertyEntity({ eventClick: exibirAuditoriaCheckListDesembarqueClick, type: types.event, visible: ko.observable(false) });
    this.Auditar = PropertyEntity({ eventClick: auditarControleEntregaClick, type: types.event, text: Localization.Resources.Cargas.ControleEntrega.Auditar, visible: ko.observable(true) });
    this.observacaoOcorrenciaval = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Observacao, def: "", getType: typesKnockout.string, visible: ko.observable("") });
};

var CanhotoParaVinculo = function () {
    this.Canhoto = PropertyEntity({ val: ko.observable(0), def: 0, codEntity: ko.observable(0), getType: typesKnockout.int });
    this.Entrega = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
}

var ChamadosEntrega = function () {
    this.ChamadosEntrega = PropertyEntity({ val: ko.observableArray([]), idGrid: guid() });
}

// #endregion Classes

// #region Funções de Inicialização

function auditarControleEntregaClick(e, sender) {
    const data = { Codigo: _entrega.Codigo.val() };
    const closureAuditoria = OpcaoAuditoria("CargaEntrega", null, e);

    closureAuditoria(data);
}

function auditarCanhotoClick(e, sender) {
    const data = { Codigo: e.Codigo };
    const clouserAuditoria = OpcaoAuditoria("Canhoto", null, _entrega);

    clouserAuditoria(data);
}

function loadEntrega() {
    loadMotivosRetificacaoColetaEntrega();
    loadAlterarDestinatario();
    loadCheckList();
    loadAlterarDataOcorrencia();
    loadControleEntregaDevolucaoDados();
    loadAreasRedex();
    loadColetaContainer();
    loadCamera();
    loadAlterarDataAgendamentoEntrega();
    loadAlterarDataPrevisaoEntregaAjustada();
    loadAlterarDataAgendamentoEntregaTransportador();
    loadConfiguracaoWidget();

    _entrega = new EntregaPedido();
    KoBindings(_entrega, "knockoutEntrega");

    HeaderAuditoria("CargaEntrega", _entrega);

    _chamadosEntrega = new ChamadosEntrega();
    KoBindings(_chamadosEntrega, "knockoutChamadosEntrega");

    _entrega.AuditarCheckList.visible(PermiteAuditar());
    _entrega.AuditarCheckListDesembarque.visible(PermiteAuditar());

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _entrega.NumeroCTe.visible(true);
        _entrega.Fechar.visible(true);
        _entrega.RejeitarEntrega.text(Localization.Resources.Gerais.Geral.Cancelar);
        _entrega.ConfirmarEntrega.text(Localization.Resources.Cargas.ControleEntrega.Finalizar);
    }

    else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        _entrega.Auditar.visible(false);
    }

    _draggImagem = $.draggImagem({
        container: ".container-imagem-entrega-dragg",
        image: ".imagem-entrega img",
    });

    //Canhotos
    $tabVisualizacaoCanhotos = $("#liVisualizacaoCanhotos");
    $tabVisualizacaoCanhotos.on('shown.bs.tab', 'a', function () {
        ControlarExibicaoGridCanhotos();
    });
    $(window).one('hashchange', function (e) {
        $tabVisualizacaoCanhotos.off('shown.bs.tab', 'a');
    });

    _draggCanhoto = $.draggImagem({
        container: ".DivConferencia",
        image: ".container-drag img"
    });

    $tabPedidos = $("#liPedidos");
    $tabPedidos.on('shown.bs.tab', 'a', function () {
        if (_pedidosCarregados == null) {
            inicializaGridPedidos();
            _gridPedidos.CarregarGrid();
            _pedidosCarregados = true;
        }
    });

    $tabGPA = $("#liGTA");
    $tabGPA.on('shown.bs.tab', 'a', function () {
        if (_GTACarregado == null) {
            inicializaGridGTA();
            _gridGTA.CarregarGrid();
            _GTACarregado = true;
        }
    });

    $tabComprovanteEntrega = $("#liComprovanteEntrega");
    $tabComprovanteEntrega.on('shown.bs.tab', 'a', function () {
        if (_comprovanteEntregaCarregado == null) {
            inicializaGridComprovanteEntrega();
            _gridComprovanteEntrega.CarregarGrid();
            _comprovanteEntregaCarregado = true;
        }
    });

    $tabOcorrencia = $('#liOcorrencia');
    $tabOcorrencia.on('shown.bs.tab', 'a', function () {
        if (_ocorrenciasMobileCarregado == null) {
            ObterOcorrencias();
            _ocorrenciasMobileCarregado = true;
        }
    })

    $tabDocumentos = $("#liChavesNFe");
    $tabDocumentos.on('shown.bs.tab', 'a', function () {
        if (_chavesNFeCarregados == null) {
            inicializaGridChavesNFe();
            _gridChavesNFe.CarregarGrid();
            _chavesNFeCarregados = true;
        }
    });

    $tabBoletos = $("#liBoletos");
    $tabBoletos.on('shown.bs.tab', 'a', function () {
        if (_boletosCarregados == null) {
            inicializaGridBoletos();
            _gridBoletos.CarregarGrid();
            _boletosCarregados = true;
        }
    });

    $tabNFTransferenciaDevolucaoPallet = $("#liDevolucaoPallet");
    $tabNFTransferenciaDevolucaoPallet.on('shown.bs.tab', 'a', function () {
        if (_nfTransferenciaPalletsCarregadas == null) {
            inicializaGridNFPalletTransferencia();
            _gridNFTransferenciaDevolucaoPallet.CarregarGrid();
            _nfTransferenciaPalletsCarregadas = true;
        }
    });
}

function loadInformacaoPessoas() {
    _entrega.SupervisorDados.Nome(_entrega.Supervisor.val().Nome);
    _entrega.SupervisorDados.Email(_entrega.Supervisor.val().Email);
    _entrega.SupervisorDados.Telefone(_entrega.Supervisor.val().Telefone);

    _entrega.GerenteDados.Nome(_entrega.Gerente.val().Nome);
    _entrega.GerenteDados.Email(_entrega.Gerente.val().Email);
    _entrega.GerenteDados.Telefone(_entrega.Gerente.val().Telefone);

    _entrega.GerenteRegionalDados.Nome(_entrega.GerenteRegional.val().Nome);
    _entrega.GerenteRegionalDados.Email(_entrega.GerenteRegional.val().Email);
    _entrega.GerenteRegionalDados.Telefone(_entrega.GerenteRegional.val().Telefone);

    _entrega.GerenteNacionalDados.Nome(_entrega.GerenteNacional.val().Nome);
    _entrega.GerenteNacionalDados.Email(_entrega.GerenteNacional.val().Email);
    _entrega.GerenteNacionalDados.Telefone(_entrega.GerenteNacional.val().Telefone);

    _entrega.VendedorDados.Nome(_entrega.Vendedor.val().Nome);
    _entrega.VendedorDados.Email(_entrega.Vendedor.val().Email);
    _entrega.VendedorDados.Telefone(_entrega.Vendedor.val().Telefone);


    if ((_entrega.Supervisor.val().Nome || _entrega.Supervisor.val().Email || _entrega.Supervisor.val().Telefone) && (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda.Supervisor.val()))
        _entrega.Supervisor.visible(true);
    else
        _entrega.Supervisor.visible(false);


    if ((_entrega.Gerente.val().Nome || _entrega.Gerente.val().Email || _entrega.Gerente.val().Telefone) && (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda.Gerente.val()))
        _entrega.Gerente.visible(true);
    else
        _entrega.Gerente.visible(false);

    if ((_entrega.GerenteNacional.val().Nome || _entrega.GerenteNacional.val().Email || _entrega.GerenteNacional.val().Telefone) && (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda.GerenteNacional.val()))
        _entrega.GerenteNacional.visible(true);
    else
        _entrega.GerenteNacional.visible(false);

    if ((_entrega.GerenteRegional.val().Nome || _entrega.GerenteRegional.val().Email || _entrega.GerenteRegional.val().Telefone) && (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda.GerenteRegional.val()))
        _entrega.GerenteRegional.visible(true);
    else
        _entrega.GerenteRegional.visible(false);

    if ((_entrega.Vendedor.val().Nome || _entrega.Vendedor.val().Email || _entrega.Vendedor.val().Telefone) && (_configuracaoExibicaoDetalhesEntregaCadeiaAjuda.Vendedor.val()))
        _entrega.Vendedor.visible(true);
    else
        _entrega.Vendedor.visible(false);
}

function inicializaGridGTA() {
    _gridGTA = new GridView(_entrega.GridGTA.id, "ControleEntrega/ObterGuiasTransporteAnimal", {
        Codigo: _entrega.Codigo
    });
}

function inicializaGridPedidos() {
    const removerReentrega = {
        descricao: Localization.Resources.Cargas.ControleEntrega.RemoverReentrega,
        id: guid(),
        evento: "onclick",
        visibilidade: VisibilidadeRemoverReentrega,
        metodo: removerPedidoReentregaClick
    };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [removerReentrega], tamanho: 5 };

    _gridPedidos = new GridView("grid-controle-entrega-pedidos", "ControleEntrega/ObterPedidos", {
        Codigo: _entrega.Codigo
    }, menuOpcoes);

    _gridPedidos.SetPermitirEdicaoColunas(true);
    _gridPedidos.SetSalvarPreferenciasGrid(true);
    _gridPedidos.SetHabilitarModelosGrid(true);
    _gridPedidos.SetHabilitarScrollHorizontal(true, 200);
}

function inicializaGridChavesNFe() {
    const removerChaveNFe = {
        descricao: "Remover chave NFe",
        id: guid(),
        evento: "onclick",
        visibilidade: VisibilidadeRemoverChaveNFe,
        metodo: removerChaveNFeClick
    };

    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, opcoes: [removerChaveNFe], tamanho: 5 };

    _gridChavesNFe = new GridView(_entrega.GridChavesNFe.id, "ControleEntrega/ObterChavesNFe", {
        Codigo: _entrega.Codigo
    }, menuOpcoes);
}

function adicionarChaveNFeClick() {
    executarReST("ControleEntrega/AdicionarChaveNFe", { CodigoEntrega: _entrega.Codigo.val(), ChaveNFe: _entrega.ChaveNFe.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {

                _gridChavesNFe.CarregarGrid();
                _entrega.ChaveNFe.val("");
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function FinalizarEnvioChavesColetaClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.RealmentedDesejaFinalizarEnvioNFe, function () {
        executarReST("ControleEntrega/FinalizarEnvioChavesNFe", { Codigo: _entrega.Codigo.val() }, function (arg) {
            if (arg.Success) {
                executarEnvioDocumentosRest()
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}
function executarEnvioDocumentosRest() {
    executarReST("CargaDocumentosFiscais/ConfirmarEnvioDosDocumentosFiscais", { Entrega: _entrega.Codigo.val(), LiberouProdutosDivergentes: false }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.FinalizacaoEnvioNFEFConcluida);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 20000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function inicializaGridComprovanteEntrega() {

    const menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = [
        { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), evento: "onclick", metodo: onClickDetalharComprovanteEntrega, tamanho: "20", icone: "" },
    ];

    _gridComprovanteEntrega = new GridView(_entrega.GridComprovanteEntrega.id, "ControleEntrega/ObterComprovanteEntrega", {
        Codigo: _entrega.Codigo
    }, menuOpcoes);
}

function inicializaGridBoletos() {
    _gridBoletos = new GridView(_entrega.GridBoletos.id, "ControleEntrega/ObterBoletos", { NotasFiscais: _entrega.NotasFiscais });
}

function inicializaGridNFPalletTransferencia() {
    _gridNFTransferenciaDevolucaoPallet = new GridView(_entrega.GridNFTransferenciaDevolucaoPallet.id, "ControleEntrega/ObterGridNFTransferenciaDevolucaoPallet", { Codigo: _entrega.Codigo });
}

function salvarNotaTransferenciaDevolucaoPalletClick() {
    let dados = {
        ChaveNfTransferencia: _entrega.ChaveNFTransferenciaPallet.val(),
        CodigoCargaEntrega: _entrega.Codigo.val(),
        NFeTransferencia: _entrega.NFeTransferencia.val()
    }
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.RealmenteDesejaAdicionarEssaNota, function () {
        executarReST('ControleEntrega/AdicionarNfTransferenciaDevolucaoPallet', dados, function (arg) {
            if (arg.Success) {
                _gridNFTransferenciaDevolucaoPallet.CarregarGrid();
                LimparCampo(_entrega.ChaveNFTransferenciaPallet);
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function alterarDestinatarioClick() {
    AbrirModalAlteracaoDestinatario();
}

function adicionarOcorrenciaEntregaClick() {
    let codigoChamado = 0;
    if (_chamado != null && _chamado.Codigo.val() > 0)
        codigoChamado = _chamado.Codigo.val();

    let ocorrenciaTipoOcorrencia = 0;
    if (_ocorrencia != null && _ocorrencia.TipoOcorrencia.codEntity() > 0)
        ocorrenciaTipoOcorrencia = _ocorrencia.TipoOcorrencia.codEntity();

    let ocorrenciaComponenteFrete = 0;
    if (_ocorrencia != null && _ocorrencia.ComponenteFrete.codEntity() > 0)
        ocorrenciaComponenteFrete = _ocorrencia.ComponenteFrete.codEntity();

    let ocorrenciaValorOcorrencia = 0;
    if (_ocorrencia != null && _ocorrencia.ValorOcorrencia.val())
        ocorrenciaValorOcorrencia = _ocorrencia.ValorOcorrencia.val();

    const adicionado = {
        Codigo: _entrega.Ocorrencia.val(),
        Descricao: $("#" + _entrega.Ocorrencia.id + " option:selected").text(),
        DataOcorrencia: _entrega.DataOcorrencia.val(),
        ObservacaoOcorrencia: _entrega.ObservacaoOcorrencia.val(),
        CodigoChamado: codigoChamado,
        CodigoTipoOcorrencia: ocorrenciaTipoOcorrencia,
        ComponenteFrete: ocorrenciaComponenteFrete,
        ValorOcorrencia: ocorrenciaValorOcorrencia,
    }

    const anexos = new FormData();
    _entrega.AnexosOcorrencia.list.forEach(function (anexo) {
        if (isNaN(anexo.Codigo))
            anexos.append("AnexosOcorrencia", anexo.Arquivo)
    });

    const dados = { Codigo: _entrega.Codigo.val(), Ocorrencia: adicionado.Codigo, DataOcorrencia: _entrega.DataOcorrencia.val(), ObservacaoOcorrencia: _entrega.ObservacaoOcorrencia.val(), TipoOcorrencia: adicionado.CodigoTipoOcorrencia, ComponenteFrete: adicionado.ComponenteFrete, ValorOcorrencia: adicionado.ValorOcorrencia, CodigoChamado: adicionado.CodigoChamado }

    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.RealmenteDesejaAdicionarEssaOcorrencia, function () {
        enviarArquivo("ControleEntregaEntrega/AdicionarOcorrencia", dados, anexos, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    _gridOcorrencia.CarregarGrid(arg.Data.Ocorrencias);
                    _entrega.DataOcorrencia.val(Global.DataHoraAtual());
                    limparAnexosOcorrenciaAdicionar();
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}
function limparAnexosOcorrenciaAdicionar() {
    _entrega.AnexosOcorrencia.list = new Array();
    RecarregarGridAnexoOcorrenciaAdicionar();
}

function ObterOcorrencias() {
    const ocorrenciasOption = new Array();
    executarReST("TipoOcorrencia/BuscarOcorrenciasMobile", { UsadoParaMotivoRejeicaoColetaEntrega: false, TipoAplicacaoColetaEntrega: _entrega.Coleta.val() ? EnumTipoAplicacaoColetaEntrega.Coleta : EnumTipoAplicacaoColetaEntrega.Entrega, Codigo: _entrega.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                const ocorrencias = arg.Data;
                if (ocorrencias.length > 0) {
                    for (let i = 0; i < ocorrencias.length; i++) {
                        ocorrenciasOption.push({ text: ocorrencias[i].Descricao, value: ocorrencias[i].Codigo });
                    }
                    _entrega.Ocorrencia.options(ocorrenciasOption);
                    _entrega.Ocorrencia.val(ocorrencias[0].Codigo);
                    return;
                }
            }
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Sugestao, arg.Msg, 16000);
        }
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, arg.Msg);
    }, null);
}

function exibirDetalhesEntrega(fluxoEntrega, entrega) {
    _etapaAtualFluxo = fluxoEntrega;

    if (_entrega == null) {

        _entrega = new EntregaPedido();
        KoBindings(_entrega, "knockoutEntrega");
    }

    limparCamposDetalhesEntrega();

    BuscarDetalhesEntrega(entrega);
}

function BuscarDetalhesEntrega(entrega) {
    const path = document.location.href;
    const page = path.split("/").pop();

    executarReST("ControleEntregaEntrega/BuscarDetalhesEntrega", { Codigo: entrega.Codigo, Page: page }, function (arg) {
        if (arg.Success) {
            const data = arg.Data;
            if (data !== false) {
                loadConfiguracaoWidget();
                PreencherDetalhesEntrega(data);
                _dadosDetalhesEntrega = data;
                visibilidadeAbasControleEntrega(data);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sucesso.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Sucesso.Falha, arg.Msg);
        }
    });

}

function PreencherDetalhesEntrega(dados) {
    const fluxoFinalizado = _etapaAtualFluxo.DataFimViagem.val() != "";
    _pedidosCarregados = null;
    _comprovanteEntregaCarregado = null;
    _GTACarregado = null;
    _ocorrenciasMobileCarregado = null;
    _chavesNFeCarregados = null;
    _boletosCarregados = null;

    PreencherObjetoKnout(_entrega, { Data: dados });
    preencherListaPedidoProduto(dados.Produtos);
    setarTituloModalEntrega(dados.Numero);
    _entrega.Vendedor.val(dados.Vendedor);
    _entrega.Supervisor.val(dados.Supervisor);
    _entrega.Gerente.val(dados.GerenteArea);
    _entrega.GerenteRegional.val(dados.GerenteRegional);
    _entrega.GerenteNacional.val(dados.GerenteNacional);
    _entrega.DataPrevisaoEntrega.visible(false);
    _entrega.DataEntregaReprogramada.visible(false);

    _entrega.DataInicio.visible(_entrega.DataInicio.val() != "" && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataInicio.val());
    _entrega.DataFim.visible(_entrega.DataFim.val() != "" && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataFim.val());
    _entrega.DataConfirmacao.visible(_entrega.DataConfirmacao.val() != "" && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataConfirmacao.val());
    _entrega.NumeroPedidoCliente.visible(_entrega.NumeroPedidoCliente.val() != "" && _configuracaoExibicaoDetalhesEntregaEntregaColeta.NumeroPedidoCliente.val());
    _entrega.PermitirTransportadorConfirmarRejeitarEntrega.val(dados.PermitirTransportadorConfirmarRejeitarEntrega);

    // Localidade
    const ehEntrega = !_entrega.Coleta.val();
    const ehColeta = _entrega.Coleta.val();

    _entrega.LocalidadeEntrega.text(ehEntrega ? Localization.Resources.Cargas.ControleEntrega.LocalidadeEntrega.getFieldDescription() : Localization.Resources.Cargas.ControleEntrega.Localidade.getFieldDescription());

    if (_entrega.DataSaidaRaio.val() != "" && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataSaidaRaio.val())
        _entrega.DataSaidaRaio.visible(true);
    else
        _entrega.DataSaidaRaio.visible(false);

    if (_entrega.DataEntradaRaio.val() != "" && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataEntradaRaio.val())
        _entrega.DataEntradaRaio.visible(true);
    else
        _entrega.DataEntradaRaio.visible(false);

    if (dados.LocalConfirmacaoChegada.Latitude != null && dados.LocalConfirmacaoChegada.Longitude != null && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataEntradaRaio.val()) {
        _entrega.DataEntradaRaio.verLocalVisible(true);
    } else {
        _entrega.DataEntradaRaio.verLocalVisible(false);
    }

    if (dados.QuantidadePlanejada != null) {
        _entrega.QuantidadePlanejada.val(dados.QuantidadePlanejada);
    }

    if (dados.QuantidadeTotal != null) {
        _entrega.QuantidadeTotal.val(dados.QuantidadeTotal);
    }

    const isNotaCoberturaEPodeAlterarEntrega = dados.PossuiNotaCobertura && dados.EnumSituacao == EnumSituacaoEntrega.NaoEntregue;

    _entrega.RemoverReentrega.visible(dados.PermiteRemoverReentrega);
    _entrega.AlterarDestinatario.visible(isNotaCoberturaEPodeAlterarEntrega);
    _entrega.BaixarComprovanteColeta.visible(ehColeta && dados.TipoOperacao.PermiteBaixarComprovanteColeta);

    if (isNotaCoberturaEPodeAlterarEntrega)
        _entrega.RemoverEntrega.visible(true);
    else
        _entrega.RemoverEntrega.visible(false);

    if (dados.Assinatura == null || dados.Assinatura.Miniatura == "") {
        _entrega.Assinatura.val("");
        _entrega.AssinarManualmente.text("Assinar Manualmente");
    } else {
        _entrega.Assinatura.val(dados.Assinatura.Miniatura);
        _entrega.AssinarManualmente.text("Refazer Assinatura");
    }

    if (dados.AvaliacaoEntrega != null) {
        $("#liVisualizacaoAvaliacaoEntrega").show();
        PreencherObjetoKnout(_entrega, { Data: dados.AvaliacaoEntrega });
        _entrega.Questionario.perguntas(dados.AvaliacaoEntrega.Questionario);
    } else {
        $("#liVisualizacaoAvaliacaoEntrega").hide();
    }

    _entrega.ControlarTempo.val(dados.TipoOperacao.ControlarTempo);

    _entrega.DataRejeitado.visible(false);
    _entrega.ConfirmarEntrega.visible(false);
    _entrega.RejeitarEntrega.visible(false);
    _entrega.Retificar.visible(false);
    _entrega.AlterarDataEntrega.visible(false);
    _entrega.MostrarAbaNotasFiscais.val(dados.MostrarAbaNotasFiscais);
    _entrega.MostrarAbaGta.val(dados.MostrarAbaGta);
    _entrega.Retificar.visible(false);
    _entrega.OnTime.visible(false);
    _entrega.JustificativaOnTime.visible(false);

    if (ehEntrega && _configuracaoExibicaoDetalhesEntregaCliente.DataPrevisaoEntrega.val())
        _entrega.DataPrevisaoEntrega.visible(true);

    if ((!fluxoFinalizado || dados.TipoOperacao.PermitirAtualizarEntregasCargasFinalizadas) && (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente, _PermissoesPersonalizadasControleEntrega))) {
        if (dados.EnumSituacao == EnumSituacaoEntrega.Entregue && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataFim.val()) {
            _entrega.DataFim.visible(true);
            _entrega.Retificar.visible(true);
        }
        else if (dados.EnumSituacao == EnumSituacaoEntrega.Rejeitado && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataRejeitado.val()) {
            _entrega.DataRejeitado.visible(true);
            _entrega.Retificar.visible(true);

            if (dados.DadosDevolucao.PermitirEntregarMaisTarde)
                _entrega.ConfirmarEntrega.visible(true);
        }
        else {
            if (ehEntrega && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataEntregaReprogramada.val())
                _entrega.DataEntregaReprogramada.visible(true);

            if (!string.IsNullOrWhiteSpace(dados.InicioViagemRealizada))
                _entrega.ConfirmarEntrega.visible(true);

            _entrega.RejeitarEntrega.visible(true)
        }

        if (dados.DataInicioEntregaSugerida)
            _entrega.DataInicioEntregaInformada.val(dados.DataInicioEntregaSugerida);
        if (dados.DataEntregaSugerida)
            _entrega.DataEntregaInformada.val(dados.DataEntregaSugerida);

        if (dados.DataFim)
            _entrega.DataEntregaInformada.val(dados.DataFim);
        if (dados.DataInicio)
            _entrega.DataInicioEntregaInformada.val(dados.DataInicio);
    }
    else if (dados.PermitirTransportadorConfirmarRejeitarEntrega && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe) {
        _entrega.ConfirmarEntrega.visible(true);
        _entrega.RejeitarEntrega.visible(true);
    }

    if (dados.EnumSituacao == EnumSituacaoEntrega.Entregue && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataFim.val()) {
        _entrega.DataFim.visible(true);

        if (_CONFIGURACAO_TMS.ExibirOpcaoAjustarEntregaOnTime && VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarSituacaoOnTime, _PermissoesPersonalizadasControleEntrega)) {
            if (_configuracaoExibicaoDetalhesEntregaEntregaColeta.OnTime.val())
                _entrega.OnTime.visible(true);

            if (_configuracaoExibicaoDetalhesEntregaEntregaColeta.JustificativaOnTime.val())
                _entrega.JustificativaOnTime.visible(true);
        }

    }
    else if (dados.EnumSituacao == EnumSituacaoEntrega.Rejeitado && _configuracaoExibicaoDetalhesEntregaEntregaColeta.DataRejeitado.val()) {
        _entrega.DataRejeitado.visible(true);
    }

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataEntrega, _PermissoesPersonalizadasControleEntrega)) {
        _entrega.AlterarDataEntrega.visible(true);
    }

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        _entrega.AlterarDataReagendamento.visible(true);

    if (_entrega.InfoMotivoFalhaGTA.val() != "")
        _entrega.InfoMotivoFalhaGTA.visible(true);
    else
        _entrega.InfoMotivoFalhaGTA.visible(false);

    if (_entrega.InfoMotivoFalhaNotaFiscal.val() != "")
        _entrega.InfoMotivoFalhaNotaFiscal.visible(true);
    else
        _entrega.InfoMotivoFalhaNotaFiscal.visible(false);

    if (_entrega.JustificativaEntregaForaRaio.val() != "")
        _entrega.JustificativaEntregaForaRaio.visible(true);
    else
        _entrega.JustificativaEntregaForaRaio.visible(false);


    if (dados.EnumSituacao == EnumSituacaoEntrega.Rejeitado && _configuracaoExibicaoDetalhesEntregaEntregaColeta.InfoMotivoRejeicao.val()) {
        _entrega.InfoMotivoRejeicao.visible(true);

        if (dados.DadosDevolucao.PermitirEntregarMaisTarde)
            _entrega.InfoPermiteEntregarMaisTarde.visible(true);
    } else {
        _entrega.InfoMotivoRejeicao.visible(false);
        _entrega.InfoPermiteEntregarMaisTarde.visible(false);
    }

    if (dados.ChamadosEntrega != null && dados.ChamadosEntrega.Chamados.length > 1) {
        _entrega.VerDetalhesChamado.visible(false);
        _entrega.VerMultiplosChamadosCarga.visible(true);
    } else {
        _entrega.VerDetalhesChamado.visible(true);
        _entrega.VerMultiplosChamadosCarga.visible(false);
    }

    SetarTimerTempoRestanteAtendimento(dados);

    CarregarGridAnexoOcorrenciaAdicionar();
    CarregarGridOcorrencias(dados);
    CarregarInformacoesCheckList(dados);
    CarregarInformacoesCheckListDesembarque(dados);

    if (dados.ChamadosEntrega != null)
        CarregarGridChamadosEntrega(dados.ChamadosEntrega.Chamados);

    ControlarTabsEntregaCTe();
    loadInformacaoPessoas();
    loadAnexoOcorrencia();

    preencherControleEntregaDevolucaoDados(dados.DadosDevolucao, dados.CodigoChamado);
    CarregarGridSobras(dados);

    if (_entrega.ExigeConferenciaProdutos.val())
        _conferenciaProduto.CarregarConferenciaProdutos(dados);

    exibeModalEtapa('#divModalEntrega', function () {
        loadMapaEntrega(dados);
        ExibirSliceCanhotosEntrega();
    });

    if (dados.TipoOperacao.PermitirEscanearChavesNfe || (dados.TipoOperacao.PermiteAdicionarNotasControleEntrega && ehColeta)) {
        $("#liChavesNFe").show();
    } else {
        $("#liChavesNFe").hide();
    }

    if (dados.TipoOperacao.PermiteAdicionarNotasControleEntrega && ehColeta) {
        _entrega.ChaveNFe.visible(true);
        _entrega.AdicionarChaveNFe.visible(true);
        _entrega.FinalizarEnvioChavesColeta.visible(true);
        _entrega.EtapaStage.visible(true);
    } else {
        _entrega.ChaveNFe.visible(false);
        _entrega.AdicionarChaveNFe.visible(false);
        _entrega.FinalizarEnvioChavesColeta.visible(false);
        _entrega.EtapaStage.visible(false);
    }

    if (dados.TipoOperacao.SolicitarReconhecimentoFacialDoRecebedor) {
        $("#divFotoRecebedor").show();
    } else {
        $("#divFotoRecebedor").hide();
    }

    if (_draggImagem == null) {
        _draggImagem = $.draggImagem({
            container: ".container-imagem-entrega-dragg",
            image: ".imagem-entrega img",
        });
    }

    // Fronteira
    if (dados.Fronteira) {
        _entrega.Fronteira.val(true);
        _entrega.NomeRecebedor.text("Fronteira: ");
        $('#liPedidos').hide();
        $('#liFotos').hide();
        $('#liOcorrencia').hide();
        $('#liVisualizacaoCanhotos').hide();
    } else if (dados.Parqueamento) {
        _entrega.Parqueamento.val(true);
        _entrega.NomeRecebedor.text("Parqueamento: ");
        $('#liPedidos').hide();
        $('#liFotos').hide();
        $('#liOcorrencia').hide();
        $('#liVisualizacaoCanhotos').hide();
    } else {
        _entrega.Fronteira.val(false);
        _entrega.NomeRecebedor.text("Cliente: ");
        $('#liPedidos').show();
        $('#liFotos').show();
        $('#liOcorrencia').show();
        $('#liVisualizacaoCanhotos').show();
    }

    _entrega.Imprimir.visible(dados.AtivarConfirmacaoEntrega);

    setTimeout(_draggImagem.centralize, 500);

    if (dados.DataAgendamentoDeEntrega != "")
        _entrega.DataAgendamentoDeEntrega.val(dados.DataAgendamentoDeEntrega);



    _entrega.OrigemSituacaoDataAgendamentoEntrega.val(dados.OrigemSituacaoEntrega);

    if (ehColeta) {
        document.getElementById("TituloModalEntregaColeta").innerText = "Dados da Coleta";
        document.getElementById("DatasRelevantesEntregaColeta").innerText = "Datas Relevantes (Coleta)";
        _entrega.DataAgendamentoDeEntrega.text(Localization.Resources.Cargas.ControleEntrega.DataAgendamentoDeColeta.getFieldDescription());
        _entrega.StatusEntregaNota.text(Localization.Resources.Cargas.ControleEntrega.StatusColetaNota.getFieldDescription());
        _entrega.DataConfirmacao.text(Localization.Resources.Cargas.ControleEntrega.DataConfirmacaoColeta.getFieldDescription());
        _entrega.StatusTendenciaEntrega.text(Localization.Resources.Cargas.ControleEntrega.TendenciaColeta.getFieldDescription());

    } else {
        document.getElementById("TituloModalEntregaColeta").innerText = "Dados da Entrega";
        document.getElementById("DatasRelevantesEntregaColeta").innerText = "Datas Relevantes (Entrega)";
        _entrega.DataAgendamentoDeEntrega.text(Localization.Resources.Cargas.ControleEntrega.DataAgendamentoDeEntrega.getFieldDescription());
        _entrega.StatusEntregaNota.text(Localization.Resources.Cargas.ControleEntrega.StatusEntregaNota.getFieldDescription());
        _entrega.DataConfirmacao.text(Localization.Resources.Cargas.ControleEntrega.DataConfirmacaoEntrega.getFieldDescription());
        _entrega.StatusTendenciaEntrega.text(Localization.Resources.Cargas.ControleEntrega.TendenciaEntrega.getFieldDescription());
    }

    _entrega.DataAgendamentoEntregaTransportador.val(dados.DataAgendamentoEntregaTransportador);

    if (dados.AlterarDataAgendamentoEntregaTransportador)
        _entrega.AlterarDataAgendamentoEntregaTransportador.visible(dados.AlterarDataAgendamentoEntregaTransportador);

    _entrega.AlterarDataAgendamentoEntregaTransportador.val(dados.AlterarDataAgendamentoEntregaTransportador);

    if (dados.AlterarDataAgendamentoDeEntrega)
        _entrega.AlterarDataAgendamentoDeEntrega.visible(dados.AlterarDataAgendamentoDeEntrega);

    _entrega.AlterarDataAgendamentoDeEntrega.val(dados.AlterarDataAgendamentoDeEntrega);
}

function SetarTimerTempoRestanteAtendimento(dados) {
    _entrega.TempoRestanteChamado.val("");

    if (dados.DataEstimadaResolucaoChamado) {
        $("#" + _entrega.TempoRestanteChamado.id)
            .countdown(moment(dados.DataEstimadaResolucaoChamado, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
            .on('update.countdown', function (event) {
                if (event.elapsed)
                    _entrega.TempoRestanteChamado.val("Esgotado");
                else if (event.offset.totalDays > 0)
                    _entrega.TempoRestanteChamado.val(event.strftime('%-Dd %H:%M:%S'))
                else
                    _entrega.TempoRestanteChamado.val(event.strftime('%H:%M:%S'));
            });
    }
}

function CarregarGridOcorrencias(dados) {
    const opcaoReenviarEmailOcorrencia = {
        descricao: Localization.Resources.Cargas.ControleEntrega.ReenviarEmail,
        id: guid(),
        evento: "onclick",
        visibilidade: true,
        metodo: ReenviarEmailOcorrenciaClick
    };

    const opcaoCopiarLink = {
        descricao: Localization.Resources.Cargas.ControleEntrega.CopiarLinkDoAcompanhamento,
        id: guid(),
        evento: "onclick",
        visibilidade: true,
        metodo: CopiarLinkOcorrenciaClick
    };

    const opcaoVisualizarImagens = {
        descricao: Localization.Resources.Cargas.ControleEntrega.VisualizarImagens,
        id: guid(),
        evento: "onclick",
        visibilidade: true,
        metodo: VisualizarImagensClick
    };

    const opcaoAuditar = {
        descricao: Localization.Resources.Cargas.ControleEntrega.Auditar,
        id: guid(),
        evento: "onclick",
        visibilidade: true,
        metodo: AuditarOcorrenciaClick
    };

    const opcaoVisualizarNoMapa = {
        descricao: Localization.Resources.Cargas.ControleEntrega.VisualizarNoMapa,
        id: guid(),
        evento: "onclick",
        visibilidade: true,
        metodo: VisualizarOcorrenciaNoMapaClick
    };
    const ObservacaoOcorrencia = {
        descricao: Localization.Resources.Cargas.ControleEntrega.Observacao,
        id: guid(),
        evento: "onclick",
        visibilidade: true,
        metodo: ObservacaoOcorrenciaClick
    };
    const opcoes = [opcaoReenviarEmailOcorrencia, opcaoCopiarLink, opcaoAuditar, opcaoVisualizarImagens, opcaoVisualizarNoMapa, ObservacaoOcorrencia];

    if (_CONFIGURACAO_TMS.UsuarioAdministrador || VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteAlterarDataOcorrencias, _PermissoesPersonalizadasControleEntrega)) {
        const opcaoAlterarData = {
            descricao: Localization.Resources.Cargas.ControleEntrega.AlterarData,
            id: guid(),
            evento: "onclick",
            visibilidade: true,
            metodo: AlterarDataOcorrenciaClick
        };

        opcoes.push(opcaoAlterarData);
    }

    const menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: Localization.Resources.Gerais.Geral.Opcoes,
        tamanho: 7,
        opcoes: opcoes
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "LinkOcorrencia", visible: false },
        { data: "CodigoPedido", visible: false },
        { data: "CodigoOcorrencia", visible: false },
        { data: "Descricao", title: Localization.Resources.Cargas.ControleEntrega.Ocorrencia, width: "35%", className: "text-align-left" },
        { data: "DataOcorrencia", title: Localization.Resources.Cargas.ControleEntrega.Data, width: "10%", className: "text-align-left" },
        { data: "Latitude", title: Localization.Resources.Cargas.ControleEntrega.Latitude, width: "5%", className: "text-align-left" },
        { data: "Longitude", title: Localization.Resources.Cargas.ControleEntrega.Longitude, width: "5%", className: "text-align-left" },
        { data: "DataPosicao", title: Localization.Resources.Cargas.ControleEntrega.DataPosicao, width: "10%", className: "text-align-left" },
        { data: "DataReprogramada", title: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoReprogramadaDaEntrega, width: "10%", className: "text-align-left" },
        { data: "TempoPercurso", title: Localization.Resources.Cargas.ControleEntrega.TempoPercurso, width: "5%", className: "text-align-left" },
        { data: "Distancia", title: Localization.Resources.Cargas.ControleEntrega.DistanciaAteDestino, width: "5%", className: "text-align-left" },
        { data: "Pacote", title: Localization.Resources.Cargas.ControleEntrega.Pacote, width: "5%", className: "text-align-left", visible: VisibilidadePacotesOcorrencia() },
        { data: "Volumes", title: Localization.Resources.Cargas.ControleEntrega.Volumes, width: "5%", className: "text-align-right", visible: VisibilidadePacotesOcorrencia() },
        { data: "Origem", title: Localization.Resources.Cargas.ControleEntrega.Origem, width: "10%", className: "text-align-left" },
    ];

    _gridOcorrencia = new BasicDataTable(_entrega.Ocorrencias.idGrid, header, menuOpcoes);
    _gridOcorrencia.CarregarGrid(dados.Ocorrencias);
}

function CarregarGridAnexoOcorrenciaAdicionar() {
    const linhasPorPaginas = 5;
    const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerAnexoOcorrenciaClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 20, opcoes: [opcaoRemover] };
    _entrega.Upload.file = document.getElementById(_entrega.Upload.idFile);
    const header = [
        { data: "Codigo", visible: false },
        { data: "NomeArquivo", title: Localization.Resources.Gerais.Geral.Nome, width: "100%", className: "text-align-left" }
    ];

    _gridAdicionarAnexoOcorrencia = new BasicDataTable(_entrega.AnexosOcorrenciaAdicionar.idGrid, header, menuOpcoes, null, null, linhasPorPaginas);
    _gridAdicionarAnexoOcorrencia.CarregarGrid([]);
}

function CarregarGridChamadosEntrega(dados) {
    const linhasPorPaginas = 5;
    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: verDetalhesAtendimentoClick, icone: "" };
    const menuOpcoes = { tipo: TypeOptionMenu.link, descricao: Localization.Resources.Gerais.Geral.VerDetalhes, tamanho: 20, opcoes: [opcaoDetalhes] };
    const header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: "Número", width: "10%", className: "text-align-left" },
        { data: "DataCriacao", title: "Data de criação", width: "20%", className: "text-align-left" },
        { data: "MotivoChamado", title: "Motivo Atendimento", width: "50%", className: "text-align-left" },
        { data: "Situacao", title: "Situação", width: "20%", className: "text-align-left" },
    ];

    _gridChamadosEntrega = new BasicDataTable(_chamadosEntrega.ChamadosEntrega.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridChamadosEntrega.CarregarGrid(dados);
}

function verDetalhesAtendimentoClick(e) {
    AbrirChamadoPorCodigo(e.Codigo);
}

function UploadChange() {
    if (_entrega.Upload.file.files.length > 0) {
        for (let i = 0; i <= _entrega.Upload.file.files.length - 1; i++) {
            let data = { Codigo: guid(), Arquivo: _entrega.Upload.file.files[i] }
            _entrega.AnexosOcorrencia.list.push(data);
        }
    }
    RecarregarGridAnexoOcorrenciaAdicionar()
}
function RecarregarGridAnexoOcorrenciaAdicionar() {
    const dados = []
    for (let i = 0; i <= _entrega.AnexosOcorrencia.list.length - 1; i++) {
        let arquivo = _entrega.AnexosOcorrencia.list[i];
        dados.push({ Codigo: arquivo.Codigo, NomeArquivo: arquivo.Arquivo.name })
    }
    _gridAdicionarAnexoOcorrencia.CarregarGrid(dados);
}
function removerAnexoOcorrenciaClick(e) {
    _entrega.AnexosOcorrencia.list = _entrega.AnexosOcorrencia.list.filter((obj) => obj.Codigo !== e.Codigo);
    RecarregarGridAnexoOcorrenciaAdicionar();
}

function BuscarMiniaturasEExibirGridCanhotos() {
    if (_entrega.ImagensConferencia.val() != "")
        _entrega.ImagensConferencia.val.removeAll();
    iniciarRequisicao();
    _ControlarManualmenteProgresse = true;

    executarReST('ControleEntregaEntrega/ObterMiniaturasCanhotos', { Codigo: _entrega.Codigo.val() }, function (arg) {
        if (arg.Success) {
            const imagensConferencia = [];
            for (let i = 0; i < arg.Data.Imagens.length; i++) {
                const obj = arg.Data.Imagens[i];
                imagensConferencia.push(obj);
            }

            _entrega.ImagensConferencia.val(imagensConferencia);
            if (_entrega.Coleta.val()) {
                _entrega.EnviarImagem.visible(false);
            } else {
                _entrega.EnviarImagem.visible(true);
            }
            setTimeout(_draggCanhoto.centralize, 500);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        _ControlarManualmenteProgresse = false;
        finalizarRequisicao();
    });
}

function alterarDataAgendamentoEntregaClick() {
    AbrirModalAlterarDataAgendamentoEntrega();
}
function AlterarDataPrevisaoEntregaAjustadaClick() {
    AbrirModalAlterarDataPrevisaoEntregaAjustada();
}

function alterarDataAgendamentoEntregaTransportadorClick() {
    AbrirModalAlterarDataAgendamentoEntregaTransportador();
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function exibeTelefoneVendedor() {
    const verifica = _entrega.VendedorDados.Telefone() != "";
    if (verifica) {
        if (_entrega.Vendedor.visibilidadeTelefone()) {
            _entrega.Vendedor.visibilidadeTelefone(false);
        } else {
            _entrega.Vendedor.visibilidadeTelefone(verifica);
        }
    }
}

function exibeEmailVendedor() {
    const verifica = _entrega.VendedorDados.Email() != "";
    if (verifica) {
        if (_entrega.Vendedor.visibilidadeEmail()) {
            _entrega.Vendedor.visibilidadeEmail(false);
        } else {
            _entrega.Vendedor.visibilidadeEmail(verifica);
        }
    }
}

function abrirWhatsVendedor() {
    const numeroTelefone = this.VendedorDados.Telefone();
    const nome = this.VendedorDados.Nome();

    return 'https://api.whatsapp.com/send?phone=55' + numeroTelefone + '&text=Ol%C3%A1%20' + nome;
}

function exibeTelefoneSupervisor() {
    const verifica = _entrega.SupervisorDados.Telefone() != "" && _entrega.SupervisorDados.Telefone() != "0";
    if (verifica) {
        if (_entrega.Supervisor.visibilidadeTelefone()) {
            _entrega.Supervisor.visibilidadeTelefone(false);
        } else {
            _entrega.Supervisor.visibilidadeTelefone(verifica);
        }
    }
}

function exibeEmailSupervisor() {
    const verifica = _entrega.SupervisorDados.Email() != "" && _entrega.SupervisorDados.Email() != null;
    if (verifica) {
        if (_entrega.Supervisor.visibilidadeEmail()) {
            _entrega.Supervisor.visibilidadeEmail(false);
        } else {
            _entrega.Supervisor.visibilidadeEmail(verifica);
        }
    }
}

function abrirWhatsSupervisor() {
    const numeroTelefone = this.SupervisorDados.Telefone();
    const nome = this.SupervisorDados.Nome();

    return 'https://api.whatsapp.com/send?phone=55' + numeroTelefone + '&text=Ol%C3%A1%20' + nome;
}

function exibeTelefoneGerente() {
    const verifica = _entrega.GerenteDados.Telefone() != "" && _entrega.GerenteDados.Telefone() != "0";
    if (verifica) {
        if (_entrega.Gerente.visibilidadeTelefone()) {
            _entrega.Gerente.visibilidadeTelefone(false);
        } else {
            _entrega.Gerente.visibilidadeTelefone(verifica);
        }
    }
}

function exibeEmailGerente() {
    const verifica = _entrega.GerenteDados.Email() != "";
    if (verifica) {
        if (_entrega.Gerente.visibilidadeEmail()) {
            _entrega.Gerente.visibilidadeEmail(false);
        } else {
            _entrega.Gerente.visibilidadeEmail(verifica);
        }
    }
}

function abrirWhatsGerente() {
    const numeroTelefone = this.GerenteDados.Telefone();
    const nome = this.GerenteDados.Nome();

    return 'https://api.whatsapp.com/send?phone=55' + numeroTelefone + '&text=Ol%C3%A1%20' + nome;
}

function exibeTelefoneGerenteRegional() {
    const verifica = _entrega.GerenteRegionalDados.Telefone() != "" && _entrega.GerenteRegionalDados.Telefone() != "0";
    if (verifica) {
        if (_entrega.GerenteRegional.visibilidadeTelefone()) {
            _entrega.GerenteRegional.visibilidadeTelefone(false);
        } else {
            _entrega.GerenteRegional.visibilidadeTelefone(verifica);
        }
    }
}

function exibeEmailGerenteRegional() {
    const verifica = _entrega.GerenteRegionalDados.Email() != "";
    if (verifica) {
        if (_entrega.GerenteRegional.visibilidadeEmail()) {
            _entrega.GerenteRegional.visibilidadeEmail(false);
        } else {
            _entrega.GerenteRegional.visibilidadeEmail(verifica);
        }
    }
}

function abrirWhatsGerenteRegional() {
    const numeroTelefone = this.GerenteRegionalDados.Telefone();
    const nome = this.GerenteRegionalDados.Nome();

    return 'https://api.whatsapp.com/send?phone=55' + numeroTelefone + '&text=Ol%C3%A1%20' + nome;
}

function exibeTelefoneGerenteNacional() {
    const verifica = _entrega.GerenteNacionalDados.Telefone() != "" && _entrega.GerenteNacionalDados.Telefone() != "0";
    if (verifica) {
        if (_entrega.GerenteNacional.visibilidadeTelefone()) {
            _entrega.GerenteNacional.visibilidadeTelefone(false);
        } else {
            _entrega.GerenteNacional.visibilidadeTelefone(verifica);
        }
    }
}

function exibeEmailGerenteNacional() {
    const verifica = _entrega.GerenteNacionalDados.Email() != "";
    if (verifica) {
        if (_entrega.GerenteNacional.visibilidadeEmail()) {
            _entrega.GerenteNacional.visibilidadeEmail(false);
        } else {
            _entrega.GerenteNacional.visibilidadeEmail(verifica);
        }
    }
}

function abrirWhatsGerenteNacional() {
    const numeroTelefone = this.GerenteNacionalDados.Telefone();
    const nome = this.GerenteNacionalDados.Nome();

    return 'https://api.whatsapp.com/send?phone=55' + numeroTelefone + '&text=Ol%C3%A1%20' + nome;
}

function salvarObservacaoClick() {
    executarReST("ControleEntregaEntrega/SalvarObservacao", { Codigo: _entrega.Codigo.val(), Observacao: _entrega.Observacao.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.ObservacaoSalvaComSucesso);
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
    return true;
}

function retificarClick(e) {
    if (!ValidarAcaoMultiCTe()) return;

    ObterMotivosRetificacao(_entrega.Coleta.val() ? EnumTipoAplicacaoColetaEntrega.Coleta : EnumTipoAplicacaoColetaEntrega.Entrega);
}

function removerReentregaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.RemoverReentrega, Localization.Resources.Cargas.ControleEntrega.DesejaRemoverReentrega, function () {
        executarReST("ControleEntregaEntrega/RemoverReentrega", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                atualizarControleEntrega();
                Global.fecharModal("divModalEntrega");
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function removerEntregaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.RemoverEntrega, Localization.Resources.Cargas.ControleEntrega.DesejaRemoverEntrega, function () {
        executarReST("ControleEntregaEntrega/RemoverEntrega", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.ReentregaRemovidaComSucesso);
                    atualizarControleEntrega();
                    Global.fecharModal("divModalEntrega");
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function atualizarCoordenadasClienteClick(e) {
    const msg = Localization.Resources.Cargas.ControleEntrega.DesejaAtualizarAsCoordenadasDoCadastroDePessoaComPosiçãoDoVeiculo;

    if (!ValidarAcaoMultiCTe()) return;

    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.AtualizarCoordenada, msg, function () {
        executarReST("ControleEntregaEntrega/AtualizarCoordenadas", { Codigo: e.Codigo.val() }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.CoordenadasAtualizadasComSucesso);
                    atualizarControleEntrega();


                    _dadosMapa.LocalCliente.Latitude = _dadosMapa.LocalEntrega.Latitude;
                    _dadosMapa.LocalCliente.Longitude = _dadosMapa.LocalEntrega.Longitude;


                    loadMapaEntrega(_dadosMapa);

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function atualizarCoordenadasClienteDragClick() {
    const msg = Localization.Resources.Cargas.ControleEntrega.DesejaAtualizarAsCoordenadasDoCadastroDePessoaComNovaPosicaoDoMarcadorNoMapa;

    if (!ValidarAcaoMultiCTe()) return;

    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.AtualizarCoordenada, msg, function () {
        executarReST("Pessoa/AtualizarCoordenadas", { CPF_CNPJ: _entrega.CpfCnpjCliente.val(), Latitude: _entrega.AtualizarCoordenadasClienteDrag.Latitude, Longitude: _entrega.AtualizarCoordenadasClienteDrag.Longitude }, function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.CoordenadasAtualizadasComSucesso);
                    atualizarControleEntrega();

                    _dadosMapa.LocalCliente.Latitude = _entrega.AtualizarCoordenadasClienteDrag.Latitude;
                    _dadosMapa.LocalCliente.Longitude = _entrega.AtualizarCoordenadasClienteDrag.Longitude;

                    loadMapaEntrega(_dadosMapa);

                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function atualizarProdutosClick(e) {
    atualizarListaPedidoProduto(e);
}

function atualizarDadosRecebedorClick(e) {
    atualizarDadosRecebedor(e);
}

function fecharEntregaClick(e) {
    Global.fecharModal("divModalEntrega");
}

function alterarDataEntregaClick(e) {
    if (string.IsNullOrWhiteSpace(e.DataInicioEntregaInformada.val()) && string.IsNullOrWhiteSpace(e.DataEntregaInformada.val())) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.ControleEntrega.PreenchaAoMenosUmaDasDatas);
        return;
    }

    executarReST("ControleEntregaEntrega/AlterarDatasEntrega", { Codigo: e.Codigo.val(), DataInicioEntregaInformada: e.DataInicioEntregaInformada.val(), DataEntregaInformada: e.DataEntregaInformada.val() },
        function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.DataAlteradaComSucesso);
                    atualizarControleEntrega();
                    Global.fecharModal("divModalEntrega");
                } else {
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Atencao, arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
}


function confirmarEntregaClick(e) {
    if (!ValidarCamposObrigatorios(_entrega)) {
        exibirMensagemCamposObrigatorio();
        return;
    }

    if (_entrega.ExigeConferenciaProdutos.val())
        realizarConferencia();
    else
        confirmacaoEntrega(e);
}

function confirmacaoEntrega(e) {
    if (!ValidarAcaoMultiCTe()) return;

    if (_entrega.ClientePossuiAreaRedex.val()) {
        abrirModalAreaRedex(_entrega, e);
    } else if (_entrega.ColetaDeContainer.val()) {
        abrirModalColetaDeContainer(_entrega, e);
    } else {
        confirmarEntrega(e, undefined, undefined, undefined);
    }
}

function confirmarEntrega(e, areaRedex, ColetaContainer, dataColetaContainer) {
    let msg = Localization.Resources.Cargas.ControleEntrega.RealmenteDesejaConfirmarEntrega;
    if (_entrega.Coleta.val())
        msg = Localization.Resources.Cargas.ControleEntrega.RealmenteDesejaConfirmarColeta;

    let AreaRedex = 0;
    if (areaRedex != undefined) {
        AreaRedex = areaRedex.AreaRedex.val();
    }

    let container = 0;
    let DataColetaContainer = "";
    if (ColetaContainer != undefined) {
        container = ColetaContainer;
        DataColetaContainer = dataColetaContainer
    }

    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.ConfirmacaoEntrega, msg, function () {
        executarReST("ControleEntregaEntrega/ConfirmarEntrega", { Codigo: e.Codigo.val(), DataInicioEntregaInformada: e.DataInicioEntregaInformada.val(), DataEntregaInformada: e.DataEntregaInformada.val(), MotivoRetificacao: _entrega.MotivoRetificacao.val(), Observacao: _entrega.Observacao.val(), AreaRedex: AreaRedex, ColetaContainer: container, DataColetaContainer: DataColetaContainer },
            function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.ConfirmadoComSucesso);
                        atualizarControleEntrega();
                        Global.fecharModal("divModalEntrega");
                    } else {
                        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
                }
            }, null);
    });
}

function removerAnexoClick(e) {
    executarReST("ControleEntregaEntrega/ExcluirAnexo", { Codigo: e.Codigo }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

        _entrega.Imagens.val(arg.Data);
        setTimeout(_draggImagem.centralize, 500);
        exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Imagem Adicionada com sucesso");
    });
}

function downloadImagemEntregaClick(e) {
    executarDownload("ControleEntregaEntrega/DownloadImagem", { Codigo: e.Codigo });
}

function downloadImagemNotaFiscalClick(e) {
    executarDownload("ControleEntregaEntrega/DownloadImagemNotaFiscal", { Codigo: e.Codigo });
}

function baixarComprovanteColetaClick(e) {
    executarDownload("ControleEntregaEntrega/BaixarComprovanteColeta", { Codigo: e.Codigo.val() });
}

function alterarOnTimeClick(e) {
    const data = {
        Codigo: e.Codigo.val(),
        SituacaoOnTime: e.OnTime.val(),
        Justificativa: e.JustificativaOnTime.val()
    };

    executarReST("ControleEntregaEntrega/AlterarSituacaoOnTime", data, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                atualizarControleEntrega();
                Global.fecharModal("divModalEntrega");
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, null);
}

function VisualizarNoMapaImagemEntregaClick(e) {
    Global.abrirModal("divModalLocalImagem");
    const coordenada = { lat: e.Latitude, lng: e.Longitude };
    criarMakerLocalImagem(coordenada);
    //executarDownload("ControleEntregaEntrega/DownloadImagem", { Codigo: e.Codigo });
}

function rejeitarEntregaClick(e) {
    _controleEntregaDevolucaoDados.ExibirDadosDevolucao.val(true);
    $("#li-controle-entrega-devolucao a").tab("show");
}

function verDetalhesChamadoClick(e) {
    AbrirChamadoPorCodigo(e.CodigoChamado.val());
}

function verChamadosCargaEntregaClick(e) {
    Global.abrirModal("divModalVisualizarAtendimentosCarga");
}

function NovoAtendimentoClick() {
    const Data = { codigo: _detalhesCarga.CodigoCarga.val(), carga: _detalhesCarga.CargaEmbarcador.val(), codigoCargaEntrega: _entrega.Codigo.val(), destinatario: _entrega.CpfCnpjCliente.val(), }
    AbrirNovoAtendimentoClick(Data);
}

function vincularCanhotoEsperandoVinculoClick(e) {
    if (!_canhotoParaVinculo) {
        _canhotoParaVinculo = new CanhotoParaVinculo();

        _buscaCanhotoParaVinculo = new BuscarCanhotos(_canhotoParaVinculo.Canhoto, function (canhotoSelecionado) {
            exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.ControleEntrega.DesejaRealmenteVincularCanhoto.format(canhotoSelecionado.Numero), function () {
                const dados = {
                    CanhotoEsperandoVinculo: e.Codigo,
                    CanhotoVincular: canhotoSelecionado.Codigo
                };

                executarReST("ControleEntregaEntrega/VincularCanhotoEsperandoVinculo", dados, function (retorno) {
                    if (retorno.Success) {
                        if (retorno.Data) {
                            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.CanhotoVinculadoComSucesso);
                            BuscarMiniaturasEExibirGridCanhotos();
                        }
                        else
                            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
                    }
                    else
                        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
                });
            });
        }, _canhotoParaVinculo.Entrega);
    }

    _canhotoParaVinculo.Entrega.val(_entrega.Codigo.val());
    _buscaCanhotoParaVinculo.abrirBusca();
}

function downloadCanhotoClick(e) {
    if (e.Codigo > 0 && e.GuidNomeArquivo != "") {
        const dados = {
            Codigo: e.Codigo,
            EsperandoVinculo: e.EsperandoVinculo,
        }
        executarDownload("Canhoto/DownloadCanhoto", dados);
    } else {
        exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.ControleEmntrega.CanhotoNaoEnviado, Localization.Resources.Cargas.ControleEntrega.NaoFoiEnviadoCanhotoParaEstaNota);
    }
}

function enviarImagemCanhotoClick(e) {
    var file = document.getElementById("Canhoto" + e.Numero);
    var formData = new FormData();

    for (var i = 0; i < file.files.length; i++)
        formData.append("upload", file.files[i]);

    var data = {
        Codigo: e.Codigo
    };

    enviarArquivo("Canhoto/EnviarImagemCanhoto?callback=?", data, formData, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.CanhotoEnviadoComSucesso);
                file.value = null;
                BuscarMiniaturasEExibirGridCanhotos();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg, 20000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}

function DownloadMassaClick() {
    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.DownloadMassa, Localization.Resources.Cargas.ControleEntrega.TemCertezaQueDesejaBaixarEmMassa, function () {
        var listaCodigosCanhotos = [];
        var listaImagens = _entrega.ImagensConferencia.val();

        for (i = 0; i < listaImagens.length; i++) {
            listaCodigosCanhotos.push(listaImagens[i].Codigo);
        }

        executarDownload("ControleEntregaEntrega/DownloadCanhotosEmMassa", { CodigosCanhotos: JSON.stringify(listaCodigosCanhotos) });
    });
}


function ReenviarEmailOcorrenciaClick(registroSelecionado) {
    executarReST('ControleEntregaEntrega/ReenviarEmail', { CodigoPedido: registroSelecionado.CodigoPedido, CodigoOcorrencia: registroSelecionado.CodigoOcorrencia }, function (arg) {
        if (arg.Success) {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Cargas.ControleEntrega.EmBreveEmailSeraEnviado);
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function CopiarLinkOcorrenciaClick(registroSelecionado) {
    Global.copyTextToClipboard(registroSelecionado.LinkOcorrencia);
}

function VisualizarImagensClick(registroSelecionado) {
    exibirAnexoOcorrencias(registroSelecionado);
}
function ObservacaoOcorrenciaClick(registroSelecionado) {
    Global.abrirModal("ObservacaoOcorrencias");
    document.getElementById("ObservacaoOcorrenciaId").value = registroSelecionado.ObservarcaoOcorrencia;
}

function VisualizarOcorrenciaNoMapaClick(registroSelecionado) {
    const coordenada = { lat: registroSelecionado.Latitude, lng: registroSelecionado.Longitude };

    if (registroSelecionado.Latitude == null && registroSelecionado.Longitude == null) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Cargas.ControleEntrega.OcorrenciaNaoPossuiLatitudeLongitude);
        return;
    }

    Global.abrirModal("divModalLocalOcorrencia");
    criarMakerLocaOcorrencia(coordenada);
}

function AuditarOcorrenciaClick(registroSelecionado) {
    const data = { Codigo: registroSelecionado.Codigo };
    const closureAuditoria = OpcaoAuditoria("OcorrenciaColetaEntrega", null, registroSelecionado);

    closureAuditoria(data);
}

function AlterarDataOcorrenciaClick(registroSelecionado) {
    abrirModalAlterarDataOcorrencia(registroSelecionado);
}

function abrirModalSelecionarAreaRedex() {
    //buscar areas redex cliente;

    Global.abrirModal("divModalSelecionarAreaRedex");

}

async function onClickDetalharComprovanteEntrega(comprovante) {
    VisualizarClick(comprovante);
    Global.abrirModal("divModalLoteComprovanteEntrega");
}

function removerPedidoReentregaClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Cargas.ControleEntrega.RemoverReentrega, Localization.Resources.Cargas.ControleEntrega.DesejaRemoverReentrega, function () {
        executarReST("ControleEntregaEntrega/RemoverPedidoReentrega", { CodigoCargaEntrega: e.CodigoCargaEntrega, CodigoCargaEntregaPedido: e.CodigoCargaEntregaPedido, CodigoPedidoReentrega: e.Codigo }, function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
                atualizarControleEntrega();
                Global.fecharModal("divModalEntrega");
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

function removerChaveNFeClick(registroSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a chave da NF-e dos pedidos da coleta?", function () {
        executarReST("ControleEntrega/RemoverChaveNFe", { Codigo: registroSelecionado.Codigo }, function (arg) {
            if (arg.Success) {
                if (arg.Data)
                    _gridChavesNFe.CarregarGrid();
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Sugestao, arg.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }, null);
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function centralizarPontos() {
    setTimeout(function () { _mapEntrega.draw.centerShapes(); }, 500);
}

// #endregion Funções Públicas

// #region Funções Privadas

function ControlarTabsEntregaCTe() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiCTe) {
        $("#tab-controle-entrega-produto footer").hide();
        $("#tab-controle-entrega-local-entrega footer").hide();
        /*        $("#controle-entrega-ocorrencia-adicionar").hide();*/
        $("#tab-controle-entrega-pesquisa footer").hide();
        $("#footer-controle-entrega-principal").show();
    }
}

function ativarAbaDetalhe() {
    let tabControleEntregaDetalhes = new bootstrap.Tab(document.querySelector('a[href="#tab-controle-entrega-detalhes"]'));
    tabControleEntregaDetalhes.show();
}

function carregarMapaEntrega() {
    if (_mapEntrega == null) {

        opcoesMapa = new OpcoesMapa(false, false);

        _mapEntrega = new MapaGoogle(_entrega.LocalEntrega.id, false, opcoesMapa);
    }

    _mapEntrega.clear();
}

function abrirModalLocalizacaoEntradaRaio() {
    _mapaPosicao = new MapaGoogle("mapPosicao", false, new OpcoesMapa(false, false));
    _mapaPosicao.direction.setZoom(17);
    _mapaPosicao.direction.centralizar(_dadosDetalhesEntrega.LocalConfirmacaoChegada.Latitude, _dadosDetalhesEntrega.LocalConfirmacaoChegada.Longitude);

    const markerCliente = new ShapeMarker();
    markerCliente.setPosition(_dadosDetalhesEntrega.LocalCliente.Latitude, _dadosDetalhesEntrega.LocalCliente.Longitude);
    markerCliente.icon = _mapaPosicao.draw.icons.PinChegada();

    const markerConfirmacaoChegada = new ShapeMarker();
    markerConfirmacaoChegada.setPosition(_dadosDetalhesEntrega.LocalConfirmacaoChegada.Latitude, _dadosDetalhesEntrega.LocalConfirmacaoChegada.Longitude);
    markerConfirmacaoChegada.icon = _mapaPosicao.draw.icons.PinLocal();

    _mapaPosicao.draw.addShape(markerCliente);
    _mapaPosicao.draw.addShape(markerConfirmacaoChegada);

    Global.abrirModal('divModalMapaChegada');
}


function carregarMapaLocalImagem() {
    if (_mapImagem == null) {

        opcoesMapa = new OpcoesMapa(false, false);

        _mapImagem = new MapaGoogle("mapaLocalImagem", false, opcoesMapa);
    }

    _mapImagem.clear();
}

function carregarMapaLocalOcorrencia() {
    if (_mapOcorrencia == null) {

        opcoesMapa = new OpcoesMapa(false, false);

        _mapOcorrencia = new MapaGoogle("mapaLocalOcorrencia", false, opcoesMapa);
    }

    _mapOcorrencia.clear();
}

function criarMakerCliente(coordenada) {
    const marker = new ShapeMarker();
    marker.setPosition(coordenada.lat, coordenada.lng);
    marker.title = '';

    if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.CargaEntrega_PermiteAtualizarCoordenadasDoCliente, _PermissoesPersonalizadasControleEntrega)) {
        marker.draggable = true;
        marker.setOnDragListener((marker, map, newshape) => {
            _entrega.AtualizarCoordenadasClienteDrag.Latitude = marker.latLng.lat();;
            _entrega.AtualizarCoordenadasClienteDrag.Longitude = marker.latLng.lng();

            const infowindow = new google.maps.InfoWindow({
                content: '<input type="button" value="' + Localization.Resources.Cargas.ControleEntrega.AtualizarPosicao + '" onclick="atualizarCoordenadasClienteDragClick()" class="btn btn-labeled btn-success" style="padding: 5px 22px; margin: 5px 10px" />',
            });

            infowindow.open(map, newshape);
        });
    }

    _mapEntrega.draw.addShape(marker);
}

function criarMakerLocalImagem(coordenada) {
    _mapImagem.clear();
    const marker = new ShapeMarker();
    marker.setPosition(coordenada.lat, coordenada.lng);
    marker.title = '';
    _mapImagem.draw.addShape(marker);
    setTimeout(function () {
        _mapImagem.draw.centerShapes();
        _mapImagem.direction.setZoom(15);
    }, 500);

}

function criarMakerEntrega(coordenadaEntrega, coordenadaDescarga) {
    if ((typeof coordenadaEntrega.lat) == "string")
        coordenadaEntrega.lat = Globalize.parseFloat(coordenadaEntrega.lat);

    if ((typeof coordenadaEntrega.lng) == "string")
        coordenadaEntrega.lng = Globalize.parseFloat(coordenadaEntrega.lng);

    if ((typeof coordenadaDescarga.lat) == "string")
        coordenadaDescarga.lat = Globalize.parseFloat(coordenadaDescarga.lat);

    if ((typeof coordenadaDescarga.lng) == "string")
        coordenadaDescarga.lng = Globalize.parseFloat(coordenadaDescarga.lng);

    if ((coordenadaEntrega.lat != 0) || (coordenadaEntrega.lng != 0)) {
        const markerEntrega = new ShapeMarker();
        markerEntrega.setPosition(coordenadaEntrega.lat, coordenadaEntrega.lng);
        markerEntrega.icon = _mapEntrega.draw.icons.truck();
        markerEntrega.title = '';
        _mapEntrega.draw.addShape(markerEntrega);
    }

    if ((coordenadaDescarga.lat != 0) || (coordenadaDescarga.lng != 0)) {
        const markerDescarga = new ShapeMarker();
        markerDescarga.setPosition(coordenadaDescarga.lat, coordenadaDescarga.lng);
        markerDescarga.icon = '../img/montagem-carga-mapa/markers/truck-unload.png';
        markerDescarga.title = '';
        _mapEntrega.draw.addShape(markerDescarga);
    }
}

function criarMakerLocaOcorrencia(coordenada) {
    _mapOcorrencia.clear();
    const marker = new ShapeMarker();
    marker.setPosition(coordenada.lat, coordenada.lng);
    marker.title = '';
    _mapOcorrencia.draw.addShape(marker);
    setTimeout(function () {
        _mapOcorrencia.draw.centerShapes();
        _mapOcorrencia.direction.setZoom(15);
    }, 500);

}

function desenharAreaCliente(coordenada, raio) {
    if (raio == 0)
        return;

    const latLngNormal = { lat: parseFloat(coordenada.lat), lng: parseFloat(coordenada.lng) };
    const shapeCircle = new ShapeCircle();
    shapeCircle.type = google.maps.drawing.OverlayType.CIRCLE;
    shapeCircle.fillColor = "#1E90FF";
    shapeCircle.radius = parseInt(raio);
    shapeCircle.center = latLngNormal

    _mapEntrega.draw.addShape(shapeCircle);

}

function enviarImagem() {
    const data = obterDataImagem();

    if (data) {
        enviarArquivo("ControleEntregaEntrega/enviarImagem?callback=?", { Codigo: _entrega.Codigo.val() }, data, function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    _entrega.Imagens.val(retorno.Data);
                    setTimeout(_draggImagem.centralize, 500);
                }
                else
                    exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

        });
    }
}

function expandirImagem(codigo) {
    window.open("ControleEntregaEntrega/ExibirAnexo?Codigo=" + codigo);
}

function limparCamposDetalhesEntrega() {
    LimparCampos(_entrega);
    if (_entrega.Questionario != null)
        _entrega.Questionario.perguntas([]);

    if (_entrega.ListaPedidoProduto != null)
        limparListaPedidoProduto();

    $("#knockoutEntrega input[name='avalicacao']:checked").prop('checked', false);
}

function SetarIconesLegendaMapa() {
    _entrega.Legenda.LocalCliente("http://maps.gstatic.com/mapfiles/api-3/images/spotlight-poi2.png");
    _entrega.Legenda.LocalFinalizacao(_mapEntrega.draw.icons.truck());
    _entrega.Legenda.LocalDescarga('../img/montagem-carga-mapa/markers/truck-unload.png');
}

function loadMapaEntrega(dados) {
    ativarAbaDetalhe();

    carregarMapaEntrega();
    carregarMapaLocalImagem();
    carregarMapaLocalOcorrencia();

    _dadosMapa = dados;

    const posicaoEntrega = { lat: dados.LocalEntrega.Latitude, lng: dados.LocalEntrega.Longitude, };
    const posicaoDescarga = { lat: dados.LocalDescarga.Latitude, lng: dados.LocalDescarga.Longitude, };
    const posicaoCliente = { lat: dados.LocalCliente.Latitude, lng: dados.LocalCliente.Longitude, };

    criarMakerCliente(posicaoCliente);

    desenharAreaCliente(posicaoCliente, dados.LocalCliente.Raio);

    criarMakerEntrega(posicaoEntrega, posicaoDescarga);

    _mapEntrega.direction.setZoom(17);
    _mapImagem.direction.setZoom(17);
    _mapOcorrencia.direction.setZoom(17);

    SetarIconesLegendaMapa();
}

function obterDataImagem() {
    const arquivo = document.getElementById(_entrega.Arquivo.id);

    if (arquivo.files.length == 1) {
        const formData = new FormData();

        formData.append("Arquivo", arquivo.files[0]);
        formData.append("Descricao", "");

        return formData;
    }

    return undefined;
}

function setarTituloModalEntrega(numero) {
    if (_entrega.Numero != null)
        $(".modal-entrega-title").html("CPF/CNPJ Número " + _entrega.Numero.val());
    else
        $(".modal-entrega-title").html("CPF/CNPJ Número " + numero);

    const ehColeta = _entrega.Coleta.val();
    const ehFronteira = _entrega.Fronteira.val();
    const ehParquamento = _entrega.Parqueamento.val();

    if (ehFronteira || ehParquamento)
        _entrega.DataPrevisaoEntrega.text("Previsão de chegada:");
    if (ehColeta) {
        _entrega.DataPrevisaoEntrega.text(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaColeta.getFieldDescription());
        _entrega.DataEntregaReprogramada.text(Localization.Resources.Cargas.ControleEntrega.DataDaColetaReprogramada.getFieldDescription());
        _entrega.DataPrevisaoEntregaAjustada.text(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoColetaAjustada.getFieldDescription());
        _entrega.DataFim.text(Localization.Resources.Cargas.ControleEntrega.DataTerminoDaColeta.getFieldDescription());
        _entrega.DataInicio.text(Localization.Resources.Cargas.ControleEntrega.DataInicioDaColeta.getFieldDescription());
    } else {
        _entrega.DataPrevisaoEntrega.text(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoDaEntrega.getFieldDescription());
        _entrega.DataEntregaReprogramada.text(Localization.Resources.Cargas.ControleEntrega.DataDaEntregaReprogramada.getFieldDescription());
        _entrega.DataPrevisaoEntregaAjustada.text(Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaAjustada.getFieldDescription());
        _entrega.DataFim.text(Localization.Resources.Cargas.ControleEntrega.DataTerminoDaEntrega.getFieldDescription());
        _entrega.DataInicio.text(Localization.Resources.Cargas.ControleEntrega.DataInicioEntrega.getFieldDescription());
    }
}

function ControlarExibicaoGridCanhotos() {
    if (callbackCanhotosCarregados != null) {
        callbackCanhotosCarregados();
        callbackCanhotosCarregados = null;
    }
}

function ExibirSliceCanhotosEntrega() {
    const handle = function () {
        BuscarMiniaturasEExibirGridCanhotos();
    }

    if ($tabVisualizacaoCanhotos == null) {
        $tabVisualizacaoCanhotos = $("#liVisualizacaoCanhotos");
    }

    if (!$tabVisualizacaoCanhotos.hasClass('active'))
        callbackCanhotosCarregados = handle;
    else
        handle();
}

function ValidarAcaoMultiCTe() {
    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe && !_entrega.PermitirTransportadorConfirmarRejeitarEntrega.val()) {
        exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, Localization.Resources.Gerais.Geral.VoceNaoPossuiPermissaoParaExecutarEstaAcao);
        return false;
    }

    return true;
}

function VisibilidadePacotesOcorrencia() {
    return _CONFIGURACAO_TMS.ExibirPacotesOcorrencia;
}

function VisibilidadeRemoverReentrega(data) {
    return data.Reentrega == "Sim" && _dadosDetalhesEntrega.PermitirRemoverReentregaAdicionadaAEntrega;
}

function VisibilidadeRemoverChaveNFe() {
    return _dadosDetalhesEntrega.TipoOperacao.PermiteAdicionarNotasControleEntrega;
}

function visibilidadeAbasControleEntrega(data) {
    Global.ResetarAba("tab-controle-entrega-gta");
    if (data.ExigirFotoGTA)
        $("#liTabFotoGta").show();
    else
        $("#liTabFotoGta").hide();

    /*Verifica valor da flag 'Obrigar foto do canhoto' nas configurações de 'Tipo de Operação'*/
    if (data.ObrigarFotoCanhoto) {
        $("#liVisualizacaoCanhotos").removeClass("hidden");
        return;
    } else {
        $("#liVisualizacaoCanhotos").addClass("hidden");
    }

    /*Tipo Coleta */
    if (data.Coleta && data.ObrigarFotoCanhoto) {
        $("#liVisualizacaoCanhotos").addClass("hidden");
        $("#liNotaFiscal").removeClass("hidden");
        $("#liGTA").removeClass("hidden");
    }
    /*Tipo Entrega */
    if (!data.Coleta && data.ObrigarFotoCanhoto) {
        $("#liVisualizacaoCanhotos").removeClass("hidden");
        $("#liNotaFiscal").addClass("hidden");
        $("#liGTA").addClass("hidden");
    }

    /*Notas transferência */
    if (data.MostrarAbaNFTransferenciaDevolucaoPallet) {
        $("#liDevolucaoPallet").show();
    }
}

function assinarManualmenteClick() {
    exibirModalAssinatura();
}

// #endregion Funções Privadas

function carregarImagemNaTela() {
    let arquivo = document.getElementById(_entrega.DadosRecebedorArquivoAssinatura.id);

    if (arquivo.files.length > 0) {

        const fileReader = new FileReader();
        fileReader.readAsDataURL(arquivo.files[0]);

        fileReader.onloadend = function () {
            if (fileReader.result != null && fileReader.result != undefined && fileReader.result != "") {
                let fileBase64 = fileReader.result;
                _entrega.Assinatura.val(fileBase64.replace("data:image/png;base64,", ""));
            } else {
                _entrega.Assinatura.val("");
            }
        }
    }
}
function loadCamera() {
    const webcamElement = document.getElementById('webcam');
    const canvasElement = document.getElementById('canvas');
    _camera = new Webcam(webcamElement, 'user', canvasElement);
}

async function TirarFotoClick() {
    let imagemBase64 = _camera.snap();

    const data = await ObterFormDataCamerar(imagemBase64);

    if (data == null)
        return;

    enviarArquivo("ControleEntregaEntrega/enviarImagem?callback=?", { Codigo: _entrega.Codigo.val() }, data, function (retorno) {
        if (!retorno.Success)
            return exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);

        if (!retorno.Data)
            return exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.NaoFoiPossivelAnexarArquivo, retorno.Msg);

        _entrega.Imagens.val(retorno.Data);
        setTimeout(_draggImagem.centralize, 500);
        exibirMensagem(tipoMensagem.ok, "Sucesso", "Imagem Carregada");
    });

    _camera.stop();
    $("#CardCamera").hide();
    _entrega.TiraFoto.visible(false);
    _entrega.AbrirCamera.visible(true);
}

function AbrirCamaraClick() {
    _camera.start().then(result => {

    }).catch(err => {
        exibirMensagem(tipoMensagem.falha, "Error", "Não foi possivel Abrir a Camera");
    });
    _entrega.TiraFoto.visible(true);
    _entrega.AbrirCamera.visible(false);
    $("#CardCamera").show();
}

async function ObterFormDataCamerar(img) {
    if (img == null)
        return null;

    const formData = new FormData();
    formData.append("Arquivo", await ConvertBase64ToFile(img));
    formData.append("Descricao", "");
    return formData
}

async function ConvertBase64ToFile(imgBase64) {
    const regex = new RegExp(/^data:(.+);base64/);
    const [, typeImage] = imgBase64.match(regex);
    const res = await fetch(imgBase64);
    const blob = await res.blob();

    let [, extension] = typeImage.split("/");
    return new File([blob], `${guid()}.${extension}`, { type: typeImage });
}

function FecharModalCamera() {
    _camera.stop();
    $("#CardCamera").hide();
    _entrega.TiraFoto.visible(false);
    _entrega.AbrirCamera.visible(true);
}

function imprimirClick(e) {
    executarDownload("ControleEntregaEntrega/BaixarRecebimentoProduto", { Codigo: _entrega.Codigo.val() });
}


//#region Parametos de Calculo
var _parametroDeCalculo;

function ParametosDeCalculo() {
    this.DataBaseConsiderada = PropertyEntity({ val: ko.observable(""), text: "Data Base Considerada: " });
    this.PeriodoUtilDeDislocamento = PropertyEntity({ val: ko.observable(""), text: "Período Útil de Deslocamento: " });
    this.VelocidadeVazio = PropertyEntity({ val: ko.observable(""), text: "Velocidade Vazio: " });
    this.VelocidadeCarregado = PropertyEntity({ val: ko.observable(""), text: "Velocidade Carregado: " });
    this.TempoColeta = PropertyEntity({ val: ko.observable(""), text: "Tempo de Coleta: " });
    this.TempoEntrega = PropertyEntity({ val: ko.observable(""), text: "Tempo de Entrega: " });
    this.DesconsideraSabado = PropertyEntity({ val: ko.observable(""), text: "Desconsidera Sábados:" });
    this.DesconsideraDomingo = PropertyEntity({ val: ko.observable(""), text: "Desconsidera Domingos: " });
    this.DesconsideraFeriados = PropertyEntity({ val: ko.observable(""), text: "Desconsidera Feriados:" });
    this.DesconsideraFeriados = PropertyEntity({ val: ko.observable(""), text: "Desconsidera Feriados:" });
    this.ConsideraJornadaMotorista = PropertyEntity({ val: ko.observable(""), text: "Considera Jornada Motorista:" });
    this.DistanciaAteDestino = PropertyEntity({ val: ko.observable(""), text: "Distância Ponto Atual até o Destino:" });
    this.TempoIntervaloAlmoco = PropertyEntity({ val: ko.observable(""), text: "Intervalo Almoço Motorista:" });
    this.DistanciaEntrega = PropertyEntity({ val: ko.observable(""), text: " Distância Entre Entregas: " });
    this.TempodePercurso = PropertyEntity({ val: ko.observable(""), text: " Tempo de Percurso: " });
    this.TempoParaChegaNaEntrega = PropertyEntity({ val: ko.observable(0), text: "Tempo para Chegar na Entrega: " });
    this.TempoConsideradoBalsa = PropertyEntity({ val: ko.observable(""), text: "Tempo Considerado Balsa: " });
}

function AbriModalParametrosCalculo(e) {
    _parametroDeCalculo = new ParametosDeCalculo();
    KoBindings(_parametroDeCalculo, "knockoutParametrosCalculo");
    PreencherObjetoKnout(_parametroDeCalculo, { Data: e.ParametrosDeCalculo.val() });
    Global.abrirModal("divModalParametroConsulta");
}
//#endregion