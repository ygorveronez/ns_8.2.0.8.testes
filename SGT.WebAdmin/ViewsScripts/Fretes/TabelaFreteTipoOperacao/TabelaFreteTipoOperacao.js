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
/// <reference path="../../Consultas/TabelaFrete.js" />


//*******MAPEAMENTO KNOUCKOUT*******


var _gridTabelaFreteTipoOperacao;
var _tabelaFreteTipoOperacao;
var _pesquisaTabelaFreteTipoOperacao;

var _tipoOperacaoEmissao = [
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.vendaNormal), value: EnumTipoOperacaoEmissao.vendaNormal },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.EntregaArmazem), value: EnumTipoOperacaoEmissao.EntregaArmazem },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaArmazemCliente), value: EnumTipoOperacaoEmissao.VendaArmazemCliente },
        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaComRedespacho), value: EnumTipoOperacaoEmissao.VendaComRedespacho }
];

var PesquisaTabelaFreteTipoOperacao = function () {
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTabelaFreteTipoOperacao.CarregarGrid();
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

var TabelaFreteTipoOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacaoEmissao = PropertyEntity({ val: ko.observable(EnumTipoOperacaoEmissao.vendaNormal), options: _tipoOperacaoEmissao, text: "*Tipo de Operação: ", eventChange: mudarTipoOperacaoOnChange, def: EnumTipoOperacaoEmissao.vendaNormal });
    this.TabelaFrete = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Tabela de Frete:", idBtnSearch: guid() });
    this.TabelaFreteRedespacho = PropertyEntity({ type: types.entity, visible: ko.observable(false), codEntity: ko.observable(0), required: false, text: "Tabela de Frete de Redespacho:", idBtnSearch: guid() });
    this.PagarPorTonelada = PropertyEntity({ val: ko.observable(false), def: false, visible: ko.observable(false), getType: typesKnockout.bool, text: "Se exceder o limite percentual cobrar por tonelada?" });
    this.PagarPorToneladaRedespacho = PropertyEntity({ val: ko.observable(false), def: false, visible: ko.observable(false), getType: typesKnockout.bool, text: "Se exceder o limite percentual cobrar por tonelada?" });
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTabelaFreteTipoOperacao() {

    _tabelaFreteTipoOperacao = new TabelaFreteTipoOperacao();
    KoBindings(_tabelaFreteTipoOperacao, "knockoutCadastroTabelaFreteTipoOperacao");

    _pesquisaTabelaFreteTipoOperacao = new PesquisaTabelaFreteTipoOperacao();
    KoBindings(_pesquisaTabelaFreteTipoOperacao, "knockoutPesquisaTabelaFreteTipoOperacao", false, _pesquisaTabelaFreteTipoOperacao.Pesquisar.id);

    new BuscarTabelasDeFrete(_tabelaFreteTipoOperacao.TabelaFrete, retornoTabelaFrete);
    new BuscarTabelasDeFrete(_tabelaFreteTipoOperacao.TabelaFreteRedespacho, retornoTabelaRedespacho);

    buscarTabelaFreteTipoOperacaos();

}

function retornoTabelaFrete(arg) {
    _tabelaFreteTipoOperacao.TabelaFrete.codEntity(arg.Codigo);
    _tabelaFreteTipoOperacao.TabelaFrete.val(arg.Descricao);
    if (arg.TipoTabelaFrete == EnumTipoTabelaFrete.tabelaComissaoProduto) {
        _tabelaFreteTipoOperacao.PagarPorTonelada.visible(true);
    } else {
        _tabelaFreteTipoOperacao.PagarPorTonelada.visible(false);
        _tabelaFreteTipoOperacao.PagarPorTonelada.val(_tabelaFreteTipoOperacao.PagarPorTonelada.def);
    }
}

function retornoTabelaRedespacho(arg){
    _tabelaFreteTipoOperacao.TabelaFreteRedespacho.codEntity(arg.Codigo);
    _tabelaFreteTipoOperacao.TabelaFreteRedespacho.val(arg.Descricao);
    if (arg.TipoTabelaFrete == EnumTipoTabelaFrete.tabelaComissaoProduto) {
        _tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.visible(true);
    } else {
        _tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.visible(false);
        _tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.val(_tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.def);
    }
}

function mudarTipoOperacaoOnChange(e, sender) {

    if (_tabelaFreteTipoOperacao.TipoOperacaoEmissao.val() == EnumTipoOperacaoEmissao.VendaComRedespacho) {
        _tabelaFreteTipoOperacao.TabelaFreteRedespacho.visible(true);
        _tabelaFreteTipoOperacao.TabelaFreteRedespacho.required = true;
    } else {
        _tabelaFreteTipoOperacao.TabelaFreteRedespacho.visible(false);
        _tabelaFreteTipoOperacao.TabelaFreteRedespacho.required = false;
    }

}

function adicionarClick(e, sender) {
    Salvar(e, "TabelaFreteTipoOperacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "cadastrado");
                _gridTabelaFreteTipoOperacao.CarregarGrid();
                limparCamposTabelaFreteTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TabelaFreteTipoOperacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "sucesso", "Atualizado com sucesso");
                _gridTabelaFreteTipoOperacao.CarregarGrid();
                limparCamposTabelaFreteTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração de leilão para a operação " + EnumTipoOperacaoEmissaoDescricao(_tabelaFreteTipoOperacao.TipoOperacaoEmissao.val()) + "?", function () {
        ExcluirPorCodigo(_tabelaFreteTipoOperacao, "TabelaFreteTipoOperacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTabelaFreteTipoOperacao.CarregarGrid();
                limparCamposTabelaFreteTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTabelaFreteTipoOperacao();
}

//*******MÉTODOS*******


function buscarTabelaFreteTipoOperacaos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTabelaFreteTipoOperacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTabelaFreteTipoOperacao = new GridView(_pesquisaTabelaFreteTipoOperacao.Pesquisar.idGrid, "TabelaFreteTipoOperacao/Pesquisa", _pesquisaTabelaFreteTipoOperacao, menuOpcoes, null);
    _gridTabelaFreteTipoOperacao.CarregarGrid();
}

function editarTabelaFreteTipoOperacao(tabelaFreteTipoOperacaoGrid) {
    limparCamposTabelaFreteTipoOperacao();
    _tabelaFreteTipoOperacao.Codigo.val(tabelaFreteTipoOperacaoGrid.Codigo);
    BuscarPorCodigo(_tabelaFreteTipoOperacao, "TabelaFreteTipoOperacao/BuscarPorCodigo", function (arg) {
        _pesquisaTabelaFreteTipoOperacao.ExibirFiltros.visibleFade(false);
        _tabelaFreteTipoOperacao.Atualizar.visible(true);
        _tabelaFreteTipoOperacao.Cancelar.visible(true);
        _tabelaFreteTipoOperacao.Excluir.visible(true);
        _tabelaFreteTipoOperacao.Adicionar.visible(false);

        if (arg.Data.TipoOperacaoEmissao == EnumTipoOperacaoEmissao.VendaComRedespacho) {
            _tabelaFreteTipoOperacao.TabelaFreteRedespacho.visible(true);
            _tabelaFreteTipoOperacao.TabelaFreteRedespacho.required = true;
        } else {
            _tabelaFreteTipoOperacao.TabelaFreteRedespacho.visible(false);
            _tabelaFreteTipoOperacao.TabelaFreteRedespacho.required = false;
        }

        if (arg.Data.TabelaFrete.TipoTabelaFrete == EnumTipoTabelaFrete.tabelaComissaoProduto) {
            _tabelaFreteTipoOperacao.PagarPorTonelada.visible(true);
        } else {
            _tabelaFreteTipoOperacao.PagarPorTonelada.visible(false);
            _tabelaFreteTipoOperacao.PagarPorTonelada.val(_tabelaFreteTipoOperacao.PagarPorTonelada.def);
        }

        if (arg.Data.TabelaFreteRedespacho != null && arg.Data.TabelaFreteRedespacho.TipoTabelaFrete == EnumTipoTabelaFrete.tabelaComissaoProduto) {
            _tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.visible(true);
        } else {
            _tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.visible(false);
            _tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.val(_tabelaFreteTipoOperacao.PagarPorToneladaRedespacho.def);
        }


    }, null);
}

function limparCamposTabelaFreteTipoOperacao() {
    _tabelaFreteTipoOperacao.Atualizar.visible(false);
    _tabelaFreteTipoOperacao.Cancelar.visible(false);
    _tabelaFreteTipoOperacao.Excluir.visible(false);
    _tabelaFreteTipoOperacao.Adicionar.visible(true);

    _tabelaFreteTipoOperacao.TabelaFreteRedespacho.visible(false);
    _tabelaFreteTipoOperacao.TabelaFreteRedespacho.required = false;

    LimparCampos(_tabelaFreteTipoOperacao);
}

