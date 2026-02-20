/// <reference path="Transportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _codigoComercialDistribuidorTransportador;
var _gridCodigoComercialDistribuidorTransportador;

/*
 * Declaração das Classes
 */

var CodigoComercialDistribuidorTransportador = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.CodigoComercialDistribuidor = PropertyEntity({ text: Localization.Resources.Transportadores.Transportador.CodigoComercialDistribuidor.getRequiredFieldDescription(), maxlength: 50, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCodigoComercialDistribuidorTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCodigoComercialDistribuidorTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCodigoComercialDistribuidorTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCodigoComercialDistribuidorTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCodigoComercialDistribuidorTransportador() {
    _codigoComercialDistribuidorTransportador = new CodigoComercialDistribuidorTransportador();
    KoBindings(_codigoComercialDistribuidorTransportador, "knockoutCodigosComercialDistribuidor");

    loadGridCodigoComercialDistribuidorTransportador();
}

function loadGridCodigoComercialDistribuidorTransportador() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCodigoComercialDistribuidorTransportadorClick }] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoComercialDistribuidor", title: Localization.Resources.Transportadores.Transportador.CodigoComercialDistribuidor, width: "80%" }
    ];

    _gridCodigoComercialDistribuidorTransportador = new BasicDataTable(_codigoComercialDistribuidorTransportador.Grid.id, header, menuOpcoes);
    recarregarGridCodigoComercialDistribuidorTransportador();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCodigoComercialDistribuidorTransportadorClick() {
    if (ValidarCamposObrigatorios(_codigoComercialDistribuidorTransportador)) {
        if (!isCodigoComercialDistribuidorTransportadorExiste()) {
            _codigoComercialDistribuidorTransportador.Codigo.val(_codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val());

            _transportador.CodigosComercialDistribuidor.list.push(SalvarListEntity(_codigoComercialDistribuidorTransportador));

            recarregarGridCodigoComercialDistribuidorTransportador();
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Transportadores.Transportador.CodigoExistente, Localization.Resources.Transportadores.Transportador.CodigoComercialDistribuidorCadastrada.format(_codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val()));

        $("#" + _codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.id).focus();

        limparCamposCodigoComercialDistribuidorTransportador();
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
}

function atualizarCodigoComercialDistribuidorTransportadorClick() {
    if (ValidarCamposObrigatorios(_codigoComercialDistribuidorTransportador)) {
        if (!isCodigoComercialDistribuidorTransportadorExiste()) {
            $.each(_transportador.CodigosComercialDistribuidor.list, function (i, codigoComercialDistribuidorTransportador) {
                if (codigoComercialDistribuidorTransportador.Codigo.val == _codigoComercialDistribuidorTransportador.Codigo.val()) {
                    _codigoComercialDistribuidorTransportador.Codigo.val(_codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val());

                    AtualizarListEntity(_codigoComercialDistribuidorTransportador, codigoComercialDistribuidorTransportador);

                    return false;
                }
            });

            recarregarGridCodigoComercialDistribuidorTransportador();
        }
        else
            exibirMensagem(tipoMensagem.aviso, Localization.Transportadores.Transportador.CodigoExistente, Localization.Resources.Transportadores.Transportador.CodigoComercialDistribuidorCadastrada.format(_codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val()));

        limparCamposCodigoComercialDistribuidorTransportador();
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Transportadores.Transportador.CamposObrigatorios, Localization.Resources.Transportadores.Transportador.InformeCamposObrigatorios);
}

function cancelarCodigoComercialDistribuidorTransportadorClick() {
    limparCamposCodigoComercialDistribuidorTransportador();
}

function editarCodigoComercialDistribuidorTransportadorClick(registroSelecionado) {
    _codigoComercialDistribuidorTransportador.Atualizar.visible(true);
    _codigoComercialDistribuidorTransportador.Cancelar.visible(true);
    _codigoComercialDistribuidorTransportador.Excluir.visible(true);
    _codigoComercialDistribuidorTransportador.Adicionar.visible(false);

    EditarListEntity(_codigoComercialDistribuidorTransportador, registroSelecionado);
}

function excluirCodigoComercialDistribuidorTransportadorClick() {
    for (var i = 0; i < _transportador.CodigosComercialDistribuidor.list.length; i++) {
        var codigoComercialDistribuidorTransportador = _transportador.CodigosComercialDistribuidor.list[i];

        if (_codigoComercialDistribuidorTransportador.Codigo.val() == codigoComercialDistribuidorTransportador.Codigo.val)
            _transportador.CodigosComercialDistribuidor.list.splice(i, 1);
    }

    limparCamposCodigoComercialDistribuidorTransportador();
    recarregarGridCodigoComercialDistribuidorTransportador();
}

/*
 * Declaração das Funções
 */

function isCodigoComercialDistribuidorTransportadorExiste() {
    var existe = false;

    $.each(_transportador.CodigosComercialDistribuidor.list, function (i, codigoComercialDistribuidorTransportador) {
        console.log(codigoComercialDistribuidorTransportador.Codigo.val);
        console.log(_codigoComercialDistribuidorTransportador.Codigo.val());

        if (
            (codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val == _codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val()) &&
            (codigoComercialDistribuidorTransportador.Codigo.val != _codigoComercialDistribuidorTransportador.Codigo.val())
        ) {
            existe = true;

            return false;
        }
    });

    return existe;
}

function limparCamposCodigoComercialDistribuidorTransportador() {
    _codigoComercialDistribuidorTransportador.Atualizar.visible(false);
    _codigoComercialDistribuidorTransportador.Excluir.visible(false);
    _codigoComercialDistribuidorTransportador.Cancelar.visible(false);
    _codigoComercialDistribuidorTransportador.Adicionar.visible(true);

    LimparCampos(_codigoComercialDistribuidorTransportador);
}

function recarregarGridCodigoComercialDistribuidorTransportador() {
    var data = new Array();

    $.each(_transportador.CodigosComercialDistribuidor.list, function (i, codigoComercialDistribuidorTransportador) {
        data.push({
            CodigoComercialDistribuidor: codigoComercialDistribuidorTransportador.CodigoComercialDistribuidor.val,
            Codigo: codigoComercialDistribuidorTransportador.Codigo.val
        });
    });

    _gridCodigoComercialDistribuidorTransportador.CarregarGrid(data);
}