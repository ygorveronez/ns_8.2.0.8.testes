$(document).ready(function () {
    CarregarConsultadeClientes("btnBuscarParceiro", "btnBuscarParceiro", RetornoConsultaParceiro, true, false, "");
    FormatarCampoDate("txtDataInicial");
    FormatarCampoDate("txtDataFinal");

    $("#txtParceiro").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoParceiro").val("");
            } else {
                e.preventDefault();
            }
        }
    });

    $("#btnSalvar").click(function () {
        Salvar();
    });

    $("#btnCancelar").click(function () {
        LimparCampos();
    });

    $("#btnAtualizar").click(function () {
        if (ValidarCampos())
            Atualizar();
    });

    $("#btnGerarRelatorio").click(function () {
        DownloadRelatorio();
    });

    LimparCampos();
});

function RetornoConsultaParceiro(cliente) {
    $("#txtParceiro").val(cliente.CPFCNPJ + " - " + cliente.Nome);
    $("#hddCodigoParceiro").val(cliente.CPFCNPJ);
}

function LimparCampos() {
    var date = new Date();

    $("#txtDataInicial").val(Globalize.format(date, "dd/MM/yyyy"));
    $("#txtDataFinal").val(Globalize.format(date, "dd/MM/yyyy"));
    $("#hddCodigoParceiro").val("");
    $("#txtParceiro").val("");
    $("#selTipo").val($("#selTipo option:first").val());
    $("#txtPeso").val("0,00");
    $("#txtValoFreteLiquido").val("0,00");
    $("#txtValorComissao").val("0,00");
    $("#txtValorTotalComissao").val("0,00");

    Atualizar();
}

function Salvar() {
    if (ValidarFechamento()) {

        jConfirm("Após o Fechamento a alteração dos Fretes Subcontratados será bloqueada, deseja continuar?", "Atenção", function (r) {
            if (r) {
                var dados = ObterFiltros();

                executarRest("/FreteSubcontratadoFechamento/FecharFretes?callback=?", dados, function (r) {
                    if (r.Sucesso) {
                        ExibirMensagemSucesso("Fretes Subcontratados Fechados com sucesso!", "Sucesso");
                        Atualizar();
                    } else {
                        ExibirMensagemErro(r.Erro, "Atenção");
                    }
                });
            }
        });
    }
}

function Atualizar() {
    CarregarInformacoesFretes();
    CarregarTotaisFretes();
}

function ObterFiltros() {
    var filtros = {
        inicioRegistros: 0,
        Tipo: $("#selTipo").val(),
        DataInicial: $("#txtDataInicial").val(),
        DataFinal: $("#txtDataFinal").val(),
        CpfCnpjCliente: $("#hddCodigoParceiro").val(),
        NumeroDocumento: $("#txtDocumento").val(),
        NumeroCTe: $("#txtCTe").val()
    };

    return filtros;
}

function CarregarInformacoesFretes() {
    var dados = ObterFiltros();

    CriarGridView("/FreteSubcontratadoFechamento/Consultar?callback=?", dados, "tbl_fretes_table", "tbl_fretes", "tbl_paginacao_fretes", null, [0], null);
}

function CarregarTotaisFretes() {
    var dados = ObterFiltros();

    executarRest("/FreteSubcontratadoFechamento/ObterTotaisPendentes?callback=?", dados, function (r) {
        if (r.Sucesso) {
            $("#txtPeso").val(r.Objeto.Peso);
            $("#txtValoFreteLiquido").val(r.Objeto.FreteLiquido);
            $("#txtValorComissao").val(r.Objeto.ValorComissao);
            $("#txtValorTotalComissao").val(r.Objeto.ValorTotalComissao);

        } else {
            ExibirMensagemErro(r.Erro, "Atenção!");
        }
    });
}

function ValidarCampos() {
    var DataInicial = $("#txtDataInicial").val();
    var DataFinal = $("#txtDataFinal").val();
    var Parceiro = $("#hddCodigoParceiro").val()
    var valido = true;

    if (DataInicial == null || DataInicial == "") {
        CampoComErro("#txtDataInicial");
        valido = false;
    } else {
        CampoSemErro("#txtDataInicial");
    }

    if (DataFinal == null || DataFinal == "") {
        CampoComErro("#txtDataFinal");
        valido = false;
    } else {
        CampoSemErro("#txtDataFinal");
    }

    if (Parceiro == null || Parceiro == "") {
        CampoComErro("#txtParceiro");
        valido = false;
    } else {
        CampoSemErro("#txtParceiro");
    }

    if (!valido)
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são de preenchimento obrigatório!", "Atenção!");

    return valido;
}

function ValidarFechamento() {
    var valorComissao = Globalize.parseFloat($("#txtValorComissao").val());
    var valido = true;

    if (isNaN(valorComissao) || valorComissao <= 0) {
        valido = false;
        ExibirMensagemAlerta("Não foram carregados Fretes Subcontratados para efetuar o fechamento!", "Atenção!");
    };

    return valido;
}

function DownloadRelatorio() {
    if (ValidarFechamento()) {
        var dados = {
            Parceiro: $("#hddCodigoParceiro").val(),
            DataEntradaInicio: "",
            DataEntradaFim: "",
            DataEntregaInicio: $("#txtDataInicial").val(),
            DataEntregaFim: $("#txtDataFinal").val(),
            Tipo: $("#selTipo").val(),
            Status: 0,
            TipoArquivo: $("#selTipoArquivo").val()
        };

        executarDownload("/FreteSubcontratadoFechamento/DownloadRelatorio", dados);
    }
}