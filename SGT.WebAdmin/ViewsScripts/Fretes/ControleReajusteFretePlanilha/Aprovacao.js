/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _aprovacao;
var _detalheAutorizacao;
var _gridAutorizacoes;
var _modalDetalhesAutorizacao;

var Aprovacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Solicitante = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Solicitante:", enable: ko.observable(true) });
    this.DataSolicitacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data da Solicitação:", enable: ko.observable(true) });
    this.AprovacoesNecessarias = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações Necessárias:", enable: ko.observable(true) });
    this.Aprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Aprovações:", enable: ko.observable(true) });
    this.Reprovacoes = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Reprovações:", enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Situação:", enable: ko.observable(true) });

    this.DetalheAprovadores = PropertyEntity({ type: types.map, getType: typesKnockout.bool, visible: ko.observable(true), val: ko.observable(true), def: true });
    this.PossuiRegras = PropertyEntity({ type: types.map, getType: typesKnockout.bool, val: ko.observable(true), def: true });

    this.Regras = PropertyEntity({ type: types.map, idGrid: guid() });
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
    _modalDetalhesAutorizacao = new bootstrap.Modal(document.getElementById("divModalDetalhesAutorizacao"), { backdrop: true, keyboard: true });
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "ControleReajusteFretePlanilha/DetalhesAutorizacao", function (arg) {
        if (arg.Success) {
            if (arg.Data != null) {
                _modalDetalhesAutorizacao.show();
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
    //-- Grid autorizadores
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacao.Regras.idGrid, "ControleReajusteFretePlanilha/PesquisaAutorizacoes", _aprovacao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}


function ListarAprovacoes(data) {
    _aprovacao.Codigo.val(data.Codigo);
    if (_controleReajusteFretePlanilha.Situacao.val() == EnumSituacaoControleReajusteFretePlanilha.SemRegra) {
        _aprovacao.PossuiRegras.val(false);
        _aprovacao.DetalheAprovadores.visible(true);
    } else {
        _aprovacao.PossuiRegras.val(true);
        _aprovacao.DetalheAprovadores.visible(true);

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