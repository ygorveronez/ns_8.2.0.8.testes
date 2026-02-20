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
/// <reference path="../../Enumeradores/EnumSituacaoPagamento.js" />

var _percentualFechamentoPagamento;
var _fechamentoPagamento;
var _CRUDFechamentoPagamento;

var PercentualFechamentoPagamento = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var FechamentoPagamento = function () {
    this.Contabilizacao = PropertyEntity({ text: "Resumo Pagamento: ", idGrid: guid() });
};

var CRUDFechamentoPagamento = function () {
    this.ConfirmarPagamento = PropertyEntity({ eventClick: confirmarPagamentoClick, type: types.event, text: "Confirmar Fechamento", idGrid: guid(), visible: ko.observable(false) });
    this.ProcessarNovamente = PropertyEntity({ eventClick: reprocessarPagamentoClick, type: types.event, text: "Reprocessar Pagamento", idGrid: guid(), visible: ko.observable(false) });
    this.CancelarPagamento = PropertyEntity({ eventClick: cancelarFechamentoPagamentoClick, type: types.event, text: "Remover Pagamento", idGrid: guid(), visible: ko.observable(true) });
};

function confirmarPagamentoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja finalizar o processo de pagamento?", function () {
        executarReST("PagamentoFechamento/ConfirmarFechamentoPagamento", { Codigo: _pagamento.Codigo.val() }, function (e) {
            if (e.Success) {
                if (e.Data !== false) {
                    exibirMensagem(tipoMensagem.Success, "Sucesso", "Pagamento Finalizada com Sucesso");
                    BuscarPagamentoPorCodigo(_pagamento.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    });
}

function reprocessarPagamentoClick() {
    var _handle = function () {
        var data = { Codigo: _pagamento.Codigo.val() };
        executarReST("PagamentoFechamento/ReprocessarPagamento", data, function (e) {
            if (e.Success) {
                if (e.Data !== false) {
                    exibirMensagem(tipoMensagem.Success, "Sucesso", "Pagamento Finalizada com Sucesso");
                    BuscarPagamentoPorCodigo(_pagamento.Codigo.val());
                } else {
                    exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    }

    if (_pagamento.CargaEmCancelamento.val()) {
        if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.Pagamento_AutorizarPagmentoComCargaCancelada, _PermissoesPersonalizadasPagamento)) {
            exibirConfirmacao("Confirmação", "Esse pagamento possui documentos vinculados a uma carga com registro de cancelamento. Tem certeza que deseja confirmar o pagamento?", _handle);
        } else {
            exibirMensagem(tipoMensagem.atencao, "Atenção", "Esse pagamento possui documentos vinculados a uma carga com registro de cancelamento. Somente usuário com permissão especial pode avançar o pagamento.", 10000);
        }
    } else {
        exibirConfirmacao("Confirmação", "Realmente deseja reprocessar a pagamento?", _handle);
    }
}

function cancelarFechamentoPagamentoClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja remover a pagamento?", function () {
        var data = { Codigo: _pagamento.Codigo.val() };
        executarReST("PagamentoFechamento/CancelarPagamento", data, function (e) {
            if (e.Success) {
                if (e.Data !== false) {
                    exibirMensagem(tipoMensagem.Success, "Sucesso", "Cancelamento Confirmado");
                    LimparCamposPagamento();
                    _gridPagamento.CarregarGrid();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
                }
            }
            else {
                exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
            }
        });
    });
}

function LoadFechamentoPagamento() {
    CarregarHTMLFechamentoPagamento().then(function () {
        _fechamentoPagamento = new FechamentoPagamento();
        KoBindings(_fechamentoPagamento, "knoutFechamentoPagamento");

        _percentualFechamentoPagamento = new PercentualFechamentoPagamento();
        KoBindings(_percentualFechamentoPagamento, "knockoutPercentualFechamentoPagamento");

        _CRUDFechamentoPagamento = new CRUDFechamentoPagamento();
        KoBindings(_CRUDFechamentoPagamento, "knoutCRUDFechamentoPagamento");

        loadPagamentoAutorizacao();
    });
}

function ObterDetalhesFechamentoPagamento() {
    var data = { Codigo: _pagamento.Codigo.val() };
    executarReST("PagamentoFechamento/ObterDetalhesFechamento", data, function (e) {
        if (e.Success) {
            if (e.Data !== false) {
                var header = [
                    { data: "CodigoContaContabil", title: "Código", width: "10%" },
                    { data: "DescricaoContaContabil", title: "Conta Contábil", width: "35%" },
                    { data: "CodigoCentroResultado", title: "Centro de Resultado", width: "15%" },
                    { data: "ValorContabilizacaoFormatado", title: "Valor", width: "15%" },
                    { data: "DescricaoTipoContabilizacao", title: "Contabilização", width: "15%" }
                ];
                var tableContabilizacao = new BasicDataTable(_fechamentoPagamento.Contabilizacao.idGrid, header, null, { column: 0, dir: orderDir.asc });
                tableContabilizacao.CarregarGrid(e.Data);
            }
            else
                exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);

        $("#knoutFechamentoPagamento").show();
    });
}

function CarregarHTMLFechamentoPagamento(callback) {
    var prom = new promise.Promise();
    $.get("Content/Static/Escrituracao/FechamentoPagamento.html?dyn=" + guid(), function (data) {
        $("#contentFechamentoPagamento").html(data);
        prom.done();
    });
    return prom;
}

function ocultarEtapasFechamentoTodos() {
    SetarPercentualProcessamentoFechamentoPagamento(0);

    $("#knoutFechamentoPagamento").hide(); 
    $("#knockoutPercentualFechamentoPagamento").hide();
    $("#divMotivoProblemaFechamento").hide();

    _CRUDFechamentoPagamento.ConfirmarPagamento.visible(!_CONFIGURACAO_TMS.UtilizarAlcadaAprovacaoPagamento);
    _CRUDFechamentoPagamento.ProcessarNovamente.visible(false);
}

function buscarDadosFechamentoPagamento() {
    ocultarEtapasFechamentoTodos();

    if ((_pagamento.Situacao.val() === EnumSituacaoPagamento.PendenciaFechamento) || (_pagamento.Situacao.val() === EnumSituacaoPagamento.Reprovado)) {
        _CRUDFechamentoPagamento.ProcessarNovamente.visible(true);
        _CRUDFechamentoPagamento.ConfirmarPagamento.visible(false);

        $("#divMotivoProblemaFechamento").show();
        $("#pMotivoProblemaFechamento").text(_pagamento.MotivoRejeicaoFechamentoPagamento.val());
    }
    else if (_pagamento.GerandoMovimentoFinanceiro.val())
        $("#knockoutPercentualFechamentoPagamento").show();
    else {
        _CRUDFechamentoPagamento.ProcessarNovamente.visible(_pagamento.Situacao.val() === EnumSituacaoPagamento.EmFechamento);

        ObterDetalhesFechamentoPagamento();
    }
}

function ocultarBotoesFechamento() {
    _CRUDFechamentoPagamento.ConfirmarPagamento.visible(false);
    _CRUDFechamentoPagamento.ProcessarNovamente.visible(false);
}

function SetarPercentualProcessamentoFechamentoPagamento(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualFechamentoPagamento.PercentualProcessado.val(strPercentual);
    $("#" + _percentualFechamentoPagamento.PercentualProcessado.id).css("width", strPercentual);
}

function LimparCamposProcessaomentoFechamentoPagamento() {
    SetarPercentualProcessamentoFechamentoPagamento(0);
    LimparCampos(_percentualFechamentoPagamento);
}
