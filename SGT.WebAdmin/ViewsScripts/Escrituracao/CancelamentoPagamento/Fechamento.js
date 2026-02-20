/// <reference path="SolicitacaoAvaria.js" />
/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _percentualFechamentoCancelamento;
var _fechamentoPagamento;

var PercentualFechamentoPagamento = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var FechamentoPagamento = function () {
    this.Contabilizacao = PropertyEntity({ text: "Resumo Cancelamento: ", idGrid: guid() });
};

function LoadFechamentoPagamento() {
    CarregarHTMLFechamentoPagamento().then(function () {
        _fechamentoPagamento = new FechamentoPagamento();
        KoBindings(_fechamentoPagamento, "knoutFechamentoPagamento");

        _percentualFechamentoCancelamento = new PercentualFechamentoPagamento();
        KoBindings(_percentualFechamentoCancelamento, "knockoutPercentualFechamentoPagamento");

    });
}

function ObterDetalhesFechamentoPagamento() {
    var data = { Codigo: _cancelamentoPagamento.Codigo.val() };
    executarReST("CancelamentoPagamentoFechamento/ObterDetalhesFechamento", data, function (e) {
        if (e.Success) {
            if (e.Data !== false) {
                var header = [
                    { data: "CodigoContaContabil", title: "Código", width: "15%" },
                    { data: "DescricaoContaContabil", title: "Conta Contábil", width: "30%" },
                    { data: "CodigoCentroResultado", title: "Centro de Resultado", width: "15%" },
                    { data: "ValorContabilizacaoFormatado", title: "Valor", width: "15%" },
                    { data: "DescricaoTipoContabilizacao", title: "Contabilização", width: "15%" }
                ];
                var tableContabilizacao = new BasicDataTable(_fechamentoPagamento.Contabilizacao.idGrid, header, null, { column: 0, dir: orderDir.asc });
                tableContabilizacao.CarregarGrid(e.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
        
        $("#knoutFechamentoPagamento").show();
    });
}

function CarregarHTMLFechamentoPagamento(callback) {
    return $.get("Content/Static/Escrituracao/CancelamentoFechamentoPagamento.html?dyn=" + guid(), function (data) {
        $("#contentFechamentoCancelamentoPagamento").html(data);
    });
}

function ocultarEtapasFechamentoTodos() {
    SetarPercentualProcessamentoFechamentoCancelamento(0);
    $("#knoutFechamentoPagamento").hide();
    $("#knockoutPercentualFechamentoPagamento").hide();
    $("#divMotivoProblemaFechamento").hide();
}

function buscarDadosFechamentoCancelamentoPagamento() {
    ocultarEtapasFechamentoTodos();
    if (_cancelamentoPagamento.Situacao.val() === EnumSituacaoCancelamentoPagamento.PendenciaCancelamento) {
        $("#divMotivoProblemaFechamento").show();
        $("#pMotivoProblemaFechamento").text(_cancelamentoPagamento.MotivoRejeicaoFechamentoPagamento.val());
    } else {
        if (_cancelamentoPagamento.GerandoMovimentoFinanceiro.val()) {
            $("#knockoutPercentualFechamentoPagamento").show();
        } else {
            ObterDetalhesFechamentoPagamento();
        }
    }
}

function SetarPercentualProcessamentoFechamentoCancelamento(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualFechamentoCancelamento.PercentualProcessado.val(strPercentual);
    $("#" + _percentualFechamentoCancelamento.PercentualProcessado.id).css("width", strPercentual);
}

function LimparCamposProcessaomentoFechamentoPagamento() {
    SetarPercentualProcessamentoFechamentoCancelamento(0);
    LimparCampos(_percentualFechamentoCancelamento);
}