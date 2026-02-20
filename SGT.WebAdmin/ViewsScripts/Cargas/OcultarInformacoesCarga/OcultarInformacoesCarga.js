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

var _pesquisaOcultarInformacoesCarga;
var _ocultarInformacoesCarga;
var _gridOcultarInformacoesCarga;
var _CRUDOcultarInformacoesCarga;

var PesquisaOcultarInformacoesCarga = function () {
    this.Descricao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Descricao.getFieldDescription(), required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridOcultarInformacoesCarga.CarregarGrid();
        }, type: types.event, text: Localization.Resources.Gerais.Geral.Pesquisar, idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ExibirFiltros, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var OcultarInformacoesCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição", getType: typesKnockout.string, required: true, val: ko.observable("") });

    this.ValorFrete = PropertyEntity({ text: "Valor do Frete", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.Rota = PropertyEntity({ text: "Rota", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.ValorNotaFiscal = PropertyEntity({ text: "Valor da Nota Fiscal", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.ValorProduto = PropertyEntity({ text: "Valor do Produto", getType: typesKnockout.bool, def: false, val: ko.observable(false) });
    this.VisualizarRota = PropertyEntity({ text: "Visualizar Rota", getType: typesKnockout.bool, def: false, val: ko.observable(false) });

    this.Usuarios = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.PerfisAcesso = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
}

var CRUDOcultarInformacoesCarga = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
}

function loadOcultarInformacoesCarga() {
    _pesquisaOcultarInformacoesCarga = new PesquisaOcultarInformacoesCarga();
    KoBindings(_pesquisaOcultarInformacoesCarga, "knockoutPesquisaOcultarInformacoesCarga", false, _pesquisaOcultarInformacoesCarga.Pesquisar.id);

    _ocultarInformacoesCarga = new OcultarInformacoesCarga();
    KoBindings(_ocultarInformacoesCarga, "knockoutOcultarInformacoesCarga");

    _CRUDOcultarInformacoesCarga = new CRUDOcultarInformacoesCarga();
    KoBindings(_CRUDOcultarInformacoesCarga, "knockoutCRUDOcultarInformacoesCarga");

    HeaderAuditoria("OcultarInformacoesCarga", _ocultarInformacoesCarga);

    LoadOcultarInformacoesUsuarios();
    LoadOcultarInformacoesPerfisAcesso();

    new BuscarFuncionario(_usuarios.Usuario, null, _gridUsuarios);
    new BuscarPerfilAcesso(_perfisAcesso.PerfilAcesso, null, null, _gridPerfisAcesso);

    BuscarOcultarInformacoesCarga();
}

function BuscarOcultarInformacoesCarga() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarOcultarInformacoesCargaClick, tamanho: "20", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [editar] };

    _gridOcultarInformacoesCarga = new GridView(_pesquisaOcultarInformacoesCarga.Pesquisar.idGrid, "OcultarInformacoesCarga/Pesquisa", _pesquisaOcultarInformacoesCarga, menuOpcoes, null);
    _gridOcultarInformacoesCarga.CarregarGrid();
}

function adicionarClick(e, sender) {
    preencherListasSelecao();

    Salvar(_ocultarInformacoesCarga, "OcultarInformacoesCarga/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridOcultarInformacoesCarga.CarregarGrid();
                LimparCamposOcultarInformacoesCargaClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    preencherListasSelecao();

    Salvar(_ocultarInformacoesCarga, "OcultarInformacoesCarga/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridOcultarInformacoesCarga.CarregarGrid();
                LimparCamposOcultarInformacoesCargaClick();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_ocultarInformacoesCarga, "OcultarInformacoesCarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridOcultarInformacoesCarga.CarregarGrid();
                    LimparCamposOcultarInformacoesCargaClick();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function cancelarClick() {
    LimparCamposOcultarInformacoesCargaClick();
}

function editarOcultarInformacoesCargaClick(itemGrid) {
    LimparCamposOcultarInformacoesCargaClick();

    _ocultarInformacoesCarga.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_ocultarInformacoesCarga, "OcultarInformacoesCarga/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                RecarregarGridUsuarios();
                RecarregarGridPerfisAcesso();
                AlterarVisibilidadeCamposOcultarInformacoes();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}


function preencherListasSelecao() {
    _ocultarInformacoesCarga.Usuarios.val(JSON.stringify(_gridUsuarios.BuscarRegistros()));
    _ocultarInformacoesCarga.PerfisAcesso.val(JSON.stringify(_gridPerfisAcesso.BuscarRegistros()));
}

function AlterarVisibilidadeCamposOcultarInformacoes() {
    _pesquisaOcultarInformacoesCarga.ExibirFiltros.visibleFade(false);
    _CRUDOcultarInformacoesCarga.Atualizar.visible(true);
    _CRUDOcultarInformacoesCarga.Excluir.visible(true);
    _CRUDOcultarInformacoesCarga.Cancelar.visible(true);
    _CRUDOcultarInformacoesCarga.Adicionar.visible(false);
}

function LimparCamposOcultarInformacoesCargaClick() {
    _CRUDOcultarInformacoesCarga.Atualizar.visible(false);
    _CRUDOcultarInformacoesCarga.Cancelar.visible(false);
    _CRUDOcultarInformacoesCarga.Excluir.visible(false);
    _CRUDOcultarInformacoesCarga.Adicionar.visible(true);
    LimparCamposUsuario();
    LimparCamposPerfilAcesso();
    LimparCampos(_ocultarInformacoesCarga);
    Global.ResetarAbas();
}