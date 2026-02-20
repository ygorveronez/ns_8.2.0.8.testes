
function avaliar(nota) {
    marcarEstrela(nota);

    // Envia para o server a nota

    $.ajax({
        url: "/rastreio-entrega/feedback",
        dataType: "json",
        type: "Post",
        async: true,
        data: {
            token: codigoRastreio,
            Avaliacao: nota,
        }
    });
}

function marcarEstrela(nota) {
    $(`.box-avalie > a`).removeClass("estrela-preenchida");

    for (var i = 0; i < nota; i++) {
        $(`.box-avalie > a`).eq(i).addClass("estrela-preenchida");
    }
}