/// <reference path="Pagamento.js" />

var _bloqueioDocumento;

var BloqueioDocumento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.MotivoBloqueio = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string, text: "*Motivo:", required: true });

    this.Cancelar = PropertyEntity({ eventClick: LimparModalBloqueioDocumento, type: types.event, text: "Cancelar", visible: ko.observable(true) });
    this.Desbloquear = PropertyEntity({ eventClick: DesbloquearDocumentoClick, type: types.event, text: "Desbloquear", visible: ko.observable(true) });
    this.Bloquear = PropertyEntity({ eventClick: BloquearDocumentoClick, type: types.event, text: "Bloquear", visible: ko.observable(true) });
}

function LoadBloqueioDocumento() {
    _bloqueioDocumento = new BloqueioDocumento();
    KoBindings(_bloqueioDocumento, "knockoutBloqueioDocumento");
}

function BloquearDocumentoClick() {
    if (!ValidarCamposObrigatorios(_bloqueioDocumento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    exibirConfirmacao("Atenção!", "Deseja realmente bloquear o documento?", function () {
        executarReST("Pagamento/BloquearDocumento", { Codigo: _bloqueioDocumento.Codigo.val(), MotivoBloqueio: _bloqueioDocumento.MotivoBloqueio.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Documento bloqueado com sucesso.");
                    _gridDocumentosSelecao.CarregarGrid();
                    LimparModalBloqueioDocumento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Falha!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function DesbloquearDocumentoClick() {
    if (!ValidarCamposObrigatorios(_bloqueioDocumento))
        return exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Por favor, informe os campos obrigatórios.");

    exibirConfirmacao("Atenção!", "Deseja realmente desbloquear o documento?", function () {
        executarReST("Pagamento/DesbloquearDocumento", { Codigo: _bloqueioDocumento.Codigo.val() }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso!", "Documento desbloqueado com sucesso.");
                    _gridDocumentosSelecao.CarregarGrid();
                    LimparModalBloqueioDocumento();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Falha!", r.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha!", r.Msg);
            }
        });
    });
}

function LimparModalBloqueioDocumento() {
    LimparCampos(_bloqueioDocumento);
    Global.fecharModal('divModalBloqueioDocumento');
}
