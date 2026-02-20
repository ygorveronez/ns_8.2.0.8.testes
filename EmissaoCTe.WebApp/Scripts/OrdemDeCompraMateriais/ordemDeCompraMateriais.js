$(document).ready(function () {
    $("#txtNumero").mask("9?99999");
    FormatarCampoDate("txtData");

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnDownloadOrdem").click(function () {
        DownloadOrdem();
    });

    LimparCampos();
});

function LimparCampos() {
    $("body").data('codigo', 0);
    
    $("#txtNomeSolicitante").val("");
    $("#txtNomeSolicitante").data("codigo", 0);

    SolicitantePesquisado = false;
    $("#btnBuscarSolicitate").prop('disabled', false);

    $("#txtNumero").val("0");
    $("#txtData").val(Globalize.format((new Date()), "dd/MM/yyyy"));
    $("#txtServico").val("");
    $("#txtSetor").val("");
    $("#txtFornecedor").val("");
    $("#txtFornecedor").data("codigo", 0);
    $("#txtDescricao").val("");

    $("#btnDownloadOrdem").hide();

    BuscarProximoNumero();
}

function BuscarProximoNumero() {
    executarRest("/OrdemCompra/ObterProximoNumeroMateriais?callback=?", '', function (r) {
        if (r.Sucesso) {
            $("#txtNumero").val(r.Objeto.Numero);
        } else {
            $("#txtNumero").val(1);
        }
    });
}

function ValidarCampos() {
    var erros = [];

    if (isNaN($("#txtNumero").val()) || parseInt($("#txtNumero").val()) <= 0) {
        CampoComErro("#txtNumero");
        erros.push("Número da ordem é obrigatório.");
    } else {
        CampoSemErro("#txtNumero");
    }

    if ($("#txtData").val() == "") {
        CampoComErro("#txtData");
        erros.push("Data da ordem é obrigatório.");
    } else {
        CampoSemErro("#txtData");
    }

    if ($("#txtNomeSolicitante").val() == "") {
        CampoComErro("#txtNomeSolicitante");
        erros.push("Nome do Solicitante é obrigatório.");
    } else {
        CampoSemErro("#txtNomeSolicitante");
    }

    if ($("#txtDescricao").val() == "") {
        CampoComErro("#txtDescricao");
        erros.push("Descrição é obrigatório.");
    } else {
        CampoSemErro("#txtDescricao");
    }

    return erros;
}

function Salvar() {
    var erros = ValidarCampos();
    if (erros.length == 0) {
        var ordemDeCompra = {
            Codigo: $("body").data("codigo"),
            Numero: $("#txtNumero").val(),
            Data: $("#txtData").val(),
            Servico: $("#txtServico").val(),
            Solicitante: $("#txtNomeSolicitante").data('codigo'),
            NomeSolicitante: $("#txtNomeSolicitante").val(),
            Setor: $("#txtSetor").val(),
            Descricao: $("#txtDescricao").val(),
            Fornecedor: $("#txtFornecedor").data('codigo'),
            Tipo: "0",
        };

        executarRest("/OrdemCompra/Salvar?callback=?", ordemDeCompra, function (r) {
            if (r.Sucesso) {
                ExibirMensagemSucesso("Dados salvos e carregados com sucesso.", "Sucesso!");
                ObterDetalhesOrdem(r.Objeto);
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
        $("#messages-placeholder").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:");
    }
}

function ObterDetalhesOrdem(ordem) {
    executarRest("/OrdemCompra/ObterDetalhes?callback=?", { Codigo: ordem.Codigo }, function (r) {
        if (r.Sucesso) {

            $("body").data('codigo', r.Objeto.Codigo);

            PreencheConsulta($("#txtVeiculo"), r.Objeto.Veiculo);
            PreencheConsulta($("#txtModelo"), r.Objeto.ModeloVeiculo);
            PreencheConsulta($("#txtFornecedor"), r.Objeto.Fornecedor);

            if (r.Objeto.Solicitante != null) {
                PreencheConsulta($("#txtNomeSolicitante"), r.Objeto.Solicitante);
                SolicitantePesquisado = true;
                $("#btnBuscarSolicitate").prop('disabled', false);
            } else {
                $("#txtNomeSolicitante").val(r.Objeto.NomeSolicitante);
                SolicitantePesquisado = false;
                $("#btnBuscarSolicitate").prop('disabled', true);
            }

            $("#txtNumero").val(r.Objeto.Numero);
            $("#txtSetor").val(r.Objeto.Setor);
            $("#txtServico").val(r.Objeto.Servico);
            $("#txtDescricao").val(r.Objeto.Descricao);
            $("#txtData").val(r.Objeto.Data);

            $("#btnDownloadOrdem").show();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function DownloadOrdem() {
    executarDownload("/OrdemCompra/DownloadOrdem", { Codigo: $("body").data("codigo"), Tipo: "0" });
}

function PreencheConsulta($el, data) {
    if (data == null)
        data = {
            Codigo: 0,
            Descricao: ""
        };

    $el.data('codigo', data.Codigo);
    $el.val(data.Descricao);
}