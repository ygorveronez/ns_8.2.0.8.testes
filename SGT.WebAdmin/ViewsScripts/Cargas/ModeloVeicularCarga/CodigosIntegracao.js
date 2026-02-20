/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />


//*******MAPEAMENTO KNOUCKOUT*******
var _gridCodigoIntegracao;
var _codigoIntegracao;

var CodigoIntegracao = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoDeIntegracao.getRequiredFieldDescription(), maxlength: 50, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCodigoIntegracaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}


//*******EVENTOS*******
function loadCodigoIntegracao() {
    _codigoIntegracao = new CodigoIntegracao();
    KoBindings(_codigoIntegracao, "knockoutCodigosIntegracao");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCodigoIntegracaoClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoIntegracao", title: Localization.Resources.Cargas.ModeloVeicularCarga.CodigoDeIntegracao, width: "80%" }
    ];

    _gridCodigoIntegracao = new BasicDataTable(_codigoIntegracao.Grid.id, header, menuOpcoes);
    RecarregarGridCodigoIntegracao();
}

function editarCodigoIntegracaoClick(data) {
    _codigoIntegracao.Atualizar.visible(true);
    _codigoIntegracao.Cancelar.visible(true);
    _codigoIntegracao.Excluir.visible(true);
    _codigoIntegracao.Adicionar.visible(false);
    EditarListEntity(_codigoIntegracao, data);
}

function cancelarCodigoIntegracaoClick(e) {
    LimparCamposCodigoIntegracao();
}

function excluirCodigoIntegracaoClick() {
    for (var i = 0; i < _modeloVeicularCarga.CodigosIntegracao.list.length; i++) {
        codigos = _modeloVeicularCarga.CodigosIntegracao.list[i];
        if (_codigoIntegracao.Codigo.val() == codigos.Codigo.val)
            _modeloVeicularCarga.CodigosIntegracao.list.splice(i, 1);
    }
    LimparCamposCodigoIntegracao();
    RecarregarGridCodigoIntegracao();
}

function adicionarCodigoIntegracaoClick(e, sender) {
    var tudoCerto = ValidarCamposObrigatorios(_codigoIntegracao);
    if (tudoCerto) {
        var existe = false;
        $.each(_modeloVeicularCarga.CodigosIntegracao.list, function (i, codigoIntegracao) {
            if (codigoIntegracao.CodigoIntegracao.val == _codigoIntegracao.CodigoIntegracao.val()) {
                existe = true;
                return false;
            }
        });
        if (!existe) {
            _modeloVeicularCarga.CodigosIntegracao.list.push(SalvarListEntity(_codigoIntegracao));
            RecarregarGridCodigoIntegracao();
            $("#" + _codigoIntegracao.CodigoIntegracao.id).focus();
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.ModeloVeicularCarga.CadigoJaExistente, Localization.Resources.Cargas.ModeloVeicularCarga.CodigoDeIntegracaoJaEstaCadastrada.format(_codigoIntegracao.CodigoIntegracao.val()));
        }
        LimparCamposCodigoIntegracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }

}

function atualizarCodigoIntegracaoClick(e, sender) {
    if (ValidarCamposObrigatorios(_codigoIntegracao)) {
        $.each(_modeloVeicularCarga.CodigosIntegracao.list, function (i, codigoIntegracao) {
            if (codigoIntegracao.Codigo.val == _codigoIntegracao.Codigo.val()) {
                _codigoIntegracao.Codigo.val(_codigoIntegracao.CodigoIntegracao.val());
                AtualizarListEntity(_codigoIntegracao, codigoIntegracao)
                return false;
            }
        });
        RecarregarGridCodigoIntegracao();
        LimparCamposCodigoIntegracao();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}


//*******METODOS*******
function RecarregarGridCodigoIntegracao() {
    var data = [];
    $.each(_modeloVeicularCarga.CodigosIntegracao.list, function (i, codigoIntegracao) {
        var codigoIntegracaoGrid = {
            CodigoIntegracao: codigoIntegracao.CodigoIntegracao.val,
            Codigo: codigoIntegracao.Codigo.val
        };
        data.push(codigoIntegracaoGrid);
    });
    _gridCodigoIntegracao.CarregarGrid(data);
}

function LimparCamposCodigoIntegracao() {
    _codigoIntegracao.Atualizar.visible(false);
    _codigoIntegracao.Excluir.visible(false);
    _codigoIntegracao.Cancelar.visible(false);
    _codigoIntegracao.Adicionar.visible(true);
    LimparCampos(_codigoIntegracao);
}