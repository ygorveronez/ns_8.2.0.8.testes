/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoLancamentoNFSManual.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _averbacoes;
var _gridAverbacoes;

var Averbacoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Averbacoes = PropertyEntity({ type: types.map, idGrid: guid() });

    this.Adicional = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "Adicional:", visible: ko.observable(true) });
    this.IOF = PropertyEntity({ type: types.map, getType: typesKnockout.decimal, val: ko.observable("0,00"), def: "", text: "IOF:", visible: ko.observable(true) });
    this.AtualizarValores = PropertyEntity({ eventClick: atualizarValoresClick, type: types.event, text: "Atualizar", visible: ko.observable(true) });
}

//*******EVENTOS*******
function loadAverbacoes() {
    _averbacoes = new Averbacoes();
    KoBindings(_averbacoes, "knockoutAverbacoes");

    GridAverbacoes();
}
function atualizarValoresClick(e, sender) {
    Salvar(_averbacoes, "FechamentoAverbacao/AtualizarValores", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");
                _gridAverbacoes.CarregarGrid();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

//*******MÉTODOS*******
function GridAverbacoes() {
    //-- Grid autorizadores
    _gridAverbacoes = new GridView(_averbacoes.Averbacoes.idGrid, "FechamentoAverbacao/PesquisaAverbacoes", _averbacoes, null, null, 25);
}

function EditarAverbacoes(data) {
    _averbacoes.Codigo.val(data.Codigo);
    _gridAverbacoes.CarregarGrid();
    if (data.DadosFechamento != null) {
        PreencherObjetoKnout(_averbacoes, { Data: data.Averbacoes });
    }

    if (_fechamentoAverbacao.Situacao.val() == EnumSituacaoFechamentoAverbacoes.Aberta) {
        _averbacoes.Adicional.visible(true);
        _averbacoes.IOF.visible(true);
        _averbacoes.AtualizarValores.visible(true);
    }
}

function LimparCamposAverbacoes() {
    LimparCampos(_averbacoes);
    _averbacoes.Adicional.visible(false);
    _averbacoes.IOF.visible(false);
    _averbacoes.AtualizarValores.visible(false);
}