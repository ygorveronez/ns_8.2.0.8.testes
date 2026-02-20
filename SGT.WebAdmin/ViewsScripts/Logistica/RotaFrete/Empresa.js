/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/Tranportador.js" />

// #region Objetos Globais do Arquivo

var _gridEmpresa;
var _listaEmpresa;
var _empresa;

// #endregion Objetos Globais do Arquivo

// #region Classes

var ListaEmpresa = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ eventClick: adicionarEmpresaModalClick, type: types.event, text: ko.observable(Localization.Resources.Logistica.RotaFrete.AdicionarTransportador), visible: true });
}

var Empresa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.Transportador.getRequiredFieldDescription(), idBtnSearch: guid(), required: true });
    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Logistica.RotaFrete.ModeloVeicularCarga.getRequiredFieldDescription(), idBtnSearch: guid(), required: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga, visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga });
    this.PercentualCargasDaRota = PropertyEntity({ type: types.map, text: Localization.Resources.Logistica.RotaFrete.PercentualCargas.getRequiredFieldDescription(), required: !_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade, getType: typesKnockout.decimal, visible: ko.observable(!_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade), enable: ko.observable(true), maxlength: 6 });
    this.Prioridade = PropertyEntity({ text: Localization.Resources.Logistica.RotaFrete.Prioridade.getRequiredFieldDescription(), required: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade, getType: typesKnockout.int, visible: ko.observable(_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade), enable: ko.observable(true), maxlength: 6 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarEmpresaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarEmpresaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarEmpresaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirEmpresaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadEmpresa() {
    _listaEmpresa = new ListaEmpresa();
    KoBindings(_listaEmpresa, "knockoutEmpresaRotaFrete");

    _empresa = new Empresa();
    KoBindings(_empresa, "knockoutCadastroEmpresa");

    new BuscarTransportadores(_empresa.Empresa);
    new BuscarModelosVeicularesCarga(_empresa.ModeloVeicularCarga);

    loadGridEmpresa();
}

function loadGridEmpresa() {
    var ordenacaoPadrao = { column: (_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade ? 6 : 3), dir: orderDir.asc };
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarEmpresaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 10, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "CodigoEmpresa", visible: false },
        { data: "CodigoModeloVeicularCarga", visible: false },
        { data: "DescricaoEmpresa", title: Localization.Resources.Logistica.RotaFrete.Transportador, width: "70%" },
        { data: "DescricaoModeloVeicularCarga", title: Localization.Resources.Logistica.RotaFrete.ModeloVeicularCarga, width: "70%", visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorModeloVeicularCarga },
        { data: "PercentualCargasDaRota", title: Localization.Resources.Logistica.RotaFrete.PercentualCargas, width: "20%", className: "text-align-center", visible: !_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade },
        { data: "Prioridade", title: Localization.Resources.Logistica.RotaFrete.Prioridade, width: "20%", className: "text-align-center", visible: _CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade }
    ];

    _gridEmpresa = new BasicDataTable(_listaEmpresa.Grid.id, header, menuOpcoes, ordenacaoPadrao);
    _gridEmpresa.CarregarGrid(new Array());
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarEmpresaClick() {
    if (!validarCadastroEmpresa())
        return;

    _empresa.Codigo.val(guid());

    var empresas = _gridEmpresa.BuscarRegistros();

    empresas.push(obterEmpresaSalvar());

    _gridEmpresa.CarregarGrid(empresas);

    fecharEmpresaModal();
}

function adicionarEmpresaModalClick() {
    var isEdicao = false;

    controlarBotoesEmpresaHabilitados(isEdicao);
    exibirEmpresaModal();
}

function atualizarEmpresaClick() {
    if (!validarCadastroEmpresa())
        return;

    var empresas = _gridEmpresa.BuscarRegistros();

    for (var i = 0; i < empresas.length; i++) {
        if (_empresa.Codigo.val() == empresas[i].Codigo) {
            empresas[i] = obterEmpresaSalvar();
            break;
        }
    }

    _gridEmpresa.CarregarGrid(empresas);

    fecharEmpresaModal();
}

function cancelarEmpresaClick() {
    fecharEmpresaModal();
}

function editarEmpresaClick(registroSelecionado) {
    var isEdicao = true;

    preencherDadosEmpresa(registroSelecionado);
    controlarBotoesEmpresaHabilitados(isEdicao);
    exibirEmpresaModal();
}

function excluirEmpresaClick() {
    var empresas = _gridEmpresa.BuscarRegistros();

    for (var i = 0; i < empresas.length; i++) {
        if (_empresa.Codigo.val() == empresas[i].Codigo) {
            empresas.splice(i, 1);
            break;
        }
    }

    _gridEmpresa.CarregarGrid(empresas);
    fecharEmpresaModal();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function limparCamposEmpresa() {
    _gridEmpresa.CarregarGrid(new Array());
}

function preencherEmpresa(empresas) {
    _gridEmpresa.CarregarGrid(empresas);
}

function preencherEmpresaSalvar(rotaFrete) {
    rotaFrete["Empresas"] = JSON.stringify(obterListaEmpresa());
}

// #endregion Funções Públicas

// #region Funções Privadas

function controlarBotoesEmpresaHabilitados(isEdicao) {
    _empresa.Adicionar.visible(!isEdicao);
    _empresa.Atualizar.visible(isEdicao);
    _empresa.Cancelar.visible(isEdicao);
    _empresa.Excluir.visible(isEdicao);
}

function exibirEmpresaModal() {
    Global.abrirModal('divModalCadastroEmpresa');
    $("#divModalCadastroEmpresa").one('hidden.bs.modal', function () {
        LimparCampos(_empresa);
    });
}

function fecharEmpresaModal() {
    Global.fecharModal('divModalCadastroEmpresa');
}

function isCadastroEmpresaDuplicado() {
    var empresas = _gridEmpresa.BuscarRegistros();

    for (var i = 0; i < empresas.length; i++) {
        var empresa = empresas[i];

        if ((empresa.Codigo != _empresa.Codigo.val()) && (empresa.CodigoEmpresa == _empresa.Empresa.codEntity()) && (empresa.CodigoModeloVeicularCarga == _empresa.ModeloVeicularCarga.codEntity()))
            return true;
    }

    return false;
}

function isPercentualDeCargasExcedido() {
    if (_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade)
        return false;

    var empresas = _gridEmpresa.BuscarRegistros();
    var percentual = Globalize.parseFloat(_empresa.PercentualCargasDaRota.val())

    for (var i = 0; i < empresas.length; i++) {
        var empresa = empresas[i];

        if ((empresa.Codigo != _empresa.Codigo.val()) && (empresa.CodigoModeloVeicularCarga == _empresa.ModeloVeicularCarga.codEntity()))
            percentual += Globalize.parseFloat(empresa.PercentualCargasDaRota)
    }

    return (percentual > 100);
}

function isPrioridadeDuplicada() {
    if (!_CONFIGURACAO_TMS.DisponibilizarCargaParaTransportadoresPorPrioridade)
        return false;

    var empresas = _gridEmpresa.BuscarRegistros();

    for (var i = 0; i < empresas.length; i++) {
        var empresa = empresas[i];

        if ((empresa.Codigo != _empresa.Codigo.val()) && (empresa.Prioridade == _empresa.Prioridade.val()))
            return true;
    }

    return false;
}

function obterEmpresaSalvar() {
    return {
        Codigo: _empresa.Codigo.val(),
        CodigoEmpresa: _empresa.Empresa.codEntity(),
        CodigoModeloVeicularCarga: _empresa.ModeloVeicularCarga.codEntity(),
        DescricaoEmpresa: _empresa.Empresa.val(),
        DescricaoModeloVeicularCarga: _empresa.ModeloVeicularCarga.val(),
        PercentualCargasDaRota: _empresa.PercentualCargasDaRota.val(),
        Prioridade: _empresa.Prioridade.val()
    };
}

function obterListaEmpresa() {
    return _gridEmpresa.BuscarRegistros().slice();
}

function preencherDadosEmpresa(registroSelecionado) {
    _empresa.Codigo.val(registroSelecionado.Codigo);
    _empresa.Empresa.codEntity(registroSelecionado.CodigoEmpresa);
    _empresa.Empresa.val(registroSelecionado.DescricaoEmpresa);
    _empresa.ModeloVeicularCarga.codEntity(registroSelecionado.CodigoModeloVeicularCarga);
    _empresa.ModeloVeicularCarga.val(registroSelecionado.DescricaoModeloVeicularCarga);
    _empresa.PercentualCargasDaRota.val(registroSelecionado.PercentualCargasDaRota);
    _empresa.Prioridade.val(registroSelecionado.Prioridade);
}

function validarCadastroEmpresa() {
    if (!ValidarCamposObrigatorios(_empresa)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.RotaFrete.CamposObrigatorios, Localization.Resources.Logistica.RotaFrete.InformeOsCamposObrigatorios);
        return false;
    }

    if (isCadastroEmpresaDuplicado()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.RotaFrete.RegistroDuplicado, Localization.Resources.Logistica.RotaFrete.TransportadorInformadoJaCadastrado );
        return false;
    }

    if (isPercentualDeCargasExcedido()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.RotaFrete.PercentualInvalido, Localization.Resources.Logistica.RotaFrete.SomaDosPercentuais );
        return false;
    }

    if (isPrioridadeDuplicada()) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Logistica.RotaFrete.RegistroDuplicado, Localization.Resources.Logistica.RotaFrete.PrioridadeInformadaJaEstaCadastrada);
        return false;
    }

    return true;
}

// #endregion Funções Privadas
