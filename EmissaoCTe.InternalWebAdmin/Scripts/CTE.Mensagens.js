function ExibirMensagemErro(mensagem, titulo) {
    $("#divMensagemErro label").text(mensagem);
    $("#divMensagemErro span").text(titulo);
    $("#divMensagemErro").slideDown();
    setTimeout(function () {
        $("#divMensagemErro").slideUp();
    }, 5000);
}

function ExibirMensagemSucesso(mensagem, titulo) {
    $("#divMensagemSucesso label").text(mensagem);
    $("#divMensagemSucesso span").text(titulo);
    $("#divMensagemSucesso").slideDown();
    setTimeout(function () {
        $("#divMensagemSucesso").slideUp();
    }, 2000);
}

function ExibirMensagemAlerta(mensagem, titulo) {
    $("#divMensagemAlerta label").text(mensagem);
    $("#divMensagemAlerta span").text(titulo);
    $("#divMensagemAlerta").slideDown();
    setTimeout(function () {
        $("#divMensagemAlerta").slideUp();
    }, 5000);
}