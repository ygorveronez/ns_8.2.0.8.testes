$(document).ready(function () {
    $("#btnInutilizarCTe").click(function () {
        InutilizarCTe();
    });
    $("#btnCancelarInutilizacaoCTe").click(function () {
        FecharTelaInutilizacaoCTe();
    });
});
function FecharTelaInutilizacaoCTe() {
    $("#txtJustificativaInutilizacaoCTe").val('');
    $("#hddCodigoCTE").val('');
    $("#divInutilizacaoCTe").modal("hide");
    VoltarAoTopoDaTela();
}
function InutilizarCTe() {
    executarRest("/ConhecimentoDeTransporteEletronico/Inutilizar?callback=?", { CodigoCTe: $("#hddCodigoCTE").val(), Justificativa: $("#txtJustificativaInutilizacaoCTe").val() }, function (r) {
        if (r.Sucesso) {
            jAlert("CT-e <b style='color: #FF0000;'>em processo de inutilização</b>!", "Atenção");
            FecharTelaInutilizacaoCTe();
        } else {
            ExibirMensagemErro(r.Erro, "Erro!", "placeholder-msgInutilizacaoCTe");
        }
        AtualizarGridCTes();
    });
}
function AbrirTelaInutilizacaoCTe(cte) {
    jConfirm("Deseja realmente inutilizar o CT-e nº <b>" + cte.data.Numero + "</b>?", "Atenção", function (r) {
        if (r) {
            $("#hddCodigoCTE").val(cte.data.Codigo);
            $("#divInutilizacaoCTe").modal("show");
        }
    });
}