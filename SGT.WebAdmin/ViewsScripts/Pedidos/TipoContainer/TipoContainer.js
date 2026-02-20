/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="TipoContainerAssociado.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoContainer;
var _tipoContainer;
var _crudTipoContainer;
var _pesquisaTipoContainer;

var PesquisaTipoContainer = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoContainer.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() === true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var TipoContainer = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({
        text: "Código Integração: ", required: ko.observable(false), maxlength: 50
    });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(true) });
    this.CodigoDocumento = PropertyEntity({ text: "Cod. Documentação: ", required: ko.observable(false), maxlength: 50, visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) });
    this.PesoMaximo = PropertyEntity({ text: "Peso Máximo:", getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, enable: ko.observable(true), required: ko.observable(false) });

    this.FFE = PropertyEntity({ text: "FFE: ", required: ko.observable(false), maxlength: 500 });
    this.TEU = PropertyEntity({ text: "TEU: ", required: ko.observable(false), maxlength: 500 });

    this.PesoLiquido = PropertyEntity({ text: "Peso Líquido:", getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false) });
    this.Tara = PropertyEntity({ text: "Tara:", getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false) });
    this.MetrosCubicos = PropertyEntity({ text: "M³:", getType: typesKnockout.decimal, maxlength: 16, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(false) });
    this.TipoPes = PropertyEntity({ val: ko.observable(""), options: EnumTipoPesCarregamentoNavio.obterOpcoes(), def: EnumTipoPesCarregamentoNavio.NaoInformado, text: "Tipo: ", required: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS && _CONFIGURACAO_TMS.Transportador.AtivarControleCarregamentoNavio) });
    this.TipoContainersAssociado = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TipoMeioTransporteEDI = PropertyEntity({ text: "Tipo Meio Transporte EDI: ", required: ko.observable(false), maxlength: 100 });
};

var CRUDTipoContainer = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadTipoContainer() {
    _tipoContainer = new TipoContainer();
    KoBindings(_tipoContainer, "knockoutCadastroTipoContainer");

    HeaderAuditoria("ContainerTipo", _tipoContainer);

    _crudTipoContainer = new CRUDTipoContainer();
    KoBindings(_crudTipoContainer, "knockoutCRUDTipoContainer");

    _pesquisaTipoContainer = new PesquisaTipoContainer();
    KoBindings(_pesquisaTipoContainer, "knockoutPesquisaTipoContainer", false, _pesquisaTipoContainer.Pesquisar.id);

    buscarTipoContainer();

    loadTipoContainerAssociado();
}

function adicionarClick(e, sender) {
    preencherListasSelecaoTipoContainerAssociado();

    Salvar(_tipoContainer, "TipoContainer/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoContainer.CarregarGrid();
                console.log(_tipoContainer);
                limparCamposTipoContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecaoTipoContainerAssociado();

    Salvar(_tipoContainer, "TipoContainer/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoContainer.CarregarGrid();
                limparCamposTipoContainer();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo Container " + _tipoContainer.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoContainer, "TipoContainer/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoContainer.CarregarGrid();
                limparCamposTipoContainer();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoContainer();
}

//*******MÉTODOS*******


function buscarTipoContainer() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoContainer, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoContainer = new GridView(_pesquisaTipoContainer.Pesquisar.idGrid, "TipoContainer/Pesquisa", _pesquisaTipoContainer, menuOpcoes, null);
    _gridTipoContainer.CarregarGrid();
}

function editarTipoContainer(tipoContainerGrid) {
    limparCamposTipoContainer();
    _tipoContainer.Codigo.val(tipoContainerGrid.Codigo);
    BuscarPorCodigo(_tipoContainer, "TipoContainer/BuscarPorCodigo", function (arg) {
        _pesquisaTipoContainer.ExibirFiltros.visibleFade(false);
        _crudTipoContainer.Atualizar.visible(true);
        _crudTipoContainer.Cancelar.visible(true);
        _crudTipoContainer.Excluir.visible(true);
        _crudTipoContainer.Adicionar.visible(false);

        RecarregarGridTipoContainerAssociado();
    }, null);
}

function limparCamposTipoContainer() {
    
    _crudTipoContainer.Atualizar.visible(false);
    _crudTipoContainer.Cancelar.visible(false);
    _crudTipoContainer.Excluir.visible(false);
    _crudTipoContainer.Adicionar.visible(true);
    LimparCampos(_tipoContainer);
    limparCamposTipoContainerAssociado();
    Global.ResetarAbas();
}