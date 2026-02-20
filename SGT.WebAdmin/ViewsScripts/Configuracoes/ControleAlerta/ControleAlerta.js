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
/// <reference path="../../Consultas/Usuario.js" />
/// <reference path="../../Enumeradores/EnumControleAlertaForma.js" />
/// <reference path="../../Enumeradores/EnumControleAlertaTela.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemServicoFrota.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridControleAlerta;
var _controleAlerta;
var _CRUDControleAlerta;
var _pesquisaControleAlerta;

var PesquisaControleAlerta = function () {
    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Funcionário:", idBtnSearch: guid(), visible: ko.observable(true) });
    this.FormaAlerta = PropertyEntity({ text: "Forma(s) Alerta:", val: ko.observable([]), options: EnumControleAlertaForma.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple });
    this.TelaAlerta = PropertyEntity({ text: "Tela(s) Alerta:", val: ko.observable([]), options: EnumControleAlertaTela.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple });
    this.Status = PropertyEntity({ val: ko.observable(false), options: _statusPesquisa, def: false, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridControleAlerta.CarregarGrid();
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

var ControleAlerta = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.QuantidadeDias = PropertyEntity({ val: ko.observable(0), def: 0, text: "Qtd. Dias:", required: false, maxlength: 10, getType: typesKnockout.int });
    this.QuantidadeDiasAlertaOsInterna = PropertyEntity({ val: ko.observable(0), def: 0, text: "Qtd. Dias Alertas O.S. Interna:", required: false, maxlength: 10, getType: typesKnockout.int });
    this.QuantidadeDiasAlertaOsExterna = PropertyEntity({ val: ko.observable(0), def: 0, text: "Qtd. Dias Alertas O.S. Externa:", required: false, maxlength: 10, getType: typesKnockout.int });
    this.Status = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Status: " });

    this.FormaAlerta = PropertyEntity({ text: "*Forma(s) Alerta:", val: ko.observable([]), options: EnumControleAlertaForma.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple });
    this.TelaAlerta = PropertyEntity({ text: "*Tela(s) Alerta:", val: ko.observable([]), options: EnumControleAlertaTela.obterOpcoes(), def: [], getType: typesKnockout.selectMultiple });
    this.Situacao = PropertyEntity({ text: "Situação O.S.:", val: ko.observable([]), options: EnumSituacaoOrdemServicoFrota.ObterOpcoes(), def: [], getType: typesKnockout.selectMultiple });

    this.Funcionario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Funcionário:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
};

var CRUDControleAlerta = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadControleAlerta() {

    _pesquisaControleAlerta = new PesquisaControleAlerta();
    KoBindings(_pesquisaControleAlerta, "knockoutPesquisaControleAlerta", false, _pesquisaControleAlerta.Pesquisar.id);

    _controleAlerta = new ControleAlerta();
    KoBindings(_controleAlerta, "knockoutCadastroControleAlerta");

    HeaderAuditoria("ControleAlerta", _controleAlerta);

    _CRUDControleAlerta = new CRUDControleAlerta();
    KoBindings(_CRUDControleAlerta, "knockoutCRUDCadastroControleAlerta");

    new BuscarFuncionario(_pesquisaControleAlerta.Funcionario);
    new BuscarFuncionario(_controleAlerta.Funcionario);

    buscarControleAlertas();
}

function adicionarClick(e, sender) {
    if (_controleAlerta.FormaAlerta.val().length === 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informar ao menos uma forma para o Alerta!");
        return;
    }
    if (_controleAlerta.TelaAlerta.val().length === 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informar ao menos uma tela para o Alerta!");
        return;
    }

    Salvar(_controleAlerta, "ControleAlerta/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridControleAlerta.CarregarGrid();
                limparCamposControleAlerta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (_controleAlerta.FormaAlerta.val().length === 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informar ao menos uma forma para o Alerta!");
        return;
    }
    if (_controleAlerta.TelaAlerta.val().length === 0) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "Por favor informar ao menos uma tela para o Alerta!");
        return;
    }

    Salvar(_controleAlerta, "ControleAlerta/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridControleAlerta.CarregarGrid();
                limparCamposControleAlerta();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Controle de Alerta?", function () {
        ExcluirPorCodigo(_controleAlerta, "ControleAlerta/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridControleAlerta.CarregarGrid();
                    limparCamposControleAlerta();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposControleAlerta();
}

//*******MÉTODOS*******

function buscarControleAlertas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarControleAlerta, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridControleAlerta = new GridView(_pesquisaControleAlerta.Pesquisar.idGrid, "ControleAlerta/Pesquisa", _pesquisaControleAlerta, menuOpcoes, null);
    _gridControleAlerta.CarregarGrid();
}

function editarControleAlerta(controleAlertaGrid) {
    limparCamposControleAlerta();
    _controleAlerta.Codigo.val(controleAlertaGrid.Codigo);
    BuscarPorCodigo(_controleAlerta, "ControleAlerta/BuscarPorCodigo", function (arg) {
        _pesquisaControleAlerta.ExibirFiltros.visibleFade(false);
        _CRUDControleAlerta.Atualizar.visible(true);
        _CRUDControleAlerta.Cancelar.visible(true);
        _CRUDControleAlerta.Excluir.visible(true);
        _CRUDControleAlerta.Adicionar.visible(false);
    }, null);
}

function limparCamposControleAlerta() {
    _CRUDControleAlerta.Atualizar.visible(false);
    _CRUDControleAlerta.Cancelar.visible(false);
    _CRUDControleAlerta.Excluir.visible(false);
    _CRUDControleAlerta.Adicionar.visible(true);
    LimparCampos(_controleAlerta);
}