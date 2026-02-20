$(document).ready(function () {
    $("#txtNumeroCIOT").mask("999999999999");
    $("#txtCNPJResponsavelCIOT").mask("99999999999?999");

    $("#txtCNPJResponsavelCIOT").on("focus", function () {
        var valor = $("#txtCNPJResponsavelCIOT").val();
        $("#txtCNPJResponsavelCIOT").val(valor.replace(/[^0-9]/g, ''));
    });
    $("#txtCNPJResponsavelCIOT").on("blur", function () {
        var valor = $("#txtCNPJResponsavelCIOT").val().replace(/[^0-9]/g, '');

        if (valor.length == 14)
            valor = FormataMascara(valor, "##.###.###/####-##");
        else if (valor.length == 11)
            valor = FormataMascara(valor, "###.###.###-##");
        else
            valor = "";

        $("#txtCNPJResponsavelCIOT").val(valor);
    });

    $("#btnSalvarCIOT").click(function () {
        SalvarCIOT();
    });

    $("#btnCancelarCIOT").click(function () {
        LimparCamposCIOT();
    });

    $("#btnExcluirCIOT").click(function () {
        ExcluirCIOT();
    });

    $("#divEmissaoMDFe").on("hide.bs.modal", function () {
        StateCIOT.clear();
        LimparCamposCIOT();
    });

    CarregarConsultadeClientes("btnBuscarResponsavel", "btnBuscarResponsavel", RetornoConsultaResponsavelCIOT, true, false);

    StateCIOT = new State({
        name: "ciot",
        id: "Id",
        render: RenderizarCIOT
    });
});

var StateCIOT;
var IdCIOTEmEdicao = 0;

function LimparCamposCIOT() {
    IdCIOTEmEdicao = 0;
    $("#txtNumeroCIOT").val("");
    $("#txtCNPJResponsavelCIOT").val("");
    CampoSemErro($("#txtNumeroCIOT"));
    CampoSemErro($("#txtCNPJResponsavelCIOT"));

    $("#btnExcluirCIOT").hide();
}

function RetornoConsultaResponsavelCIOT(cliente) {
    $("#txtCNPJResponsavelCIOT").val(cliente.CPFCNPJ.replace(/[^0-9]/g, '')).trigger("blur");
}

function SalvarCIOT() {
    var erros = ValidaCIOT();
    if (erros.length == 0) {
        var ciot = {
            Id: IdCIOTEmEdicao,
            CIOT: $("#txtNumeroCIOT").val(),
            CPF_CNPJ: $("#txtCNPJResponsavelCIOT").val()
        };

        InsereCIOT(ciot);
        LimparCamposCIOT();
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-seguro").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-ciot");
    }
}

function ValidaCIOT() {
    var valido = [];
    if ($("#txtNumeroCIOT").val() == "") {
        valido.push("Número do CIOT é obrigatório.");
        CampoComErro($("#txtNumeroCIOT"));
    } else {
        CampoSemErro($("#txtNumeroCIOT"));
    }

    var CNPJResponsavel = $("#txtCNPJResponsavelCIOT").val().replace(/[^0-9]/g, '');
    if (CNPJResponsavel.length != 11 && CNPJResponsavel.length != 14) {
        valido.push("CPF/CNPJ do Responsável é obrigatório.");
        CampoComErro($("#txtCNPJResponsavelCIOT"));
    } else if (CNPJResponsavel.length == 14 && !ValidarCNPJ(CNPJResponsavel)) {
        valido.push("CNPJ do Responsável pelo seguro informado é inválido.");
        CampoComErro($("#txtCNPJResponsavelCIOT"));
    } else if (CNPJResponsavel.length == 11 && !ValidarCPF(CNPJResponsavel)) {
        valido.push("CPF do Responsável pelo seguro informado é inválido.");
        CampoComErro($("#txtCNPJResponsavelCIOT"));
    } else {
        CampoSemErro($("#txtCNPJResponsavelCIOT"));
    }

    return valido;
}

function InsereCIOT(obj) {
    var obj = $.extend({
        Id: 0,
        CIOT: 0,
        CPF_CNPJ: "",
        Excluir: false
    }, obj);
    
    obj.CPF_CNPJ = obj.CPF_CNPJ.replace(/[^0-9]/g, '');

    if (obj.Id != 0)
        StateCIOT.update(obj);
    else
        StateCIOT.insert(obj);
}

function ExcluirCIOT() {
    StateCIOT.remove({ Id: IdCIOTEmEdicao });
    LimparCamposCIOT();
}

function EditarCIOT(info) {
    IdCIOTEmEdicao = info.Id;
    $("#txtNumeroCIOT").val(info.CIOT).trigger("blur");
    $("#txtCNPJResponsavelCIOT").val(info.CPF_CNPJ).trigger("blur");
    $("#btnExcluirCIOT").show();
}

function RenderizarCIOT() {
    var itens = StateCIOT.get();
    var $tabela = $("#tblCIOT");

    $tabela.find("tbody").html("");

    itens.forEach(function (info) {
        if (!info.Excluir) {
            var cnpj = "";

            if (info.CPF_CNPJ && info.CPF_CNPJ.length == 14)
                cnpj = FormataMascara(info.CPF_CNPJ, "##.###.###/####-##");
            else if (info.CPF_CNPJ && info.CPF_CNPJ.length == 11)
                cnpj = FormataMascara(info.CPF_CNPJ, "###.###.###-##");

            var $row = $("<tr>" +
                "<td>" + info.CIOT + "</td>" +
                "<td>" + cnpj + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block'>Editar</button></td>" +
            "</tr>");

            $row.on("click", "button", function () {
                EditarCIOT(info);
            });

            $tabela.find("tbody").append($row);
        }
    });

    if ($tabela.find("tbody tr").length == 0)
        $tabela.find("tbody").html("<tr><td colspan='" + $tabela.find("thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}