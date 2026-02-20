$(document).ready(function () {
    $("#btnFecharIntegracaoRetorno").click(function () {
        FecharTelaIntegracaoRetorno();
    });

    $("#btnConsultarIntegracaoRetorno").click(function () {
        CarregarIntegracaoRetorno($("body").data("codigoCTeIntegracaoRetorno"));
    });

    $("#btnFecharIntegracaoRetornoLog").click(function () {
        FecharTelaIntegracaoRetornoLog();
    });
});

function AbrirTelaIntegracaoRetorno() {
    $("#divIntegracaoRetorno").modal({ keyboard: false });
}

function FecharTelaIntegracaoRetorno() {
    $("#divIntegracaoRetorno").modal('hide');
}

function AbrirTelaIntegracaoRetornoLog(integracao) {
    $("#divIntegracaoRetornoLog").modal({ keyboard: false });
    $("#tituloIntegracaoRetornoLog").text("Log Integrações");
    CarregarIntegracaoRetornoLog(integracao.data.Codigo);
}

function FecharTelaIntegracaoRetornoLog() {
    $("#divIntegracaoRetornoLog").modal('hide');
}


function CarregarIntegracaoRetorno(codigoCTe) {

    var dados = {
        CodigoCTe: codigoCTe
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Reenviar", Evento: ReenviarIntegracaoRetorno });
    opcoes.push({ Descricao: "Log", Evento: AbrirTelaIntegracaoRetornoLog });

    CriarGridView("/CTeIntegracaoRetorno/BuscarPorCTe?callback=?", dados, "tbl_IntegracaoRetorno_table", "tbl_IntegracaoRetorno", "tbl_paginacao_IntegracaoRetorno", opcoes, [0], null);
}

function ReenviarIntegracaoRetorno(log) {
    executarRest("/CTeIntegracaoRetorno/ReenviarIntegracao?callback=?", { Codigo: log.data.Codigo }, function (r) {
        if (r.Sucesso) {
            CarregarIntegracaoRetorno($("body").data("codigoCTeIntegracaoRetorno"));
            ExibirMensagemSucesso("Integração reenviada com sucesso.", "Sucesso!", "placeholder-placeholderIntegracaoRetorno");
        } else {
            CarregarIntegracaoRetorno($("body").data("codigoCTeIntegracaoRetorno"));
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-placeholderIntegracaoRetorno");
        }
    });
}

function ConsultarIntegracaoRetorno(CTe) {
    CarregarIntegracaoRetorno(CTe.data.Codigo);
    $("#tituloIntegracaoRetorno").text("Integrações CT-e " + CTe.data.Numero + " - " + CTe.data.Serie);
    $("body").data("codigoCTeIntegracaoRetorno", CTe.data.Codigo);
    AbrirTelaIntegracaoRetorno();
}


function CarregarIntegracaoRetornoLog(codigo) {

    var dados = {
        Codigo: codigo
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar Requisição", Evento: BaixarRequisicao });
    opcoes.push({ Descricao: "Baixar Resposta", Evento: BaixarResposta });

    CriarGridView("/CTeIntegracaoRetorno/BuscarLogIntegracao?callback=?", dados, "tbl_IntegracaoRetornoLog_table", "tbl_IntegracaoRetornoLog", "tbl_paginacao_IntegracaoRetornoLog", opcoes, [0], null);
}

function BaixarRequisicao(log) {
    executarDownload("/CTeIntegracaoRetorno/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 0 }, null, null, null, "placeholder-placeholderIntegracaoRetornoLog");
}

function BaixarResposta(log) {
    executarDownload("/CTeIntegracaoRetorno/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 1 }, null, null, null, "placeholder-placeholderIntegracaoRetornoLog");
}