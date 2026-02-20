/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/Filial.js" />  
/// <reference path="../../Consultas/TipoDocumentoTransporte.js" />  
/// <reference path="../../Consultas/CanalEntrega.js" />  
/// <reference path="../../Consultas/CanalVenda.js" />  
/// <reference path="../../Consultas/CategoriaPessoa.js" />  
/// <reference path="../../Consultas/TipoOperacao.js" />  
/// <reference path="../../Enumeradores/EnumTipoModal.js" />
/// <reference path="../../Enumeradores/EnumSimNaoPesquisa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridRegraTipoOperacao;
var _regraTipoOperacao;
var _pesquisaRegraTipoOperacao;

var PesquisaRegraTipoOperacao = function () {
    this.TipoDocumentoTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Documento Transporte: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Entrega: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MultiplaEtapa = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Todos), options: EnumSimNaoPesquisa.obterOpcoesPesquisa(), def: EnumSimNaoPesquisa.Todos, text: "Múltiplas Etapas: ", enable: ko.observable(true) });
    this.CategoriaCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Categoria Pessoa: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Ativo = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Operação: ", idBtnSearch: guid(), visible: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridRegraTipoOperacao.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });   
};

var RegraTipoOperacao = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoDocumentoTransporte = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Documento Transporte: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.MultiplaEtapa = PropertyEntity({ val: ko.observable(EnumSimNaoPesquisa.Sim), options: EnumSimNaoPesquisa.obterOpcoes(), def: EnumSimNaoPesquisa.Sim, text: "Múltiplas Etapas: ", enable: ko.observable(true) });
    this.CategoriaCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Categoria Pessoa: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.Descricao = PropertyEntity({ val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "Tipo Operação: ", idBtnSearch: guid(), visible: ko.observable(true) });
    this.TipoModal = PropertyEntity({ val: ko.observable(EnumTipoModal.Rodoviario), options: EnumTipoModal.obterOpcoesPesquisa(), def: EnumTipoModal.Rodoviario, text: "Modal: ", enable: ko.observable(true) });
    this.CteGlobalizado = PropertyEntity({ val: ko.observable(EnumSimNao.Nao), options: EnumSimNao.obterOpcoes(), def: EnumSimNao.Sim, text: "CT-e Globalizado: ", enable: ko.observable(true) });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "Situação: " });

    this.Filiais = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Expedidores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Recebedores = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CanalVenda = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Destinatario = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.CanalEntrega = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.TiposOperacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
};

var CRUDRegraTipoOperacao = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadRegraTipoOperacao() {
    _regraTipoOperacao = new RegraTipoOperacao();
    KoBindings(_regraTipoOperacao, "knockoutCadastroRegraTipoOperacao");

    HeaderAuditoria("RegraTipoOperacao", _regraTipoOperacao);

    _crudRegraTipoOperacao = new CRUDRegraTipoOperacao();
    KoBindings(_crudRegraTipoOperacao, "knockoutCRUDRegraTipoOperacao");

    _pesquisaRegraTipoOperacao = new PesquisaRegraTipoOperacao();
    KoBindings(_pesquisaRegraTipoOperacao, "knockoutPesquisaRegraTipoOperacao", false, _pesquisaRegraTipoOperacao.Pesquisar.id);

    new BuscarTipoDocumentoTransporte(_pesquisaRegraTipoOperacao.TipoDocumentoTransporte);
    new BuscarTipoDocumentoTransporte(_regraTipoOperacao.TipoDocumentoTransporte);

    new BuscarCategoriaPessoa(_pesquisaRegraTipoOperacao.CategoriaCliente);
    new BuscarCategoriaPessoa(_regraTipoOperacao.CategoriaCliente);

    new BuscarTiposOperacao(_regraTipoOperacao.TipoOperacao);
    new BuscarTiposOperacao(_pesquisaRegraTipoOperacao.TipoOperacao);

    buscarRegraTipoOperacao();
    LoadRegraTipoOperacaoFiliais();
    LoadRegraTipoOperacaoExpedidores();
    LoadRegraTipoOperacaoRecebedores();
    LoadRegraTipoOperacaoCanalVenda();
    LoadRegraTipoOperacaoCanalEntrega();
    LoadRegraTipoOperacaoDestinatario();
    LoadRegraTipoOperacaoTiposOperacao();
}

function adicionarClick(e, sender) {
    preencherListasRegraTipoOperacao();
    preencherListasRegraTipoOperacaoExpedidores();
    preencherListasRegraTipoOperacaoRecebedores();
    preencherListasRegraTipoOperacaoCanalVenda();
    preencherListasRegraTipoOperacaoCanalEntrega();
    preencherListasRegraTipoOperacaoDestinatario();
    preencherListasRegraTipoOperacaoTiposOperacao();
    Salvar(_regraTipoOperacao, "RegraTipoOperacao/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridRegraTipoOperacao.CarregarGrid();
                limparCamposRegraTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasRegraTipoOperacao();
    preencherListasRegraTipoOperacaoExpedidores();
    preencherListasRegraTipoOperacaoRecebedores();
    preencherListasRegraTipoOperacaoCanalVenda();
    preencherListasRegraTipoOperacaoCanalEntrega();
    preencherListasRegraTipoOperacaoDestinatario();
    preencherListasRegraTipoOperacaoTiposOperacao();
    Salvar(_regraTipoOperacao, "RegraTipoOperacao/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridRegraTipoOperacao.CarregarGrid();
                limparCamposRegraTipoOperacao();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a a Regra Tipo Operação " + _regraTipoOperacao.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_regraTipoOperacao, "RegraTipoOperacao/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridRegraTipoOperacao.CarregarGrid();
                    limparCamposRegraTipoOperacao();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposRegraTipoOperacao();
}

function preencherListasRegraTipoOperacao() {
    _regraTipoOperacao.Filiais.val(JSON.stringify(_gridRegraTipoOperacaoFiliais.BuscarRegistros()));
}
function preencherListasRegraTipoOperacaoExpedidores() {
    _regraTipoOperacao.Expedidores.val(JSON.stringify(_gridRegraTipoOperacaoExpedidores.BuscarRegistros()));
}

function preencherListasRegraTipoOperacaoRecebedores() {
    _regraTipoOperacao.Recebedores.val(JSON.stringify(_gridRegraTipoOperacaoRecebedores.BuscarRegistros()));
}

function preencherListasRegraTipoOperacaoCanalVenda() {
    _regraTipoOperacao.CanalVenda.val(JSON.stringify(_gridRegraTipoOperacaoCanalVenda.BuscarRegistros()));
}

function preencherListasRegraTipoOperacaoCanalEntrega() {
    _regraTipoOperacao.CanalEntrega.val(JSON.stringify(_gridRegraTipoOperacaoCanalEntrega.BuscarRegistros()));
}

function preencherListasRegraTipoOperacaoDestinatario() {
    _regraTipoOperacao.Destinatario.val(JSON.stringify(_gridRegraTipoOperacaoDestinatario.BuscarRegistros()));
}

function preencherListasRegraTipoOperacaoTiposOperacao() {
    _regraTipoOperacao.TiposOperacao.val(JSON.stringify(_gridTipoOperacao.BuscarRegistros()));
}

//*******MÉTODOS*******

function buscarRegraTipoOperacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarRegraTipoOperacao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    var configExportacao = {
        url: "RegraTipoOperacao/ExportarPesquisa",
        titulo: "Regras para Tipo de Operação"
    };

    _gridRegraTipoOperacao = new GridView(_pesquisaRegraTipoOperacao.Pesquisar.idGrid, "RegraTipoOperacao/Pesquisa", _pesquisaRegraTipoOperacao, menuOpcoes, null, 20, null, null, null, null, null, null, configExportacao);
    _gridRegraTipoOperacao.CarregarGrid();
}

function editarRegraTipoOperacao(regraTipoOperacao) {
    limparCamposRegraTipoOperacao();
    _regraTipoOperacao.Codigo.val(regraTipoOperacao.Codigo);
    BuscarPorCodigo(_regraTipoOperacao, "RegraTipoOperacao/BuscarPorCodigo", function (arg) {
        _pesquisaRegraTipoOperacao.ExibirFiltros.visibleFade(false);
        _crudRegraTipoOperacao.Atualizar.visible(true);
        _crudRegraTipoOperacao.Cancelar.visible(true);
        _crudRegraTipoOperacao.Excluir.visible(true);
        _crudRegraTipoOperacao.Adicionar.visible(false);
        RecarregarGridRegraTipoOperacaoFiliais();
        RecarregarGridRegraTipoOperacaoExpedidores();
        RecarregarGridRegraTipoOperacaoRecebedores();
        RecarregarGridRegraTipoOperacaoCanalEntrega();
        RecarregarGridRegraTipoOperacaoCanalVenda();
        RecarregarGridRegraTipoOperacaoDestinatario();
        RecarregarGridRegraTipoOperacaoTiposOperacao();
    }, null);
}

function limparCamposRegraTipoOperacao() {
    _crudRegraTipoOperacao.Atualizar.visible(false);
    _crudRegraTipoOperacao.Cancelar.visible(false);
    _crudRegraTipoOperacao.Excluir.visible(false);
    _crudRegraTipoOperacao.Adicionar.visible(true);
    LimparCampos(_regraTipoOperacao);
    LimparCamposRegraTipoOperacaoFiliais();
    LimparCamposRegraTipoOperacaoExpedidor();
    LimparCamposRegraTipoOperacaoRecebedor();
    LimparCamposRegraTipoOperacaoCanalEntrega();
    LimparCamposRegraTipoOperacaoCanalVenda();
    LimparCamposRegraTipoOperacaoDestinatario();
    RecarregarGridRegraTipoOperacaoTiposOperacao();
}