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
/// <reference path="../../../ViewsScripts/Consultas/Cliente.js" />
/// <reference path="../../../ViewsScripts/Consultas/Filial.js" />
/// <reference path="../../../ViewsScripts/Consultas/TipoOperacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoTempoChamado;
var _configuracaoTempoChamado;
var _pesquisaConfiguracaoTempoChamado;

var PesquisaConfiguracaoTempoChamado = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoTempoChamado.CarregarGrid();
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

var ConfiguracaoTempoChamado = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.TempoAtendimento = PropertyEntity({ text: "*Tempo Atendimento (minutos): ", getType: typesKnockout.int, required: ko.observable(true) });

    this.Cliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Cliente:", idBtnSearch: guid() });
    this.Filial = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Filial:", idBtnSearch: guid() });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid() });
};

var CRUDConfiguracaoTempoChamado = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoTempoChamado() {
    _configuracaoTempoChamado = new ConfiguracaoTempoChamado();
    KoBindings(_configuracaoTempoChamado, "knockoutCadastroConfiguracaoTempoChamado");

    HeaderAuditoria("ConfiguracaoTempoChamado", _configuracaoTempoChamado);

    _crudConfiguracaoTempoChamado = new CRUDConfiguracaoTempoChamado();
    KoBindings(_crudConfiguracaoTempoChamado, "knockoutCRUDConfiguracaoTempoChamado");

    _pesquisaConfiguracaoTempoChamado = new PesquisaConfiguracaoTempoChamado();
    KoBindings(_pesquisaConfiguracaoTempoChamado, "knockoutPesquisaConfiguracaoTempoChamado", false, _pesquisaConfiguracaoTempoChamado.Pesquisar.id);

    new BuscarClientes(_pesquisaConfiguracaoTempoChamado.Cliente);
    new BuscarFilial(_pesquisaConfiguracaoTempoChamado.Filial);
    new BuscarTiposOperacao(_pesquisaConfiguracaoTempoChamado.TipoOperacao);

    new BuscarClientes(_configuracaoTempoChamado.Cliente);
    new BuscarFilial(_configuracaoTempoChamado.Filial);
    new BuscarTiposOperacao(_configuracaoTempoChamado.TipoOperacao);

    buscarConfiguracaoTempoChamado();
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoTempoChamado, "ConfiguracaoTempoChamado/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridConfiguracaoTempoChamado.CarregarGrid();
                limparCamposConfiguracaoTempoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoTempoChamado, "ConfiguracaoTempoChamado/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoTempoChamado.CarregarGrid();
                limparCamposConfiguracaoTempoChamado();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a taxa " + _configuracaoTempoChamado.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_configuracaoTempoChamado, "ConfiguracaoTempoChamado/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoTempoChamado.CarregarGrid();
                    limparCamposConfiguracaoTempoChamado();
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
    limparCamposConfiguracaoTempoChamado();
}

//*******MÉTODOS*******

function buscarConfiguracaoTempoChamado() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracaoTempoChamado, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoTempoChamado = new GridView(_pesquisaConfiguracaoTempoChamado.Pesquisar.idGrid, "ConfiguracaoTempoChamado/Pesquisa", _pesquisaConfiguracaoTempoChamado, menuOpcoes);
    _gridConfiguracaoTempoChamado.CarregarGrid();
}

function editarConfiguracaoTempoChamado(configuracaoTempoChamadoGrid) {
    limparCamposConfiguracaoTempoChamado();
    _configuracaoTempoChamado.Codigo.val(configuracaoTempoChamadoGrid.Codigo);
    BuscarPorCodigo(_configuracaoTempoChamado, "ConfiguracaoTempoChamado/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoTempoChamado.ExibirFiltros.visibleFade(false);
        _crudConfiguracaoTempoChamado.Atualizar.visible(true);
        _crudConfiguracaoTempoChamado.Cancelar.visible(true);
        _crudConfiguracaoTempoChamado.Excluir.visible(true);
        _crudConfiguracaoTempoChamado.Adicionar.visible(false);
    }, null);
}

function limparCamposConfiguracaoTempoChamado() {
    _crudConfiguracaoTempoChamado.Atualizar.visible(false);
    _crudConfiguracaoTempoChamado.Cancelar.visible(false);
    _crudConfiguracaoTempoChamado.Excluir.visible(false);
    _crudConfiguracaoTempoChamado.Adicionar.visible(true);
    LimparCampos(_configuracaoTempoChamado);
}