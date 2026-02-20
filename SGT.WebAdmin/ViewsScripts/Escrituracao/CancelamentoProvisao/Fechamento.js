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
/// <reference path="../../Enumeradores/EnumSituacaoCancelamentoProvisao.js" />

var _percentualCancelamentoFechamentoProvisao;
var _aprovacaoCancelamento;
var _cancelamentoFechamentoProvisao;

var PercentualCancelamentoFechamentoProvisao = function () {
    this.PercentualProcessado = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
};

var AprovacaoCancelamento = function () {
    this.Autorizacao = PropertyEntity({ visible: ko.observable(true) });
    this.SemRegraAprovacao = PropertyEntity({ visible: ko.observable(false) });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: ko.observable("Reprocessar Regras"), visible: ko.observable(false) });
    this.ReenviarAprovacao = PropertyEntity({ eventClick: reenviarAprovacaoCancelamento, type: types.event, text: ko.observable("Reenviar para Aprovação"), visible: ko.observable(false) });
    this.CancelarProcessamento = PropertyEntity({ eventClick: cancelarProcessamento, type: types.event, text: ko.observable("Cancelar Processamento"), visible: ko.observable(false) });
    this.UsuariosAutorizadores = PropertyEntity({ idGrid: guid(), visible: ko.observable(true) });
}

var CancelamentoFechamentoProvisao = function () {
    var dataAtual = moment().format("DD/MM/YYYY");

    this.Contabilizacao = PropertyEntity({ text: "Resumo Provisão: ", idGrid: guid() });
    this.DataLancamento = PropertyEntity({ text: "Data do Lançamento: ", getType: typesKnockout.date, visible: ko.observable(true), val: ko.observable(dataAtual), def: dataAtual, enable: false });
};

function LoadCancelamentoFechamentoProvisao() {
    CarregarHTMLCancelamentoFechamentoProvisao().then(function () {
        _cancelamentoFechamentoProvisao = new CancelamentoFechamentoProvisao();
        KoBindings(_cancelamentoFechamentoProvisao, "knoutCancelamentoFechamentoProvisao");

        _percentualCancelamentoFechamentoProvisao = new PercentualCancelamentoFechamentoProvisao();
        KoBindings(_percentualCancelamentoFechamentoProvisao, "knockoutPercentualCancelamentoFechamentoProvisao");

        _aprovacaoCancelamento = new AprovacaoCancelamento();
        KoBindings(_aprovacaoCancelamento, "knoutAprovacaoCancelamento");

        _gridAutorizacoes = new GridView(_aprovacaoCancelamento.UsuariosAutorizadores.idGrid, "CancelamentoProvisao/ConsultarAutorizacoes", _cancelamentoProvisao, null, null, null, null, null, null, null);
        _gridAutorizacoes.CarregarGrid();
    });

}

function ObterDetalhesFaturaContabilizacao() {
    var data = { Codigo: _cancelamentoProvisao.Codigo.val() };
    executarReST("CancelamentoProvisaoFechamento/ObterDetalhesFechamento", data, function (e) {
        if (e.Success) {
            if (e.Data !== false) {
                var header = [
                    { data: "CodigoContaContabil", title: "Código", width: "15%" },
                    { data: "DescricaoContaContabil", title: "Conta Contábil", width: "30%" },
                    { data: "CodigoCentroResultado", title: "Centro de Resultado", width: "15%" },
                    { data: "ValorContabilizacaoFormatado", title: "Valor", width: "15%" },
                    { data: "DescricaoTipoContabilizacao", title: "Contabilização", width: "15%" }
                ];
                var tableContabilizacao = new BasicDataTable(_cancelamentoFechamentoProvisao.Contabilizacao.idGrid, header, null, { column: 0, dir: orderDir.asc });
                tableContabilizacao.CarregarGrid(e.Data.DocumentosContabeis);
                _cancelamentoFechamentoProvisao.DataLancamento.val(e.Data.DataLancamento);
            } else {
                exibirMensagem(tipoMensagem.atencao, "atenção", e.Msg);
            }
        }
        else {
            exibirMensagem(tipoMensagem.falha, "falha", e.Msg);
        }
        $("#knoutCancelamentoFechamentoProvisao").show();
    });
}

function CarregarHTMLCancelamentoFechamentoProvisao(callback) {
    var prom = new promise.Promise();
    $.get("Content/Static/Escrituracao/CancelamentoFechamentoProvisao.html?dyn=" + guid(), function (data) {
        $("#contentCancelamentoFechamentoProvisao").html(data);
        prom.done();
    });
    return prom;
}

function ocultarEtapasFechamentoTodos() {
    SetarPercentualProcessamentoCancelamentoFechamentoProvisao(0);
    $("#knoutCancelamentoFechamentoProvisao").hide();
    $("#knockoutPercentualCancelamentoFechamentoProvisao").hide();
    $("#divMotivoProblemaFechamento").hide();
}

function buscarDadosCancelamentoFechamentoProvisao() {
    ocultarEtapasFechamentoTodos();

    if (_cancelamentoProvisao.Situacao.val() === EnumSituacaoCancelamentoProvisao.PendenciaFechamento) {
        $("#divMotivoProblemaFechamento").show();
        $("#pMotivoProblemaFechamento").text(_cancelamentoProvisao.MotivoRejeicaoCancelamentoFechamentoProvisao.val());
    }
    else if (_cancelamentoProvisao.GerandoMovimentoFinanceiro.val() && (_cancelamentoProvisao.Situacao.val() != EnumSituacaoCancelamentoProvisao.SemRegraAprovacao && _cancelamentoProvisao.Situacao.val() != EnumSituacaoCancelamentoProvisao.AgAprovacaoSolicitacao && _cancelamentoProvisao.Situacao.val() != EnumSituacaoCancelamentoProvisao.SolicitacaoReprovada)) {
        $("#knockoutPercentualCancelamentoFechamentoProvisao").show();
    }
    else if (!_cancelamentoProvisao.GerandoMovimentoFinanceiro.val()) {
        ObterDetalhesFaturaContabilizacao();
    }

}

function SetarPercentualProcessamentoCancelamentoFechamentoProvisao(percentual) {
    var strPercentual = parseInt(percentual) + "%";
    _percentualCancelamentoFechamentoProvisao.PercentualProcessado.val(strPercentual);
    $("#" + _percentualCancelamentoFechamentoProvisao.PercentualProcessado.id).css("width", strPercentual);
}

function LimparCamposProcessaomentoCancelamentoFechamentoProvisao() {
    SetarPercentualProcessamentoCancelamentoFechamentoProvisao(0);
    LimparCampos(_percentualCancelamentoFechamentoProvisao);
}

function reprocessarRegrasClick() {
    executarReST("AutorizacaoEstornoProvisao/Reprocessar", { Codigo: _cancelamentoProvisao.Codigo.val() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data.RegraReprocessada) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Regras de aprovação reprocessadas com sucesso.");
                _gridAutorizacoes.CarregarGrid();
                BuscarCancelamentoProvisaoPorCodigo(_cancelamentoProvisao.Codigo.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", retorno.Msg || "Nenhuma regra de aprovação encontrada.");
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function reenviarAprovacaoCancelamento() {
    executarReST("CancelamentoProvisao/ReenviarAprovacaoCancelamento", { Codigo: _cancelamentoProvisao.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de cancelamento reenviada com sucesso.");
        LimparCamposCancelamentoProvisao();
    })
}
function cancelarProcessamento() {
    executarReST("CancelamentoProvisao/CancelarProcessamento", { Codigo: _cancelamentoProvisao.Codigo.val() }, (arg) => {
        if (!arg.Success)
            return exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);

        exibirMensagem(tipoMensagem.ok, "Sucesso", "Solicitação de cancelamento reenviada com sucesso.");
        LimparCamposCancelamentoProvisao();
    })
}