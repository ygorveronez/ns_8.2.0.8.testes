$(document).ready(function () {
    $("#btnFecharCompraValePedagio").click(function () {
        FecharTelaCompraValePedagio();
    });

    $("#btnEnviarCompraValePedagio").click(function () {
        EnviarCompraValePedagio();
    });

    $("#btnConsultarCompraValePedagio").click(function () {
        CarregarCompraValePedagio($("body").data("codigoMDFeCompraValePedagio"));
    });

    $("#btnFecharCompraValePedagioLog").click(function () {
        FecharTelaCompraValePedagioLog();
    });
});

function AbrirTelaCompraValePedagio() {
    $("#divCompraValePedagioMDFe").modal({ keyboard: false });
}

function FecharTelaCompraValePedagio() {
    $("#divCompraValePedagioMDFe").modal('hide');
}

function AbrirTelaCompraValePedagioLog(integracao) {
    $("#divCompraValePedagioMDFeLog").modal({ keyboard: false });
    $("#tituloCompraValePedagioMDFeLog").text("Log de " + integracao.data.Tipo);
    CarregarCompraValePedagioLog(integracao.data.Codigo);
}

function FecharTelaCompraValePedagioLog() {
    $("#divCompraValePedagioMDFeLog").modal('hide');
}

function CarregarCompraValePedagio(codigoMDFe) {

    var dados = {
        CodigoMDFe: codigoMDFe
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Reenviar", Evento: ReenviarCompraValePedagio });
    opcoes.push({ Descricao: "Documento Compra", Evento: DownloadDocumentoCompra });
    opcoes.push({ Descricao: "Log de integrações", Evento: AbrirTelaCompraValePedagioLog });

    CriarGridView("/ValePedagioMDFeCompra/BuscarPorMDFe?callback=?", dados, "tbl_compraValePedagio_table", "tbl_compraValePedagio", "tbl_paginacao_compraValePedagio", opcoes, [0], null);
}

function ReenviarCompraValePedagio(integracao) {
    executarRest("/ValePedagioMDFeCompra/ReenviarIntegracao?callback=?", { Codigo: integracao.data.Codigo }, function (r) {
        if (r.Sucesso) {
            CarregarCompraValePedagio($("body").data("codigoMDFeCompraValePedagio"));
            ExibirMensagemSucesso("Integração reenviada com sucesso.", "Sucesso!", "placeholder-placeholderCompraValePedagioMDFe");
        } else {
            CarregarCompraValePedagio($("body").data("codigoMDFeCompraValePedagio"));
            ExibirMensagemErro(r.Erro, "Atenção!", "placeholder-placeholderCompraValePedagioMDFe");
        }
    });
}

function ConsultarCompraValePedagioMDFe(mdfe) {
    CarregarCompraValePedagio(mdfe.data.Codigo);
    $("#tituloCompraValePedagioMDFe").text("Compras de Vale Pedágio do MDFe-e " + mdfe.data.Numero + " - " + mdfe.data.Serie);
    $("body").data("codigoMDFeCompraValePedagio", mdfe.data.Codigo);
    AbrirTelaCompraValePedagio();
}


function CarregarCompraValePedagioLog(codigo) {

    var dados = {
        Codigo: codigo
    };

    var opcoes = new Array();
    opcoes.push({ Descricao: "Baixar XML Requisição", Evento: BaixarXMLRequisicao });
    opcoes.push({ Descricao: "Baixar XML Resposta", Evento: BaixarXMLResposta });

    CriarGridView("/ValePedagioMDFeCompra/BuscarLogIntegracao?callback=?", dados, "tbl_compraValePedagioLog_table", "tbl_compraValePedagioLog", "tbl_paginacao_compraValePedagioLog", opcoes, [0], null);
}

function BaixarXMLRequisicao(log) {
    executarDownload("/ValePedagioMDFeCompra/DownloadXMLLog", { Codigo: log.data.Codigo, Tipo: 0 }, null, null, null, "placeholder-placeholderCompraValePedagioMDFeLog");
}

function BaixarXMLResposta(log) {
    executarDownload("/ValePedagioMDFeCompra/DownloadXMLLog", { Codigo: log.data.Codigo, Tipo: 1 }, null, null, null, "placeholder-placeholderCompraValePedagioMDFeLog");
}

function DownloadDocumentoCompra(integracao){
    executarDownload("/ValePedagioMDFeCompra/DownloadDocumento", { Codigo: integracao.data.Codigo }, null, null, null, "placeholder-placeholderCompraValePedagioMDFe");
}