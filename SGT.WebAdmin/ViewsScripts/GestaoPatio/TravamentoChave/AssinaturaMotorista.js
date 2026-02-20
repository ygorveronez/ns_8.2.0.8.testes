/// <reference path="TravamentoChave.js" />

var _assinaturaMotoristaLiberacaoChave;

var canvasAssinaturaMotoristaLiberacaoChave;
var signaturePadAssinaturaMotoristaLiberacaoChave;

var AssinaturaMotorista = function () {
    this.AssinaturaMotorista = PropertyEntity({ text: "Assinatura Motorista: ", visible: ko.observable(true) });
    this.LimparAssinaturaMotorista = PropertyEntity({ text: "Limpar Assinatura", eventClick: limparAssinaturaMotoristaClick, type: types.event, enable: ko.observable(true) });
    
    this.Confirmar = PropertyEntity({ text: "Confirmar Assinatura", eventClick: confirmarAssinaturaClick, type: types.event, visible: ko.observable(true) });
}

function loadLiberacaoChaveAssinaturaMotorista() {
    _assinaturaMotoristaLiberacaoChave = new AssinaturaMotorista();
    KoBindings(_assinaturaMotoristaLiberacaoChave, "modalAssinaturaMotoristaLiberacaoChave");

    canvasAssinaturaMotoristaLiberacaoChave = document.getElementById(_assinaturaMotoristaLiberacaoChave.AssinaturaMotorista.id);
    signaturePadAssinaturaMotoristaLiberacaoChave = new SignaturePad(canvasAssinaturaMotoristaLiberacaoChave, { backgroundColor: 'rgb(255, 255, 255)' });

    window.onresize = resizeCanvasAssinaturaMotoristaLiberacaoChave;
}

function confirmarAssinaturaClick(e, sender) {
    if (validarAssinaturaMotoristaLiberacaoChave()) {
        var assinatura = obterAssinaturaMotoristaLiberacaoChave();
        
        executarReST("TravamentoChave/GravarAssinaturaMotorista", { FluxoGestaoPatio: _fluxoAtual.Codigo.val(), TravamentoChave: _travamentoChave.Codigo.val(), Imagem: assinatura }, function (r) {
            if (r.Success) {
                if (r.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Assinatura gravada com sucesso.");
                    Global.fecharModal("modalAssinaturaMotoristaLiberacaoChave");
                } else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", r.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
        });
    }
}

function exibirModalAssinaturaMotoristaEtapaLiberacaoChave() {
    buscarAssinatura();

    if (_travamentoChave.Etapa.val() == 30 || (_travamentoChave.Etapa.val() == 50 && !_travamentoChave.Travado.val())) {
        _assinaturaMotoristaLiberacaoChave.LimparAssinaturaMotorista.enable(false);
        _assinaturaMotoristaLiberacaoChave.Confirmar.visible(false);
        signaturePadAssinaturaMotoristaLiberacaoChave.off(); //desabilita o canvas
    } else {
        _assinaturaMotoristaLiberacaoChave.LimparAssinaturaMotorista.enable(true);
        _assinaturaMotoristaLiberacaoChave.Confirmar.visible(true);
        signaturePadAssinaturaMotoristaLiberacaoChave.on(); //habilita o canvas
    }


    Global.abrirModal("modalAssinaturaMotoristaLiberacaoChave");
    $("#modalAssinaturaMotoristaLiberacaoChave")
        .on("shown.bs.modal", resizeCanvasAssinaturaMotoristaLiberacaoChave)
        .on("hidden.bs.modal", limparCamposAssinaturaMotoristaLiberacaoChave);
}

function limparCamposAssinaturaMotoristaLiberacaoChave() {
    limparSignaturePadAssinaturaMotoristaLiberacaoChave();
    LimparCampos(_assinaturaMotoristaLiberacaoChave);
}

function preencherAssinaturaMotoristaLiberacaoChave(data) {
    setTimeout(function (data) {
        resizeCanvasAssinaturaMotoristaLiberacaoChave();

        setTimeout(function (data) {
            if (!string.IsNullOrWhiteSpace(data.AssinaturaMotorista))
                signaturePadAssinaturaMotoristaLiberacaoChave.fromDataURL(data.AssinaturaMotorista);
        }, 500, data);
    }, 1000, data);
}

function obterAssinaturaMotoristaLiberacaoChave() {
    var assinatura = !signaturePadAssinaturaMotoristaLiberacaoChave.isEmpty() ? canvasAssinaturaMotoristaLiberacaoChave.toDataURL() : "";
    return assinatura.replace("data:image/png;base64,", "");
}

function validarAssinaturaMotoristaLiberacaoChave() {
    if (signaturePadAssinaturaMotoristaLiberacaoChave.isEmpty()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar a assinatura do motorista.");
        return false;
    }

    return true;
}

function limparAssinaturaMotoristaClick() {
    signaturePadAssinaturaMotoristaLiberacaoChave.clear();
}

function resizeCanvasAssinaturaMotoristaLiberacaoChave() {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    var ratio = Math.max(window.devicePixelRatio || 1, 1);

    resizeCanvasAssinaturaMotoristaLiberacaoChaveElement(ratio, canvasAssinaturaMotoristaLiberacaoChave);

    limparSignaturePadAssinaturaMotoristaLiberacaoChave();
}

function resizeCanvasAssinaturaMotoristaLiberacaoChaveElement(ratio, canvas) {
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext("2d").scale(ratio, ratio);
}

function limparSignaturePadAssinaturaMotoristaLiberacaoChave() {
    signaturePadAssinaturaMotoristaLiberacaoChave.clear();
}

function buscarAssinatura() {
    executarReST("TravamentoChave/BuscarAssinaturaMotorista", { FluxoGestaoPatio: _fluxoAtual.Codigo.val(), TravamentoChave: _travamentoChave.Codigo.val() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (r.Data != null && r.Data != undefined && r.Data != "") {
                    let imagemBase64 = "data:image/png;base64,";
                    imagemBase64 += r.Data;

                    preencherAssinaturaMotoristaLiberacaoChave({ AssinaturaMotorista: imagemBase64 });
                }
            }
        } else
            exibirMensagem(tipoMensagem.falha, "Falha", r.Msg);
    });
}