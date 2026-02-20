

var _coleta;

var Coleta = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Remetente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), val: ko.observable(""), def: "", text: Localization.Resources.Cargas.ControleEntrega.LocalDaColeta.getFieldDescription(), enable: ko.observable(true), visible: ko.observable(true), idBtnSearch: guid() });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNovaColetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarNovaColetaClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}


function adicionarColetaClick() {
    loadAdicionarColetaControleEntrega(_etapaAtualFluxo.Carga.val());
}


function exibirModalAdicionarColetaControleEntrega() {
    Global.abrirModal("divModalAdicionarColeta");
}

function loadAdicionarColetaControleEntrega(carga) {
    _coleta = new Coleta();
    KoBindings(_coleta, "knockouEventosModalAdicionarColeta");
    new BuscarClientes(_coleta.Remetente);
    _coleta.Carga.val(carga);
    exibirModalAdicionarColetaControleEntrega();
}

function adicionarNovaColetaClick(e, sender) {


    Salvar(_coleta, "ControleEntrega/AdicionarColeta", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AdicionadaComSucesso);

                atualizarControleEntrega();

                Global.fecharModal("divModalAdicionarColeta");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function cancelarNovaColetaClick() {
    LimparCampos(_coleta);

    Global.fecharModal("divModalAdicionarColeta");
}
