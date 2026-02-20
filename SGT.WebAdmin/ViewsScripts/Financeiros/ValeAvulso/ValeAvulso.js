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
/// <reference path="../../Enumeradores/EnumSituacaoValeAvulso.js" />
/// <reference path="../../Enumeradores/EnumTipoDocumentoValeAvulso.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _gridValeAvulso;
var _valeAvulso;
var _pesquisaValeAvulso;


var PesquisaValeAvulso = function () {
    this.Pessoa = PropertyEntity({ text: "Pessoa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
    this.Numero = PropertyEntity({ text: "Número Vale: ", maxlength: 200, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoValeAvulso.Todos), options: EnumSituacaoValeAvulso.obterOpcoesPesquisa(), def: EnumSituacaoValeAvulso.Todos, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ text: "Tipo Documento: ", val: ko.observable(EnumTipoDocumentoValeAvulso.Todos), options: EnumTipoDocumentoValeAvulso.obterOpcoesPesquisa(), def: EnumTipoDocumentoValeAvulso.Todos, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridValeAvulso.CarregarGrid();
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

var ValeAvulso = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Numero = PropertyEntity({ text: "Número Vale:", maxlength: 200, visible: ko.observable(true), enable: ko.observable(false) });
    this.RecebidoDe = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "*Recebido de:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Valor = PropertyEntity({ text: "*Valor:", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false }, maxlength: 12, required: true, enable: ko.observable(true) });
    this.Data = PropertyEntity({ text: "*Data:", getType: typesKnockout.date, val: ko.observable(""), def: "", required: ko.observable(true), enable: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(EnumSituacaoValeAvulso.Aberto), options: EnumSituacaoValeAvulso.obterOpcoes(), def: EnumSituacaoValeAvulso.Aberto, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.TipoDocumento = PropertyEntity({ text: "*Tipo de Documento: ", required: ko.observable(true), val: ko.observable(EnumTipoDocumentoValeAvulso.Todos), options: EnumTipoDocumentoValeAvulso.obterOpcoes(), def: EnumTipoDocumentoValeAvulso.Todos, required: true, enable: ko.observable(true), visible: ko.observable(true) });
    this.Pessoa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: ko.observable(true), text: "*Pessoa:", idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(true) });
    this.Correspondente = PropertyEntity({ text: "Correspondente ao:", maxlength: 2000, enable: ko.observable(true), visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.VisualizarVale = PropertyEntity({ eventClick: visualizarValeClick, type: types.event, text: "Visualizar Vale", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadValeAvulso() {
    _pesquisaValeAvulso = new PesquisaValeAvulso();
    KoBindings(_pesquisaValeAvulso, "knockoutPesquisaValeAvulso", false, _pesquisaValeAvulso.Pesquisar.id);

    _valeAvulso = new ValeAvulso();
    KoBindings(_valeAvulso, "knockoutCadastroValeAvulso");

    HeaderAuditoria("ValeAvulso", _valeAvulso);

    buscarValeAvulso();

    BuscarEmpresa(_valeAvulso.RecebidoDe);
    BuscarClientes(_valeAvulso.Pessoa);
    BuscarClientes(_pesquisaValeAvulso.Pessoa);
}

function adicionarClick(e, sender) {
    Salvar(e, "ValeAvulso/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridValeAvulso.CarregarGrid();
                limparCamposValeAvulso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "ValeAvulso/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridValeAvulso.CarregarGrid();
                limparCamposValeAvulso();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse Vale Avulso?", function () {
        ExcluirPorCodigo(_valeAvulso, "ValeAvulso/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridValeAvulso.CarregarGrid();
                limparCamposValeAvulso();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposValeAvulso();
}

function visualizarValeClick(e) {
    executarDownload("ValeAvulso/DownloadValeAvulso", { Codigo: _valeAvulso.Codigo.val() });
}

//*******MÉTODOS*******


function buscarValeAvulso() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarValeAvulso, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridValeAvulso = new GridView(_pesquisaValeAvulso.Pesquisar.idGrid, "ValeAvulso/Pesquisa", _pesquisaValeAvulso, menuOpcoes, null);
    _gridValeAvulso.CarregarGrid();
}

function editarValeAvulso(valeAvulsoGrid) {
    limparCamposValeAvulso();
    _valeAvulso.Codigo.val(valeAvulsoGrid.Codigo);
    BuscarPorCodigo(_valeAvulso, "ValeAvulso/BuscarPorCodigo", function (arg) {
        _pesquisaValeAvulso.ExibirFiltros.visibleFade(false);
        _valeAvulso.Atualizar.visible(true);
        _valeAvulso.Cancelar.visible(true);
        _valeAvulso.Excluir.visible(true);
        _valeAvulso.Adicionar.visible(false);

        controlarCamposHabilitados();

    }, null);
}

function limparCamposValeAvulso() {
    _valeAvulso.Atualizar.visible(false);
    _valeAvulso.Cancelar.visible(false);
    _valeAvulso.VisualizarVale.visible(false);
    _valeAvulso.Excluir.visible(false);
    _valeAvulso.Adicionar.visible(true);
    LimparCampos(_valeAvulso);
    controlarCamposHabilitados()
}

function controlarCamposHabilitados() {
    var isEdicao = _valeAvulso.Codigo.val() > 0;
    var isPermiteEditar = _valeAvulso.Situacao.val() === EnumSituacaoValeAvulso.Aberto;
    var isStatusFinalizado = _valeAvulso.Situacao.val() === EnumSituacaoValeAvulso.Finalizado;    

    var habilitarEdicao = !isEdicao || isPermiteEditar;

    _valeAvulso.RecebidoDe.enable(habilitarEdicao);
    _valeAvulso.Valor.enable(habilitarEdicao);
    _valeAvulso.Data.enable(habilitarEdicao);
    _valeAvulso.Situacao.enable(habilitarEdicao);
    _valeAvulso.Pessoa.enable(habilitarEdicao);
    _valeAvulso.Correspondente.enable(habilitarEdicao);
    _valeAvulso.TipoDocumento.enable(habilitarEdicao);

    if (!habilitarEdicao) {
        _valeAvulso.Atualizar.visible(false);
        _valeAvulso.Excluir.visible(false);
        _valeAvulso.VisualizarVale.visible(isStatusFinalizado);
        exibirMensagem(tipoMensagem.aviso, "Aviso", "O status do Vale não permite edição.");
    }
}
