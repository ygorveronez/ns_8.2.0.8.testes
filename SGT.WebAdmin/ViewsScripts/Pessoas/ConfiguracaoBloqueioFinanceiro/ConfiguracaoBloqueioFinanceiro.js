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
/// <reference path="../../Consultas/GrupoPessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridConfiguracaoBloqueioFinanceiro;
var _configuracaoBloqueioFinanceiro;
var _pesquisaConfiguracaoBloqueioFinanceiro;
var _crudConfiguracaoBloqueioFinanceiro;

var PesquisaConfiguracaoBloqueioFinanceiro = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConfiguracaoBloqueioFinanceiro.CarregarGrid();
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

var ConfiguracaoBloqueioFinanceiro = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string });

    this.HabilitarRegra = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(true), options: Global.ObterOpcoesBooleano("Sim", "Não"), text: "Habilitar Regra de Bloqueio", def: true });

    this.QuantidadeDiasAtrasoPagamento = PropertyEntity({ text: "Quantidade Dias Atraso Pagamento:", val: ko.observable(0), getType: typesKnockout.int, def: 0 });
    this.QuantidadeDiasNovoBloqueio = PropertyEntity({ text: "Quantidade Dias Novo Bloqueio:", val: ko.observable(0), getType: typesKnockout.int, def: 0 });

    this.GrupoPessoa = PropertyEntity({ text: "Grupo de Pessoa: ", type: types.entity, codEntity: ko.observable(0), idBtnSearch: guid() });
};

var CRUDConfiguracaoBloqueioFinanceiro = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadConfiguracaoBloqueioFinanceiro() {
    _pesquisaConfiguracaoBloqueioFinanceiro = new PesquisaConfiguracaoBloqueioFinanceiro();
    KoBindings(_pesquisaConfiguracaoBloqueioFinanceiro, "knockoutPesquisaConfiguracaoBloqueioFinanceiro");

    _configuracaoBloqueioFinanceiro = new ConfiguracaoBloqueioFinanceiro();
    KoBindings(_configuracaoBloqueioFinanceiro, "knockoutConfigConfiguracaoBloqueioFinanceiro");

    _crudConfiguracaoBloqueioFinanceiro = new CRUDConfiguracaoBloqueioFinanceiro();
    KoBindings(_crudConfiguracaoBloqueioFinanceiro, "knockoutCRUDConfiguracaoBloqueioFinanceiro");

    new BuscarGruposPessoas(_configuracaoBloqueioFinanceiro.GrupoPessoa);
    new BuscarGruposPessoas(_pesquisaConfiguracaoBloqueioFinanceiro.GrupoPessoa);

    HeaderAuditoria("ConfiguracaoBloqueioFinanceiro", _configuracaoBloqueioFinanceiro);

    loadGrid();
}

function loadGrid() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConfiguracao, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridConfiguracaoBloqueioFinanceiro = new GridView(_pesquisaConfiguracaoBloqueioFinanceiro.Pesquisar.idGrid, "ConfiguracaoBloqueioFinanceiro/Pesquisa", _pesquisaConfiguracaoBloqueioFinanceiro, menuOpcoes, null);
    _gridConfiguracaoBloqueioFinanceiro.CarregarGrid();
}

function editarConfiguracao(configuracaoBloqueioFinanceiro) {
    limparCamposConfiguracaoBloqueioFinanceiro();
    _configuracaoBloqueioFinanceiro.Codigo.val(configuracaoBloqueioFinanceiro.Codigo);
    BuscarPorCodigo(_configuracaoBloqueioFinanceiro, "ConfiguracaoBloqueioFinanceiro/BuscarPorCodigo", function (arg) {
        _pesquisaConfiguracaoBloqueioFinanceiro.ExibirFiltros.visibleFade(false);
        _crudConfiguracaoBloqueioFinanceiro.Atualizar.visible(true);
        _crudConfiguracaoBloqueioFinanceiro.Cancelar.visible(true);
        _crudConfiguracaoBloqueioFinanceiro.Excluir.visible(true);
        _crudConfiguracaoBloqueioFinanceiro.Adicionar.visible(false);


    }, null);
}

function adicionarClick(e, sender) {
    Salvar(_configuracaoBloqueioFinanceiro, "ConfiguracaoBloqueioFinanceiro/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridConfiguracaoBloqueioFinanceiro.CarregarGrid();
                limparCamposConfiguracaoBloqueioFinanceiro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_configuracaoBloqueioFinanceiro, "ConfiguracaoBloqueioFinanceiro/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConfiguracaoBloqueioFinanceiro.CarregarGrid();
                limparCamposConfiguracaoBloqueioFinanceiro();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a configuração" + _configuracaoBloqueioFinanceiro.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_configuracaoBloqueioFinanceiro, "ConfiguracaoBloqueioFinanceiro/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConfiguracaoBloqueioFinanceiro.CarregarGrid();
                    limparCamposConfiguracaoBloqueioFinanceiro();
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
    limparCamposConfiguracaoBloqueioFinanceiro();
}

function limparCamposConfiguracaoBloqueioFinanceiro() {
    _crudConfiguracaoBloqueioFinanceiro.Atualizar.visible(false);
    _crudConfiguracaoBloqueioFinanceiro.Cancelar.visible(false);
    _crudConfiguracaoBloqueioFinanceiro.Excluir.visible(false);
    _crudConfiguracaoBloqueioFinanceiro.Adicionar.visible(true);
    LimparCampos(_configuracaoBloqueioFinanceiro);
}
