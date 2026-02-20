$(document).ready(function () {
    $("#btnSalvarDocumento").click(function () {
        SalvarDocumento();
    });

    $("#btnCancelarDocumento").click(function () {
        LimparCamposDocumento();
    });

    $("#btnExcluirDocumento").click(function () {
        ExcluirDocumento();
    });

    $("#txtDocumentoChave").blur(function () {
        CompletaNumeroSerie();
    });

    LimparCamposDocumento();

    StateDocumentos = new State({
        name: "documentos",
        id: "Id",
        render: RenderizarDocumento
    });

    StateDocumentos.render();
});

var StateDocumentos;
var IdDocumentoEmEdicao = 0;

function CompletaNumeroSerie() {
    var chave = $("#txtDocumentoChave").val().replace(/[^0-9]/g, '');

    if (!ValidaChave(chave))
        return false;

    var serie = chave.substring(22, 25);
    var numero = chave.substring(26, 34);

    if ($("#txtDocumentoSerie").val() == "")
        $("#txtDocumentoSerie").val(parseInt(serie));

    if ($("#txtDocumentoNumero").val() == "")
        $("#txtDocumentoNumero").val(parseInt(numero));
}

function ExcluirDocumento() {
    StateDocumentos.remove({ Id: IdDocumentoEmEdicao });
    LimparCamposDocumento();
}

function EditarDocumento(info) {
    IdDocumentoEmEdicao = info.Id;

    $("#txtDocumentoNumero").val(info.Numero);
    $("#txtDocumentoSerie").val(info.Serie);
    $("#txtDocumentoChave").val(info.Chave).trigger("blur");
    $("#txtDocumentoDataEmissao").val(info.DataEmissao).trigger("blur");
    $("#txtDocumentoValor").val(info.Valor).trigger("keydown");
    $("#txtDocumentoPeso").val(info.Peso).trigger("keydown");

    $("#btnExcluirDocumento").show();
}

function SalvarDocumento() {
    var erros = ValidaDocumento();
    if (erros.length == 0) {
        var documento = {
            Id: IdDocumentoEmEdicao,
            Chave: $("#txtDocumentoChave").val(),
            Numero: $("#txtDocumentoNumero").val(),
            Serie: $("#txtDocumentoSerie").val(),
            DataEmissao: $("#txtDocumentoDataEmissao").val(),
            Valor: $("#txtDocumentoValor").val(),
            Peso: $("#txtDocumentoPeso").val()
        };

        InsereDocumento(documento);
        LimparCamposDocumento();
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#messages-placeholder-documento").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", 'messages-placeholder-documento');
    }
}

function ValidaDocumento() {
    var valido = [];

    var chave = $("#txtDocumentoChave").val().replace(/[^0-9]/g, '');
    var serie = $("#txtDocumentoSerie").val();
    var numero = $("#txtDocumentoNumero").val();

    if ($("#txtDocumentoSerie").val() == "") {
        valido.push("Série é obrigatório.");
        CampoComErro($("#txtDocumentoSerie"));
    } else {
        CampoSemErro($("#txtDocumentoSerie"));
    }

    if ($("#txtDocumentoNumero").val() == "") {
        valido.push("Número é obrigatório.");
        CampoComErro($("#txtDocumentoNumero"));
    } else {
        CampoSemErro($("#txtDocumentoNumero"));
    }

    var duplicado = false;
    StateDocumentos.get().forEach(function (info) {
        if (!info.Excluir) {
            var itemEmEdicao = (IdDocumentoEmEdicao == 0 || (info.Id != IdDocumentoEmEdicao));
            if (chave != "" && info.Chave == chave && itemEmEdicao)
                duplicado = true;
            else if ((serie != "" && numero != "") && (info.Serie == serie && info.Numero == numero) && itemEmEdicao)
                duplicado = true;
        }
    });

    if (duplicado)
        valido.push("Número e série já existem.");

    return valido;
}

function InsereDocumento(obj) {
    var obj = $.extend({
        Id: 0,
        Chave: "",
        Numero: "",
        Serie: "",
        DataEmissao: "",
        Valor: 0,
        Peso: 0,
        Excluir: false
    }, obj);

    // Limpa mascara
    obj.Chave = obj.Chave.replace(/[^0-9]/g, '');
    obj.Valor = parseFloat(obj.Valor.replace('.', '').replace(',', '.'));
    obj.Peso = parseFloat(obj.Peso.replace('.', '').replace(',', '.'));

    if (obj.Id != 0)
        StateDocumentos.update(obj);
    else
        StateDocumentos.insert(obj);
}

function LimparCamposDocumento() {
    IdDocumentoEmEdicao = 0;

    $("#txtDocumentoChave").val("");
    $("#txtDocumentoNumero").val("");
    $("#txtDocumentoSerie").val("");
    $("#txtDocumentoDataEmissao").val("");
    $("#txtDocumentoValor").val("0,00");
    $("#txtDocumentoPeso").val("0,0000");

    CampoSemErro($("#txtDocumentoSerie"));
    CampoSemErro($("#txtDocumentoNumero"));

    $("#btnExcluirDocumento").hide();
}

function FormataValorDocumento(valor, decimais) {
    return valor.toFixed(decimais).replace(".", ",");
}

function RenderizarDocumento() {
    var itens = StateDocumentos.get();
    var $tabela = $("#tblDocumentos");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var chave = "";

            if (info.Chave != "")
                chave = FormataMascara(info.Chave, "####.####.####.####.####.####.####.####.####.####.####");

            var $row = $("<tr>" +
                "<td>" + info.Numero + "</td>" +
                "<td>" + chave + "</td>" +
                "<td>" + info.Serie + "</td>" +
                "<td>" + info.DataEmissao + "</td>" +
                "<td>" + FormataValorDocumento(info.Valor, 2) + "</td>" +
                "<td>" + FormataValorDocumento(info.Peso, 4) + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                EditarDocumento(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td class='text-center' colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}

function ValidaChave(chave) {
    if (chave.length != 44)
        return false;

    if (isNaN(parseInt(chave)))
        return false;

    return true;
}