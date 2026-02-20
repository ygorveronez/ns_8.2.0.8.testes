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
/// <reference path="../../../ViewsScripts/Consultas/Justificativa.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoValorContratoFreteADA.js" />
/// <reference path="../../../ViewsScripts/Enumeradores/EnumTipoCalculoContratoFreteADA.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridContratoFreteADA;
var _contratoFreteADA;
var _pesquisaContratoFreteADA;

var PesquisaContratoFreteADA = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Justificativa: ", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridContratoFreteADA.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var ContratoFreteADA = function () {

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 150 });
    this.Justificativa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Justificativa:", idBtnSearch: guid(), required: true });
    this.Valor = PropertyEntity({ text: "*Valor: ", getType: typesKnockout.decimal, maxlength: 18, required: true });
    this.TipoValor = PropertyEntity({ text: "*Tipo de Valor: ", val: ko.observable(EnumTipoValorContratoFreteADA.Fixo), options: EnumTipoValorContratoFreteADA.obterOpcoes(), def: EnumTipoValorContratoFreteADA.Fixo, required: true });
    this.TipoCalculo = PropertyEntity({ text: "*Calcular Por: ", val: ko.observable(EnumTipoCalculoContratoFreteADA.DiasEntreAgendamentoPrevisaoSaida), options: EnumTipoCalculoContratoFreteADA.obterOpcoes(), def: EnumTipoCalculoContratoFreteADA.DiasEntreAgendamentoPrevisaoSaida, required: false, visibleFade: ko.observable(false), idFade: guid() });
    this.Observacoes = PropertyEntity({ text: "Observações: ", required: false, maxlength: 400 });
    this.TipoValor.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoValorContratoFreteADA.Calculado) {
            _contratoFreteADA.TipoCalculo.visibleFade(true);
            _contratoFreteADA.TipoCalculo.required = true;
        } else {
            _contratoFreteADA.TipoCalculo.visibleFade(false);
            _contratoFreteADA.TipoCalculo.required = false;
        }
    });
};

var CRUD_ContratoFreteADA = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadContratoFreteADA() {
    _contratoFreteADA = new ContratoFreteADA();
    KoBindings(_contratoFreteADA, "knockoutCadastroContratoFreteADA");

    HeaderAuditoria("ContratoFreteADA", ContratoFreteADA);

    _crudContratoFreteADA = new CRUD_ContratoFreteADA();
    KoBindings(_crudContratoFreteADA, "knockoutCRUD_ContratoFreteADA");

    _pesquisaContratoFreteADA = new PesquisaContratoFreteADA();
    KoBindings(_pesquisaContratoFreteADA, "knockoutPesquisaContratoFreteADA", false, _pesquisaContratoFreteADA.Pesquisar.id);

    new BuscarJustificativas(_contratoFreteADA.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);
    new BuscarJustificativas(_pesquisaContratoFreteADA.Justificativa, null, null, [EnumTipoFinalidadeJustificativa.ContratoFrete]);

    buscarContratoFreteADA();
}

function adicionarClick(e, sender) {
    Salvar(_contratoFreteADA, "ContratoFreteAcrescimoDescontoAutomatico/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridContratoFreteADA.CarregarGrid();
                limparCamposContratoFreteADA();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_contratoFreteADA, "ContratoFreteAcrescimoDescontoAutomatico/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridContratoFreteADA.CarregarGrid();
                limparCamposContratoFreteADA();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a regra " + _contratoFreteADA.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_contratoFreteADA, "ContratoFreteAcrescimoDescontoAutomatico/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridContratoFreteADA.CarregarGrid();
                    limparCamposContratoFreteADA();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposContratoFreteADA();
}

//*******MÉTODOS*******


function buscarContratoFreteADA() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarContratoFreteADA, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridContratoFreteADA = new GridView(_pesquisaContratoFreteADA.Pesquisar.idGrid, "ContratoFreteAcrescimoDescontoAutomatico/Pesquisa", _pesquisaContratoFreteADA, menuOpcoes);
    _gridContratoFreteADA.CarregarGrid();
}

function editarContratoFreteADA(rad) {
    limparCamposContratoFreteADA();
    _contratoFreteADA.Codigo.val(rad.Codigo);
    BuscarPorCodigo(_contratoFreteADA, "ContratoFreteAcrescimoDescontoAutomatico/BuscarPorCodigo", function (arg) {
        _pesquisaContratoFreteADA.ExibirFiltros.visibleFade(false);
        _crudContratoFreteADA.Atualizar.visible(true);
        _crudContratoFreteADA.Cancelar.visible(true);
        _crudContratoFreteADA.Excluir.visible(true);
        _crudContratoFreteADA.Adicionar.visible(false);
    }, null);
}

function limparCamposContratoFreteADA() {
    _crudContratoFreteADA.Atualizar.visible(false);
    _crudContratoFreteADA.Cancelar.visible(false);
    _crudContratoFreteADA.Excluir.visible(false);
    _crudContratoFreteADA.Adicionar.visible(true);
    LimparCampos(_contratoFreteADA);
}