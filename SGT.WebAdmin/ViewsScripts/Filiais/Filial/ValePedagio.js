/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Tranportador.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cadastroValePedagioTransportador;
var _gridValePedagioTransportador;
var _valePedagio;

/*
 * Declaração das Classes
 */

var CadastroValePedagioTransportador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.ComprarValePedagio = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, def: false, text: Localization.Resources.Filiais.Filial.ComprarValePedagio });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Filiais.Filial.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true) });
    this.TipoIntegracaoValePedagio = PropertyEntity({ val: ko.observable(_tiposIntegracaoValePedagio[0].value), options: _tiposIntegracaoValePedagio, text: Localization.Resources.Filiais.Filial.OperadoraValePedagio.getFieldDescription(), def: _tiposIntegracaoValePedagio[0].value, required: true, visible: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarValePedagioTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarValePedagioTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirValePedagioTransportadorClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var ValePedagio = function () {
    this.ListaTransportador = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarValePedagioTransportadorModalClick, type: types.event, text: Localization.Resources.Filiais.Filial.AdicionarTransportador });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridValePedagioTransportador() {
    var linhasPorPaginas = 5;
    var ordenacao = { column: 4, dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarValePedagioTransportadorClick, icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTransportador", visible: false },
        { data: "ComprarValePedagio", visible: false },
        { data: "TipoIntegracaoValePedagio", visible: false },
        { data: "DescricaoTransportador", title: Localization.Resources.Filiais.Filial.Transportador, width: "50%", className: "text-align-left", orderable: false },
        { data: "DescricaoComprarValePedagio", title: Localization.Resources.Filiais.Filial.ComprarValePedagio, width: "20%", className: "text-align-center", orderable: false },
        { data: "DescricaoTipoIntegracaoValePedagio", title: Localization.Resources.Filiais.Filial.OperadoraValePedagio, width: "20%", className: "text-align-center", orderable: false }
    ];

    _gridValePedagioTransportador = new BasicDataTable(_valePedagio.ListaTransportador.idGrid, header, menuOpcoes, ordenacao, null, linhasPorPaginas);
    _gridValePedagioTransportador.CarregarGrid([]);
}

function loadValePedagio() {
    _valePedagio = new ValePedagio();
    KoBindings(_valePedagio, "knockoutValePedagio");

    _cadastroValePedagioTransportador = new CadastroValePedagioTransportador();
    KoBindings(_cadastroValePedagioTransportador, "knockoutCadastroValePedagioTransportador");

    new BuscarTransportadores(_cadastroValePedagioTransportador.Transportador);

    loadGridValePedagioTransportador();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarValePedagioTransportadorClick() {
    if (!ValidarCamposObrigatorios(_cadastroValePedagioTransportador)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!validarCadastroValePedagioTransportadorDuplicado())
        return;

    _valePedagio.ListaTransportador.val().push(obterCadastroValePedagioTransportadorSalvar());

    recarregarGridValePedagioTransportador();
    fecharModalCadastroValePedagioTransportador();
}

function adicionarValePedagioTransportadorModalClick() {
    _cadastroValePedagioTransportador.Codigo.val(guid());
    _cadastroValePedagioTransportador.Transportador.enable(true);

    controlarBotoesCadastroValePedagioTransportadorHabilitados(false);
    exibirModalCadastroValePedagioTransportador();
}

function atualizarValePedagioTransportadorClick() {
    if (!ValidarCamposObrigatorios(_cadastroValePedagioTransportador)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }

    if (!validarCadastroValePedagioTransportadorDuplicado())
        return;

    var listaValePedagioTransportador = obterListaValePedagioTransportador();

    listaValePedagioTransportador.forEach(function (valePedagioTransportador, i) {
        if (_cadastroValePedagioTransportador.Codigo.val() == valePedagioTransportador.Codigo) {
            listaValePedagioTransportador.splice(i, 1, obterCadastroValePedagioTransportadorSalvar());
        }
    });

    _valePedagio.ListaTransportador.val(listaValePedagioTransportador);

    recarregarGridValePedagioTransportador();
    fecharModalCadastroValePedagioTransportador();
}

function editarValePedagioTransportadorClick(registroSelecionado) {
    PreencherObjetoKnout(_cadastroValePedagioTransportador, { Data: registroSelecionado });

    _cadastroValePedagioTransportador.Transportador.val(registroSelecionado.DescricaoTransportador);
    _cadastroValePedagioTransportador.Transportador.entityDescription(registroSelecionado.DescricaoTransportador);
    _cadastroValePedagioTransportador.Transportador.codEntity(registroSelecionado.CodigoTransportador);
    _cadastroValePedagioTransportador.Transportador.enable(false);

    controlarBotoesCadastroValePedagioTransportadorHabilitados(true);
    exibirModalCadastroValePedagioTransportador();
}

function excluirValePedagioTransportadorClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Gerais.Geral.DesejaExcluirRegistro, function () {
        removerValePedagioTransportador(_cadastroValePedagioTransportador.Codigo.val());
        fecharModalCadastroValePedagioTransportador();
    });
}

/*
 * Declaração das Funções Públicas
 */

function limparCamposValePedagio() {
    _valePedagio.ListaTransportador.val([]);
    recarregarGridValePedagioTransportador();
}

function preencherValePedagio(valePedagioTransportadores) {
    _valePedagio.ListaTransportador.val(valePedagioTransportadores);
    recarregarGridValePedagioTransportador();
}

function preencherValePedagioSalvar(filial) {
    filial["ValePedagioTransportadores"] = obterListaValePedagioTransportadorSalvar();
}

/*
 * Declaração das Funções
 */

function controlarBotoesCadastroValePedagioTransportadorHabilitados(isEdicao) {
    _cadastroValePedagioTransportador.Adicionar.visible(!isEdicao);
    _cadastroValePedagioTransportador.Atualizar.visible(isEdicao);
    _cadastroValePedagioTransportador.Excluir.visible(isEdicao);
}

function exibirModalCadastroValePedagioTransportador() {
    Global.abrirModal('divModalCadastroValePedagioTransportador');
    $("#divModalCadastroValePedagioTransportador").one('hidden.bs.modal', function () {
        LimparCampos(_cadastroValePedagioTransportador);
    });
}

function fecharModalCadastroValePedagioTransportador() {
    Global.fecharModal('divModalCadastroValePedagioTransportador');
}

function obterCadastroValePedagioTransportadorSalvar() {
    return {
        Codigo: _cadastroValePedagioTransportador.Codigo.val(),
        CodigoTransportador: _cadastroValePedagioTransportador.Transportador.codEntity(),
        ComprarValePedagio: _cadastroValePedagioTransportador.ComprarValePedagio.val(),
        TipoIntegracaoValePedagio: _cadastroValePedagioTransportador.TipoIntegracaoValePedagio.val(),
        DescricaoTransportador: _cadastroValePedagioTransportador.Transportador.val(),
        DescricaoComprarValePedagio: _cadastroValePedagioTransportador.ComprarValePedagio.val() ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
        DescricaoTipoIntegracaoValePedagio: _cadastroValePedagioTransportador.ComprarValePedagio.val() ? obterDescricaoTipoIntegracaoValePedagio(_cadastroValePedagioTransportador.TipoIntegracaoValePedagio.val()) : ""
    };
}

function obterDescricaoTipoIntegracaoValePedagio(tipoIntegracaoValePedagio) {
    for (var i = 0; i < _tiposIntegracaoValePedagio.length; i++) {
        if (_tiposIntegracaoValePedagio[i].value == tipoIntegracaoValePedagio)
            return _tiposIntegracaoValePedagio[i].text;
    }

    return "";
}

function obterListaValePedagioTransportador() {
    return _valePedagio.ListaTransportador.val().slice();
}

function obterListaValePedagioTransportadorSalvar() {
    var listaValePedagioTransportador = obterListaValePedagioTransportador();
    var listaValePedagioTransportadorSalvar = new Array();

    for (var i = 0; i < listaValePedagioTransportador.length; i++) {
        var valePedagioTransportador = listaValePedagioTransportador[i];

        listaValePedagioTransportadorSalvar.push({
            Codigo: valePedagioTransportador.Codigo,
            CodigoTransportador: valePedagioTransportador.CodigoTransportador,
            ComprarValePedagio: valePedagioTransportador.ComprarValePedagio,
            TipoIntegracaoValePedagio: valePedagioTransportador.TipoIntegracaoValePedagio
        });
    }

    return JSON.stringify(listaValePedagioTransportadorSalvar);
}

function recarregarGridValePedagioTransportador() {
    var listaValePedagioTransportador = obterListaValePedagioTransportador();

    _gridValePedagioTransportador.CarregarGrid(listaValePedagioTransportador);
}

function removerValePedagioTransportador(codigo) {
    var listaValePedagioTransportador = obterListaValePedagioTransportador();

    listaValePedagioTransportador.forEach(function (valePedagioTransportador, i) {
        if (codigo == valePedagioTransportador.Codigo)
            listaValePedagioTransportador.splice(i, 1);
    });

    _valePedagio.ListaTransportador.val(listaValePedagioTransportador);
    recarregarGridValePedagioTransportador();
}

function validarCadastroValePedagioTransportadorDuplicado() {
    var listaValePedagioTransportador = obterListaValePedagioTransportador();

    for (var i = 0; i < listaValePedagioTransportador.length; i++) {
        var valePedagioTransportador = listaValePedagioTransportador[i];

        if ((_cadastroValePedagioTransportador.Codigo.val() != valePedagioTransportador.Codigo) && (_cadastroValePedagioTransportador.Transportador.codEntity() == valePedagioTransportador.CodigoTransportador)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Gerais.Geral.RegistroDuplicadoMensagem);
            return false;
        }
    }

    return true;
}
