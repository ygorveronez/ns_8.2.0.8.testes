function ExibirMensagemErro(mensagem, titulo, id) {
    if (id == null)
        id = "divMensagemErro";

    $("#"+id+" label").text(mensagem);
    $("#" + id + " span").text(titulo);
    $("#" + id).slideDown();
    setTimeout(function () {
        $("#" + id).slideUp();
    }, 5000);
}

function ExibirMensagemSucesso(mensagem, titulo, id) {
    if (id == null)
        id = "divMensagemSucesso";

    $("#" + id + " label").text(mensagem);
    $("#" + id + " span").text(titulo);
    $("#" + id).slideDown();
    setTimeout(function () {
        $("#" + id).slideUp();
    }, 2000);
}

function ExibirMensagemAlerta(mensagem, titulo, id) {
    if (id == null)
        id = "divMensagemAlerta";

    $("#" + id + " label").text(mensagem);
    $("#" + id + " span").text(titulo);
    $("#" + id).slideDown();
    setTimeout(function () {
        $("#" + id).slideUp();
    }, 5000);
}