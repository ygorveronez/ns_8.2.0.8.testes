/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../Consultas/Cliente.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="../../Enumeradores/EnumTipoTransportadorTerceiroCentroCarregamento.js" />
/// <reference path="CentroCarregamento.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _centroCarregamentoTransportadorTerceiro;
var _centroCarregamentoTransportadorTerceiroCadastro;
var _crudCentroCarregamentoTransportadorTerceiroCadastro;
var _gridCentroCarregamentoTransportadorTerceiro;

/*
 * Declaração das Classes
 */

var CRUDCentroCarregamentoTransportadorTerceiroCadastro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarCentroCarregamentoTransportadorTerceiroClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCentroCarregamentoTransportadorTerceiroClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirCentroCarregamentoTransportadorTerceiroClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

var CentroCarregamentoTransportadorTerceiro = function () {
    var self = this;

    this.LiberarCargaManualmenteParaTransportadoresTerceiros = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LiberarCargasMaualmenteParaAsTransportadorasTerceiros, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.LiberarCargaAutomaticamenteParaTransportadorasTerceiros = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LiberarCargasAutomaticamenteParaAsTransportadorasTerceiros, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true) });
    this.TipoTransportadorTerceiroCentroCarregamento = PropertyEntity({ val: ko.observable(EnumTipoTransportadorTerceiroCentroCarregamento.Todos), options: EnumTipoTransportadorTerceiroCentroCarregamento.obterOpcoes(), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeTransportadorTerceiro.getFieldDescription(), def: EnumTipoTransportadorTerceiroCentroCarregamento.Todos });
    this.ListaCentroCarregamentoTransportadorTerceiro = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), idBtnSearch: guid() });
    this.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.int, text: Localization.Resources.Logistica.CentroCarregamento.TempoParaAguardarInteresseDoTransportadorMinutos.getFieldDescription(), maxlength: 7, visible: ko.observable(false) });
    this.TipoTransportadorTerceiroSecundarioCentroCarregamento = PropertyEntity({ val: ko.observable(EnumTipoTransportadorTerceiroCentroCarregamento.Nenhum), options: EnumTipoTransportadorTerceiroCentroCarregamento.obterOpcoesSecundario(), text: Localization.Resources.Logistica.CentroCarregamento.TipoDeTransportadorSecundario.getFieldDescription(), def: EnumTipoTransportadorTerceiroCentroCarregamento.Nenhum, visible: ko.observable(false) });

    this.ListaCentroCarregamentoTransportadorTerceiro.val.subscribe(function () {
        recarregarGridCentroCarregamentoTransportadorTerceiro();
    });

    this.TipoTransportadorTerceiroCentroCarregamento.val.subscribe(function (tipoSelecionado) {
        var tipoPorFila = tipoSelecionado == EnumTipoTransportadorTerceiroCentroCarregamento.PorPrioridadeFilaCarregamento;

        self.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente.visible(tipoPorFila);
        self.TipoTransportadorTerceiroSecundarioCentroCarregamento.visible(tipoPorFila);

        if (!tipoPorFila) {
            self.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente.val("");
            self.TipoTransportadorTerceiroSecundarioCentroCarregamento.val(EnumTipoTransportadorTerceiroCentroCarregamento.Todos);
        }
    });

    this.LiberarCargaManualmenteParaTransportadoresTerceiros.val.subscribe(function (valor) {
        if (valor) {
            self.LiberarCargaAutomaticamenteParaTransportadorasTerceiros.val(false);
            self.LiberarCargaAutomaticamenteParaTransportadorasTerceiros.visible(false);
        }
        else
            self.LiberarCargaAutomaticamenteParaTransportadorasTerceiros.visible(true);
    });

    this.LiberarCargaAutomaticamenteParaTransportadorasTerceiros.val.subscribe(function (valor) {
        if (valor) {
            self.LiberarCargaManualmenteParaTransportadoresTerceiros.val(false);
            self.LiberarCargaManualmenteParaTransportadoresTerceiros.visible(false);
        }
        else
            self.LiberarCargaManualmenteParaTransportadoresTerceiros.visible(true);
    });

    this.Adicionar = PropertyEntity({ eventClick: adicionarCentroCarregamentoTransportadorTerceiroModalClick, type: types.event, text: Localization.Resources.Logistica.CentroCarregamento.AdicionarTransportadorTerceiro });
}

var CentroCarregamentoTransportadorTerceiroCadastro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: Localization.Resources.Logistica.CentroCarregamento.TransportadorTerceiro.getRequiredFieldDescription(), idBtnSearch: guid() });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCentroCarregamentoTransportadorTerceiro() {
    _centroCarregamentoTransportadorTerceiro = new CentroCarregamentoTransportadorTerceiro();
    KoBindings(_centroCarregamentoTransportadorTerceiro, "knockoutCentroCarregamentoTransportadorTerceiro");

    _centroCarregamentoTransportadorTerceiroCadastro = new CentroCarregamentoTransportadorTerceiroCadastro();
    KoBindings(_centroCarregamentoTransportadorTerceiroCadastro, "knockoutCentroCarregamentoTransportadorTerceiroCadastro");

    _crudCentroCarregamentoTransportadorTerceiroCadastro = new CRUDCentroCarregamentoTransportadorTerceiroCadastro();
    KoBindings(_crudCentroCarregamentoTransportadorTerceiroCadastro, "knockoutCRUDCentroCarregamentoTransportadorTerceiroCadastro");

    loadGridCentroCarregamentoTransportadorTerceiro();

    new BuscarClientes(_centroCarregamentoTransportadorTerceiroCadastro.Transportador);
}

function loadGridCentroCarregamentoTransportadorTerceiro() {
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarCentroCarregamentoTransportadorTerceiroClick }] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTransportador", visible: false },
        { data: "DescricaoTransportador", title: Localization.Resources.Gerais.Geral.Descricao, width: "80%" }
    ];

    _gridCentroCarregamentoTransportadorTerceiro = new BasicDataTable(_centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.idGrid, header, menuOpcoes);
    _gridCentroCarregamentoTransportadorTerceiro.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarCentroCarregamentoTransportadorTerceiroClick() {
    if (validarCentroCarregamentoTransportadorTerceiro()) {
        _centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.val().push(obterCentroCarregamentoTransportadorTerceiroSalvar());

        recarregarGridCentroCarregamentoTransportadorTerceiro();
        fecharModalCadastroCentroCarregamentoTransportadorTerceiro();
    }
}

function adicionarCentroCarregamentoTransportadorTerceiroModalClick() {
    _centroCarregamentoTransportadorTerceiroCadastro.Codigo.val(guid());

    controlarBotoesCentroCarregamentoTransportadorTerceiroCadastroHabilitados(false);
    exibirModalCadastroCentroCarregamentoTransportadorTerceiro();
}

function atualizarCentroCarregamentoTransportadorTerceiroClick() {
    if (validarCentroCarregamentoTransportadorTerceiro()) {
        var listaCentroCarregamentoTransportadorTerceiro = obterListaCentroCarregamentoTransportadorTerceiro();

        for (var i = 0; i < listaCentroCarregamentoTransportadorTerceiro.length; i++) {
            if (_centroCarregamentoTransportadorTerceiroCadastro.Codigo.val() == listaCentroCarregamentoTransportadorTerceiro[i].Codigo) {
                listaCentroCarregamentoTransportadorTerceiro.splice(i, 1, obterCentroCarregamentoTransportadorTerceiroSalvar());
                break;
            }
        }

        _centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.val(listaCentroCarregamentoTransportadorTerceiro)

        fecharModalCadastroCentroCarregamentoTransportadorTerceiro();
    }
}

function editarCentroCarregamentoTransportadorTerceiroClick(registroSelecionado) {
    var centroCarregamentoTransportadorTerceiro = obterCentroCarregamentoTransportadorTerceiroPorCodigo(registroSelecionado.Codigo);

    if (centroCarregamentoTransportadorTerceiro) {
        _centroCarregamentoTransportadorTerceiroCadastro.Codigo.val(centroCarregamentoTransportadorTerceiro.Codigo);
        _centroCarregamentoTransportadorTerceiroCadastro.Transportador.codEntity(centroCarregamentoTransportadorTerceiro.Transportador.Codigo);
        _centroCarregamentoTransportadorTerceiroCadastro.Transportador.entityDescription(centroCarregamentoTransportadorTerceiro.Transportador.Descricao);
        _centroCarregamentoTransportadorTerceiroCadastro.Transportador.val(centroCarregamentoTransportadorTerceiro.Transportador.Descricao);

        controlarBotoesCentroCarregamentoTransportadorTerceiroCadastroHabilitados(true);
        exibirModalCadastroCentroCarregamentoTransportadorTerceiro();
    }
}

function excluirCentroCarregamentoTransportadorTerceiroClick() {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Logistica.CentroCarregamento.RealmenteDesejaExcluirTransportador, function () {
        var listaCentroCarregamentoTransportadorTerceiro = obterListaCentroCarregamentoTransportadorTerceiro();

        for (var i = 0; i < listaCentroCarregamentoTransportadorTerceiro.length; i++) {
            if (_centroCarregamentoTransportadorTerceiroCadastro.Codigo.val() == listaCentroCarregamentoTransportadorTerceiro[i].Codigo)
                listaCentroCarregamentoTransportadorTerceiro.splice(i, 1);
        }

        _centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.val(listaCentroCarregamentoTransportadorTerceiro);

        fecharModalCadastroCentroCarregamentoTransportadorTerceiro();
    });
}

/*
 * Declaração das Funções Públicas
 */

function preencherCentroCarregamentoTransportadorTerceiro(dadosTransportadorTerceiro) {
    PreencherObjetoKnout(_centroCarregamentoTransportadorTerceiro, { Data: dadosTransportadorTerceiro });

    _centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.val(dadosTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro);
}

function preencherCentroCarregamentoTransportadorTerceiroSalvar(centroCarregamento) {
    centroCarregamento["LiberarCargaManualmenteParaTransportadoresTerceiros"] = _centroCarregamentoTransportadorTerceiro.LiberarCargaManualmenteParaTransportadoresTerceiros.val();
    centroCarregamento["LiberarCargaAutomaticamenteParaTransportadorasTerceiros"] = _centroCarregamentoTransportadorTerceiro.LiberarCargaAutomaticamenteParaTransportadorasTerceiros.val();
    centroCarregamento["TipoTransportadorTerceiroCentroCarregamento"] = _centroCarregamentoTransportadorTerceiro.TipoTransportadorTerceiroCentroCarregamento.val();
    centroCarregamento["ListaCentroCarregamentoTransportadorTerceiro"] = obterListaCentroCarregamentoTransportadorTerceiroSalvar();
    centroCarregamento["TipoTransportadorTerceiroSecundarioCentroCarregamento"] = _centroCarregamentoTransportadorTerceiro.TipoTransportadorTerceiroSecundarioCentroCarregamento.val();
    centroCarregamento["TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente"] = _centroCarregamentoTransportadorTerceiro.TempoAguardarInteresseTransportadorTerceiroParaCargaLiberadaAutomaticamente.val();
}

function limparCamposCentroCarregamentoTransportadorTerceiro() {
    LimparCampos(_centroCarregamentoTransportadorTerceiro);

    _centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.val([]);
}

/*
 * Declaração das Funções
 */

function controlarBotoesCentroCarregamentoTransportadorTerceiroCadastroHabilitados(isEdicao) {
    _crudCentroCarregamentoTransportadorTerceiroCadastro.Atualizar.visible(isEdicao);
    _crudCentroCarregamentoTransportadorTerceiroCadastro.Excluir.visible(isEdicao);
    _crudCentroCarregamentoTransportadorTerceiroCadastro.Adicionar.visible(!isEdicao);
}

function exibirModalCadastroCentroCarregamentoTransportadorTerceiro() {
    Global.abrirModal('divModalCadastroCentroCarregamentoTransportadorTerceiro');
    $("#divModalCadastroCentroCarregamentoTransportadorTerceiro").one('hidden.bs.modal', function () {
        limparCamposCentroCarregamentoTransportadorTerceiroCadastro();
    });
}

function fecharModalCadastroCentroCarregamentoTransportadorTerceiro() {
    Global.fecharModal('divModalCadastroCentroCarregamentoTransportadorTerceiro');
}

function limparCamposCentroCarregamentoTransportadorTerceiroCadastro() {
    LimparCampos(_centroCarregamentoTransportadorTerceiroCadastro);
}

function obterCentroCarregamentoTransportadorTerceiroPorCodigo(codigo) {
    var listaCentroCarregamentoTransportadorTerceiro = obterListaCentroCarregamentoTransportadorTerceiro();

    for (var i = 0; i < listaCentroCarregamentoTransportadorTerceiro.length; i++) {
        var centroCarregamentoTransportadorTerceiro = listaCentroCarregamentoTransportadorTerceiro[i];

        if (codigo == centroCarregamentoTransportadorTerceiro.Codigo)
            return centroCarregamentoTransportadorTerceiro;
    }

    return undefined;
}

function obterCentroCarregamentoTransportadorTerceiroSalvar() {

    return {
        Codigo: _centroCarregamentoTransportadorTerceiroCadastro.Codigo.val(),
        Transportador: {
            Codigo: _centroCarregamentoTransportadorTerceiroCadastro.Transportador.codEntity(),
            Descricao: _centroCarregamentoTransportadorTerceiroCadastro.Transportador.val()
        }
    };
}

function obterListaCentroCarregamentoTransportadorTerceiro() {
    return _centroCarregamentoTransportadorTerceiro.ListaCentroCarregamentoTransportadorTerceiro.val().slice();
}

function obterListaCentroCarregamentoTransportadorTerceiroSalvar() {
    var listaCentroCarregamentoTransportadorTerceiro = obterListaCentroCarregamentoTransportadorTerceiro();
    var listaCentroCarregamentoTransportadorTerceiroSalvar = new Array();

    for (var i = 0; i < listaCentroCarregamentoTransportadorTerceiro.length; i++) {
        var centroCarregamentoTransportadorTerceiro = listaCentroCarregamentoTransportadorTerceiro[i];

        listaCentroCarregamentoTransportadorTerceiroSalvar.push({
            Codigo: centroCarregamentoTransportadorTerceiro.Codigo,
            Transportador: centroCarregamentoTransportadorTerceiro.Transportador.Codigo
        });
    }

    return JSON.stringify(listaCentroCarregamentoTransportadorTerceiroSalvar);
}

function recarregarGridCentroCarregamentoTransportadorTerceiro() {
    var listaCentroCarregamentoTransportadorTerceiro = obterListaCentroCarregamentoTransportadorTerceiro();
    var listaCentroCarregamentoTransportadorTerceiroCarregar = new Array();

    for (var i = 0; i < listaCentroCarregamentoTransportadorTerceiro.length; i++) {
        var centroCarregamentoTransportadorTerceiro = listaCentroCarregamentoTransportadorTerceiro[i];

        listaCentroCarregamentoTransportadorTerceiroCarregar.push({
            Codigo: centroCarregamentoTransportadorTerceiro.Codigo,
            CodigoTransportador: centroCarregamentoTransportadorTerceiro.Transportador.Codigo,
            DescricaoTransportador: centroCarregamentoTransportadorTerceiro.Transportador.Descricao
        });
    }

    _gridCentroCarregamentoTransportadorTerceiro.CarregarGrid(listaCentroCarregamentoTransportadorTerceiroCarregar);
}

function validarCentroCarregamentoTransportadorTerceiro() {
    if (!ValidarCamposObrigatorios(_centroCarregamentoTransportadorTerceiroCadastro)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    var listaCentroCarregamentoTransportadorTerceiro = obterListaCentroCarregamentoTransportadorTerceiro();

    for (var i = 0; i < listaCentroCarregamentoTransportadorTerceiro.length; i++) {
        var centroCarregamentoTransportadorTerceiro = listaCentroCarregamentoTransportadorTerceiro[i];

        if (
            (centroCarregamentoTransportadorTerceiro.Codigo != _centroCarregamentoTransportadorTerceiroCadastro.Codigo.val()) &&
            (centroCarregamentoTransportadorTerceiro.Transportador.Codigo == _centroCarregamentoTransportadorTerceiroCadastro.Transportador.codEntity())
        ) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.RegistroDuplicado, Localization.Resources.Logistica.CentroCarregamento.TransportadorInformadoJaEstaCadastrado);
            return false;
        }
    }

    return true;
}
