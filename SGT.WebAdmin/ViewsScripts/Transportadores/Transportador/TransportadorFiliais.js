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
/// <reference path="Transportador.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTransportadorFilial;
var _transportadorFilial;

var TransportadorFilial = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.int, enable: ko.observable(true) });
    this.CNPJ = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CNPJ.getFieldDescription(), getType: typesKnockout.cnpj, required: ko.observable(true), visible: ko.observable(true), enable: ko.observable(true) });
    this.Adicionar = PropertyEntity({ eventClick: adicionarTransportadorFilialClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local });
}

//*******EVENTOS*******

function loadTransportadorFilial() {

    _transportadorFilial = new TransportadorFilial();
    KoBindings(_transportadorFilial, "knockoutTransportadorFiliais");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Excluir, id: guid(), metodo: excluirFilialClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CNPJ", title: Localization.Resources.Transportadores.Transportador.CNPJ, width: "90%" },
    ];

    _gridTransportadorFilial = new BasicDataTable(_transportadorFilial.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridTransportadorFilial();
}

function recarregarGridTransportadorFilial() {
    var data = new Array();

    $.each(_transportador.TransportadorFiliais.list, function (i, filial) {
        var filialGrid = new Object();

        filialGrid.Codigo = filial.Codigo.val;
        filialGrid.CNPJ = filial.CNPJ.val;

        data.push(filialGrid);
    });

    _gridTransportadorFilial.CarregarGrid(data);
}

function excluirFilialClick(data) {
    for (var i = 0; i < _transportador.TransportadorFiliais.list.length; i++) {
        filialExcluir = _transportador.TransportadorFiliais.list[i];
        if (data.Codigo == filialExcluir.Codigo.val)
            _transportador.TransportadorFiliais.list.splice(i, 1);
    }
    recarregarGridTransportadorFilial();
}

function adicionarTransportadorFilialClick(e, sender) {
    var valido = ValidarCamposObrigatorios(_transportadorFilial);

    if (valido) {
        if (!ValidarCNPJ(_transportadorFilial.CNPJ.val(), true)) {
            valido = false;
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampCamposObrigatorioso, Localization.Resources.Transportadores.Transportador.OCnpjDigitadoInvalido);
            _transportadorFilial.CNPJ.requiredClass("form-control is-invalid");
            return;
        }
    }

    if (valido) {
        var existe = false;
        $.each(_transportador.TransportadorFiliais.list, function (i, filial) {
            if (filial.CNPJ.val == _transportadorFilial.CNPJ.val()) {
                existe = true;
                return;
            }
        });

        if (existe) {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.CnpjExistente, Localization.Resources.Transportadores.Transportador.CnpjEstaCadastradoComoFilial.format(_transportadorFilial.CNPJ.val()));
            return;
        }

        _transportadorFilial.Codigo.val(guid());
        _transportador.TransportadorFiliais.list.push(SalvarListEntity(_transportadorFilial));

        recarregarGridTransportadorFilial();

        $("#" + _transportadorFilial.CNPJ.id).focus();

        limparCamposTransportadorFilial();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorio, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function limparCamposTransportadorFilial() {
    LimparCampos(_transportadorFilial);
}