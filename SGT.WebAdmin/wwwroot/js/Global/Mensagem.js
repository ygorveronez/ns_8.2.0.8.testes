$(document).ready(function () {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 100,
        "timeOut": 7000,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
});

var tipoMensagem = { ok: "ok", atencao: "atencao", aviso: "aviso", falha: "falha" };

function ocultarTodasAsNotificacoes() {
    toastr.clear();
}

function exibirAlertaNotificacao(titulo, icone, data, timeout) {
    if (titulo.length > 50)
        titulo = titulo.substring(0, 50) + "...";

    toastr["info"]("<i class='fal fa-clock'></i> <i>" + data + "</i>", titulo, { timeout: timeout || 6000 });
}

function exibirMensagem(tipo, titulo, mensagem, timeout) {
    if (tipo == tipoMensagem.ok)
        toastr["success"](mensagem, titulo, { timeout: timeout || 5000 });
    else if (tipo == tipoMensagem.atencao)
        toastr["warning"](mensagem, titulo, { timeout: timeout || 6000 });
    else if (tipo == tipoMensagem.aviso)
        toastr["info"](mensagem, titulo, { timeout: timeout || 6000 });
    else if (tipo == tipoMensagem.falha)
        toastr["error"](mensagem, titulo, { timeout: timeout || 10000 });
}

function exibirConfirmacao(titulo, mensagem, callbackConfirma, callbackCancela, btnTrue, btnFalse) {
    let swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success waves-effect waves-themed mt-1',
            cancelButton: 'btn btn-danger waves-effect waves-themed mx-1 mx-sm-3 mt-1'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: titulo,
        html: mensagem,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: btnTrue || Localization.Resources.Gerais.Geral.Sim,
        cancelButtonText: btnFalse || Localization.Resources.Gerais.Geral.Nao,
        reverseButtons: true
    }).then((result) => {

        if (result.value) {
            callbackConfirma();
        } else if (result.value != true && callbackCancela) {
            callbackCancela();
        }

    });
}

function exibirConfirmacaoComTamanhoMaior(titulo, mensagem, callbackConfirma, callbackCancela, btnTrue, btnFalse) {
    let swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success waves-effect waves-themed mt-1',
            cancelButton: 'btn btn-danger waves-effect waves-themed mx-1 mx-sm-3 mt-1'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: titulo,
        html: mensagem,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: btnTrue || Localization.Resources.Gerais.Geral.Sim,
        cancelButtonText: btnFalse || Localization.Resources.Gerais.Geral.Nao,
        reverseButtons: true,
        width: '1000px'
    }).then((result) => {

        if (result.value) {
            callbackConfirma();
        } else if (result.value != true && callbackCancela) {
            callbackCancela();
        }

    });
}

function exibirAlerta(titulo, mensagem, btn, callback) {
    let swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success waves-effect waves-themed'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: titulo,
        text: mensagem,
        icon: 'warning',
        showCancelButton: false,
        confirmButtonText: btn || Localization.Resources.Gerais.Geral.Sim,
    }).then((result) => {
        if (callback)
            callback();
    });
}

function exibirSucesso(titulo, mensagem, btn, callback) {
    let swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success waves-effect waves-themed'
        },
        buttonsStyling: false
    });

    swalWithBootstrapButtons.fire({
        title: titulo,
        html: mensagem,
        icon: 'success',
        showCancelButton: false,
        confirmButtonText: btn || Localization.Resources.Gerais.Geral.Sim,
    }).then((result) => {
        if (callback)
            callback();
    });
}

var _RequisicaoIniciada = false;
var _ControlarManualmenteProgresse = false;
function iniciarRequisicao() {
    if (!_ControlarManualmenteProgresse) {
        _RequisicaoIniciada = true;
        $.blockUI({
            message: (
                '<div class="block-ui-custom">' +
                '  <div class="spinner-grow" role="status">' +
                '     <span class="sr-only">' + Localization.Resources.Gerais.Geral.Carregando + '...</span>' +
                '  </div>' +
                '  <h2>' + Localization.Resources.Gerais.Geral.ProcessandoAguarde + '...</h2>' +
                '</div>'
            ),
            baseZ: 99999,
            css: {
                border: 'none',
                padding: '0',
                opacity: 1,
                height: 0,
                cursor: 'default'
            },
            overlayCSS: {
                cursor: 'default',
                opacity: 0.4
            }
        });
    }
}

function iniciarRequisicaoComMensagemPersonalizada(mensagem) {
    if (!_ControlarManualmenteProgresse) {
        _RequisicaoIniciada = true;
        $.blockUI({
            message: (
                '<div class="block-ui-custom">' +
                '  <div class="spinner-grow" role="status">' +
                '     <span class="sr-only">' + Localization.Resources.Gerais.Geral.Carregando + '...</span>' +
                '  </div>' +
                '  <h2>' + mensagem + '</h2>' +
                '</div>'
            ),
            baseZ: 99999,
            css: {
                border: 'none',
                padding: '0',
                opacity: 1,
                height: 0,
                cursor: 'default'
            },
            overlayCSS: {
                cursor: 'default',
                opacity: 0.4
            }
        });
    }
}

function finalizarRequisicao() {
    if (!_ControlarManualmenteProgresse) {
        _RequisicaoIniciada = false;
        $.unblockUI();
    }
}

function iniciarControleManualRequisicao() {
    _ControlarManualmenteProgresse = false;
    iniciarRequisicao();
    _ControlarManualmenteProgresse = true;
}

function finalizarControleManualRequisicao() {
    if (_ControlarManualmenteProgresse) {
        _ControlarManualmenteProgresse = false;
        finalizarRequisicao();
    }
}

function desativarControleRequisicao() {
    _ControlarManualmenteProgresse = true;
}

function ativarControleRequisicao() {
    _ControlarManualmenteProgresse = false;
}