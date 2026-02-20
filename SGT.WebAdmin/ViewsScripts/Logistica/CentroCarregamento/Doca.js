/// <reference path="CentroCarregamento.js" />
/// <reference path="../../Consultas/ModeloVeicularCarga.js" />
/// <reference path="../../Consultas/TipoCarga.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _gridDoca;
var _centroCarregamentoDoca;
var _doca;

/*
 * Declaração das Classes
 */

var CentroCarregamentoDoca = function () {
    this.LimiteCargasPorLocalCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.LimiteDeCargasPorLocalDeCarregamento.getFieldDescription(), getType: typesKnockout.int, maxlength: 3 });
    this.UtilizarLocalCarregamento = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.InformarDocaCarregamentoUtilizarLocalCarregamento), def: _CONFIGURACAO_TMS.InformarDocaCarregamentoUtilizarLocalCarregamento, getType: typesKnockout.bool, visible: false });
    this.Grid = PropertyEntity({ type: types.local });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDocaModalClick, type: types.event, text: ko.observable(Localization.Resources.Logistica.CentroCarregamento.AdicionarDoca), visible: true });
}

var Doca = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.CodigoDeIntegracaoQRCode.getFieldDescription(), maxlength: 100 });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), required: true, maxlength: 150 });
    this.Numero = PropertyEntity({ text: Localization.Resources.Logistica.CentroCarregamento.NumeroDaDoca.getRequiredFieldDescription(), getType: typesKnockout.int, configInt: { precision: 0, allowZero: false, thousands: "" }, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarDocaClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Adicionar), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarDocaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirDocaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadCentroCarregamentoDoca() {
    _centroCarregamentoDoca = new CentroCarregamentoDoca();
    KoBindings(_centroCarregamentoDoca, "knockoutDoca");

    _doca = new Doca();
    KoBindings(_doca, "knockoutCadastroDoca");

    loadGridDoca();
}

function loadGridDoca() {
    var opcaoEditar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarDocaClick };
    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [opcaoEditar] };
    var header = [
        { data: "Codigo", visible: false },
        { data: "Numero", title: Localization.Resources.Logistica.CentroCarregamento.NumeroDaDoca, width: "10%", className: 'text-align-center' },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "35%" },
        { data: "CodigoIntegracao", title: Localization.Resources.Logistica.CentroCarregamento.CodigoDeIntegracaoQRCode, width: "35%" }
    ];

    _gridDoca = new BasicDataTable(_centroCarregamentoDoca.Grid.id, header, menuOpcoes);
    _gridDoca.CarregarGrid([]);
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarDocaClick() {
    if (ValidarCamposObrigatorios(_doca)) {
        if (validarDadosDuplicados()) {
            _doca.Codigo.val(guid());

            var docas = _gridDoca.BuscarRegistros();

            docas.push(obterDocaSalvar());

            _gridDoca.CarregarGrid(docas);

            fecharDocaModal();
        }
    }
    else
        exibirMensagemCamposObrigatorio();
}

function adicionarDocaModalClick() {
    var isEdicao = false;

    controlarBotoesDocaHabilitados(isEdicao);
    exibirDocaModal();
}

function atualizarDocaClick() {
    if (ValidarCamposObrigatorios(_doca)) {
        if (validarDadosDuplicados()) {
            var docas = _gridDoca.BuscarRegistros();

            for (var i = 0; i < docas.length; i++) {
                if (_doca.Codigo.val() == docas[i].Codigo) {
                    docas[i] = obterDocaSalvar();
                    break;
                }
            }

            _gridDoca.CarregarGrid(docas);

            fecharDocaModal();
        }
    }
    else
        exibirMensagemCamposObrigatorio();
}

function editarDocaClick(registroSelecionado) {
    var isEdicao = true;

    preencherDadosDoca(registroSelecionado);
    controlarBotoesDocaHabilitados(isEdicao);
    exibirDocaModal();
}

function excluirDocaClick() {
    var docas = _gridDoca.BuscarRegistros();

    for (var i = 0; i < docas.length; i++) {
        if (_doca.Codigo.val() == docas[i].Codigo) {
            docas.splice(i, 1);
            break;
        }
    }

    _gridDoca.CarregarGrid(docas);
    fecharDocaModal();
}

/*
 * Declaração das Funções
 */

function limparCamposCentroCarregamentoDoca() {
    LimparCampos(_centroCarregamentoDoca);

    _gridDoca.CarregarGrid([]);
}

function preencherCentroCarregamentoDoca(dadosDoca) {
    PreencherObjetoKnout(_centroCarregamentoDoca, { Data: dadosDoca.Dados });

    _gridDoca.CarregarGrid(dadosDoca.Docas);
}

function preencherCentroCarregamentoDocaSalvar(centroCarregamento) {
    centroCarregamento["Docas"] = obterDocas();
    centroCarregamento["LimiteCargasPorLocalCarregamento"] = _centroCarregamentoDoca.LimiteCargasPorLocalCarregamento.val();
}

/*
 * Declaração das Funções Privadas
 */

function controlarBotoesDocaHabilitados(isEdicao) {
    _doca.Adicionar.visible(!isEdicao);
    _doca.Atualizar.visible(isEdicao);
    _doca.Excluir.visible(isEdicao);
}

function exibirDocaModal() {
    Global.abrirModal('divModalCadastroDoca');
    $("#divModalCadastroDoca").one('hidden.bs.modal', function () {
        LimparCampos(_doca);
    });
}

function fecharDocaModal() {
    Global.fecharModal('divModalCadastroDoca');
}

function obterDocas() {
    return JSON.stringify(_gridDoca.BuscarRegistros());
}

function obterDocaSalvar() {
    return {
        Codigo: _doca.Codigo.val(),
        CodigoIntegracao: _doca.CodigoIntegracao.val() ? _doca.CodigoIntegracao.val() : guid(),
        Descricao: _doca.Descricao.val(),
        Numero: _doca.Numero.val()
    };
}

function preencherDadosDoca(registroSelecionado) {
    _doca.Codigo.val(registroSelecionado.Codigo);
    _doca.CodigoIntegracao.val(registroSelecionado.CodigoIntegracao);
    _doca.Descricao.val(registroSelecionado.Descricao);
    _doca.Numero.val(registroSelecionado.Numero);
}

function validarDadosDuplicados() {
    var docas = _gridDoca.BuscarRegistros();

    for (var i = 0; i < docas.length; i++) {
        if (_doca.Codigo.val() != docas[i].Codigo) {
            if (_doca.Numero.val() == docas[i].Numero) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoDuplicado, Localization.Resources.Logistica.CentroCarregamento.PorFavorInformeOutroNumero);
                return false;
            }

            if (_doca.CodigoIntegracao.val() && (_doca.CodigoIntegracao.val() == docas[i].CodigoIntegracao)) {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CampoDuplicado, Localization.Resources.Logistica.CentroCarregamento.PorFavorInformeOutroCodigoDeIntegracao);
                return false;
            }
        }
    }

    return true;
}
