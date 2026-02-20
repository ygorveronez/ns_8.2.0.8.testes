/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumRetornoCargaTipo.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoRetornoCarga;
var _tipoRetornoCarga;
var _pesquisaTipoRetornoCarga;

var PesquisaTipoRetornoCarga = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getFieldDescription() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoDeOperacao.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoRetornoCarga.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.FiltroPesquisa, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var TipoRetornoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription(), issue: 586, required: true });
    this.CodigoIntegracao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.CodigoIntegracao.getFieldDescription(), maxlength: 50, issue: 15 });
    this.Tipo = PropertyEntity({ val: ko.observable(""), options: EnumRetornoCargaTipo.obterOpcoes(), def: "", text: Localization.Resources.Gerais.Geral.Tipo.getRequiredFieldDescription(), required: true });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Gerais.Geral.TipoDeOperacao.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoOperacaoCargaColeta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.TipoRetornoCarga.TipoOperacaoCargaColeta.getFieldDescription(), required: false, idBtnSearch: guid(), visible: ko.observable(true) });

    this.ExigeClienteColeta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.TipoRetornoCarga.EsseTipoRetornoExigeQueInformeClienteColeta, def: false });
    this.GerarCargaDeColeta = PropertyEntity({ val: ko.observable(false), getType: typesKnockout.bool, text: Localization.Resources.Cargas.TipoRetornoCarga.GerarCargaColetaTipoRetorno, def: false });

    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: Localization.Resources.Gerais.Geral.Situacao.getRequiredFieldDescription(), issue: 556 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadTipoRetornoCarga() {
    _tipoRetornoCarga = new TipoRetornoCarga();
    KoBindings(_tipoRetornoCarga, "knockoutCadastroTipoRetornoCarga");

    _pesquisaTipoRetornoCarga = new PesquisaTipoRetornoCarga();
    KoBindings(_pesquisaTipoRetornoCarga, "knockoutPesquisaTipoRetornoCarga", false, _pesquisaTipoRetornoCarga.Pesquisar.id);

    HeaderAuditoria("TipoRetornoCarga", _tipoRetornoCarga);

    new BuscarTiposOperacao(_tipoRetornoCarga.TipoOperacao);
    new BuscarTiposOperacao(_tipoRetornoCarga.TipoOperacaoCargaColeta);
    new BuscarTiposOperacao(_pesquisaTipoRetornoCarga.TipoOperacao);

    buscarTipoRetornoCargas();
}

function adicionarClick(e, sender) {
    Salvar(e, "TipoRetornoCarga/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.CadastradoComSucesso);
                _gridTipoRetornoCarga.CarregarGrid();
                limparCamposTipoRetornoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TipoRetornoCarga/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.AtualizadoComSucesso);
                _gridTipoRetornoCarga.CarregarGrid();
                limparCamposTipoRetornoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Gerais.Geral.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    }, sender);
}

function cancelarClick(e) {
    limparCamposTipoRetornoCarga();
}

function excluirClick(e, sender) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.TipoRetornoCarga.RealmenteDesejaExcluirTipoRetornoCarga + _tipoRetornoCarga.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoRetornoCarga, "TipoRetornoCarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, Localization.Resources.Gerais.Geral.ExcluidoComSucesso);
                _gridTipoRetornoCarga.CarregarGrid();
                limparCamposTipoRetornoCarga();
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        }, null);
    });
}

//*******MÉTODOS*******

function buscarTipoRetornoCargas() {
    var editar = { descricao: Localization.Resources.Gerais.Geral.Editar, id: "clasEditar", evento: "onclick", metodo: editarTipoRetornoCarga, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoRetornoCarga = new GridView(_pesquisaTipoRetornoCarga.Pesquisar.idGrid, "TipoRetornoCarga/Pesquisa", _pesquisaTipoRetornoCarga, menuOpcoes, null);
    _gridTipoRetornoCarga.CarregarGrid();
}

function editarTipoRetornoCarga(tipoRetornoCargaGrid) {
    limparCamposTipoRetornoCarga();
    _tipoRetornoCarga.Codigo.val(tipoRetornoCargaGrid.Codigo);
    BuscarPorCodigo(_tipoRetornoCarga, "TipoRetornoCarga/BuscarPorCodigo", function (arg) {
        _pesquisaTipoRetornoCarga.ExibirFiltros.visibleFade(false);
        _tipoRetornoCarga.Atualizar.visible(true);
        _tipoRetornoCarga.Cancelar.visible(true);
        _tipoRetornoCarga.Excluir.visible(true);
        _tipoRetornoCarga.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoRetornoCarga() {
    _tipoRetornoCarga.Atualizar.visible(false);
    _tipoRetornoCarga.Cancelar.visible(false);
    _tipoRetornoCarga.Excluir.visible(false);
    _tipoRetornoCarga.Adicionar.visible(true);
    LimparCampos(_tipoRetornoCarga);
}
