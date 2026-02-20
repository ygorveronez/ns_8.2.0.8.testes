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
/// <reference path="../../../js/libs/jquery.globalize.js" />
/// <reference path="../../../js/libs/jquery.globalize.pt-BR.js" />
/// <reference path="../../Consultas/TipoMovimento.js" />
/// <reference path="ContratoFinanciamentoParcela.js" />
/// <reference path="../../Enumeradores/EnumTipoJustificativa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _contratoFinanciamentoParcelaValor, _gridContratoFinanciamentoParcelaValor;

var _tipoJustificativaContratoFinanciamentoParcelaValor = [
    { value: EnumTipoJustificativa.Desconto, text: "Desconto" },
    { value: EnumTipoJustificativa.Acrescimo, text: "Acréscimo" }
];

var ContratoFinanciamentoParcelaValor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoContratoFinanciamentoParcela = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Grid = PropertyEntity({ type: types.local });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500, enable: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, maxlength: 15, val: ko.observable(""), def: "", configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: ko.observable(true), enable: ko.observable(true) });
    this.Tipo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(_tipoJustificativaContratoFinanciamentoParcelaValor.Desconto), options: _tipoJustificativaContratoFinanciamentoParcelaValor, def: _tipoJustificativaContratoFinanciamentoParcelaValor.Desconto, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoMovimento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo Movimento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarContratoFinanciamentoParcelaValorClick, type: types.event, text: "Adicionar", enable: ko.observable(true), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarContratoFinanciamentoParcelaValorClick, type: types.event, text: "Atualizar", enable: ko.observable(true), visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirContratoFinanciamentoParcelaValorClick, type: types.event, text: "Excluir", enable: ko.observable(true), visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: LimparCamposContratoFinanciamentoParcelaValor, type: types.event, text: "Cancelar", enable: ko.observable(true), visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadContratoFinanciamentoParcelaValor() {
    _contratoFinanciamentoParcelaValor = new ContratoFinanciamentoParcelaValor();
    KoBindings(_contratoFinanciamentoParcelaValor, "knoutContratoFinanciamentoParcelaValor");

    new BuscarTipoMovimento(_contratoFinanciamentoParcelaValor.TipoMovimento);

    buscarContratoFinanciamentoParcelaValor();
}

//*******MÉTODOS*******

function buscarContratoFinanciamentoParcelaValor() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContratoFinanciamentoParcelaValor, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContratoFinanciamentoParcelaValor = new GridView(_contratoFinanciamentoParcelaValor.Grid.id, "ContratoFinanciamentoParcelaValor/Pesquisa", _contratoFinanciamentoParcelaValor, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridContratoFinanciamentoParcelaValor.CarregarGrid();
}

function adicionarContratoFinanciamentoParcelaValorClick(e, sender) {
    Salvar(e, "ContratoFinanciamentoParcelaValor/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridContratoFinanciamentoParcelaValor.CarregarGrid();
                _gridContratoFinanciamentoParcelas.CarregarGrid();
                LimparCamposContratoFinanciamentoParcelaValor();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarContratoFinanciamentoParcelaValorClick(e, sender) {
    Salvar(e, "ContratoFinanciamentoParcelaValor/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContratoFinanciamentoParcelaValor.CarregarGrid();
                _gridContratoFinanciamentoParcelas.CarregarGrid();
                LimparCamposContratoFinanciamentoParcelaValor();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirContratoFinanciamentoParcelaValorClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a parcela valor?", function () {
        ExcluirPorCodigo(_contratoFinanciamentoParcelaValor, "ContratoFinanciamentoParcelaValor/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridContratoFinanciamentoParcelaValor.CarregarGrid();
                    _gridContratoFinanciamentoParcelas.CarregarGrid();
                    LimparCamposContratoFinanciamentoParcelaValor();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function editarContratoFinanciamentoParcelaValor(contratoFinanciamentoParcelaValorGrid) {
    LimparCamposContratoFinanciamentoParcelaValor();
    _contratoFinanciamentoParcelaValor.Codigo.val(contratoFinanciamentoParcelaValorGrid.Codigo);
    BuscarPorCodigo(_contratoFinanciamentoParcelaValor, "ContratoFinanciamentoParcelaValor/BuscarPorCodigo", function (arg) {
        _contratoFinanciamentoParcelaValor.Atualizar.visible(true);
        _contratoFinanciamentoParcelaValor.Cancelar.visible(true);
        _contratoFinanciamentoParcelaValor.Excluir.visible(true);
        _contratoFinanciamentoParcelaValor.Adicionar.visible(false);
    }, null);
}

function LimparCamposContratoFinanciamentoParcelaValor() {
    var codigoParcela = _contratoFinanciamentoParcelaValor.CodigoContratoFinanciamentoParcela.val();
    LimparCampos(_contratoFinanciamentoParcelaValor);

    _contratoFinanciamentoParcelaValor.Adicionar.visible(true);
    _contratoFinanciamentoParcelaValor.Atualizar.visible(false);
    _contratoFinanciamentoParcelaValor.Excluir.visible(false);
    _contratoFinanciamentoParcelaValor.Cancelar.visible(false);

    _contratoFinanciamentoParcelaValor.CodigoContratoFinanciamentoParcela.val(codigoParcela);
}