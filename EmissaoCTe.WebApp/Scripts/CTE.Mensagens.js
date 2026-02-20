function ExibirMensagemErro(mensagem, titulo, placeholder) {
    CTe_Mensagens_CriarMensagemAlerta("alert alert-danger", titulo, mensagem, placeholder);
}

function ExibirMensagemSucesso(mensagem, titulo, placeholder) {
    CTe_Mensagens_CriarMensagemAlerta("alert alert-success", titulo, mensagem, placeholder);
}

function ExibirMensagemAlerta(mensagem, titulo, placeholder) {
    CTe_Mensagens_CriarMensagemAlerta("alert alert-warning", titulo, mensagem, placeholder);
}

function ExibirMensagemInformacao(mensagem, titulo, placeholder) {
    CTe_Mensagens_CriarMensagemAlerta("alert alert-info", titulo, mensagem, placeholder)
}

function CTe_Mensagens_CriarMensagemAlerta(classe, titulo, mensagem, placeholder) {

    var $div = $("<div></div>");
    $div.addClass(classe);
    $div.html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button><strong>" + titulo + "</strong> " + mensagem);

    if (placeholder == null || placeholder == "") {
        if ($("#messages-placeholder").length > 0) {
            placeholder = "messages-placeholder";
        }
    }

    if (placeholder != null && placeholder != "") { 
        $('#' + placeholder).append($div);
        CTe_Mensagens_TimeoutMensagemAlerta(6500, placeholder, $div);
    }
}

function CTe_Mensagens_TimeoutMensagemAlerta(milliseconds, placeholder, $element) {
    setTimeout(function () {
        if (!$('#' + placeholder).is(":hover")) {
            if (!$element)
                $element.alert('close');
            else
                $('#' + placeholder).children('.alert:first-child').alert('close');
        } else {
            CTe_Mensagens_TimeoutMensagemAlerta(milliseconds, placeholder, $element);
        }        
    }, milliseconds);
}

function jAlert(mensagem, titulo, clback) {
    if (bootbox) {
        if (!$.isFunction(clback)) clback = function () { };

        bootbox.dialog({
            message: mensagem,
            title: titulo,

            backdrop: true,
            animate: false,
            className: 'modal-prioritario',

            onEscape: function () {
                clback();
            },

            buttons: {
                confirm: {
                    label: "Ok",
                    className: "btn-primary",
                    callback: function () { clback(); }
                },
            }
        });
    }
}

function jConfirm(mensagem, titulo, clback) {
    if (bootbox) {
        if (!$.isFunction(clback)) clback = function () { };

        bootbox.dialog({
            message: mensagem,
            title: titulo,

            backdrop: true,
            animate: false,
            className: 'modal-prioritario',

            onEscape: function () {
                clback(false);
            },

            buttons: {
                confirm: {
                    label: "Sim",
                    className: "btn-primary",
                    callback: function () { clback(true); }
                },
                cancel: {
                    label: "Não",
                    className: "btn-default",
                    callback: function () { clback(false); }
                }
            }
        });
    }
}