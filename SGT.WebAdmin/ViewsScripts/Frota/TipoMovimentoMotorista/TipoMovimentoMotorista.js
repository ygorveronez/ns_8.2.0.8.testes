/// <reference path="../../Consultas/TipoMovimentoMotorista.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
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
/// <reference path="../../Consultas/PlanoConta.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridTipoMovimentoMotorista;
var _tipoMovimentoMotorista;
var _pesquisaTipoMovimentoMotorista;

var _situacaoPagamentoAdiantamentoMotorista = [{ text: "Entrada na fixa do motorista", value: EnumTipoMovimentoEntidade.Entrada }, { text: "Saída da fixa do motorista", value: EnumTipoMovimentoEntidade.Saida }];

var PesquisaTipoMovimentoMotorista = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta gerencial:", idBtnSearch: guid(), val: ko.observable("") });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoMovimentoMotorista.CarregarGrid();
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

var TipoMovimentoMotorista = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true });
    this.Situacao = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta gerencial:", idBtnSearch: guid(), required: true, val: ko.observable("") });
    this.TipoMovimentoEntidade = PropertyEntity({ text: "*Situação do Movimento: ", val: ko.observable(EnumTipoMovimentoEntidade.Entrada), options: _situacaoPagamentoAdiantamentoMotorista, def: EnumTipoMovimentoEntidade.Entrada, required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******


function loadTipoMovimentoMotorista() {
    _tipoMovimentoMotorista = new TipoMovimentoMotorista();
    KoBindings(_tipoMovimentoMotorista, "knockoutCadastroTipoMovimentoMotorista");

    new BuscarPlanoConta(_tipoMovimentoMotorista.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);

    _pesquisaTipoMovimentoMotorista = new PesquisaTipoMovimentoMotorista();
    KoBindings(_pesquisaTipoMovimentoMotorista, "knockoutPesquisaTipoMovimentoMotorista", false, _pesquisaTipoMovimentoMotorista.Pesquisar.id);

    HeaderAuditoria("TipoMovimentoMotorista", _tipoMovimentoMotorista);

    new BuscarPlanoConta(_pesquisaTipoMovimentoMotorista.PlanoConta, "Selecione a Conta Analítica", "Contas Analíticas", null, EnumAnaliticoSintetico.Analitico);

    buscarTipoMovimentoMotoristas();
}

function adicionarClick(e, sender) {
    Salvar(e, "TipoMovimentoMotorista/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoMovimentoMotorista.CarregarGrid();
                limparCamposTipoMovimentoMotorista();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "TipoMovimentoMotorista/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoMovimentoMotorista.CarregarGrid();
                limparCamposTipoMovimentoMotorista();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de movimento do motorista " + _tipoMovimentoMotorista.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoMovimentoMotorista, "TipoMovimentoMotorista/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoMovimentoMotorista.CarregarGrid();
                limparCamposTipoMovimentoMotorista();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoMovimentoMotorista();
}

//*******MÉTODOS*******


function buscarTipoMovimentoMotoristas() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTipoMovimentoMotorista, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoMovimentoMotorista = new GridView(_pesquisaTipoMovimentoMotorista.Pesquisar.idGrid, "TipoMovimentoMotorista/Pesquisa", _pesquisaTipoMovimentoMotorista, menuOpcoes);
    _gridTipoMovimentoMotorista.CarregarGrid();
}

function editarTipoMovimentoMotorista(tipoMovimentoMotoristaGrid) {
    limparCamposTipoMovimentoMotorista();
    _tipoMovimentoMotorista.Codigo.val(tipoMovimentoMotoristaGrid.Codigo);
    BuscarPorCodigo(_tipoMovimentoMotorista, "TipoMovimentoMotorista/BuscarPorCodigo", function (arg) {
        _pesquisaTipoMovimentoMotorista.ExibirFiltros.visibleFade(false);
        _tipoMovimentoMotorista.Atualizar.visible(true);
        _tipoMovimentoMotorista.Cancelar.visible(true);
        _tipoMovimentoMotorista.Excluir.visible(true);
        _tipoMovimentoMotorista.Adicionar.visible(false);
    }, null);
}

function limparCamposTipoMovimentoMotorista() {
    _tipoMovimentoMotorista.Atualizar.visible(false);
    _tipoMovimentoMotorista.Cancelar.visible(false);
    _tipoMovimentoMotorista.Excluir.visible(false);
    _tipoMovimentoMotorista.Adicionar.visible(true);
    LimparCampos(_tipoMovimentoMotorista);
}
