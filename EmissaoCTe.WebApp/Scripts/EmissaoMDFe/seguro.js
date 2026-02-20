$(document).ready(function () {
    $("#btnSalvarInformacaoSeguro").click(function () {
        SalvarInformacaoSeguro();
    });
    $("#btnCancelarInformacaoSeguro").click(function () {
        LimparCamposInformacaoSeguro();
    });
    $("#btnExcluirInformacaoSeguro").click(function () {
        ExcluirInformacaoSeguro();
    });

    CarregarConsultaDeApolicesDeSegurosPorCliente("btnBuscarApoliceSeguro", "btnBuscarApoliceSeguro", "", RetornoConsultaApoliceSeguro, true, false);

    $("#txtCNPJSeguradora").mask("99.999.999/9999-99");
    $("#txtCNPJResponsavelSeguro").mask("99999999999?999");

    $("#txtCNPJResponsavelSeguro").on("focus", function () {
        var valor = $("#txtCNPJResponsavelSeguro").val();
        $("#txtCNPJResponsavelSeguro").val(valor.replace(/[^0-9]/g, ''));
    });
    $("#txtCNPJResponsavelSeguro").on("blur", function () {
        var valor = $("#txtCNPJResponsavelSeguro").val().replace(/[^0-9]/g, '');

        if (valor.length == 14)
            valor = FormataMascara(valor, "##.###.###/####-##");
        else if (valor.length == 11)
            valor = FormataMascara(valor, "###.###.###-##");
        else
            valor = "";

        $("#txtCNPJResponsavelSeguro").val(valor);
    });

    $("#divEmissaoMDFe").on("hide.bs.modal", function () {
        LimparCamposInformacaoSeguro();
        setSeguros([]);
    });
});

var IdInformacaoSeguroEmEdicao = 0;

function getSeguros() {
    var seguros = [];
    var dataSeguros = $("body").data("mdfseguros");

    if ($.isArray(dataSeguros))
        seguros = dataSeguros;

    return seguros;
}

function setSeguros( seguros ) {
    if (!$.isArray(seguros))
        return false;

    $("body").data("mdfseguros", seguros);
    RenderizarInformacaoSeguro();

    return true;
}

function RetornoConsultaApoliceSeguro(apolice) {
    if (apolice.Codigo == undefined)
        apolice = apolice.data;
    $("#txtNumeroApolice").val(apolice.NumeroApolice);
    $("#txtNomeSeguradora").val(apolice.NomeSeguradora);
    $("#txtCNPJSeguradora").val(apolice.CNPJSeguradora).trigger("blur");
}

function ValidarCamposInformacaoSeguro() {
    var valido = [];

    var tipoRespon = $("#selResponsavelSeguro").val();
    
    var CNPJSeguradora = $("#txtCNPJSeguradora").val().replace(/[^0-9]/g, '');
    if (tipoRespon == 2 && CNPJSeguradora == "") {
        valido.push("CNPJ da Seguradora é obrigatório quando o tipo do responsável é contratante.");
        CampoComErro($("#txtCNPJSeguradora"));
    } else if (tipoRespon == 2 && !ValidarCNPJ(CNPJSeguradora)) {
        valido.push("CNPJ da Seguradora informado é inválido.");
        CampoComErro($("#txtCNPJSeguradora"));
    } else {
        CampoSemErro($("#txtCNPJSeguradora"));
    }

    var CNPJResponsavel = $("#txtCNPJResponsavelSeguro").val().replace(/[^0-9]/g, '');
    if (tipoRespon == 2 && CNPJResponsavel == "") {
        valido.push("CPF/CNPJ do Responsável é obrigatório quando o tipo do responsável é contratante.");
        CampoComErro($("#txtCNPJResponsavelSeguro"));
    } else if (tipoRespon == 2 && CNPJResponsavel.length == 14 && !ValidarCNPJ(CNPJResponsavel)) {
        valido.push("CNPJ do Responsável pelo seguro informado é inválido.");
        CampoComErro($("#txtCNPJResponsavelSeguro"));
    } else if (tipoRespon == 2 && CNPJResponsavel.length == 11 && !ValidarCPF(CNPJResponsavel)) {
        valido.push("CPF do Responsável pelo seguro informado é inválido.");
        CampoComErro($("#txtCNPJResponsavelSeguro"));
    } else {
        CampoSemErro($("#txtCNPJResponsavelSeguro"));
    }

    if ($("#txtNumeroApolice").val() == "") {
        valido.push("Número da Apólice é obrigatório.");
        CampoComErro($("#txtNumeroApolice"));
    } else {
        CampoSemErro($("#txtNumeroApolice"));
    }

    //if ($("#txtNumeroAverbacao").val() == "") {
    //    valido.push("Número da Averbação é obrigatório.");
    //    CampoComErro($("#txtNumeroAverbacao"));
    //} else {
    //    CampoSemErro($("#txtNumeroAverbacao"));
    //}

    if ($("#txtNomeSeguradora").val() == "") {
        valido.push("Nome da Seguradora é obrigatório.");
        CampoComErro($("#txtNomeSeguradora"));
    } else {
        CampoSemErro($("#txtNomeSeguradora"));
    }

    return valido;
}
function SalvarInformacaoSeguro() {
    var erros = ValidarCamposInformacaoSeguro();
    if (erros.length == 0) {
        var informacaoSeguro = {
            Id: parseInt(IdInformacaoSeguroEmEdicao),
            Tipo: $("#selResponsavelSeguro").val(),
            DescricaoTipo: $("#selResponsavelSeguro :selected").text(),
            Seguradora: $("#txtNomeSeguradora").val(),
            NumeroApolice: $("#txtNumeroApolice").val(),
            NumeroAverbacao: $("#txtNumeroAverbacao").val(),
            Responsavel: $("#txtCNPJResponsavelSeguro").val().replace(/[^0-9]/g, ''),
            CNPJSeguradora: $("#txtCNPJSeguradora").val().replace(/[^0-9]/g, ''),
            Excluir: false
        };

        InsereSeguro(informacaoSeguro);
        LimparCamposInformacaoSeguro();
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"
        
        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-seguro").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-seguro");
    }
}

function InsereSeguro(obj) {
    var objSeguro = $.extend({
        Id: 0,
        Tipo: 0,
        DescricaoTipo: "",
        Seguradora: "",
        NumeroApolice: 0,
        NumeroAverbacao: 0,
        CNPJSeguradora: 0,
        Responsavel: "",
        Excluir: false
    }, obj);

    if (VerificarSeSeguro(objSeguro)) {
        var infomacoesSeguro = getSeguros();

        if (objSeguro.Id == 0)
            objSeguro.Id = -(infomacoesSeguro.length + 1);

        for (var i = 0; i < infomacoesSeguro.length; i++) {
            if (infomacoesSeguro[i].Id == objSeguro.Id) {
                infomacoesSeguro.splice(i, 1);
                break;
            }
        }
        infomacoesSeguro.push(objSeguro);
        setSeguros(infomacoesSeguro);
    }
}

function VerificarSeSeguro(objSeguro) {

    var infomacoesSeguro = getSeguros();

    if (infomacoesSeguro.length > 0) {
        for (var i = 0; i < infomacoesSeguro.length; i++)
            if (infomacoesSeguro[i].Tipo == objSeguro.Tipo &&
                infomacoesSeguro[i].Seguradora == objSeguro.Seguradora &&
                infomacoesSeguro[i].NumeroApolice == objSeguro.NumeroApolice &&
                infomacoesSeguro[i].NumeroAverbacao == objSeguro.NumeroAverbacao &&
                infomacoesSeguro[i].CNPJSeguradora == objSeguro.CNPJSeguradora &&
                !infomacoesSeguro[i].Excluir)
                return false;
    }

    return true;
}

function EditarInformacaoSeguro(informacao) {
    IdInformacaoSeguroEmEdicao = informacao.Id;
    $("#selResponsavelSeguro").val(informacao.Tipo);
    $("#txtNomeSeguradora").val(informacao.Seguradora);
    $("#txtNumeroApolice").val(informacao.NumeroApolice)
    $("#txtNumeroAverbacao").val(informacao.NumeroAverbacao);
    $("#txtCNPJSeguradora").val(informacao.CNPJSeguradora).trigger("blur");
    $("#txtCNPJResponsavelSeguro").val(informacao.Responsavel).trigger("blur");
    $("#btnExcluirInformacaoSeguro").show();
}
function ExcluirInformacaoSeguro() {
    var id = IdInformacaoSeguroEmEdicao;
    var infomacoesSeguro = getSeguros();;
    for (var i = 0; i < infomacoesSeguro.length; i++) {
        if (infomacoesSeguro[i].Id == id) {
            if (id <= 0)
                infomacoesSeguro.splice(i, 1);
            else
                infomacoesSeguro[i].Excluir = true;
            break;
        }
    }
    setSeguros(infomacoesSeguro);
    LimparCamposInformacaoSeguro();
}
function LimparCamposInformacaoSeguro() {
    IdInformacaoSeguroEmEdicao = 0;
    $("#selResponsavelSeguro").val($("#selResponsavelSeguro option:first").val());
    $("#txtNomeSeguradora").val('');
    $("#txtNumeroApolice").val('');
    $("#txtNumeroAverbacao").val('');
    $("#txtCNPJSeguradora").val('');
    $("#txtCNPJResponsavelSeguro").val('');
    $("#txtCNPJResponsavel").val('');
    $("#txtValorMercadoriaParaEfeitoDeAverbacao").val('0,00');
    $("#btnExcluirInformacaoSeguro").hide();

    $("#tabSeguros .has-error").removeClass("has-error");
}

function ConverteResponsavelSeguro(enumerado) {
    /**
     * A partir do enumerador do tipo de seguro modelo antigo
     * Busca-se o equivalente no novo modelo seguindo o mapeamento
     * 
     * Sabendo o enumerador do novo modelo, retorna o objeto:
     * {
     *   Id: Enumerador do seguro
     *   Descricao: Texto descritivo do enumerador
     * }
     */

    var mapeamento = {
        // Modelo antigo => Novo Modelo
        0: 2, // Remetente
        1: 2, // Expedidor
        2: 2, // Recebedor 
        3: 2, // Destinatario 
        4: 1, // Emitente_CTE
        5: 2  // Tomador_Servico
    };

    var novomodelo = {
        1: {
            Id: 1,
            Descricao: "Emitente"
        },
        2: {
            Id: 2,
            Descricao: "Contratante"
        },
        Default: {
            Id: 0,
            Descricao: ""
        }
    };

    var mapeado = (enumerado in mapeamento) ? mapeamento[enumerado] : "Default";

    return novomodelo[mapeado];
}

function RenderizarInformacaoSeguro() {
    var infomacoesSeguro = getSeguros();
    $("#tblInformacaoSeguro tbody").html("");

    infomacoesSeguro.forEach(function (info) {
        if (!info.Excluir) {
            var cnpjSeguradora = "";
            var cnpjResponsavel = "";

            if (info.CNPJSeguradora && info.CNPJSeguradora.length == 14)
                cnpjSeguradora = FormataMascara(info.CNPJSeguradora, "##.###.###/####-##");

            if (info.Responsavel && info.Responsavel.length == 14)
                cnpjResponsavel = FormataMascara(info.Responsavel, "##.###.###/####-##");
            else if (info.Responsavel && info.Responsavel.length == 11)
                cnpjResponsavel = FormataMascara(info.Responsavel, "###.###.###-##");

            var $row = $("<tr>" +
                "<td>" + info.DescricaoTipo + "</td>" +
                "<td>" + cnpjResponsavel + "</td>" +
                "<td>" + cnpjSeguradora + "</td>" +
                "<td>" + info.Seguradora + "</td>" +
                "<td>" + info.NumeroApolice + "</td>" +
                "<td>" + info.NumeroAverbacao + "</td>" +
                "<td><button type='button' class='btn btn-default btn-xs btn-block editar'>Editar</button></td>" +
            "</tr>");

            $row.on("click", ".editar", function () {
                EditarInformacaoSeguro(info);
            });

            $("#tblInformacaoSeguro tbody").append($row);
        }
    });

    if ($("#tblInformacaoSeguro tbody tr").length == 0)
        $("#tblInformacaoSeguro tbody").html("<tr><td colspan='" + $("#tblInformacaoSeguro thead th").length + "'>Nenhum registro encontrado.</td></tr>");
}