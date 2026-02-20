/*
 * Declaração de Objetos Globais do Arquivo
 */
var _alterarDestinatario;

/*
 * Declaração das Classes
 */
var AlterarDestinatario = function () {
    this.Entrega = PropertyEntity({ val: ko.observable(0), def: 0 });
    this.Destinatario = PropertyEntity({ val: ko.observable(""), def: "", codEntity: ko.observable(0), text: Localization.Resources.Cargas.ControleEntrega.Destinatario.getFieldDescription(), type: types.entity, idBtnSearch: guid() });
    this.Peso = PropertyEntity({ val: ko.observable('0,00'), getType: typesKnockout.decimal, enable: ko.observable(true), def: '0,00', maxlength: 18, text: Localization.Resources.Cargas.ControleEntrega.Peso.getFieldDescription() });

    this.Confirmar = PropertyEntity({ eventClick: confirmarAlteracaoDestinatarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Confirmar, visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarAlteracaoDestinatarioClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(true) });
}

/*
 * Declaração das Funções de Inicialização
 */
function loadAlterarDestinatario() {
    _alterarDestinatario = new AlterarDestinatario();

    KoBindings(_alterarDestinatario, "knockouAlterarDestinatario");

    new BuscarClientes(_alterarDestinatario.Destinatario);
}


/*
 * Declaração das Funções Associadas a Eventos
 */
function confirmarAlteracaoDestinatarioClick(e, sender) {
    Salvar(_alterarDestinatario, "ControleEntregaEntrega/AlterarDestinatario", function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                Global.fecharModal("divModalAlterarDestinatario");

                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.DadosAlteradosComSucesso);

                _entrega.NomeRecebedor.val(arg.Data.NomeRecebedor);
                _entrega.DocumentoRecebedor.val(arg.Data.DocumentoRecebedor);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function cancelarAlteracaoDestinatarioClick() {
    Global.fecharModal("divModalAlterarDestinatario");
}


/*
 * Declaração das Funções
 */

function AbrirModalAlteracaoDestinatario() {
    LimparCampos(_alterarDestinatario);
    _alterarDestinatario.Entrega.val(_entrega.Codigo.val());

    Global.abrirModal("divModalAlterarDestinatario");
}