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
/// <reference path="../../Consultas/Localidade.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridNaturezaOperacao;
var _naturezaOperacao;
var _pesquisaNaturezaOperacao;

var PesquisaNaturezaOperacao = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: "});

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridNaturezaOperacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var NaturezaOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, visible: false, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ required: true, maxlength: 60, text: "*Descrição:", issue: 586 });
    this.CodigoIntegracao = PropertyEntity({ required: false, maxlength: 50, text: "Codigo de Integração:", issue: 15});

    this.Adicionar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: salvarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadNaturezaOperacao() {

    _naturezaOperacao = new NaturezaOperacao();
    KoBindings(_naturezaOperacao, "knockoutCadastroNaturezaOperacao");

    _pesquisaNaturezaOperacao = new PesquisaNaturezaOperacao();
    KoBindings(_pesquisaNaturezaOperacao, "knockoutPesquisaNaturezaOperacao", false, _pesquisaNaturezaOperacao.Pesquisar.id);

    HeaderAuditoria("NaturezaDaOperacao", _naturezaOperacao);

    buscarNaturezasOperacoes();

}

function salvarClick(e, sender) {
    Salvar(e, "NaturezaOperacao/Salvar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Dados salvos com sucesso");
                _gridNaturezaOperacao.CarregarGrid();
                limparCamposNaturezaOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposNaturezaOperacao();
}

//*******MÉTODOS*******

function buscarNaturezasOperacoes() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarNaturezaOperacao, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridNaturezaOperacao = new GridView(_pesquisaNaturezaOperacao.Pesquisar.idGrid, "NaturezaOperacao/Consultar", _pesquisaNaturezaOperacao, menuOpcoes, null);
    _gridNaturezaOperacao.CarregarGrid();
}

function editarNaturezaOperacao(naturezaOperacaoGrid) {
    limparCamposNaturezaOperacao();
    _naturezaOperacao.Codigo.val(naturezaOperacaoGrid.Codigo);
    BuscarPorCodigo(_naturezaOperacao, "NaturezaOperacao/BuscarPorCodigo", function (arg) {
        _pesquisaNaturezaOperacao.ExibirFiltros.visibleFade(false);
        _naturezaOperacao.Atualizar.visible(true);
        _naturezaOperacao.Cancelar.visible(true);
        _naturezaOperacao.Adicionar.visible(false);
    }, null);
}

function limparCamposNaturezaOperacao() {
    _naturezaOperacao.Atualizar.visible(false);
    _naturezaOperacao.Cancelar.visible(false);
    _naturezaOperacao.Adicionar.visible(true);
    LimparCampos(_naturezaOperacao);
}

