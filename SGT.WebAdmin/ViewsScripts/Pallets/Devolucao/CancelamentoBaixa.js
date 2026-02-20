
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _cancelamentoBaixaPallets;

/*
 * Declaração das Classes
 */

var CancelamentoBaixaPallets = function () {
    this.Devolucao = PropertyEntity({ val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.Motivo = PropertyEntity({ text: "Motivo:", maxlength: 2000, required: true });

    this.Confirmar = PropertyEntity({
        eventClick: function (e) {
            CancelarBaixaPalletsClick()
        }, type: types.event, text: "Confirmar", idGrid: guid(), visible: ko.observable(true)
    });
}

/*
 * Declaração das Funções de Inicialização
 */

function LoadCancelamentoBaixaPallets() {
    _cancelamentoBaixaPallets = new CancelamentoBaixaPallets();
    KoBindings(_cancelamentoBaixaPallets, "knoutCancelamentoBaixaPallets");
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AbrirTelaCancelamentoBaixaPalletsClick(devolucaoPalletsGrid) {
    LimparCamposCancelamentoBaixaPallets();
    _cancelamentoBaixaPallets.Devolucao.val(devolucaoPalletsGrid.Codigo);
    Global.abrirModal("divModalCancelamentoBaixaPallets");
}

/*
 * Declaração das Funções
 */
function LimparCamposCancelamentoBaixaPallets() {
    LimparCampos(_cancelamentoBaixaPallets);
}

function CancelarBaixaPalletsClick() {
    var valido = ValidarCamposObrigatorios(_cancelamentoBaixaPallets);

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");
        return;
    }

    exibirConfirmacao("Cancelamento de Baixa", "Tem certeza que deseja cancelar a baixa da devolução?", function () {
        var dados = RetornarObjetoPesquisa(_cancelamentoBaixaPallets);
        executarReST("Devolucao/CancelarBaixa", dados, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    Global.fecharModal('divModalCancelamentoBaixaPallets');
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Cancelamento realizado com sucesso!");
                    _gridDevolucaoPallets.CarregarGrid();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}