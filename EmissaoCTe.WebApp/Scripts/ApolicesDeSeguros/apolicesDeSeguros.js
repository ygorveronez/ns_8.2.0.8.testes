$(document).ready(function () {
    $("#txtDataInicioVigencia").mask("99/99/9999");
    $("#txtDataInicioVigencia").datepicker();
    $("#txtDataFimVigencia").mask("99/99/9999");
    $("#txtDataFimVigencia").datepicker();
    $("#txtCNPJSeguradora").mask("99.999.999/9999-99");

    CarregarConsultaDeApolicesDeSeguros("default-search", "default-search", "", RetornoConsultaApoliceSeguro, true, false);
    CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);

    RemoveConsulta($("#txtCliente"), function ($this) {
        $this.val("");
        $("#hddCliente").val("0");
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });
    $("#btnSalvar").click(function () {
        Salvar();
    });
});

function RetornoConsultaCliente(cliente) {
    $("#hddCliente").val(cliente.CPFCNPJ);
    $("#txtCliente").val(cliente.CPFCNPJ + " - " + cliente.Nome);
}

function RetornoConsultaApoliceSeguro(apolice) {
    executarRest("/ApoliceDeSeguro/ObterDetalhes?callback=?", { Codigo: apolice.Codigo }, function (r) {
        if (r.Sucesso) {
            $("#hddCodigo").val(r.Objeto.Codigo);
            $("#txtCliente").val(r.Objeto.CPFCNPJCliente + " - " + r.Objeto.NomeCliente);
            $("#hddCliente").val(r.Objeto.CPFCNPJCliente);
            $("#txtNomeSeguradora").val(r.Objeto.NomeSeguradora);
            $("#txtNumeroApolice").val(r.Objeto.NumeroApolice);
            $("#txtRamo").val(r.Objeto.Ramo);
            $("#txtDataInicioVigencia").val(r.Objeto.DataInicioVigencia);
            $("#txtDataFimVigencia").val(r.Objeto.DataFimVigencia);
            $("#selStatus").val(r.Objeto.Status);
            $("#selResponsavel").val(r.Objeto.Responsavel);
            $("#txtCNPJSeguradora").val(r.Objeto.CNPJSeguradora).trigger("blur");
            $("#txtCNPJResposavelNaObservacaoContribuinte").val(r.Objeto.CNPJResposavelNaObservacaoContribuinte);            
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function Salvar() {
    var erros = ValidarCamposInformacaoSeguro();

    if (erros.length == 0) {
        var dados = {
            Codigo: $("#hddCodigo").val(),
            CPFCNPJCliente: $("#hddCliente").val(),
            NomeSeguradora: $("#txtNomeSeguradora").val(),
            NumeroApolice: $("#txtNumeroApolice").val(),
            Ramo: $("#txtRamo").val(),
            DataInicioVigencia: $("#txtDataInicioVigencia").val(),
            DataFimVigencia: $("#txtDataFimVigencia").val(),
            CNPJSeguradora: $("#txtCNPJSeguradora").val().replace(/[^0-9]/g, ''),
            Responsavel: $("#selResponsavel").val(),
            Status: $("#selStatus").val(),
            CNPJRaiz: $("#chkCNPJRaiz").prop('checked'),
            CNPJResposavelNaObservacaoContribuinte: $("#txtCNPJResposavelNaObservacaoContribuinte").val()
        };
        executarRest("/ApoliceDeSeguro/Salvar?callback=?", dados, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos com sucesso!", "Sucesso");
                LimparCampos();
            } else {
                ExibirMensagemErro(r.Erro, "Atenção");
            }
        });
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-seguro").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:");
    }
}

function LimparCampos() {
    $("#hddCodigo").val("0");
    $("#txtCliente").val("");
    $("#hddCliente").val("");
    $("#txtNomeSeguradora").val("");
    $("#txtNumeroApolice").val("");
    $("#txtRamo").val("");
    $("#txtDataInicioVigencia").val("");
    $("#txtDataFimVigencia").val("");
    $("#selStatus").val($("#selStatus option:first").val());
    $("#selResponsavel").val($("#selResponsavel option:first").val());
    $("#txtCNPJSeguradora").val("");
    $("#txtCNPJResposavelNaObservacaoContribuinte").val("");
    $("#chkCNPJRaiz").prop('checked', false);

    $("body > form > .container .has-error").removeClass("has-error");
}


function ValidarCamposInformacaoSeguro() {
    var valido = [];

    var CNPJSeguradora = $("#txtCNPJSeguradora").val().replace(/[^0-9]/g, '');
    if (CNPJSeguradora == "") {
        valido.push("CNPJ da Seguradora é obrigatório.");
        CampoComErro($("#txtCNPJSeguradora"));
    } else if (!ValidarCNPJ(CNPJSeguradora)) {
        valido.push("CNPJ da Seguradora informado é inválido.");
        CampoComErro($("#txtCNPJSeguradora"));
    } else {
        CampoSemErro($("#txtCNPJSeguradora"));
    }

    if ($("#txtNumeroApolice").val() == "") {
        valido.push("Número da Apólice é obrigatório.");
        CampoComErro($("#txtNumeroApolice"));
    } else {
        CampoSemErro($("#txtNumeroApolice"));
    }

    if ($("#txtNomeSeguradora").val() == "") {
        valido.push("Nome da Seguradora é obrigatório.");
        CampoComErro($("#txtNomeSeguradora"));
    } else {
        CampoSemErro($("#txtNomeSeguradora"));
    }

    if ($("#txtDataInicioVigencia").val() == "") {
        valido.push("Data início da vigência é obrigatório.");
        CampoComErro($("#txtDataInicioVigencia"));
    } else {
        CampoSemErro($("#txtDataInicioVigencia"));
    }

    if ($("#txtDataFimVigencia").val() == "") {
        valido.push("Data fim da vigência é obrigatório.");
        CampoComErro($("#txtDataFimVigencia"));
    } else {
        CampoSemErro($("#txtDataFimVigencia"));
    }

    return valido;
}