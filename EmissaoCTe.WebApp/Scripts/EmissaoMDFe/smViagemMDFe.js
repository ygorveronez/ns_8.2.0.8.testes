$(document).ready(function () {
    $("#btnFecharSMViagemMDFe").click(function () {
        FecharTelaSMViagemMDFe();
    });

    $("#btnConsultarSMViagemMDFe").click(function () {
        CarregarSMViagemMDFe($("body").data("codigoMDFeSMViagemMDFe"));
    });

    $("#btnFecharSMViagemMDFeLog").click(function () {
        FecharTelaSMViagemMDFeLog();
    });

    $("#btnEnviarSMViagemMDFe").click(function () {
        ReenviarSMViagemMDFe();
    });
});

function AbrirTelaSMViagemMDFe() {
    $("#divSMViagemMDFe").modal({ keyboard: false });
}

function FecharTelaSMViagemMDFe() {
    $("#divSMViagemMDFe").modal('hide');
}

function AbrirTelaSMViagemMDFeLog(integracao) {
    $("#divSMViagemMDFeLog").modal({ keyboard: false });
    $("#tituloSMViagemMDFeLog").text("Log de " + integracao.data.Tipo);
    CarregarSMViagemMDFeLog(integracao.data.Codigo);
}

function FecharTelaSMViagemMDFeLog() {
    $("#divSMViagemMDFeLog").modal('hide');
}


function CarregarSMViagemMDFe(codigoMDFe) {

    var dados = {
        CodigoMDFe: codigoMDFe
    };

    var opcoes = new Array();
    //opcoes.push({ Descricao: "Enviar", Evento: ReenviarSMViagemMDFe });
    //opcoes.push({ Descricao: "Documento Compra", Evento: DownloadDocumentoCompra });
    opcoes.push({ Descricao: "Log", Evento: AbrirTelaSMViagemMDFeLog });

    CriarGridView("/SMViagemMDFe/BuscarPorMDFe?callback=?", dados, "tbl_SMViagemMDFe_table", "tbl_SMViagemMDFe", "tbl_paginacao_SMViagemMDFe", opcoes, [0], null);
}

function ReenviarSMViagemMDFe() {
    executarRest("/SMViagemMDFe/ReenviarIntegracao?callback=?", { CodigoMDFe : $("body").data("codigoMDFeSMViagemMDFe") }, function (r) {
        if (r.Sucesso) {
            CarregarSMViagemMDFe($("body").data("codigoMDFeSMViagemMDFe"));
            ExibirMensagemSucesso("Integração reenviada com sucesso.", "Sucesso!", "placeholder-placeholderSMViagemMDFe");
        } else {
            CarregarSMViagemMDFe($("body").data("codigoMDFeSMViagemMDFe"));
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-placeholderSMViagemMDFe");
        }
    });
}

function ConsultarSMViagemMDFe(mdfe) {
    CarregarSMViagemMDFe(mdfe.data.Codigo);
    $("#tituloSMViagemMDFe").text("Integrações SM MDFe-e " + mdfe.data.Numero + " - " + mdfe.data.Serie);
    $("body").data("codigoMDFeSMViagemMDFe", mdfe.data.Codigo);
    AbrirTelaSMViagemMDFe();
}


function CarregarSMViagemMDFeLog(codigo) {

    var dados = {
        Codigo: codigo
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar Requisição", Evento: BaixarRequisicao });
    opcoes.push({ Descricao: "Baixar Resposta", Evento: BaixarResposta });

    CriarGridView("/SMViagemMDFe/BuscarLogIntegracao?callback=?", dados, "tbl_SMViagemMDFeLog_table", "tbl_SMViagemMDFeLog", "tbl_paginacao_SMViagemMDFeLog", opcoes, [0], null);
}

function BaixarRequisicao(log) {
    executarDownload("/SMViagemMDFe/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 0 }, null, null, null, "placeholder-placeholderSMViagemMDFeLog");
}

function BaixarResposta(log) {
    executarDownload("/SMViagemMDFe/DownloadArquivoLog", { Codigo: log.data.Codigo, Tipo: 1 }, null, null, null, "placeholder-placeholderSMViagemMDFeLog");
}

//function DownloadDocumentoCompra(integracao) {
//    executarDownload("/SMViagemMDFe/DownloadDocumento", { Codigo: integracao.data.Codigo }, null, null, null, "placeholder-placeholderSMViagemMDFe");
//}