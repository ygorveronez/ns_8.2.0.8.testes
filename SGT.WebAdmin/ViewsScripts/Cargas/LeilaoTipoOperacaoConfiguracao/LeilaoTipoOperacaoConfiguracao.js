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
/// <reference path="../../Enumeradores/EnumTipoOperacaoEmissao.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridLeilaoTipoOperacaoConfiguracao;
var _leilaoTipoOperacaoConfiguracao;
var _pesquisaLeilaoTipoOperacaoConfiguracao;

var _tipoOperacaoEmissao = [
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.vendaNormal), value: EnumTipoOperacaoEmissao.vendaNormal },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.EntregaArmazem), value: EnumTipoOperacaoEmissao.EntregaArmazem },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaArmazemCliente), value: EnumTipoOperacaoEmissao.VendaArmazemCliente },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaComRedespacho), value: EnumTipoOperacaoEmissao.VendaComRedespacho },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaTriangular), value: EnumTipoOperacaoEmissao.VendaTriangular }
];

var PesquisaLeilaoTipoOperacaoConfiguracao = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridLeilaoTipoOperacaoConfiguracao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    var tipoOperacaoPesquisa = _tipoOperacaoEmissao;
    tipoOperacaoPesquisa.unshift({ text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.todas), value: EnumTipoOperacaoEmissao.todas });
    this.TipoOperacaoEmissao = PropertyEntity({ val: ko.observable(EnumTipoOperacaoEmissao.todas), options: tipoOperacaoPesquisa, text: "Tipo de Operação: ", def: EnumTipoOperacaoEmissao.todas });
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

var LeilaoTipoOperacaoConfiguracao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacaoEmissao = PropertyEntity({ val: ko.observable(EnumTipoOperacaoEmissao.vendaNormal), options: _tipoOperacaoEmissao, text: "*Tipo de Operação: ", def: EnumTipoOperacaoEmissao.vendaNormal });
    this.PermiteLeilao = PropertyEntity({ text: "Permite Leilão? ", getType : typesKnockout.bool, val: ko.observable(true), def : true, required: false });
    this.LimiteTempoLeilaoEmHoras = PropertyEntity({ text: "Limite de Tempo para Termino do leilão (Horas): ", getType: typesKnockout.int, val: ko.observable(""), def: "", required: false });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadLeilaoTipoOperacaoConfiguracao() {

    _leilaoTipoOperacaoConfiguracao = new LeilaoTipoOperacaoConfiguracao();
    KoBindings(_leilaoTipoOperacaoConfiguracao, "knockoutCadastroLeilaoTipoOperacaoConfiguracao");

    _pesquisaLeilaoTipoOperacaoConfiguracao = new PesquisaLeilaoTipoOperacaoConfiguracao();
    KoBindings(_pesquisaLeilaoTipoOperacaoConfiguracao, "knockoutPesquisaLeilaoTipoOperacaoConfiguracao", false, _pesquisaLeilaoTipoOperacaoConfiguracao.Pesquisar.id);

    HeaderAuditoria("LeilaoTipoOperacaoConfiguracao", _leilaoTipoOperacaoConfiguracao);

    buscarLeilaoTipoOperacaoConfiguracaos();

}

function adicionarClick(e, sender) {
    Salvar(e, "LeilaoTipoOperacaoConfiguracao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridLeilaoTipoOperacaoConfiguracao.CarregarGrid();
                limparCamposLeilaoTipoOperacaoConfiguracao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "LeilaoTipoOperacaoConfiguracao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridLeilaoTipoOperacaoConfiguracao.CarregarGrid();
                limparCamposLeilaoTipoOperacaoConfiguracao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração de leilão para a operação " + EnumTipoOperacaoEmissaoDescricao(_leilaoTipoOperacaoConfiguracao.TipoOperacaoEmissao.val()) + "?", function () {
        ExcluirPorCodigo(_leilaoTipoOperacaoConfiguracao, "LeilaoTipoOperacaoConfiguracao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridLeilaoTipoOperacaoConfiguracao.CarregarGrid();
                limparCamposLeilaoTipoOperacaoConfiguracao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposLeilaoTipoOperacaoConfiguracao();
}

//*******MÉTODOS*******


function buscarLeilaoTipoOperacaoConfiguracaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarLeilaoTipoOperacaoConfiguracao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridLeilaoTipoOperacaoConfiguracao = new GridView(_pesquisaLeilaoTipoOperacaoConfiguracao.Pesquisar.idGrid, "LeilaoTipoOperacaoConfiguracao/Pesquisa", _pesquisaLeilaoTipoOperacaoConfiguracao, menuOpcoes, null);
    _gridLeilaoTipoOperacaoConfiguracao.CarregarGrid();
}

function editarLeilaoTipoOperacaoConfiguracao(leilaoTipoOperacaoConfiguracaoGrid) {
    limparCamposLeilaoTipoOperacaoConfiguracao();
    _leilaoTipoOperacaoConfiguracao.Codigo.val(leilaoTipoOperacaoConfiguracaoGrid.Codigo);
    BuscarPorCodigo(_leilaoTipoOperacaoConfiguracao, "LeilaoTipoOperacaoConfiguracao/BuscarPorCodigo", function (arg) {
        _pesquisaLeilaoTipoOperacaoConfiguracao.ExibirFiltros.visibleFade(false);
        _leilaoTipoOperacaoConfiguracao.Atualizar.visible(true);
        _leilaoTipoOperacaoConfiguracao.Cancelar.visible(true);
        _leilaoTipoOperacaoConfiguracao.Excluir.visible(true);
        _leilaoTipoOperacaoConfiguracao.Adicionar.visible(false);
    }, null);
}

function limparCamposLeilaoTipoOperacaoConfiguracao() {
    _leilaoTipoOperacaoConfiguracao.Atualizar.visible(false);
    _leilaoTipoOperacaoConfiguracao.Cancelar.visible(false);
    _leilaoTipoOperacaoConfiguracao.Excluir.visible(false);
    _leilaoTipoOperacaoConfiguracao.Adicionar.visible(true);
    LimparCampos(_leilaoTipoOperacaoConfiguracao);
}

