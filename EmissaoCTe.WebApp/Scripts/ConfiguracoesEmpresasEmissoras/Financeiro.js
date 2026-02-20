var EnumTipoMovimentoAcerto = {
    PorAcerto: 0,
    Detalhado: 1
};

$(document).ready(function () {
    CarregarConsultaDePlanosDeContas("btnBuscarContaCTe", "btnBuscarContaCTe", "A", "A", RetornoConsultaPlanoCTe, true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarContaAbastecimento", "btnBuscarContaAbastecimento", "A", "A", RetornoConsultaPlanoAbastecimento, true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarContaPagamentoMotorista", "btnBuscarContaPagamentoMotorista", "A", "A", RetornoConsultaPlanoPagamentoMotorista, true, false);

    CarregarConsultaDePlanosDeContas("btnBuscarAcertoViagemContaReceitas", "btnBuscarAcertoViagemContaReceitas", "A", "A", FactoryAcertoViagemPlanoDeConta("AcertoViagemContaReceitas"), true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarAcertoViagemContaDespesas", "btnBuscarAcertoViagemContaDespesas", "A", "A", FactoryAcertoViagemPlanoDeConta("AcertoViagemContaDespesas"), true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarAcertoViagemContaDespesasAbastecimentos", "btnBuscarAcertoViagemContaDespesasAbastecimentos", "A", "A", FactoryAcertoViagemPlanoDeConta("AcertoViagemContaDespesasAbastecimentos"), true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarAcertoViagemContaDespesasAdiantamentosMotorista", "btnBuscarAcertoViagemContaDespesasAdiantamentosMotorista", "A", "A", FactoryAcertoViagemPlanoDeConta("AcertoViagemContaDespesasAdiantamentosMotorista"), true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarAcertoViagemContaReceitasDevolucoesMotorista", "btnBuscarAcertoViagemContaReceitasDevolucoesMotorista", "A", "A", FactoryAcertoViagemPlanoDeConta("AcertoViagemContaReceitasDevolucoesMotorista"), true, false);
    CarregarConsultaDePlanosDeContas("btnBuscarAcertoViagemContaDespesasPagamentosMotorista", "btnBuscarAcertoViagemContaDespesasPagamentosMotorista", "A", "A", FactoryAcertoViagemPlanoDeConta("AcertoViagemContaDespesasPagamentosMotorista"), true, false);

    RemoveConsulta("#txtAcertoViagemContaReceitas, " +
       "#txtAcertoViagemContaDespesas, " +
       "#txtAcertoViagemContaDespesasAbastecimentos, " +
       "#txtAcertoViagemContaDespesasAdiantamentosMotorista, " +
       "#txtAcertoViagemContaReceitasDevolucoesMotorista, " +
       "#txtAcertoViagemContaDespesasPagamentosMotorista", function ($this) {
           $this.val("").data("Codigo", 0);
       }, true);

    $("#txtDiasParaVencimento").mask("9?9");
    $("#txtNumeroParcelasDuplicatas").mask("9?9");
    $("#txtDiasParaAvisoVencimentos").mask("9?9");

    RemoveConsulta("#txtContaCTe", function ($this) {
        $this.val("");
        $("#hddCodigoPlanoCTe").val("0");
    });

    RemoveConsulta("#txtContaAbastecimento", function ($this) {
        $this.val("");
        $("#hddCodigoPlanoAbastecimento").val("0");
    });

    RemoveConsulta("#txtContaPagamentoMotorista", function ($this) {
        $this.val("");
        $("#txtContaPagamentoMotorista").data("codigo", 0);
    });

    $("#selAcertoViagemMovimentoDespesasAdiantamentosMotorista").change(function () {
        var movimento = $(this).val();

        if (movimento == EnumTipoMovimentoAcerto.Detalhado)
            $(".divAcertoViagemContaReceitasDevolucoesMotorista").show();
        else
            $(".divAcertoViagemContaReceitasDevolucoesMotorista").hide();
    });
});

function RetornoConsultaPlanoCTe(plano) {
    $("#txtContaCTe").val(plano.Conta + " - " + plano.Descricao);
    $("#hddCodigoPlanoCTe").val(plano.Codigo);
}

function RetornoConsultaPlanoAbastecimento(plano) {
    $("#txtContaAbastecimento").val(plano.Conta + " - " + plano.Descricao);
    $("#hddCodigoPlanoAbastecimento").val(plano.Codigo);
}

function RetornoConsultaPlanoPagamentoMotorista(plano) {
    $("#txtContaPagamentoMotorista").val(plano.Conta + " - " + plano.Descricao);
    $("#txtContaPagamentoMotorista").data("codigo", plano.Codigo);
}

function FactoryAcertoViagemPlanoDeConta(nome) {
    return function (plano) {
        $("#txt" + nome)
            .val(plano.Conta + " - " + plano.Descricao)
            .data("Codigo", plano.Codigo);
    }
}