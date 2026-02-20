/// <reference path="../DadosCarga/Carga.js" />
/// <reference path="DadosTransporte.js" />
/// <reference path="LiberacaoSemIntegracaoGRAnexo.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _liberacaoSemIntegracaoGR;

/*
 * Declaração das Classes
 */

var LiberacaoSemIntegracaoGR = function () {
    this.KnoutCarga = PropertyEntity({ ko: null, type: types.local });

    this.ProtocoloIntegracaoGR = PropertyEntity({ text: Localization.Resources.Cargas.Carga.ProtocoloIntegracaoGR.getRequiredFieldDescription(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.ListaAnexo = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Anexos, type: types.map, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), visible: ko.observable(true) });

    this.ListaAnexo.val.subscribe(function () {
        recarregarGridLiberacaoSemIntegracaoGRAnexo();
    });

    this.AdicionarAnexo = PropertyEntity({ eventClick: adicionarLiberacaoSemIntegracaoGRAnexoModalClick, type: types.event, text: Localization.Resources.Cargas.Carga.AdicionarAnexos, visible: ko.observable(true) });
    this.Confirmar = PropertyEntity({ eventClick: confirmarLiberacaoSemIntegracaoGRClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function loadLiberacaoSemIntegracaoGR() {
    _liberacaoSemIntegracaoGR = new LiberacaoSemIntegracaoGR();
    KoBindings(_liberacaoSemIntegracaoGR, "knoutLiberacaoSemIntegracaoGR");

    loadLiberacaoSemIntegracaoGRAnexo();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarLiberacaoSemIntegracaoGRClick() {
    if (!validarCamposObrigatoriosLiberacaoSemIntegracaoGR())
        return;

    _liberacaoSemIntegracaoGR.KnoutCarga.ko.LiberadoComProblemaIntegracaoGrMotoristaVeiculo.val(true);
    _liberacaoSemIntegracaoGR.KnoutCarga.ko.SalvarDadosTransporteSemSolicitarNFes.val(false);
    _liberacaoSemIntegracaoGR.KnoutCarga.ko.ProtocoloIntegracaoGR.val(_liberacaoSemIntegracaoGR.ProtocoloIntegracaoGR.val());

    SalvarDadosTransporteClick(_liberacaoSemIntegracaoGR.KnoutCarga.ko);
}

function exibirModalLiberacaoSemIntegracaoGR(e) {
    _liberacaoSemIntegracaoGR.KnoutCarga.ko = e;

    Global.abrirModal("divModalLiberacaoSemIntegracaoGR");
    
    $('#divModalLiberacaoSemIntegracaoGR').one('hidden.bs.modal', function () {
        limparCamposLiberacaoSemIntegracaoGR();
    });
}

function fecharModalLiberacaoSemIntegracaoGR() {
    Global.fecharModal("divModalLiberacaoSemIntegracaoGR");
}

function DetalhesLiberarComProblemaIntegracaoGrMotoristaVeiculoClick(e) {
    _liberacaoSemIntegracaoGR.KnoutCarga.ko = e;
    _liberacaoSemIntegracaoGR.ProtocoloIntegracaoGR.val(e.ProtocoloIntegracaoGR.val());

    _liberacaoSemIntegracaoGR.ProtocoloIntegracaoGR.enable(false);
    _liberacaoSemIntegracaoGR.AdicionarAnexo.visible(false);
    _liberacaoSemIntegracaoGR.Confirmar.visible(false);

    carregarAnexosLiberacaoSemIntegracaoGR();

    Global.abrirModal("divModalLiberacaoSemIntegracaoGR");

    $('#divModalLiberacaoSemIntegracaoGR').one('hidden.bs.modal', function () {
        limparCamposLiberacaoSemIntegracaoGR();
    });
}

/*
 * Declaração das Funções Privadas
 */

function limparCamposLiberacaoSemIntegracaoGR() {
    LimparCampos(_liberacaoSemIntegracaoGR);

    _liberacaoSemIntegracaoGR.KnoutCarga.ko = null;
    _liberacaoSemIntegracaoGR.ListaAnexo.val(new Array());

    _liberacaoSemIntegracaoGR.ProtocoloIntegracaoGR.enable(true);
    _liberacaoSemIntegracaoGR.AdicionarAnexo.visible(true);
    _liberacaoSemIntegracaoGR.Confirmar.visible(true);
}

function validarCamposObrigatoriosLiberacaoSemIntegracaoGR() {
    if (!ValidarCamposObrigatorios(_liberacaoSemIntegracaoGR)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return false;
    }

    if (obterListaLiberacaoSemIntegracaoGRAnexo().length == 0) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Cargas.Carga.PorFavorAdicioneUmOuMaisAnexos);
        return false;
    }

    return true;
}