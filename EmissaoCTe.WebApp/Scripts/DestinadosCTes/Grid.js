var $modalEventoDesacordo;
var $codigoDocumento;

$(document).ready(function () {
    FormatarCampoDate("txtFiltroDataInicial");
    FormatarCampoDate("txtFiltroDataFinal");

    $modalEventoDesacordo = $("#divEventoDesacordo");

    $("#btnAtualizarGridDocumentos").click(function () {
        AtualizarGridDocumentos();
    });

    $("#btnSalvarEventoDesacordo").click(function () {
        LancarEventiDesacordo();
    });

    var today = new Date();
    var date = new Date(today);
    date.setDate(today.getDate() - 1);
    $("#txtFiltroDataInicial").val(Globalize.format(date, "dd/MM/yyyy"));
    $("#txtFiltroDataFinal").val(Globalize.format(today, "dd/MM/yyyy"));

    $modalEventoDesacordo.on("hidden.bs.modal", function () {
        LimparCampos();
    });

    AtualizarGridDocumentos();

});

function LimparCampos() {
    $codigoDocumento = 0;
    $("#txtJustificativaEventoDesacordo").val("");
}

function AtualizarGridDocumentos() {
    var dados = {
        inicioRegistros: 0,
        DataInicial: $("#txtFiltroDataInicial").val(),
        DataFinal: $("#txtFiltroDataFinal").val(),
        NumeroInicial: $("#txtFiltroNumeroInicial").val(),
        NumeroFinal: $("#txtFiltroNumeroFinal").val(),
        CnpjEmissor: $("#txtFiltroCNPJEmissor").val(),
        NomeEmissor: $("#txtFiltroNomeEmissor").val(),
        CnpjRemetente: $("#txtFiltroCNPJRemetente").val(),
        NomeRemetente: $("#txtFiltroNomeRemetente").val(),
        CnpjTomador: $("#txtFiltroCNPJTomador").val(),
        NomeTomador: $("#txtFiltroNomeTomador").val(),
        RaizCNPJ: $("#selFiltroRaizCNPJ").val(),
        Status: $("#selFiltroStatus").val()
    };

    var opcoes = [];

    opcoes.push({ Descricao: "Evento Desacordo", Evento: AbrirEventoDesacordo });
    opcoes.push({ Descricao: "XML Autorização", Evento: DownloadXML });
    opcoes.push({ Descricao: "XML Cancelamento", Evento: DownloadXMLCancelamento });
    opcoes.push({ Descricao: "XML Evento Desacordo", Evento: DownloadXMLDesacordo });

    CriarGridView("/DestinadosCTes/Consultar?callback=?", dados, "tbl_documentos_table", "tbl_documentos", "tbl_paginacao_documentos", opcoes, [0], null);
}

function DownloadXML(documento) {
    executarDownload("/DestinadosCTes/DownloadXML", { Codigo: documento.data.Codigo });
}

function DownloadXMLCancelamento(documento) {
    executarDownload("/DestinadosCTes/DownloadXMLCancelamento", { Codigo: documento.data.Codigo });
}

function DownloadXMLDesacordo(documento) {
    executarDownload("/DestinadosCTes/DownloadXMLEventoDesacordo", { Codigo: documento.data.Codigo });
}

function AbrirEventoDesacordo(documento) {
    $codigoDocumento = documento.data.Codigo;
    $modalEventoDesacordo.modal('show');
}

function LancarEventiDesacordo() {
    var justificativa = $("#txtJustificativaEventoDesacordo").val();
    if (justificativa.length > 20 && justificativa.length < 255) {
        executarRest("/DestinadosCTes/EnviarEventoDesacordo?callback=?", {
            Justificativa: justificativa,
            Codigo: $codigoDocumento
        }, function (r) {
            if (r.Sucesso) {
                jAlert("Evento de desacordo enviado com sucesso.", "Atenção", function () {
                    $modalEventoDesacordo.modal('hide');
                    AtualizarGridDocumentos();
                });
            } else {
                ExibirMensagemErro(r.Erro, "Atenção!", "messages-placeholder-cadastroArquivo");
            }
        });

    } else {
        ExibirMensagemAlerta("A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.", "Atenção!", "messages-placeholder-cadastroArquivo");
    }
}

