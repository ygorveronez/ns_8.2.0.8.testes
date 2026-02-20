/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoFuncionarioComissao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _aprovacaoFuncionarioComissao;
var _detalheAutorizacao;
var _gridAutorizacoes;

var AprovacaoFuncionarioComissao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ValorFinal = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Final:", enable: ko.observable(true) });
    this.QuantidadeTitulos = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Quantidade de Títulos:", enable: ko.observable(true) });
    this.DataGeracao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Data Geração:", enable: ko.observable(true) });
    this.Funcionario = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Funcionário:", enable: ko.observable(true) });
    this.Operador = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Operador:", enable: ko.observable(true) });
    this.PercentualComissao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "% Comissão:", enable: ko.observable(true) });
    this.PercentualComissaoAcrescimo = PropertyEntity({ type: types.map, val: ko.observable(""), text: "+ Comissão:", enable: ko.observable(true) });
    this.PercentualComissaoTotal = PropertyEntity({ type: types.map, val: ko.observable(""), text: "% Total Comissão:", enable: ko.observable(true) });
    this.ValorComissao = PropertyEntity({ type: types.map, val: ko.observable(""), text: "Valor Comissão:", enable: ko.observable(true) });
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
function LoadAutorizacaoFuncionarioComissao() {
    _aprovacaoFuncionarioComissao = new AprovacaoFuncionarioComissao();
    KoBindings(_aprovacaoFuncionarioComissao, "knockoutAprovacaoFuncionarioComissao");

    _detalheAutorizacao = new DetalheAutorizacao();
    KoBindings(_detalheAutorizacao, "knockoutDetalheAutorizacao");

    // Inicia grids
    GridAprovadores();
}

function toggleExibirAprovadoresClick() {
    var valorAtual = _aprovacaoFuncionarioComissao.ExibirAprovadores.val();
    _aprovacaoFuncionarioComissao.ExibirAprovadores.val(!valorAtual);
    _aprovacaoFuncionarioComissao.DetalheAprovadores.visible(valorAtual);
}

function reprocessarRegrasClick(e, sender) {
    executarReST("FuncionarioComissao/ReprocessarRegras", { Codigo: _funcionarioComissao.Codigo.val() }, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Comissão está aguardando aprovação.");
                    _gridFuncionarioComissao.CarregarGrid();
                    BuscarComissaoPorCodigo(_funcionarioComissao.Codigo.val());
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sem Regra", "Nenhuma regra para aprovar a Comissão.");
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    });
}

function detalharAutorizacaoClick(dataRow) {
    _detalheAutorizacao.Codigo.val(dataRow.Codigo);

    BuscarPorCodigo(_detalheAutorizacao, "FuncionarioComissao/DetalhesAutorizacao", function (arg) {
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
    //-- Grid autorizadores
    var detalhes = { descricao: "Detalhes", id: "clasEditar", evento: "onclick", metodo: detalharAutorizacaoClick, tamanho: "4", icone: "" };
    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [detalhes]
    };

    _gridAutorizacoes = new GridView(_aprovacaoFuncionarioComissao.Regras.idGrid, "FuncionarioComissao/PesquisaAutorizacoes", _aprovacaoFuncionarioComissao, menuOpcoes);
    _gridAutorizacoes.CarregarGrid();
}


function ListarAprovacoes(data) {
    _aprovacaoFuncionarioComissao.Codigo.val(data.Codigo);
    if (data.Situacao == EnumSituacaoFuncionarioComissao.SemRegra) {
        _aprovacaoFuncionarioComissao.PossuiRegras.val(false);

    } else {
        _aprovacaoFuncionarioComissao.PossuiRegras.val(true);

        PreencherResumo(data.Resumo);

        _gridAutorizacoes.CarregarGrid();
    }
}

function PreencherResumo(data) {
    if (data == null) return;
    PreencherObjetoKnout(_aprovacaoFuncionarioComissao, { Data: data });
}

function LimparCamposDetalheAutorizacao() {
    LimparCampos(_detalheAutorizacao);
}

function LimparCamposAprovacaoFuncionarioComissao() {
    LimparCampos(_detalheAutorizacao);
    LimparCampos(_aprovacaoFuncionarioComissao);
    GridAprovadores();
}