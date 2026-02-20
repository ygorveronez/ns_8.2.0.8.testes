$(document).ready(function () {
    $modalLogs = $("#divModalLogs");
    $modalMensagem = $("#divModalMensagem");
});

var $modalLogs = null;
var $modalMensagem = null;

function LogIntegracoes(data) {
    var dados = { Integracao: data.data.Codigo };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Download Envio", Evento: DownloadEnvio });
    opcoes.push({ Descricao: "Download Retorno", Evento: DownloadRetorno });
    opcoes.push({ Descricao: "Mensagem", Evento: MensagemLog });

    CriarGridView("/LSTranslogIntegracao/ConsultarLogs?callback=?", dados, "tbl_logs_table", "tbl_logs", "tbl_paginacao_logs", opcoes, [0, 1]);
    $modalLogs.modal('show');
}

function DownloadEnvio(data) {
    FactoryDownloadArquivo("Envio", data.data.Codigo);
}

function DownloadRetorno(data) {
    FactoryDownloadArquivo("Retorno", data.data.Codigo);
}

function MensagemLog(data) {
    $modalMensagem.find(".mensagem").text(data.data.Mensagem);
    $modalMensagem.modal('show');
}

function FactoryDownloadArquivo(tipo, codigo) {
    executarDownload("/LSTranslogIntegracao/DownloadArquivo?callback=?", { Tipo: tipo, Log: codigo }, null, null, null, "placeholder-msgModalLogs");
}