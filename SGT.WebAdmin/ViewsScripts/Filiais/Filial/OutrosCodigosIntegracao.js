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


//*******MAPEAMENTO KNOUCKOUT*******

var _gridOutroCodigoIntegracao;
var _outroCodigoIntegracao;

var OutroCodigoIntegracao = function () {

    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getRequiredFieldDescription(), maxlength: 50, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarOutroCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadOutroCodigoIntegracao() {
    _outroCodigoIntegracao = new OutroCodigoIntegracao();
    KoBindings(_outroCodigoIntegracao, "knockoutOutrosCodigosIntegracao");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarOutroCodigoIntegracaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Gerais.Geral.CodigoIntegracao, width: "80%" }
    ];

    _gridOutroCodigoIntegracao = new BasicDataTable(_outroCodigoIntegracao.Grid.id, header, menuOpcoes);
    recarregarGridOutroCodigoIntegracao();
}

function recarregarGridOutroCodigoIntegracao() {
    var data = new Array();
    $.each(_filial.OutrosCodigosIntegracao.list, function (i, outroCodigoIntegracao) {
        var outroCodigoIntegracaoGrid = new Object();
        outroCodigoIntegracaoGrid.CodigoIntegracao = outroCodigoIntegracao.CodigoIntegracao.val;
        outroCodigoIntegracaoGrid.Codigo = outroCodigoIntegracao.Codigo.val;
        data.push(outroCodigoIntegracaoGrid);
    });
    _gridOutroCodigoIntegracao.CarregarGrid(data);
}

function editarOutroCodigoIntegracaoClick(data) {
    _outroCodigoIntegracao.Atualizar.visible(true);
    _outroCodigoIntegracao.Cancelar.visible(true);
    _outroCodigoIntegracao.Excluir.visible(true);
    _outroCodigoIntegracao.Adicionar.visible(false);
    EditarListEntity(_outroCodigoIntegracao, data);
}

function excluirOutroCodigoIntegracaoClick() {
    for (var i = 0; i < _filial.OutrosCodigosIntegracao.list.length; i++) {
        outroCodigos = _filial.OutrosCodigosIntegracao.list[i];
        if (_outroCodigoIntegracao.Codigo.val() == outroCodigos.Codigo.val)
            _filial.OutrosCodigosIntegracao.list.splice(i, 1);
    }
    limparCamposOutroCodigoIntegracao();
    recarregarGridOutroCodigoIntegracao();
}

function adicionarOutroCodigoIntegracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_outroCodigoIntegracao);
    if (tudoCerto) {
        var existe = false;
        $.each(_filial.OutrosCodigosIntegracao.list, function (i, outroCodigoIntegracao) {
            if (outroCodigoIntegracao.CodigoIntegracao.val == _outroCodigoIntegracao.CodigoIntegracao.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _filial.OutrosCodigosIntegracao.list.push(SalvarListEntity(_outroCodigoIntegracao));
            recarregarGridOutroCodigoIntegracao();
            $("#" + _outroCodigoIntegracao.CodigoIntegracao.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
        }
        limparCamposOutroCodigoIntegracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function atualizarOutroCodigoIntegracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_outroCodigoIntegracao);
    if (tudoCerto) {
        $.each(_filial.OutrosCodigosIntegracao.list, function (i, outroCodigoIntegracao) {
            if (outroCodigoIntegracao.Codigo.val == _outroCodigoIntegracao.Codigo.val()) {
                _outroCodigoIntegracao.Codigo.val(_outroCodigoIntegracao.CodigoIntegracao.val());
                AtualizarListEntity(_outroCodigoIntegracao, outroCodigoIntegracao)
                return false;
            }
        });
        recarregarGridOutroCodigoIntegracao();
        limparCamposOutroCodigoIntegracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}


function cancelarOutroCodigoIntegracaoClick(e) {
    limparCamposOutroCodigoIntegracao();
}

function limparCamposOutroCodigoIntegracao() {
    _outroCodigoIntegracao.Atualizar.visible(false);
    _outroCodigoIntegracao.Excluir.visible(false);
    _outroCodigoIntegracao.Cancelar.visible(false);
    _outroCodigoIntegracao.Adicionar.visible(true);
    LimparCampos(_outroCodigoIntegracao);
}