/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLoteEscrituracao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridDocumentosSelecaoMiro;
var _selecaoDocumentosMiro;

var SelecaoDocumentosMiro = function () {

    var dataAtual = moment().add(-2, 'days').format("DD/MM/YYYY");
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), def: "", defCodEntity: 0, required: false, text: ko.observable("Transportador:"), idBtnSearch: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.ModeloDocumentoFiscal = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Modelo do Documento", enable: ko.observable(true), required: true, idBtnSearch: guid(), visible: ko.observable(true), issue: 370 });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.DataInicio = PropertyEntity({ text: "Data Inicial Emissão:", getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataFim = PropertyEntity({ text: "Data Final Emissão:  ", val: ko.observable(dataAtual), def: dataAtual, getType: typesKnockout.date, enable: ko.observable(true), visible: ko.observable(true) });
    this.DataInicio.dateRangeLimit = this.DataFim;
    this.DataFim.dateRangeInit = this.DataInicio;

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(false), text: "Selecionar Todos" });

    this.Transportador.val.subscribe(function (novoValor) {
        if (string.IsNullOrWhiteSpace(novoValor))
            _selecaoDocumentosMiro.Transportador.codEntity(0);
    });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridDocumentosSelecaoMiro.CarregarGrid(function () {
                // TODO:
                // Triar o setTimeout e chamar o selecionar todos após renderizar tabela
                // Limpar o selecionar todos quando algum campo do filtro for modificado

                // Só marca todos como selecionado quando Filial, Transportador e Tomador forem selecionados
                var busca = RetornarObjetoPesquisa(_selecaoDocumentosMiro);
                if ((busca.Filial > 0 && busca.Tomador > 0 && busca.Transportador > 0) && _selecaoDocumentosMiro.SelecionarTodos.val() == false) {
                    setTimeout(_selecaoDocumentosMiro.SelecionarTodos.eventClick, 100);
                }
                else if (_selecaoDocumentosMiro.SelecionarTodos.val() == true) {
                    setTimeout(_selecaoDocumentosMiro.SelecionarTodos.eventClick, 100);
                } else {
                    _selecaoDocumentosMiro.SelecionarTodos.val(false);

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) {
                        if (busca.Transportador > 0)
                            _selecaoDocumentosMiro.SelecionarTodos.visible(true);
                        else
                            _selecaoDocumentosMiro.SelecionarTodos.visible(false);
                    } else {
                        if (busca.Tomador > 0)
                            _selecaoDocumentosMiro.SelecionarTodos.visible(true);
                        else
                            _selecaoDocumentosMiro.SelecionarTodos.visible(false);
                    }
                }
            });
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.Criar = PropertyEntity({ eventClick: criarClickMiro, type: types.event, text: "Adicionar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadSelecaoDocumentosMiro() {
    _selecaoDocumentosMiro = new SelecaoDocumentosMiro();
    KoBindings(_selecaoDocumentosMiro, "knockoutSelecaoDocumentosMiro");

    //new BuscarTransportadores(_selecaoDocumentosMiro.Transportador);
    new BuscarTiposOperacao(_selecaoDocumentosMiro.TipoOperacao);
    new BuscarModeloDocumentoFiscal(_selecaoDocumentosMiro.ModeloDocumentoFiscal, null, null, false, true, null, true);
    new BuscarCargas(_selecaoDocumentosMiro.Carga, null, null, null, null, null, null, null, null, true);

    GridSelecaoDocumentosMiro();
}

function criarClickMiro(e, sender) {
    //selecionar
}


//*******MÉTODOS*******
function GridSelecaoDocumentosMiro() {

    var detalhes = {
        descricao: "Detalhes",
        id: "clasEditar",
        evento: "onclick",
        metodo: function () { },
        tamanho: "10",
        icone: ""
    };

    var menuOpcoes = {
        tipo: TypeOptionMenu.list,
        descricao: "Opções",
        tamanho: 7,
        opcoes: [detalhes]
    };

    //-- Multi escolha
    var multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _selecaoDocumentosMiro.SelecionarTodos,
        callbackNaoSelecionado: function () {
            SelecaoModificado(false);
        },
        callbackSelecionado: function () {
            SelecaoModificado(true);
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    }

    _gridDocumentosSelecaoMiro = new GridView(_selecaoDocumentosMiro.Pesquisar.idGrid, "LoteEscrituracaoMiro/PesquisaDocumento", _selecaoDocumentosMiro, menuOpcoes, null, 25, null, null, null, multiplaescolha);
    _gridDocumentosSelecaoMiro.SetPermitirRedimencionarColunas(true);
    _gridDocumentosSelecaoMiro.CarregarGrid(function () {
        setTimeout(function () {
            if (_selecaoDocumentosMiro.Codigo.val() > 0)
                $("#btnExportarDocumento").show();
            else
                $("#btnExportarDocumento").hide();
        }, 200);
    });
}

function SelecaoModificado() {
    var itens = _gridDocumentosSelecaoMiro.ObterMultiplosSelecionados();
}

function ValidaDocumentosSelecionados() {
    var valido = true;

    // Valida Quantidade
    var itens = _gridDocumentosSelecaoMiro.ObterMultiplosSelecionados();
    // TODO: Se o btn SELECIONAR TODOS estiver clicado, 
    if (itens.length == 0 && !_selecaoDocumentosMiro.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Documentos Selecionados", "Nenhum documento selecionado.");
    }

    return valido;
}

function EditarSelecaoDocumentos(data) {
    GridSelecaoDocumentosMiro();
}

function LimparCamposSelecaoDocumentos() {
    _selecaoDocumentosMiro.SelecionarTodos.visible(false);
    _selecaoDocumentosMiro.Criar.visible(true);
    LimparCampos(_selecaoDocumentosMiro);
}
