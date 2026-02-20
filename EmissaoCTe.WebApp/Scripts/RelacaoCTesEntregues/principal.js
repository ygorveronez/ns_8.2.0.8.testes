$(document).ready(function () {
    $("#btnNovaRelacao").click(function () {
        AbrirModal({});
        LimparResultantes();
    });


    LimparModal();
    AtualizarGrid();
});