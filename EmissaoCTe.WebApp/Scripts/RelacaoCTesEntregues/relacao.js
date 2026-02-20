$(document).ready(function () {
    FormatarCampoDate("txtDataRelacao");
    FormatarCampoDate("txtDataEntrega");
    $("#txtKmInicial, #txtKmFinal").priceFormat({ prefix: '', centsLimit: 0, centsSeparator: '' });

    $("#txtDiaria, #txtValorAcrescimos, #txtValorDescontos, #txtValorTotal").priceFormat();

    CarregarConsultadeClientes("btnBuscarCliente", "btnBuscarCliente", RetornoConsultaCliente, true, false);
    RemoveConsulta($("#txtCliente"), function ($this) {
        $this.val("");
        $this.data('codigo', 0);
        CalculoRelacaoCTesEntregues = null;
        RecalcularTodosItens();
        $("#txtValorTotal").prop('disabled', false);
    });

    $("#btnSalvarRelacao").click(function () {
        SalvarRelacao(false);
    });

    $("#btnFinalizarRelacao").click(function () {
        FinalizarRelacao();
    });

    $("#selTipoDiaria").change(TipoDiariaChange);
    $("#txtValorAcrescimos").change(ValorAcrescimos);
    $("#txtValorDescontos").change(ValorDescontos);
    $("#txtDiaria").change(ValorDiaria);
    $("#txtKmInicial, #txtKmFinal").change(CalculaFranquiaKM);

    $modalRelacao = $("#divModalRelacaoCTes");

    CalculoRelacaoCTesEntregues = null;
    CalculoRelacaoCTesEntreguesChange();

    BuscarPermissaoUsuario();
});

var $modalRelacao;
var STATUS_RELACAO_ABERTA = 0;
var STATUS_RELACAO = {
    Todas: 0,
    Aberto: 1,
    Fechado: 2,
    Cancelado: 9
};
var EnumTipoDiariaRelacaoCTesEntregues = {
    SemDiaria: 0,
    Diaria: 1,
    MeiaDiaria: 2
};

// Define o modelo do objeto apenas para autocomplete
var CalculoRelacaoCTesEntregues = {
    ValorDiaria: 0,
    ValorMeiaDiaria: 0,
    PercentualPorCTe: 0,
    ValorMinimoPorCTe: 0,
    ValorMinimoCTeMesmoDestino: 0,
    FracaoKG: 0,
    ValorPorFracao: 0,
    ValorPorFracaoEmEntregasIguais: 0,
    FranquiaKM: 0,
    ValorKMExcedente: 0,
    ColetaValorPorEvento: 0,
    ColetaFracao: 0,
    ColetaValorPorFracao: 0,
    Cidades: [
        {
            Cidade: 0,
            Percentual: 0
        }
    ]
};

function BuscarPermissaoUsuario() {
    executarRest("/CalculoRelacaoCTesEntregues/BuscarPermissaoUsuario?callback=?", {}, function (r) {
        if (r.Sucesso) {
            if (r.Objeto.Permissao)            
                $("#idValorTotal").show();
            else
                $("#idValorTotal").hide();  
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function BuscaPercentualPorCidade(cidade) {
    for (var i = 0, s = CalculoRelacaoCTesEntregues.Cidades.length; i < s; i++) {
        if (CalculoRelacaoCTesEntregues.Cidades[i].Cidade == cidade)
            return CalculoRelacaoCTesEntregues.Cidades[i].Percentual;
    }

    return null;
}

/**
 * Abre modal para criar uma nova relação de CT-es Emitidos
 * A ideia é que o modal abra limpo ou com o conteúdo preenchido
 * Por isso é necessário limpar todos os campos na saída do modal
 */
function AbrirModal(preechimento) {
    // Seta os valroes 
    if (!$.isEmptyObject(preechimento)) {
        $("#txtDataRelacao").val(preechimento.DataRelacao);
        $("#txtDataEntrega").val(preechimento.DataEntrega);
        $("#txtCliente").data('codigo', preechimento.Cliente.Codigo).val(preechimento.Cliente.Descricao);
        $("#txtDescricao").val(preechimento.Descricao);
        $("#txtObservacao").val(preechimento.Observacao);
        $("#txtNumeroControle").val(preechimento.NumeroControle);
        $("#txtKmInicial").val(preechimento.KmInicial);
        $("#txtKmFinal").val(preechimento.KmFinal);

        $("#selTipoDiaria").val(preechimento.TipoDiaria);
        $("#txtDiaria").val(preechimento.Diaria);
        $("#txtValorAcrescimos").val(preechimento.ValorAcrescimos);
        $("#txtValorDescontos").val(preechimento.ValorDescontos);
        $("#txtValorTotal").val(preechimento.ValorTotal);

        StateCTes.set(preechimento.CTes);
        StateColetas.set(preechimento.Coletas);

        CalculoRelacaoCTesEntregues = preechimento.CalculoRelacaoCTesEntregues;
        
        if (CalculoRelacaoCTesEntregues != null) {
            $("#txtValorTotal").prop('disabled', true);

            switch (preechimento.TipoDiaria) {
                case EnumTipoDiariaRelacaoCTesEntregues.SemDiaria:
                    $("#txtDiaria").prop("disabled", false);
                    break;

                case EnumTipoDiariaRelacaoCTesEntregues.Diaria:
                    if (CalculoRelacaoCTesEntregues != null && CalculoRelacaoCTesEntregues.ValorDiaria > 0)
                        $("#txtDiaria").prop("disabled", true);
                    else
                        $("#txtDiaria").prop("disabled", false);
                    break;

                case EnumTipoDiariaRelacaoCTesEntregues.MeiaDiaria:
                    if (CalculoRelacaoCTesEntregues != null && CalculoRelacaoCTesEntregues.ValorMeiaDiaria > 0)
                        $("#txtDiaria").prop("disabled", true);
                    else
                        $("#txtDiaria").prop("disabled", false);
                    break;
            }
        }
    }


    // Exibe botoes conforme status
    if (STATUS_RELACAO_ABERTA == STATUS_RELACAO.Aberto || STATUS_RELACAO_ABERTA == STATUS_RELACAO.Todas) {
        $("#txtDataRelacao").prop('disabled', false);
        $("#txtDataEntrega").prop('disabled', false);
        $("#txtCliente").prop('disabled', false);
        $("#btnBuscarCliente").prop('disabled', false);
        $("#txtNumeroControle").prop('disabled', false);
        $("#txtDescricao").prop('disabled', false);
        $("#txtObservacao").prop('disabled', false);
        $("#txtKmInicial").prop('disabled', false);
        $("#txtKmFinal").prop('disabled', false);

        $("#selTipoDiaria").prop('disabled', false);
        $("#txtValorAcrescimos").prop('disabled', false);
        $("#txtValorDescontos").prop('disabled', false);

        $("#txtChave").prop('disabled', false);
        $("#btnSalvarChave").show();

        $("#txtColetaDescricao").prop('disabled', false);
        $("#txtColetaPesoTotal").prop('disabled', false);
        $("#btnSalvarColeta").show();

        $("#btnSalvarRelacao").show();
        $("#btnFinalizarRelacao").show();
        $(".pode-modificar-grid").show();
    }

    // Abertura padrão
    if (STATUS_RELACAO_ABERTA == STATUS_RELACAO.Todas) {
        $("#txtDiaria").prop('disabled', false);
        $("#txtValorTotal").prop('disabled', false);

        $("#btnFinalizarRelacao").hide();
        $(".pode-modificar-grid").show();
    }

    // Desabilita campos quando status for finalizado
    if (STATUS_RELACAO_ABERTA == STATUS_RELACAO.Fechado) {
        $("#txtDataRelacao").prop('disabled', true);
        $("#txtDataEntrega").prop('disabled', true);
        $("#txtCliente").prop('disabled', true);
        $("#btnBuscarCliente").prop('disabled', true);
        $("#txtNumeroControle").prop('disabled', true);
        $("#txtDescricao").prop('disabled', true);
        $("#txtObservacao").prop('disabled', true);
        $("#txtKmInicial").prop('disabled', true);
        $("#txtKmFinal").prop('disabled', true);

        $("#txtDiaria").prop('disabled', true);
        $("#selTipoDiaria").prop('disabled', true);
        $("#txtValorAcrescimos").prop('disabled', true);
        $("#txtValorDescontos").prop('disabled', true);
        $("#txtValorTotal").prop('disabled', true);

        $("#txtChave").prop('disabled', true);
        $("#btnSalvarChave").hide();

        $("#txtColetaDescricao").prop('disabled', true);
        $("#txtColetaPesoTotal").prop('disabled', true);
        $("#btnSalvarColeta").hide();

        $("#btnSalvarRelacao").hide();
        $("#btnFinalizarRelacao").hide();
        $(".pode-modificar-grid").hide();
    }

    // Exibe Modal
    $modalRelacao.modal('show', {
        backdrop: false
    });

    // Evento Fechar modal
    $modalRelacao.one('hidden.bs.modal', FechouModal);
}

function RetornoConsultaCliente(cliente) {
    $("#txtCliente").val(cliente.Nome);
    $("#txtCliente").data('codigo', cliente.CPFCNPJ.replace(/[^0-9]/g, ''));

    BuscarTabelaCalculo();
}

function BuscarTabelaCalculo()
{
    var cnpjEmitente = "0";
    StateCTes.get().forEach(function (info) {
        if (!info.Excluir) {
            cnpjEmitente = info.CNPJEmitente;
        }
    });

    var dados = {
        codigo: $("#txtCliente").data('codigo'),
        emissor: cnpjEmitente
    };

    executarRest("/CalculoRelacaoCTesEntregues/ObterDetalhesPorCliente?callback=?", dados, function (r) { 
        if (r.Sucesso) {
            CalculoRelacaoCTesEntregues = r.Objeto;

            CalculoRelacaoCTesEntreguesChange();
            RecalcularTodosItens();
        } else {
            ExibirMensagemErro(r.Erro, "Atenção");
        }
    });
}

function CalculoRelacaoCTesEntreguesChange() {
    if (CalculoRelacaoCTesEntregues != null)
        $("#txtValorTotal").prop('disabled', true);
    else {
        $("#txtValorTotal").prop('disabled', false);
        $("#txtDiaria").prop('disabled', false);
    }
}

function FechouModal() {
    LimparModal();
}

function FinalizarRelacao() {
    jConfirm("Você tem certeza que deseja finalizar a operação?", "Finalizar Relação", function (r) {
        if (r) {
            SalvarRelacao(true);
        }
    });
}

function SalvarRelacao(finalizar) {
    var erros = ValidaRelacao(finalizar);
    if (erros.length == 0) {
        // Cria objeto para salvar a relação
        var relacao = {
            Codigo: $("body").data('codigo'),
            DataRelacao: $("#txtDataRelacao").val(),
            DataEntrega: $("#txtDataEntrega").val(),
            KmInicial: $("#txtKmInicial").val().replace(".", ""),
            KmFinal: $("#txtKmFinal").val().replace(".", ""),
            NumeroControle: $("#txtNumeroControle").val(),
            Cliente: $("#txtCliente").data('codigo'),
            Descricao: $("#txtDescricao").val(),

            TipoDiaria: $("#selTipoDiaria").val(),
            Diaria: $("#txtDiaria").val(),
            ValorAcrescimos: $("#txtValorAcrescimos").val(),
            ValorDescontos: $("#txtValorDescontos").val(),
            ValorTotal: $("#txtValorTotal").val(),

            Observacao: $("#txtObservacao").val(),

            CTes: CTesGrid(),
            Coletas: StateColetas.toJson(),
            Finalizar: finalizar
        };

        // Envia pro servidor
        executarRest("/RelacaoCTesEntregues/Salvar?callback=?", relacao, function (r) {
            if (r.Sucesso) {
                $("body").data('codigo', r.Objeto.Codigo);
                $("#txtNumero").val(r.Objeto.Numero);
                if (!finalizar) $("#btnFinalizarRelacao").show();
                AtualizarGrid();
                $modalRelacao.modal('hide');
                ExibirMensagemSucesso("Os dados foram salvos com sucesso", "Dados Salvos.");
            } else {
                jAlert(r.Erro, "Erro ao Salvar");
            }
        });
    } else {
        // Cria lista de erros
        var listaErros = "<ul>";
        for (var e in erros) listaErros += "<li>" + erros[e] + "</li>";
        listaErros += "</ul>"

        // Limpa quaisquer erros existentes
        $("#placeholder-validacao-relacao").html("");

        // Exibe erros
        ExibirMensagemAlerta(listaErros, "Os seguinte erros foram encontrados:", "placeholder-validacao-relacao");
    }

}

function ValidaRelacao(finalizar) {
    var valido = [];

    if ($("#txtDataRelacao").val() == "") {
        valido.push("Data Bipagem é obrigatório.");
        CampoComErro($("#txtDataRelacao"));
    } else {
        CampoSemErro($("#txtDataRelacao"));
    }

    if ($("#txtDataEntrega").val() == "") {
        valido.push("Data Entrega é obrigatório.");
        CampoComErro($("#txtDataEntrega"));
    } else {
        CampoSemErro($("#txtDataEntrega"));
    }

    if ($("#txtCliente").data('codigo') == 0) {
        valido.push("Cliente é obrigatório.");
        CampoComErro($("#txtCliente"));
    } else {
        CampoSemErro($("#txtCliente"));
    }

    if (finalizar && StateCTes.count() == 0)
        valido.push("Nenhum CT-e selecionado.");

    return valido;
}

function LimparModal() {
    $("body").data('codigo', 0);
    $("#txtNumero").val('Automático');
    $("#txtDataRelacao").val('');
    $("#txtDataEntrega").val('');
    $("#txtNumeroControle").val('');
    $("#txtCliente").val('').data('codigo', 0);
    $("#txtDescricao").val('');
    $("#txtObservacao").val('');
    $("#txtKmInicial").val('0');
    $("#txtKmFinal").val('0');
    $("#txtDiaria").val('0,00');
    $("#txtValorAcrescimos").val('0,00');
    $("#txtValorDescontos").val('0,00');
    $("#txtValorTotal").val('0,00');
    $("#selTipoDiaria").val($("#selTipoDiaria option:first").val());

    LimparCTes();
    LimparColetas();
    STATUS_RELACAO_ABERTA = STATUS_RELACAO.Todas;

    $("a[href=#divDadosGerais]").click();
    $("#btnFinalizarRelacao").hide();
}

function TipoDiariaChange() {
    switch (parseInt($("#selTipoDiaria").val())) {
        case EnumTipoDiariaRelacaoCTesEntregues.SemDiaria:
            $("#txtDiaria").prop("disabled", false);
            break;

        case EnumTipoDiariaRelacaoCTesEntregues.Diaria:
            if (CalculoRelacaoCTesEntregues != null && CalculoRelacaoCTesEntregues.ValorDiaria > 0)
                $("#txtDiaria").val(Globalize.format(CalculoRelacaoCTesEntregues.ValorDiaria, "n2")).prop("disabled", true);
            else
                $("#txtDiaria").prop("disabled", false);
            break;

        case EnumTipoDiariaRelacaoCTesEntregues.MeiaDiaria:
            if (CalculoRelacaoCTesEntregues != null && CalculoRelacaoCTesEntregues.ValorMeiaDiaria > 0)
                $("#txtDiaria").val(Globalize.format(CalculoRelacaoCTesEntregues.ValorMeiaDiaria, "n2")).prop("disabled", true);
            else
                $("#txtDiaria").prop("disabled", false);
            break;
    }
    ValorDiaria();
}