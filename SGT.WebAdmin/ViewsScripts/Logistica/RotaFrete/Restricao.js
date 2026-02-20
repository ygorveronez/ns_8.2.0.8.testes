/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />

// #region Objetos Globais do Arquivo

var _gridRestricao;
var _listaRestricao;
var _restricao;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ListaRestricao = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRestricaoModalClick, type: types.event, text: ko.observable(Localization.Resources.Logistica.RotaFrete.AdicionarRestricao), visible: true });
}

var Restricao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.HoraInicio = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.HoraInicio.getRequiredFieldDescription(), getType: typesKnockout.time, required: true });
    this.HoraTermino = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.HoraTermino.getRequiredFieldDescription(), getType: typesKnockout.time, required: true });
    this.ModeloVeicular = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.ModeloVeicular.getFieldDescription(), idBtnSearch: guid(), required: false });
    this.TipoCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.TipoDeCarga.getFieldDescription(), idBtnSearch: guid(), required: false });

    this.Segunda = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.SegundaFeira, val: ko.observable(false), def: false });
    this.Terca = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.TercaFeira, val: ko.observable(false), def: false });
    this.Quarta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.QuartaFeira, val: ko.observable(false), def: false });
    this.Quinta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.QuintaFeira, val: ko.observable(false), def: false });
    this.Sexta = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.SextaFeira, val: ko.observable(false), def: false });
    this.Sabado = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.Sabado, val: ko.observable(false), def: false });
    this.Domingo = PropertyEntity({ getType: typesKnockout.bool, text: Localization.Resources.Logistica.RotaFrete.Domingo, val: ko.observable(false), def: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarRestricaoClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarRestricaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarRestricaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirRestricaoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadRestricao() {
    _listaRestricao = new ListaRestricao();
    KoBindings(_listaRestricao, "knockoutRestricaoRotaFrete");

    _restricao = new Restricao();
    KoBindings(_restricao, "knockoutCadastroRestricao");

    new BuscarModelosVeicularesCarga(_restricao.ModeloVeicular);
    new BuscarTiposdeCarga(_restricao.TipoCarga);

    loadGridRestricao();
}

function loadGridRestricao() {
    var opcaoEditar = { descricao: "Editar", id: guid(), metodo: editarRestricaoClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoTipoCarga", visible: false },
        { data: "CodigoModeloVeicular", visible: false },
        { data: "Segunda", visible: false },
        { data: "Terca", visible: false },
        { data: "Quarta", visible: false },
        { data: "Quinta", visible: false },
        { data: "Sexta", visible: false },
        { data: "Sabado", visible: false },
        { data: "Domingo", visible: false },
        { data: "DescricaoTipoCarga", title: Localization.Resources.Logistica.RotaFrete.TipoDeCarga, width: "16%" },
        { data: "DescricaoModeloVeicular", title: Localization.Resources.Logistica.RotaFrete.ModeloVeicular, width: "16%" },
        { data: "DescricaoDias", title: Localization.Resources.Logistica.RotaFrete.Dias, width: "26%" },
        { data: "HoraInicio", title: Localization.Resources.Logistica.RotaFrete.HoraInicio, width: "14%" },
        { data: "HoraTermino", title: Localization.Resources.Logistica.RotaFrete.HoraTermino, width: "14%" }
    ];

    _gridRestricao = new BasicDataTable(_listaRestricao.Grid.id, header, menuOpcoes);
    _gridRestricao.CarregarGrid(new Array());
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarRestricaoClick() {
    if (ValidarCamposObrigatorios(_restricao)) {
        if (validarDiasInformados()) {
            _restricao.Codigo.val(guid());

            var restricoes = _gridRestricao.BuscarRegistros();

            restricoes.push(obterRestricaoSalvar());

            _gridRestricao.CarregarGrid(restricoes);

            fecharRestricaoModal();
        }
        else
            exibirMensagemDiasNaoInformados();
    }
    else
        exibirMensagemCamposObrigatorioRestricao();
}

function adicionarRestricaoModalClick() {
    var isEdicao = false;

    controlarBotoesHabilitadosRestricao(isEdicao);
    exibirRestricaoModal();
}

function atualizarRestricaoClick() {
    if (ValidarCamposObrigatorios(_restricao)) {
        if (validarDiasInformados()) {
        var restricoes = _gridRestricao.BuscarRegistros();

        for (var i = 0; i < restricoes.length; i++) {
            if (_restricao.Codigo.val() == restricoes[i].Codigo) {
                restricoes[i] = obterRestricaoSalvar();
                break;
            }
        }

        _gridRestricao.CarregarGrid(restricoes);

            fecharRestricaoModal();
        }
        else
            exibirMensagemDiasNaoInformados();
    }
    else
        exibirMensagemCamposObrigatorioRestricao();
}

function cancelarRestricaoClick() {
    fecharRestricaoModal();
}

function editarRestricaoClick(registroSelecionado) {
    var isEdicao = true;
    
    preencherDadosRestricao(registroSelecionado);
    controlarBotoesHabilitadosRestricao(isEdicao);
    exibirRestricaoModal();
}

function excluirRestricaoClick() {
    var restricoes = _gridRestricao.BuscarRegistros();

    for (var i = 0; i < restricoes.length; i++) {
        if (_restricao.Codigo.val() == restricoes[i].Codigo) {
            restricoes.splice(i, 1);
            break;
        }
    }

    _gridRestricao.CarregarGrid(restricoes);
    fecharRestricaoModal();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposRestricao() {
    _gridRestricao.CarregarGrid(new Array());
}

function preencherRestricao(restricoes) {
    _gridRestricao.CarregarGrid(restricoes);
}

function preencherRestricaoSalvar(rotaFrete) {
    rotaFrete["Restricoes"] = JSON.stringify(obterListaRestricao());
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesHabilitadosRestricao(isEdicao) {
    _restricao.Adicionar.visible(!isEdicao);
    _restricao.Atualizar.visible(isEdicao);
    _restricao.Cancelar.visible(isEdicao);
    _restricao.Excluir.visible(isEdicao);
}

function exibirMensagemCamposObrigatorioRestricao() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Logistica.RotaFrete.PorFavorInformeCamposObrigatorios);
}

function exibirMensagemDiasNaoInformados() {
    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeUmDiaDaSemana);
}

function exibirRestricaoModal() {
    Global.abrirModal('divModalCadastroRestricao');
    $("#divModalCadastroRestricao").one('hidden.bs.modal', function () {
        LimparCampos(_restricao);
    });
}

function fecharRestricaoModal() {
    Global.fecharModal('divModalCadastroRestricao');
}

function obterDescricaoDias() {
    var dias = new Array();

    if (_restricao.Segunda.val()) dias.push(Localization.Resources.Logistica.RotaFrete.SegundaFeira);
    if (_restricao.Terca.val()) dias.push(Localization.Resources.Logistica.RotaFrete.TercaFeira);
    if (_restricao.Quarta.val()) dias.push(Localization.Resources.Logistica.RotaFrete.QuartaFeira);
    if (_restricao.Quinta.val()) dias.push(Localization.Resources.Logistica.RotaFrete.QuintaFeira);
    if (_restricao.Sexta.val()) dias.push(Localization.Resources.Logistica.RotaFrete.SextaFeira);
    if (_restricao.Sabado.val()) dias.push(Localization.Resources.Logistica.RotaFrete.Sabado);
    if (_restricao.Domingo.val()) dias.push(Localization.Resources.Logistica.RotaFrete.Domingo);

    if (dias.length === 7)
        return Localization.Resources.Logistica.RotaFrete.TodosOsDias;

    return dias.join(", ");
}

function obterListaRestricao() {
    return _gridRestricao.BuscarRegistros().slice();
}

function obterRestricaoSalvar() {
    return {
        Codigo: _restricao.Codigo.val(),
        CodigoTipoCarga: _restricao.TipoCarga.codEntity(),
        CodigoModeloVeicular: _restricao.ModeloVeicular.codEntity(),
        DescricaoDias: obterDescricaoDias(),
        DescricaoTipoCarga: _restricao.TipoCarga.val(),
        DescricaoModeloVeicular: _restricao.ModeloVeicular.val(),
        HoraInicio: _restricao.HoraInicio.val(),
        HoraTermino: _restricao.HoraTermino.val(),
        Segunda: _restricao.Segunda.val(),
        Terca: _restricao.Terca.val(),
        Quarta: _restricao.Quarta.val(),
        Quinta: _restricao.Quinta.val(),
        Sexta: _restricao.Sexta.val(),
        Sabado: _restricao.Sabado.val(),
        Domingo: _restricao.Domingo.val(),
    };
}

function preencherDadosRestricao(registroSelecionado) {
    _restricao.Codigo.val(registroSelecionado.Codigo);
    _restricao.TipoCarga.codEntity(registroSelecionado.CodigoTipoCarga);
    _restricao.TipoCarga.val(registroSelecionado.DescricaoTipoCarga);
    _restricao.ModeloVeicular.codEntity(registroSelecionado.CodigoModeloVeicular);
    _restricao.ModeloVeicular.val(registroSelecionado.DescricaoModeloVeicular);
    _restricao.HoraInicio.val(registroSelecionado.HoraInicio);
    _restricao.HoraTermino.val(registroSelecionado.HoraTermino);
    _restricao.Segunda.val(registroSelecionado.Segunda);
    _restricao.Terca.val(registroSelecionado.Terca);
    _restricao.Quarta.val(registroSelecionado.Quarta);
    _restricao.Quinta.val(registroSelecionado.Quinta);
    _restricao.Sexta.val(registroSelecionado.Sexta);
    _restricao.Sabado.val(registroSelecionado.Sabado);
    _restricao.Domingo.val(registroSelecionado.Domingo);
}

function validarDiasInformados() {
    return _restricao.Segunda.val() || _restricao.Terca.val() || _restricao.Quarta.val() || _restricao.Quinta.val() || _restricao.Sexta.val() || _restricao.Sabado.val() || _restricao.Domingo.val();
}

// #endregion Funções Privadas
