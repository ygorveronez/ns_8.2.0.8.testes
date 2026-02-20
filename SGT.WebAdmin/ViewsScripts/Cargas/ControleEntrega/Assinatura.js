/// <reference path="Entrega.js" />

var _assinatura;

var canvasAssinatura;
var signaturePadAssinatura;

var Assinatura = function () {
    this.Assinatura = PropertyEntity({ text: "Assinatura: ", visible: ko.observable(true) });
    this.LimparAssinatura = PropertyEntity({ text: "Limpar Assinatura", eventClick: limparAssinaturaClick, type: types.event, enable: ko.observable(true) });

    this.Confirmar = PropertyEntity({ text: "Confirmar Assinatura", eventClick: confirmarAssinaturaClick, type: types.event, visible: ko.observable(true) });
}

function loadAssinatura() {
    _assinatura = new Assinatura();
    KoBindings(_assinatura, "modalAssinaturaControleEntrega");

    canvasAssinatura = document.getElementById(_assinatura.Assinatura.id);
    signaturePadAssinatura = new SignaturePad(canvasAssinatura, { backgroundColor: 'rgb(255, 255, 255)' });

    window.onresize = resizeCanvasAssinatura;

    let modalAssinatura = $("#modalAssinaturaControleEntrega");
    modalAssinatura.on("shown.bs.modal", resizeCanvasAssinatura)
        .on("hidden.bs.modal", limparSignaturePadAssinatura);
}

function confirmarAssinaturaClick(e, sender) {
    if (validarAssinatura()) {
        var assinatura = obterAssinatura();

        if (assinatura.Assinatura != "") {
            var ass = assinatura.Assinatura.replace("data:image/png;base64,", ""); //Remove esse trecho, pois ao jogar a imagem na tela ele adiciona diretamente no html esse trecho no inicio da string
            _entrega.Assinatura.val(ass);
            _entrega.DadosRecebedorArquivoAssinatura.val(""); //Limpa o diretório da imagem carregada
        }

        Global.fecharModal("modalAssinaturaControleEntrega");
    }
}

function exibirModalAssinatura() {
    Global.abrirModal("modalAssinaturaControleEntrega");
}

function limparCamposAssinatura() {
    limparSignaturePadAssinatura();
    LimparCampos(_assinatura);
}

function preencherAssinatura() {
    setTimeout(function (data) {
        resizeCanvasAssinatura();

        setTimeout(function (data) {
            if (!string.IsNullOrWhiteSpace(data.AssinaturaMotorista))
                signaturePadAssinatura.fromDataURL(data.AssinaturaMotorista);
        }, 500, data);
    }, 1000, data);
}

function obterAssinatura() {
    var dados = {
        Assinatura: !signaturePadAssinatura.isEmpty() ? canvasAssinatura.toDataURL() : ""
    };

    return dados;
}

function validarAssinatura() {
    if (signaturePadAssinatura.isEmpty()) {
        exibirMensagem(tipoMensagem.atencao, "Atenção", "É necessário informar a assinatura.");
        return false;
    }

    return true;
}

function limparAssinaturaClick() {
    limparSignaturePadAssinatura();
}

function resizeCanvasAssinatura() {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    var ratio = Math.max(window.devicePixelRatio || 1, 1);

    resizeCanvasAssinaturaElement(ratio, canvasAssinatura);

    limparSignaturePadAssinatura();
}

function resizeCanvasAssinaturaElement(ratio, canvas) {
    canvas.width = canvas.offsetWidth * ratio;
    canvas.height = canvas.offsetHeight * ratio;
    canvas.getContext("2d").scale(ratio, ratio);
}

function limparSignaturePadAssinatura() {
    signaturePadAssinatura.clear();
}