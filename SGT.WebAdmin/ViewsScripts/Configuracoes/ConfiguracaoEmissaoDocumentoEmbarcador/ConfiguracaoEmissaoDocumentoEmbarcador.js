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

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoEmissaoDocumentoEmbarcador;
var _configuracaoEmissaoDocumentoEmbarcador;
var _pesquisaConfiguracaoEmissaoDocumentoEmbarcador;

var PesquisaConfiguracaoEmissaoDocumentoEmbarcador = function () {
    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoEmissaoDocumentoEmbarcador.CarregarGrid();
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

var ConfiguracaoEmissaoDocumentoEmbarcador = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Cliente:", idBtnSearch: guid(), required: ko.observable(true) });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Tipo de Operação:", idBtnSearch: guid(), required: ko.observable(true) });
};

var CRUDConfiguracaoEmissaoDocumentoEmbarcador = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoEmissaoDocumentoEmbarcador() {
    _configuracaoEmissaoDocumentoEmbarcador = new ConfiguracaoEmissaoDocumentoEmbarcador();
    KoBindings(_configuracaoEmissaoDocumentoEmbarcador, "knockoutCadastroknockoutConfiguracaoEmissaoDocumentoEmbarcador");

    HeaderAuditoria("ConfiguracaoEmissaoDocumentoEmbarcador", _configuracaoEmissaoDocumentoEmbarcador);

    _crudConfiguracaoEmissaoDocumentoEmbarcador = new CRUDConfiguracaoEmissaoDocumentoEmbarcador();
    KoBindings(_crudConfiguracaoEmissaoDocumentoEmbarcador, "knockoutCRUDConfiguracaoEmissaoDocumentoEmbarcador");

    _pesquisaConfiguracaoEmissaoDocumentoEmbarcador = new PesquisaConfiguracaoEmissaoDocumentoEmbarcador();
    KoBindings(_pesquisaConfiguracaoEmissaoDocumentoEmbarcador, "knockoutPesquisaConfiguracaoEmissaoDocumentoEmbarcador", false, _pesquisaConfiguracaoEmissaoDocumentoEmbarcador.Pesquisar.id);

    buscarConfiguracaoEmissaoDocumentoEmbarcador();

    new BuscarTiposOperacao(_pesquisaConfiguracaoEmissaoDocumentoEmbarcador.TipoOperacao);
    new BuscarClientes(_pesquisaConfiguracaoEmissaoDocumentoEmbarcador.Cliente);

    new BuscarTiposOperacao(_configuracaoEmissaoDocumentoEmbarcador.TipoOperacao);
    new BuscarClientes(_configuracaoEmissaoDocumentoEmbarcador.Cliente);
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoEmissaoDocumentoEmbarcador, "ConfiguracaoEmissaoDocumentoEmbarcador/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridConfiguracaoEmissaoDocumentoEmbarcador.CarregarGrid();
                limparCamposConfiguracaoEmissaoDocumentoEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoEmissaoDocumentoEmbarcador, "ConfiguracaoEmissaoDocumentoEmbarcador/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoEmissaoDocumentoEmbarcador.CarregarGrid();
                limparCamposConfiguracaoEmissaoDocumentoEmbarcador();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Configuração de Emissão de documentos?", function () {
        ExcluirPorCodigo(_configuracaoEmissaoDocumentoEmbarcador, "ConfiguracaoEmissaoDocumentoEmbarcador/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoEmissaoDocumentoEmbarcador.CarregarGrid();
                    limparCamposConfiguracaoEmissaoDocumentoEmbarcador();
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
    limparCamposConfiguracaoEmissaoDocumentoEmbarcador();
}

//*******MÉTODOS*******

function buscarConfiguracaoEmissaoDocumentoEmbarcador() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoEmissaoDocumentoEmbarcador, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoEmissaoDocumentoEmbarcador = new GridView(_pesquisaConfiguracaoEmissaoDocumentoEmbarcador.Pesquisar.idGrid, "ConfiguracaoEmissaoDocumentoEmbarcador/Pesquisa", _pesquisaConfiguracaoEmissaoDocumentoEmbarcador, menuOpcoes);
    _gridConfiguracaoEmissaoDocumentoEmbarcador.CarregarGrid();
}

function editarConfiguracaoEmissaoDocumentoEmbarcador(configuracaoEmissaoDocumentoEmbarcadorGrid) {
    limparCamposConfiguracaoEmissaoDocumentoEmbarcador();
    _configuracaoEmissaoDocumentoEmbarcador.Codigo.val(configuracaoEmissaoDocumentoEmbarcadorGrid.Codigo);
    BuscarPorCodigo(_configuracaoEmissaoDocumentoEmbarcador, "ConfiguracaoEmissaoDocumentoEmbarcador/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoEmissaoDocumentoEmbarcador.ExibirFiltros.visibleFade(false);
        _crudConfiguracaoEmissaoDocumentoEmbarcador.Atualizar.visible(true);
        _crudConfiguracaoEmissaoDocumentoEmbarcador.Cancelar.visible(true);
        _crudConfiguracaoEmissaoDocumentoEmbarcador.Excluir.visible(true);
        _crudConfiguracaoEmissaoDocumentoEmbarcador.Adicionar.visible(false);
    }, null);
}

function limparCamposConfiguracaoEmissaoDocumentoEmbarcador() {
    _crudConfiguracaoEmissaoDocumentoEmbarcador.Atualizar.visible(false);
    _crudConfiguracaoEmissaoDocumentoEmbarcador.Cancelar.visible(false);
    _crudConfiguracaoEmissaoDocumentoEmbarcador.Excluir.visible(false);
    _crudConfiguracaoEmissaoDocumentoEmbarcador.Adicionar.visible(true);
    LimparCampos(_configuracaoEmissaoDocumentoEmbarcador);
}