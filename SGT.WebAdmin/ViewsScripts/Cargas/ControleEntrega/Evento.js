/*
 * Declaração de Objetos Globais do Arquivo
 */

var _eventosControleEntrega;

/*
 * Declaração das Classes
 */

var Eventos = function () {
    this.Carga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoEvento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Eventos.getRequiredFieldDescription(), val: ko.observable(true), options: EnumTipoEvento.obterOpcoes(), def: EnumTipoEvento.Espera });
    this.DataEvento = PropertyEntity({ text: Localization.Resources.Cargas.ControleEntrega.Data.getFieldDescription(), getType: typesKnockout.dateTime, def: Global.DataHoraAtual(), val: ko.observable(Global.DataHoraAtual()), visible: ko.observable(false) });    

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */


function exibirModalEventoControleEntrega() {
    Global.abrirModal("divModalEventosControleEntrega");
}

function loadEventosControleEntrega(carga) {
    _eventosControleEntrega = new Eventos();
    KoBindings(_eventosControleEntrega, "knockouEventosModalControleEntrega");

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware === EnumTipoServicoMultisoftware.MultiTMS) {
        _eventosControleEntrega.DataEvento.visible(true);
    }

    _eventosControleEntrega.Carga.val(carga);

    exibirModalEventoControleEntrega();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function adicionarClick(e, sender) {
    

    Salvar(_eventosControleEntrega, "ControleEntrega/AdicionarEvento", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _eventosControleEntrega.DataEvento.val(Global.DataHoraAtual());

                atualizarControleEntrega();

                Global.fecharModal("divModalEventosControleEntrega");
            }
            else
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, retorno.Msg);
    }, sender);
}

function cancelarClick() {
    LimparCampos(_eventosControleEntregas);
    Global.fecharModal("divModalEventosControleEntrega");
}


