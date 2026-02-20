/*
 * Declaração de Objetos Globais do Arquivo
 */

var _alterarDataPrevisaoEntregaAjustada;

/*
 * Declaração das Classes
 */

var AlterarDataPrevisaoEntregaAjustada = function () {
    this.CodigoCargaEntrega = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.DataPrevisaoEntregaAjustada = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.DataPrevisaoEntregaAjustada, getType: typesKnockout.dateTime });
    this.TituloModalETA = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.TituloModalETA, getType: typesKnockout.dateTime });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlterarDataPrevisaoEntregaAjustadaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlterarDataPrevisaoEntregaAjustadaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadAlterarDataPrevisaoEntregaAjustada() {
    _alterarDataPrevisaoEntregaAjustada = new AlterarDataPrevisaoEntregaAjustada();
    KoBindings(_alterarDataPrevisaoEntregaAjustada, "knockouAlterarDataPrevisaoEntregaAjustada");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function confirmarAlterarDataPrevisaoEntregaAjustadaClick() {
    executarReST("ControleEntregaEntrega/AlterarDataEntregaAjustada", { CodigoCargaEntrega: _entrega.Codigo.val(), DataPrevisaoEntregaAjustada: _alterarDataPrevisaoEntregaAjustada.DataPrevisaoEntregaAjustada.val() }, function (arg) {
        if (arg.Success) {
            Global.fecharModal("divModalAlterarDataPrevisaoEntregaAjustada");
            Global.fecharModal("divModalEntrega");

            _entrega.DataPrevisaoEntregaAjustada.val(arg.Data.DataPrevisaoEntregaAjustada);

            exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, arg.Msg);
        } else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
    }, null);
}

function cancelarAlterarDataPrevisaoEntregaAjustadaClick() {
    Global.fecharModal("divModalDataPrevisaoEntregaAjustada");
}

/*
 * Declaração das Funções
 */

function AbrirModalAlterarDataPrevisaoEntregaAjustada() {
    LimparCampos(_alterarDataPrevisaoEntregaAjustada);
    _alterarDataPrevisaoEntregaAjustada.CodigoCargaEntrega.val(_entrega.Codigo.val());

    Global.abrirModal("divModalAlterarDataPrevisaoEntregaAjustada");
}