$(document).ready(function () {
    $("#btnFecharIntegracaoRetorno").click(function () {
        FecharTelaIntegracaoRetorno();
    });

    $("#btnConsultarIntegracaoRetorno").click(function () {
        CarregarIntegracaoRetorno($("body").data("codigoNFSeIntegracaoRetorno"));
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


function CarregarIntegracaoRetorno(codigoNFSe) {

    var dados = {
        CodigoNFSe: codigoNFSe
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Reenviar", Evento: ReenviarIntegracaoRetorno });
    opcoes.push({ Descricao: "Log", Evento: AbrirTelaIntegracaoRetornoLog });

    CriarGridView("/NFSeIntegracaoRetorno/BuscarPorNFSe?callback=?", dados, "tbl_IntegracaoRetorno_table", "tbl_IntegracaoRetorno", "tbl_paginacao_IntegracaoRetorno", opcoes, [0], null);
}

function ReenviarIntegracaoRetorno(log) {
    executarRest("/NFSeIntegracaoRetorno/ReenviarIntegracao?callback=?", { Codigo: log.data.Codigo }, function (r) {
        if (r.Sucesso) {
            CarregarIntegracaoRetorno($("body").data("codigoNFSeIntegracaoRetorno"));
            ExibirMensagemSucesso("Integração reenviada com sucesso.", "Sucesso!", "placeholder-placeholderIntegracaoRetorno");
        } else {
            CarregarIntegracaoRetorno($("body").data("codigoNFSeIntegracaoRetorno"));
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-placeholderIntegracaoRetorno");
        }
    });
}

function ConsultarIntegracaoRetorno(NFSe) {
    CarregarIntegracaoRetorno(NFSe.data.Codigo);
    $("#tituloIntegracaoRetorno").text("Integrações NFS-e " + NFSe.data.Numero + " - " + NFSe.data.Serie);
    $("body").data("codigoNFSeIntegracaoRetorno", NFSe.data.Codigo);
    AbrirTelaIntegracaoRetorno();
}


function CarregarIntegracaoRetornoLog(codigo) {

    var dados = {
        Codigo: codigo
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar Requisição", Evento: BaixarRequisicao });
    opcoes.push({ Descricao: "Baixar Resposta", Evento: BaixarResposta });

    CriarGridView("/NFSeIntegracaoRetorno/BuscarLogIntegracao?callback=?", dados, "tbl_IntegracaoRetornoLog_table", "tbl_IntegracaoRetornoLog", "tbl_paginacao_IntegracaoRetornoLog", opcoes, [0], null);
}

function BaixarRequisicao(log) {
    executarDownload("/NFSeIntegracaoRetorno/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 0 }, null, null, null, "placeholder-placeholderIntegracaoRetornoLog");
}

function BaixarResposta(log) {
    executarDownload("/NFSeIntegracaoRetorno/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 1 }, null, null, null, "placeholder-placeholderIntegracaoRetornoLog");
}