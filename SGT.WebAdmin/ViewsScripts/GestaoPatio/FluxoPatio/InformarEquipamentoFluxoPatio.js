
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _informarEquipamentoFluxoPatio = false;

/*
 * Declaração das Classes
 */
var InformarEquipamentoFluxoPatio = function () {
    this.CodigoFluxoPatio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Equipamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Informar Equipamento:", idBtnSearch: guid(), required: ko.observable(true), visible: ko.observable(true), val: ko.observable(""), def: "" });

    this.Adicionar = PropertyEntity({ eventClick: informarEquipamentoFluxoPatioClick, type: types.event, text: ko.observable("Adicionar"), visible: true });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadInformarEquipamentoFluxoPatio() {
    _informarEquipamentoFluxoPatio = new InformarEquipamentoFluxoPatio();

    KoBindings(_informarEquipamentoFluxoPatio, "knockoutInformarEquipamentoFluxoPatio");

    BuscarEquipamentos(_informarEquipamentoFluxoPatio.Equipamento);

}
function informarEquipamentoFluxoPatioClick() {
    if (_informarEquipamentoFluxoPatio.Equipamento.codEntity() > 0)
        exibirConfirmacao("Confirmação", "Deseja realmente confirmar um novo equipamento?",  () => { informarEquipamentoFluxoPatio(); });
    else
        informarEquipamentoFluxoPatio();
}

function abrirModalInformarEquipamentoFluxoPatio() {
    $("#divModalInformarEquipamentoFluxoPatio").modal("show").on("hidden.bs.modal", function () {
        LimparCampos(_informarEquipamentoFluxoPatio);
        
    });
}

function exibirModalInformarEquipamentoFluxoPatio(codigoFluxoPatio) {

    _informarEquipamentoFluxoPatio.CodigoFluxoPatio.val(codigoFluxoPatio);
    executarReST("FluxoPatio/BuscarEquipamento", { FluxoGestaoPatio: _informarEquipamentoFluxoPatio.CodigoFluxoPatio.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_informarEquipamentoFluxoPatio, retorno);
                abrirModalInformarEquipamentoFluxoPatio();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    });
}


function fecharInformarEquipamentoFluxoPatioModal() {
    Global.fecharModal('divModalInformarEquipamentoFluxoPatio');
}

function informarEquipamentoFluxoPatio() {
    if (!ValidarCamposObrigatorios(_informarEquipamentoFluxoPatio)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
        return;
    }
    
    executarReST("FluxoPatio/InformarEquipamentoFluxoPatio", RetornarObjetoPesquisa(_informarEquipamentoFluxoPatio), function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Equipamento informado com sucesso");

                fecharInformarEquipamentoFluxoPatioModal();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}