$(document).ready(function () {
    $("#txtQuantidadeDespesa").priceFormat({ prefix: '' });
    $("#txtValorUnitarioDespesa").priceFormat({ prefix: '' });
    $("#txtDataDespesa").mask("99/99/9999");
    $("#txtDataDespesa").datepicker();
    $("#txtFornecedorDespesa").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddDescricaoFornecedor").val("");
                $("#hddCodigoFornecedor").val("");
                $("#txtDescricaoFornecedorNaoCadastrado").attr("disabled", false);
            } else {
                e.preventDefault();
            }
        }
    });
    $("#txtTipoDespesa").keydown(function (e) {
        if (e.which != 9 && e.which != 16) {
            if (e.which == 8 || e.which == 46) {
                $(this).val("");
                $("#hddCodigoTipoDespesa").val("0");
                $("#hddDescricaoTipoDespesa").val("");
            } else {
                e.preventDefault();
            }
        }
    });
    $("#txtDescricaoFornecedorNaoCadastrado").blur(function () {
        if ($(this).val().trim() == "") {
            $("#btnBuscarFornecedorDespesa").attr("disabled", false);
        } else {
            $("#btnBuscarFornecedorDespesa").attr("disabled", true);
        }
    });
    $("#btnSalvarDespesa").click(function () {
        SalvarDespesa();
    });
    $("#btnExcluirDespesa").click(function () {
        ExcluirDespesa();
    });
    $("#btnCancelarDespesa").click(function () {
        LimparCamposDespesa();
    });
    CarregarConsultadeClientes("btnBuscarFornecedorDespesa", "btnBuscarFornecedorDespesa", RetornoConsultaFornecedor, true, false);
    CarregarConsultaDeTiposDeDespesas("btnBuscarTipoDespesa", "btnBuscarTipoDespesa", "A", RetornoConsultaTipoDespesa, true, false);
    SetarDadosPadraoDespesa();
});
function SetarDadosPadraoDespesa() {
    $("#txtDataDespesa").val(Globalize.format(new Date(), "dd/MM/yyyy"));
}
function RetornoConsultaFornecedor(fornecedor) {
    $("#txtFornecedorDespesa").val(fornecedor.CPFCNPJ + " - " + fornecedor.Nome);
    $("#hddDescricaoFornecedor").val(fornecedor.CPFCNPJ + " - " + fornecedor.Nome);
    $("#hddCodigoFornecedor").val(fornecedor.CPFCNPJ);
    $("#txtDescricaoFornecedorNaoCadastrado").attr("disabled", true);
}
function RetornoConsultaTipoDespesa(tipo) {
    $("#txtTipoDespesa").val(tipo.Descricao);
    $("#hddCodigoTipoDespesa").val(tipo.Codigo);
    $("#hddDescricaoTipoDespesa").val(tipo.Descricao);
}
function LimparCamposDespesa() {
    $("#hddCodigoDespesa").val("0");
    $("#txtFornecedorDespesa").val("");
    $("#hddDescricaoFornecedor").val("");
    $("#hddCodigoFornecedor").val("");
    $("#txtTipoDespesa").val("");
    $("#hddCodigoTipoDespesa").val("0");
    $("#hddDescricaoTipoDespesa").val("");
    $("#txtDescricaoDespesa").val("");
    $("#txtQuantidadeDespesa").val("0,00");
    $("#txtValorUnitarioDespesa").val("0,00");
    $("#txtDataDespesa").val("");
    $("#chkDespesaPaga").prop("checked", false);
    $("#btnBuscarFornecedorDespesa").attr("disabled", false);
    $("#txtDescricaoFornecedorNaoCadastrado").attr("disabled", false);
    $("#txtObservacaoDespesa").val("");
    $("#txtDescricaoFornecedorNaoCadastrado").val("");
    $("#btnExcluirDespesa").hide();
    $("#btnCancelarDespesa").hide();
    SetarDadosPadraoDespesa();
}
function ValidarCamposDespesa() {
    var tipoDespesa = Globalize.parseInt($("#hddCodigoTipoDespesa").val());
    var descricao = $("#txtDescricaoDespesa").val().trim();
    var valido = true;
    if (tipoDespesa == 0 || isNaN(tipoDespesa)) {
        CampoComErro("#txtTipoDespesa");
        valido = false;
    } else {
        CampoSemErro("#txtTipoDespesa");
    }
    if (descricao == "") {
        CampoComErro("#txtDescricaoDespesa");
        valido = false;
    } else {
        CampoSemErro("#txtDescricaoDespesa");
    }
    return valido;
}
function SalvarDespesa() {
    if (ValidarCamposDespesa()) {
        var despesa = {
            Codigo: Globalize.parseInt($("#hddCodigoDespesa").val()),
            DescricaoFornecedor: $("#hddCodigoFornecedor").val().trim() != "" ? $("#txtFornecedorDespesa").val() : $("#txtDescricaoFornecedorNaoCadastrado").val(),
            CodigoFornecedor: $("#hddCodigoFornecedor").val(),
            CodigoTipoDespesa: $("#hddCodigoTipoDespesa").val(),
            DescricaoTipoDespesa: $("#hddDescricaoTipoDespesa").val(),
            Descricao: $("#txtDescricaoDespesa").val(),
            Quantidade: $("#txtQuantidadeDespesa").val(),
            ValorUnitario: $("#txtValorUnitarioDespesa").val(),
            Data: $("#txtDataDespesa").val(),
            Paga: $("#chkDespesaPaga")[0].checked,
            Observacao: $("#txtObservacaoDespesa").val(),
            Excluir: false
        };

        var despesas = $("#hddDespesas").val() == "" ? new Array() : JSON.parse($("#hddDespesas").val());

        despesas.sort(function (a, b) { return a.Codigo < b.Codigo ? -1 : 1; });

        if (despesa.Codigo == 0)
            despesa.Codigo = (despesas.length > 0 ? (despesas[0].Codigo > 0 ? -1 : (despesas[0].Codigo - 1)) : -1);

        if (despesas.length > 0) {
            for (var i = 0; i < despesas.length; i++) {
                if (despesas[i].Codigo == despesa.Codigo) {
                    despesas.splice(i, 1);
                    break;
                }
            }
        }

        despesas.push(despesa);

        $("#hddDespesas").val(JSON.stringify(despesas));

        RenderizarDespesa();
        LimparCamposDespesa();
        AtualizarValores();
    } else {
        ExibirMensagemAlerta("Os campos em vermelho ou com asterísco (*) são obrigatórios.", "Atenção!", "mensagensDespesas-placeholder");
    }
}
function EditarDespesa(despesa) {
    LimparCamposDespesa();
    $("#txtDataDespesa").val("");

    $("#hddCodigoDespesa").val(despesa.Codigo);
    if (despesa.CodigoFornecedor == "") {
        $("#txtDescricaoFornecedorNaoCadastrado").val(despesa.DescricaoFornecedor);
        if (despesa.DescricaoFornecedor)
            $("#btnBuscarFornecedorDespesa").attr("disabled", true);
    } else {
        $("#txtFornecedorDespesa").val(despesa.DescricaoFornecedor);
        $("#hddDescricaoFornecedor").val(despesa.DescricaoFornecedor);
        $("#txtDescricaoFornecedorNaoCadastrado").attr("disabled", true);
    }
    $("#hddCodigoFornecedor").val(despesa.CodigoFornecedor);
    $("#txtTipoDespesa").val(despesa.DescricaoTipoDespesa);
    $("#hddCodigoTipoDespesa").val(despesa.CodigoTipoDespesa);
    $("#hddDescricaoTipoDespesa").val(despesa.DescricaoTipoDespesa);
    $("#txtDescricaoDespesa").val(despesa.Descricao);
    $("#txtQuantidadeDespesa").val(despesa.Quantidade);
    $("#txtValorUnitarioDespesa").val(despesa.ValorUnitario);
    $("#txtDataDespesa").val(despesa.Data);
    $("#chkDespesaPaga").prop("checked", despesa.Paga);
    $("#txtObservacaoDespesa").val(despesa.Observacao);
    $("#btnExcluirDespesa").show();
    $("#btnCancelarDespesa").show();
}
function ExcluirDespesa() {
    jConfirm("Deseja realmente excluir esta despesa da viagem?", "Atenção", function (r) {
        if (r) {
            var codigo = Globalize.parseInt($("#hddCodigoDespesa").val());
            var despesas = $("#hddDespesas").val() == "" ? new Array() : JSON.parse($("#hddDespesas").val());
            for (var i = 0; i < despesas.length; i++) {
                if (despesas[i].Codigo == codigo) {
                    if (codigo > 0) {
                        despesas[i].Excluir = true;
                    } else {
                        despesas.splice(i, 1);
                    }
                    break;
                }
            }
            $("#hddDespesas").val(JSON.stringify(despesas));
            RenderizarDespesa();
            LimparCamposDespesa();
            AtualizarValores();
        }
    });
}
function RenderizarDespesa() {
    $("#tblDespesas tbody").html("");
    var despesas = $("#hddDespesas").val() == "" ? new Array() : JSON.parse($("#hddDespesas").val());
    for (var i = 0; i < despesas.length; i++) {
        if (!despesas[i].Excluir) {
            $("#tblDespesas tbody").append("<tr><td>" + despesas[i].DescricaoFornecedor + "</td><td>" + despesas[i].DescricaoTipoDespesa + "</td><td>" + despesas[i].Quantidade + "</td><td>" + despesas[i].ValorUnitario + "</td><td>" + despesas[i].Data + "</td><td>" + (despesas[i].Paga ? "Sim" : "Não") + "</td><td><button type='button' class='btn btn-default btn-xs btn-block' onclick='EditarDespesa(" + JSON.stringify(despesas[i]) + ")'>Editar</button></td></tr>");
        }
    }
    if ($("#tblDespesas tbody").html() == "") {
        $("#tblDespesas tbody").html("<tr><td colspan='7'>Nenhum registro encontrado!</td></tr>");
    }
}
function BuscarDespesas(acertoDeViagem) {
    executarRest("/DespesaDoAcertoDeViagem/BuscarPorAcertoDeViagem?callback=?", { CodigoAcertoViagem: acertoDeViagem.Codigo }, function (r) {
        if (r.Sucesso) {
            $("#hddDespesas").val(JSON.stringify(r.Objeto));
            RenderizarDespesa();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção", "mensagensDespesas-placeholder");
        }
    });
}