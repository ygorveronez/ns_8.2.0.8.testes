$(document).ready(function () {

    FormatarCampoDateTime("txtDataCancelamentoCTe");

    $("#btnCancelamentoCTe").click(function () {
        CancelarCTe();
    });

    $("#btnCancelarCancelamentoCTe").click(function () {
        FecharTelaCancelamentoCTe();
    });
});
function FecharTelaCancelamentoCTe() {
    $("#txtJustificativaCancelamentoCTe").val('');
    $("#txtDataCancelamentoCTe").val('');
    $("#hddCodigoCTE").val('');
    $("#divCancelamentoCTe").modal("hide");
    VoltarAoTopoDaTela();
}
function CancelarCTe() {
    var configuracaoEmpresa = $("#hddConfiguracoesEmpresa").val() == "" ? null : JSON.parse($("#hddConfiguracoesEmpresa").val());
    if (configuracaoEmpresa != null && configuracaoEmpresa.ExibirCobrancaCancelamento != null && configuracaoEmpresa.ExibirCobrancaCancelamento == true)
    {
        if ( $("#selCobrarCancelamento").val() == "")
        {
            CampoComErro("#selCobrarCancelamento");
            return;
        } 
    }
    else
        $("#selCobrarCancelamento").val("Sim");

    executarRest("/ConhecimentoDeTransporteEletronico/Cancelar?callback=?", { CodigoCTe: $("#hddCodigoCTE").val(), Justificativa: $("#txtJustificativaCancelamentoCTe").val(), DataCancelamento: $("#txtDataCancelamentoCTe").val(), CobrarCancelamento: $("#selCobrarCancelamento").val() }, function (r) {
        if (r.Sucesso) {
            jConfirm("O CT-e está <b style='color: #FF0000;'>em processo de cancelamento</b>!<br/><br/>Deseja emitir um novo CT-e com os mesmos dados?", "Atenção", function (ret) {
                if (ret) {
                    executarRest("/ConhecimentoDeTransporteEletronico/ObterDetalhes?callback=?", { CodigoCTe: $("#hddCodigoCTE").val() }, function (r) {
                        if (r.Sucesso) {
                            PreencherCTe(r.Objeto, null, true);
                        } else {
                            jAlert(r.Erro, "Atenção");
                        }
                    });
                }
                FecharTelaCancelamentoCTe();
            });
            AtualizarGridCTes();
        } else {
            ExibirMensagemErro(r.Erro, "Erro!", "placeholder-msgCancelamentoCTe");
        }
    });
}
function AbrirTelaCancelamentoCTe(cte) {
    jConfirm("Deseja realmente cancelar o CT-e nº <b>" + cte.data.Numero + "</b>?", "Atenção", function (r) {
        if (r) {
            CampoSemErro("#selCobrarCancelamento");
            $("#selCobrarCancelamento").val($("#selCobrarCancelamento option:first").val());
            $("#txtDataCancelamentoCTe").val(Globalize.format(new Date(), "dd/MM/yyyy HH:mm"));
            $("#hddCodigoCTE").val(cte.data.Codigo);
            BuscarJustificativaAnterior(cte.data.Codigo);
            $("#divCancelamentoCTe").modal("show");
        }
    });
}

function BuscarJustificativaAnterior(codigoCte) {
    executarRest("/ConhecimentoDeTransporteEletronico/ObterJustificativaCancelamento?callback=?", { CodigoCTe: codigoCte }, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.Justificativa != null)
                $("#txtJustificativaCancelamentoCTe").val(r.Objeto.Justificativa);
        }
        else $("#txtJustificativaCancelamentoCTe").val("");
    });
}
