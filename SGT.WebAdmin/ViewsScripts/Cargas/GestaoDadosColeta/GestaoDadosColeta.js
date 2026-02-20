/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Globais.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoGestaoDadosColeta.js" />
/// <reference path="../../Enumeradores/EnumSituacaoGestaoDadosColeta.js" />
/// <reference path="../../Enumeradores/EnumSimNao.js" />
/// <reference path="DadosTransporte.js" />
/// <reference path="FotoNota.js" />

// #region Objetos Globais do Arquivo

var _gestaoDadosColeta;
var _gridGestaoDadosColeta;
var _pesquisaGestaoDadosColeta;

// #endregion Objetos Globais do Arquivo

// #region Classes

var PesquisaGestaoDadosColeta = function () {
    this.CodigoCargaEmbarcador = PropertyEntity({ text: "Número da Carga:", val: ko.observable(""), def: "" });
    this.DataInicial = PropertyEntity({ text: "Data de Criação inicial: ", getType: typesKnockout.dateTime });
    this.DataFinal = PropertyEntity({ text: "Data de Criação final: ", getType: typesKnockout.dateTime });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Transportador:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Fornecedor:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });
    this.OrigemCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Origem:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });
    this.DestinoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Destino:", idBtnSearch: guid(), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiCTe) });
    this.Situacao = PropertyEntity({ val: ko.observable(""), options: EnumSituacaoGestaoDadosColeta.obterOpcoesPesquisa(), text: "Situação: " });
    this.RetornoConfirmacao = PropertyEntity({ text: "Retorno Confirmação:", val: ko.observable(EnumSituacaoGestaoDadosColetaRetornoConfirmacao.Todas), def: ko.observable(EnumSituacaoGestaoDadosColetaRetornoConfirmacao.Todas), options: EnumSituacaoGestaoDadosColetaRetornoConfirmacao.obterOpcoesPesquisa()});

    this.Pesquisar = PropertyEntity({ type: types.event, eventClick: recarregarGridGestaoDadosColeta, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Exibir Filtros", idFade: guid(), visibleFade: ko.observable(true), visible: ko.observable(true)
    });
}

var GestaoDadosColeta = function () {
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), visible: false });

    this.AdicionarDadosNfe = PropertyEntity({ type: types.event, eventClick: exibirModalAdicionarGestaoDadosColetaDadosNfeClick, text: "Adicionar Dados da NF-e", visible: ko.observable(true) });
    this.AdicionarDadosTransporte = PropertyEntity({ type: types.event, eventClick: exibirModalAdicionarGestaoDadosColetaDadosTransporteClick, text: "Adicionar Dados de Transporte", visible: ko.observable(true) });
    this.OpcaoUM = PropertyEntity({ type: types.event, eventClick: recarregarGridGestaoDadosColeta, text: "Opção 1", visible: ko.observable(false) }); //renomear quando tiver as opções do botão
    this.OpcaoDois = PropertyEntity({ type: types.event, eventClick: recarregarGridGestaoDadosColeta, text: "Opção 2", visible: ko.observable(false) }); //renomear quando tiver as opções do botão
}

// #endregion Classes

// #region Funções de Inicialização

function loadGestaoDadosColeta() {
    _pesquisaGestaoDadosColeta = new PesquisaGestaoDadosColeta();
    KoBindings(_pesquisaGestaoDadosColeta, "knockoutPesquisaGestaoDadosColeta");

    _gestaoDadosColeta = new GestaoDadosColeta();
    KoBindings(_gestaoDadosColeta, "knockoutGestaoDadosColeta");

    HeaderAuditoria("GestaoDadosColeta", _gestaoDadosColeta);

    BuscarTransportadores(_pesquisaGestaoDadosColeta.Transportador);
    BuscarFilial(_pesquisaGestaoDadosColeta.Filial);
    BuscarClientes(_pesquisaGestaoDadosColeta.Cliente);
    BuscarLocalidades(_pesquisaGestaoDadosColeta.OrigemCarga);
    BuscarLocalidades(_pesquisaGestaoDadosColeta.DestinoCarga);

    loadGestaoDadosColetaDadosTransporte();
    loadGestaoDadosColetaDadosNfe();
    loadGridGestaoDadosColeta();
}

function loadGridGestaoDadosColeta() {
    var opcaoDetalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: exibirModalEditarGestaoDadosColetaClick,
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [opcaoDetalhes]
    };

    var multiplaEscolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: null,
        callbackNaoSelecionado: exibirMultiplasOpcoesGestaoDadosColeta,
        callbackSelecionado: exibirMultiplasOpcoesGestaoDadosColeta,
        callbackSelecionarTodos: exibirMultiplasOpcoesGestaoDadosColeta,
        somenteLeitura: false
    };

    var configuracaoExportacao = {
        url: "GestaoDadosColeta/ExportarPesquisa",
        titulo: "Gestao de Dados de Coleta"
    };

    _gridGestaoDadosColeta = new GridView("grid-gestao-dados-coleta", "GestaoDadosColeta/Pesquisa", _pesquisaGestaoDadosColeta, menuOpcoes, null, 25, null, null, null, multiplaEscolha, null, null, configuracaoExportacao, null, null, null, callbackColumnDefaultGestaoDadosColeta);
    _gridGestaoDadosColeta.SetPermitirEdicaoColunas(false);
    _gridGestaoDadosColeta.SetSalvarPreferenciasGrid(false);
    _gridGestaoDadosColeta.CarregarGrid();
}

function callbackColumnDefaultGestaoDadosColeta(cabecalho, valorColuna, dadosLinha) {
    if (cabecalho.name == "Duracao") {
        let elementId = cabecalho.name + '-' + dadosLinha.DT_RowId;
        let element = $('<span id="' + elementId + '"></span>');

        if (dadosLinha.DataAprovacaoFormatada) {
            let startDate = moment(dadosLinha.DataInicial, "DD/MM/YYYY HH:mm:ss");
            let endDate = moment(dadosLinha.DataAprovacaoFormatada, "DD/MM/YYYY HH:mm:ss");
            let duration = moment.duration(endDate.diff(startDate));

            let durationText;
            if (duration.days() > 0) {
                durationText = duration.days() + 'd ' + moment.utc(duration.asMilliseconds()).format('HH:mm:ss');
            } else {
                durationText = moment.utc(duration.asMilliseconds()).format('HH:mm:ss');
            }

            element.text(durationText);
        } else {
            setTimeout(function () {
                $('#' + elementId)
                    .countdown(moment(dadosLinha.DataInicial, "DD/MM/YYYY HH:mm:ss").format("YYYY/MM/DD HH:mm:ss"), { elapse: true, precision: 1000 })
                    .on('update.countdown', function (event) {
                        let durationText = event.offset.totalDays > 0 ? event.strftime('%-Dd %H:%M:%S') : event.strftime('%H:%M:%S');
                        $(this).text(durationText);
                    });
            }, 1000);
        }

        return element.prop('outerHTML');
    }
}


// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function exibirModalAdicionarGestaoDadosColetaDadosNfeClick() {
    exibirModalAdicionarGestaoDadosColetaDadosNfe();
}

function exibirModalAdicionarGestaoDadosColetaDadosTransporteClick() {
    exibirModalAdicionarGestaoDadosColetaDadosTransporte();
}

function exibirModalEditarGestaoDadosColetaClick(registroSelecionado) {
    if (registroSelecionado.Tipo == EnumTipoGestaoDadosColeta.DadosNfe)
        exibirModalEditarGestaoDadosColetaDadosNfe(registroSelecionado);
    else
        exibirModalEditarGestaoDadosColetaDadosTransporte(registroSelecionado);
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function recarregarGridGestaoDadosColeta() {
    _gridGestaoDadosColeta.CarregarGrid();
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirMultiplasOpcoesGestaoDadosColeta() {
    var existemRegistrosSelecionados = _gridGestaoDadosColeta.ObterMultiplosSelecionados().length > 0;

    if (existemRegistrosSelecionados) {
        _gestaoDadosColeta.OpcaoUM.visible(true);
        _gestaoDadosColeta.OpcaoDois.visible(true);
    }
    else {
        _gestaoDadosColeta.OpcaoUM.visible(false);
        _gestaoDadosColeta.OpcaoDois.visible(false);
    }
}

// #endregion Funções Privadas
