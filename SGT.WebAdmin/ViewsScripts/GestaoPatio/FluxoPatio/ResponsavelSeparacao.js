/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Validacao.js" />
/// <reference path="../../Consultas/Usuario.js" />

var _responsavelSeparacao;

var ResponsavelSeparacao = function () {
    this.Responsavel = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.GestaoPatio.FluxoPatio.ResponsavelSeparacao.getRequiredFieldDescription(), idBtnSearch: guid() });
    this.CapacidadeSeparacao = PropertyEntity({ getType: typesKnockout.int, val: ko.observable(0), def: 0, required: true, text: Localization.Resources.GestaoPatio.FluxoPatio.CapacidadeDeSeparacaoCaixas.getRequiredFieldDescription() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarResponsavelSeparacaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
}

function loadResponsavelSeparacao() {
    _responsavelSeparacao = new ResponsavelSeparacao();
    KoBindings(_responsavelSeparacao, "knockoutResponsavelSeparacao");

    new BuscarFuncionario(_responsavelSeparacao.Responsavel);
}

function adicionarResponsavelSeparacaoClick() {
    if (!ValidarCamposObrigatorios(_responsavelSeparacao)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    if (existeResponsavelSeparacaoAdicionado(_responsavelSeparacao.Responsavel.codEntity())) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, Localization.Resources.GestaoPatio.FluxoPatio.JaExisteUmRegistroParaResponsavel.format(_responsavelSeparacao.Responsavel.val()));
        return;
    }

    var responsavelAdicionar = {
        Codigo: guid(),
        CodigoResponsavel: _responsavelSeparacao.Responsavel.codEntity(),
        Descricao: _responsavelSeparacao.Responsavel.val(),
        CapacidadeSeparacao: _responsavelSeparacao.CapacidadeSeparacao.val()
    };

    _separacaoMercadoria.Separadores.val.push(responsavelAdicionar);
    Global.fecharModal('divModalResponsavelSeparacao');
}

function abrirModalResponsavelSeparacao() {
    $("#divModalResponsavelSeparacao")
        .modal('show')
        .one('hidden.bs.modal', function () {
            LimparCampos(_responsavelSeparacao);
        });
}

function recarregarGridSeparadores() {
    var registros = _separacaoMercadoria.Separadores.val();

    _gridSeparadores.CarregarGrid(registros);
}

function existeResponsavelSeparacaoAdicionado(codigo) {
    var registros = _gridSeparadores.BuscarRegistros();

    return registros.some(function (elemento) {
        return elemento.CodigoResponsavel == codigo;
    });
}