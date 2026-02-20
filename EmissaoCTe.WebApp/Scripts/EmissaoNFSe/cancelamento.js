$(document).ready(function () {
    $("#btnSalvarCancelamentoNFSe").click(function () {
        FinalizarCancelamentoNFSe();
    });

    $("#btnCancelarCancelamentoNFSe").click(function () {
        FecharTelaCancelamentoNFSe();
    });

    $("#btnSalvarCancelamentoNFSePrefeitura").click(function () {
        FinalizarCancelamentoNFSePrefeitura();
    });

    $("#btnCancelarCancelamentoNFSePrefeitura").click(function () {
        FecharTelaCancelamentoNFSePrefeitura();
    });
});

function CancelarNFSe(nfse) {
    AbrirTelaCancelamentoNFSe();
    $("body").data("nfseCancelamento", nfse.data);
}

function AbrirTelaCancelamentoNFSe() {
    $("#divCancelamentoNFSe").modal({ keyboard: false, backdrop: 'static' });
    $("#txtJustificativaCancelamentoNFSe").val("");
    $("body").data("nfseCancelamento", null);
}

function FecharTelaCancelamentoNFSe() {
    $("#divCancelamentoNFSe").modal('hide');
    $("#txtJustificativaCancelamentoNFSe").val("");
    $("body").data("nfseCancelamento", null);
    VoltarAoTopoDaTela();
}

function FinalizarCancelamentoNFSe() {
    var justificativa = $("#txtJustificativaCancelamentoNFSe").val();

    if (justificativa.length > 20 && justificativa.length < 255) {

        executarRest("/NotaFiscalDeServicosEletronica/Cancelar?callback=?", { Justificativa: justificativa, CodigoNFSe: $("body").data("nfseCancelamento") != null ? $("body").data("nfseCancelamento").Codigo : 0 }, function (r) {
            if (r.Sucesso) {
                jAlert("A NFS-e está em processo de cancelamento.", "Atenção", function () {
                    FecharTelaCancelamentoNFSe();
                    ConsultarNFSes();
                });
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgCancelamentoNFSe");
            }
        });

    } else {
        ExibirMensagemAlerta("A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.", "Atenção!", "placeholder-msgCancelamentoNFSe");
    }
}

function CancelarNFSePrefeitura(nfse) {
    AbrirTelaCancelamentoNFSePrefeitura();
    $("body").data("nfseCancelamentoPrefeitura", nfse.data);
}

function AbrirTelaCancelamentoNFSePrefeitura() {
    $("#divCancelamentoNFSePrefeitura").modal({ keyboard: false, backdrop: 'static' });
    $("#txtJustificativaCancelamentoNFSePrefeitura").val("");
    $("body").data("nfseCancelamentoPrefeitura", null);
}

function FecharTelaCancelamentoNFSePrefeitura() {
    $("#divCancelamentoNFSePrefeitura").modal('hide');
    $("#txtJustificativaCancelamentoNFSePrefeitura").val("");
    $("body").data("nfseCancelamentoPrefeitura", null);
    VoltarAoTopoDaTela();
}

function FinalizarCancelamentoNFSePrefeitura() {
    var justificativa = $("#txtJustificativaCancelamentoNFSePrefeitura").val();

    if (justificativa.length > 20 && justificativa.length < 255) {

        executarRest("/NotaFiscalDeServicosEletronica/InformarCancelamentoPrefeitura?callback=?", { Justificativa: justificativa, CodigoNFSe: $("body").data("nfseCancelamentoPrefeitura") != null ? $("body").data("nfseCancelamentoPrefeitura").Codigo : 0 }, function (r) {
            if (r.Sucesso) {
                jAlert("Situação da NFS-e atualizada.", "Atenção", function () {
                    FecharTelaCancelamentoNFSePrefeitura();
                    ConsultarNFSes();
                });
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-msgCancelamentoNFSePrefeitura");
            }
        });

    } else {
        ExibirMensagemAlerta("A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.", "Atenção!", "placeholder-msgCancelamentoNFSePrefeitura");
    }
}

