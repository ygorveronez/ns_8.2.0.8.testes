$(document).ready(function () {
    $("#btnFecharIntegracaoRetorno").click(function () {
        FecharTelaIntegracaoRetorno();
    });

    $("#btnConsultarIntegracaoRetorno").click(function () {
        CarregarIntegracaoRetorno($("body").data("codigoMDFeIntegracaoRetorno"));
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


function CarregarIntegracaoRetorno(codigoMDFe) {

    var dados = {
        CodigoMDFe: codigoMDFe
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Reenviar", Evento: ReenviarIntegracaoRetorno });
    opcoes.push({ Descricao: "Log", Evento: AbrirTelaIntegracaoRetornoLog });

    CriarGridView("/MDFeIntegracaoRetorno/BuscarPorMDFe?callback=?", dados, "tbl_IntegracaoRetorno_table", "tbl_IntegracaoRetorno", "tbl_paginacao_IntegracaoRetorno", opcoes, [0], null);
}

function ReenviarIntegracaoRetorno(log) {
    executarRest("/MDFeIntegracaoRetorno/ReenviarIntegracao?callback=?", { Codigo: log.data.Codigo }, function (r) {
        if (r.Sucesso) {
            CarregarIntegracaoRetorno($("body").data("codigoMDFeIntegracaoRetorno"));
            ExibirMensagemSucesso("Integração reenviada com sucesso.", "Sucesso!", "placeholder-placeholderIntegracaoRetorno");
        } else {
            CarregarIntegracaoRetorno($("body").data("codigoMDFeIntegracaoRetorno"));
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-placeholderIntegracaoRetorno");
        }
    });
}

function ConsultarIntegracaoRetorno(MDFe) {
    CarregarIntegracaoRetorno(MDFe.data.Codigo);
    $("#tituloIntegracaoRetorno").text("Integrações MDFe-e " + MDFe.data.Numero + " - " + MDFe.data.Serie);
    $("body").data("codigoMDFeIntegracaoRetorno", MDFe.data.Codigo);
    AbrirTelaIntegracaoRetorno();
}


function CarregarIntegracaoRetornoLog(codigo) {

    var dados = {
        Codigo: codigo
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar Requisição", Evento: BaixarRequisicaoIntegracao });
    opcoes.push({ Descricao: "Baixar Resposta", Evento: BaixarRespostaIntegracao });

    CriarGridView("/MDFeIntegracaoRetorno/BuscarLogIntegracao?callback=?", dados, "tbl_IntegracaoRetornoLog_table", "tbl_IntegracaoRetornoLog", "tbl_paginacao_IntegracaoRetornoLog", opcoes, [0], null);
}

function BaixarRequisicaoIntegracao(log) {
    executarDownload("/MDFeIntegracaoRetorno/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 0 }, null, null, null, "placeholder-placeholderIntegracaoRetornoLog");
}

function BaixarRespostaIntegracao(log) {
    executarDownload("/MDFeIntegracaoRetorno/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 1 }, null, null, null, "placeholder-placeholderIntegracaoRetornoLog");
}

