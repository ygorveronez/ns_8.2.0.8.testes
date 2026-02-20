/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoOrdemCompra.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Solicitante = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Solicitante:", enable: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data da Solicitação:", enable: ko.observable(true) });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(false), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });

    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });

    this.ReprocessarRegras = PropertyEntity({ eventClick: reprocessarRegrasClick, type: types.event, text: "Reprocessar Regras", visible: ko.observable(false) });
    this.ExibirAprovadores = PropertyEntity({
        eventClick: toggleExibirAprovadoresClick, type: types.event, text: ko.pureComputed(function () {
            return this.ExibirAprovadores.val() ? "Exibir Aprovadores" : "Ocultar Aprovadores";
        }, this), visible: ko.observable(false), val: ko.observable(true), def: true
    });
};

var DetalheAutorizacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Regra = PropertyEntity({ text: "Regra:", val: ko.observable("") });
    this.Data = PropertyEntity({ text: "Data: ", val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação:", val: ko.observable("") });
    this.Usuario = PropertyEntity({ text: "Usuário:", val: ko.observable("") });
    this.Motivo = PropertyEntity({ text: "Motivo:", val: ko.observable(""), visible: ko.observable(true) });
};

//*******EVENTOS*******

function LoadAutorizacao() {
    _aprovacao = new Aprovacao();
    KoBindings(_aprovacao, "knockoutAprovacao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    GridAprovadores();
}

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacao.ExibirAprovadores.val();
    _aprovacao.ExibirAprovadores.val(!valorAtual);
    _aprovacao.DetalheAprovadores.visible(valorAtual);
}

function reprocessarRegrasClick(e, sender) {
    executarReST("OrdemCompra/ReprocessarRegras", { Codigo: _ordemCompra.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Ordem de Compra está aguardando aprovação.");
                    EditarOrdemPorCodigo(_ordemCompra.Codigo.val());
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Ordem de Compra.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Ordem de Compra.");
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Sem Regra", "Nenhuma regra para aprovar a Ordem de Compra.");
        }
    });
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "OrdemCompra/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                Global.abrirModal('divModalDetalhesAutorizacao');
                $("#divModalDetalhesAutorizacao").one('hidden.bs.modal', function () {
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
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "15", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "OrdemCompra/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}

function ListarAprovacoes(data) {
    _aprovacao.Codigo.val(data.Codigo);
    if (data.Situacao == EnumSituacaoOrdemCompra.SemRegra) {
        _aprovacao.PossuiRegras.val(false);

    } else {
        _aprovacao.PossuiRegras.val(true);

        PreencherResumo(data.Resumo);

        _gridAutorizacoes.CarregarGrid();
    }
}

function PreencherResumo(data) {
    if (data == null) return;
    PreencherObjetoKnout(_aprovacao, { Data: data });
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function LimparCamposAprovacao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacao);
    GridAprovadores();
}