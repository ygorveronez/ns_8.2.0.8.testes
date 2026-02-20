/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoPagamentoAgregado.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;
var $detalhesAutorizacao;

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:" });
    this.Data = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data:" });
    this.Usuario = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Operador Soliciotou:" });
    this.Cliente = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Cliente:" });

    this.Valor = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Pagamento:" });
    this.ValorAdiantado = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Adiantamento:" });
    this.ValorAcrescimo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Acréscimo:" });
    this.ValorDesconto = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Desconto:" });
    this.DataPagamento = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data Pagamento:" });

    this.TotalAbastecimentos = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Abastecimentos:" });
    this.TotalAdiantamentos = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Adiantamentos:" });
    this.TotalPagamento = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Pagamento:" });
    this.Saldo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Saldo:" });
    this.TotalSaldo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Saldo Contrato:" });

    this.DataRetorno = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data do Retorno:" });
    this.Aprovador = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Operador Retorno:" });

    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });
    this.ResumoSolicitacao = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(true), def: true });
    this.ResumoRetorno = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(true), def: true });

    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Ocultar Aprovadores" : "Exibir Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
}

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadAprovacao() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    // Inicia grids
    GridAprovadores();

    $detalhesAutorizacao = $("#divModalDetalhesAutorizacao");
}

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();
    _aprovacao.ExibirAprovadores.val(!valorAtual);
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "PagamentoAgregado/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                $detalhesAutorizacao.modal("show");
                $detalhesAutorizacao.one('hidden.bs.modal', function () {
                    LimparCamposDetalheAutorizacao();
                });
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    });
}



//*******MÉTODOS*******
function GridAprovadores() {
    //-- Grid autorizadores
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "PagamentoAgregado/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}

function ListarAprovacoes(data) {
    _aprovacao.Codigo.val(data.Codigo);

    if (data.Situacao == EnumSituacaoPagamentoAgregado.Finalizado || data.Situacao == EnumSituacaoPagamentoAgregado.AgAprovacao || data.Situacao == EnumSituacaoPagamentoAgregado.Rejeitada) {
        _aprovacao.PossuiRegras.val(true);
        _aprovacao.ExibirAprovadores.visible(true);

        _gridAutorizacoes.CarregarGrid();

        _aprovacao.ResumoSolicitacao.visible(true);
        _aprovacao.ResumoRetorno.visible(true);
        PreencherResumo(data.Resumo);
    }

    if (data.Situacao == EnumSituacaoPagamentoAgregado.SemRegra) {
        _aprovacao.PossuiRegras.val(false);

        _aprovacao.ResumoSolicitacao.visible(false);
        _aprovacao.ResumoRetorno.visible(false);
    } else if (data.Situacao == EnumSituacaoPagamentoAgregado.AgAprovacao) {
        _aprovacao.ExibirAprovadores.val(true);
    } else if (data.Situacao == EnumSituacaoPagamentoAgregado.Finalizado) {
        _aprovacao.ExibirAprovadores.val(false);
    } else if (data.Situacao == EnumSituacaoPagamentoAgregado.Rejeitada) {
        _aprovacao.ExibirAprovadores.visible(false);
        _aprovacao.ExibirAprovadores.val(true);

        _aprovacao.ResumoSolicitacao.visible(false);
        _aprovacao.ResumoRetorno.visible(false);
    }
}

function PreencherResumo(data) {
    if (data == null) return;

    if (data.ResumoSolicitacao != null)
        PreencherObjetoKnout(_aprovacao, { Data: data.ResumoSolicitacao });
    else
        _aprovacao.ResumoSolicitacao.visible(false);

    if (data.ResumoRetorno != null)
        PreencherObjetoKnout(_aprovacao, { Data: data.ResumoRetorno });
    else
        _aprovacao.ResumoRetorno.visible(false);
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function LimparCamposAprovacao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);
    GridAprovadores();
}