var NotasImportadasSelecionados = [];

$(document).ready(function () {
    //$("#txtDDDataInicial").mask("99/99/9999");
    //$("#txtDDDataFinal").mask("99/99/9999");
    $("#txtDDDataInicial").datepicker({});
    $("#txtDDDataFinal").datepicker({});

    $("#txtDDDataInicial").mask("99/99/9999");
    $("#txtDDDataFinal").mask("99/99/9999");

    $("#txtDDNumero").mask("9?99999999");
    $("#txtDDNumeroFinal").mask("9?99999999");
    $("#txtDDCPFCNPJEmiente").mask("99999999999?999");
    $("#txtDDChave").mask("9999 9999 9999 9999 9999 9999 9999 9999 9999 9999 9999");

    $("#btnImportarNFeSalvas").click(function () {
        $("#divNotasImportadas").modal('show');
        AtualizarGridNotasImportadas();
    });

    $("#btnGerarNFSeNotasImportadas").click(function () {
        GerarNFSeNotasImportadas();
    });

    $("#txtDDCPFCNPJEmiente").blur(function () {
        HabilitaSelecionarTodos();
    });
    //$("#txtDDNumero").blur(function () {
    //    HabilitaSelecionarTodos();
    //});
    //$("#txtDDNumeroFinal").blur(function () {
    //    HabilitaSelecionarTodos();
    //});

    $("#btnConsultarNotasImportadas").click(function () {
        AtualizarGridNotasImportadas();
    });

    $("#btnSelecionarTodosNotasImportadas").click(function () {
        SelecionarTodosNotasImportadas();
    });

    $("#btnLimparSelecaoNotasImportadas").click(function () {
        LimparSelecaoNotasImportadas();
    });

    LimparCamposNotasImportadas();

    $("#btnSelecionarTodosNotasImportadas").prop("disabled", true);
});

function LimparCamposNotasImportadas() {
    $("a[href=#tabNotasImportadas]").click();

    var today = new Date();
    var yesterday = new Date(today);
    var tomorrow = new Date(today);
    yesterday.setDate(today.getDate() - 1);
    tomorrow.setDate(today.getDate() + 1);

    $("#txtDDDataInicial").val(Globalize.format(today, "dd/MM/yyyy"));
    $("#txtDDDataFinal").val(Globalize.format(tomorrow, "dd/MM/yyyy"));
    
    $("#btnLimparSelecaoNotasImportadas").hide();
    $("#txtDDNotasSelecionadas").val("0");

    LimparSelecaoNotasImportadas();
}

function DadosFiltroNotasImportadas() {
    return {
        DataInicial: $("#txtDDDataInicial").val(),
        DataFinal: $("#txtDDDataFinal").val(),
        CPFCNPJEmiente: $("#txtDDCPFCNPJEmiente").val(),
        Emiente: $("#txtDDEmiente").val(),
        Numero: $("#txtDDNumero").val(),
        NumeroFinal: $("#txtDDNumeroFinal").val(),
        Chave: $("#txtDDChave").val(),
        NotasSemNFSe: $("#selDDNotasSemNFSe").val()
    };
}

function AtualizarGridNotasImportadas() {
    var dados = DadosFiltroNotasImportadas();

    var opcoes = new Array();
    opcoes.push({ Descricao: "Selecionar", Evento: SelecionarNotasImportadas });

    CriarGridView("/XMLNotaFiscalEletronica/ConsultarNotasImportadas?callback=?", dados, "tbl_NotasImportadas_table", "tbl_NotasImportadas", "tbl_paginacao_NotasImportadas", opcoes, [0], null, null, 20);
}

function SelecionarTodosNotasImportadas() {
    var dados = DadosFiltroNotasImportadas();

    executarRest("/XMLNotaFiscalEletronica/SelecionarTodosNotasImportadas", dados, function (r) {
        if (r.Sucesso) {
            for (var i = 0; i < r.Objeto.length; i++)
                SelecionarNotasImportadas(r.Objeto[i]);
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function SelecionarNotasImportadas(evt) {
    var data = "data" in evt ? evt.data : evt;

    if (NotasImportadasJaInserido(data.Codigo))
        return;

    var html = [
        '<li class="tag-item tag-item-delete-experience" id="ddSelecionado_' + data.Codigo + '">',
        '<span class="tag-container tag-container-delete-experience">',
        '<span class="tag-box tag-box-delete-experience">',
        '<b>' + data.Numero + '',
        '</span>',
        '<span class="tag-delete tag-box tag-box-delete-experience">&nbsp;</span>',
        '</span>',
        '</li>'
    ];
    var $li = $(html.join(""));

    $li.on('click', '.tag-delete', function () {
        RemoverNotasImportadasSelecionado(data);
    });

    $("#containerNotasImportadasSelecionados").append($li);

    NotasImportadasSelecionados.push(data);
    $("#btnTelaNotasImportadasProximo").prop("disabled", false);
    $("#btnLimparSelecaoNotasImportadas").show();

    $("#txtDDNotasSelecionadas").val(NotasImportadasSelecionados.length.toString());
}

function GerarNFSeNotasImportadas() {

    var dados = {
        NotasImportadas: JSON.stringify(NotasImportadasSelecionados.map(function (dd) { return dd.Codigo; }))
    };
    executarRest("/NotaFiscalDeServicosEletronica/GerarNFSeNotasImportadas", dados, function (r) {
        if (r.Sucesso) {
            LimparSelecaoNotasImportadas();
            AtualizarGridNotasImportadas();
            ConsultarNFSes();
            $('#divNotasImportadas').modal("hide");
        } else {
            jAlert(r.Erro, "Atenção");
        }
    });
}

function LimparSelecaoNotasImportadas() {
    NotasImportadasSelecionados = [];
    $("#txtDDNotasSelecionadas").val("0");

    $("#containerNotasImportadasSelecionados").off("click", '.tag-delete').html("");
    $("#btnLimparSelecaoNotasImportadas").hide();
}

function NotasImportadasJaInserido(codigo) {
    for (var i = 0; i < NotasImportadasSelecionados.length; i++) {
        if (NotasImportadasSelecionados[i].Codigo == codigo)
            return true;
    }

    return false;
}

function RemoverNotasImportadasSelecionado(data) {
    for (var i = 0; i < NotasImportadasSelecionados.length; i++) {
        if (NotasImportadasSelecionados[i].Codigo == data.Codigo)
            NotasImportadasSelecionados.splice(i, 1);
    }

    $("#txtDDNotasSelecionadas").val(NotasImportadasSelecionados.length.toString());

    $("#ddSelecionado_" + data.Codigo).remove();

    if (NotasImportadasSelecionados.length <= 0) {
        $("#btnLimparSelecaoNotasImportadas").hide();
    }
}

//Precisa adicionar também validação para permitir selecionar por período
function HabilitaSelecionarTodos() {
    var possuiEmitente = $("#txtDDCPFCNPJEmiente").val().length >= 11;
//    var possuiFiltroNumeros = ($("#txtDDNumero").val() != "" && $("#txtDDNumeroFinal").val() != "");

//    if (possuiEmitente || possuiFiltroNumeros)
    if (possuiEmitente)
        $("#btnSelecionarTodosNotasImportadas").prop("disabled", false);
    else
        $("#btnSelecionarTodosNotasImportadas").prop("disabled", true);
}