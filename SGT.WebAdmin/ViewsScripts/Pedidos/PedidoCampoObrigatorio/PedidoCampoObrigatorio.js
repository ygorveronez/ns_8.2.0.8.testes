/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPedidoCampoObrigatorio;
var _pedidoCampoObrigatorio;
var _pesquisaPedidoCampoObrigatorio;
var _gridPedidoCampoObrigatorio;

/*
 * Declaração das Classes
 */

var CRUDPedidoCampoObrigatorio = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var PedidoCampoObrigatorio = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });

    this.Campo = PropertyEntity({ type: types.event, text: "Adicionar Campo", idBtnSearch: guid(), enable: ko.observable(true) });
    this.GridCampo = PropertyEntity({ type: types.local });
    this.Campos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.ObrigatorioInformarProdutoPedido = PropertyEntity({ text: "Obrigatório informar produto no pedido", getType: typesKnockout.bool, val: ko.observable(false) });
};

var PesquisaPedidoCampoObrigatorio = function () {
    this.TipoOperacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Operação:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: true });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridPedidoCampoObrigatorio, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridPedidoCampoObrigatorio() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, false); }, tamanho: "10", icone: "" };
    var opcaoDuplicar = { descricao: "Duplicar", id: guid(), evento: "onclick", metodo: function (registroSelecionado) { EditarClick(registroSelecionado, true); }, tamanho: "10", icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [opcaoEditar, opcaoDuplicar] };
    var configuracoesExportacao = { url: "PedidoCampoObrigatorio/ExportarPesquisa", titulo: "Campos Obrigatórios para o Pedido" };

    _gridPedidoCampoObrigatorio = new GridViewExportacao(_pesquisaPedidoCampoObrigatorio.Pesquisar.idGrid, "PedidoCampoObrigatorio/Pesquisa", _pesquisaPedidoCampoObrigatorio, menuOpcoes, configuracoesExportacao);
    _gridPedidoCampoObrigatorio.CarregarGrid();
}

function LoadPedidoCampoObrigatorio() {
    _pedidoCampoObrigatorio = new PedidoCampoObrigatorio();
    KoBindings(_pedidoCampoObrigatorio, "knockoutPedidoCampoObrigatorio");

    HeaderAuditoria("PedidoCampoObrigatorio", _pedidoCampoObrigatorio);

    _CRUDPedidoCampoObrigatorio = new CRUDPedidoCampoObrigatorio();
    KoBindings(_CRUDPedidoCampoObrigatorio, "knockoutCRUDPedidoCampoObrigatorio");

    _pesquisaPedidoCampoObrigatorio = new PesquisaPedidoCampoObrigatorio();
    KoBindings(_pesquisaPedidoCampoObrigatorio, "knockoutPesquisaPedidoCampoObrigatorio", false, _pesquisaPedidoCampoObrigatorio.Pesquisar.id);

    new BuscarTiposOperacao(_pedidoCampoObrigatorio.TipoOperacao);
    new BuscarTiposOperacao(_pesquisaPedidoCampoObrigatorio.TipoOperacao);

    LoadCampoPedido();
    LoadGridPedidoCampoObrigatorio();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    _pedidoCampoObrigatorio.Campos.val(JSON.stringify(_pedidoCampoObrigatorio.Campo.basicTable.BuscarRegistros()));

    Salvar(_pedidoCampoObrigatorio, "PedidoCampoObrigatorio/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridPedidoCampoObrigatorio();
                LimparCamposPedidoCampoObrigatorio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    _pedidoCampoObrigatorio.Campos.val(JSON.stringify(_pedidoCampoObrigatorio.Campo.basicTable.BuscarRegistros()));

    Salvar(_pedidoCampoObrigatorio, "PedidoCampoObrigatorio/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso.");

                RecarregarGridPedidoCampoObrigatorio();
                LimparCamposPedidoCampoObrigatorio();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposPedidoCampoObrigatorio();
}

function EditarClick(registroSelecionado, duplicar) {
    LimparCamposPedidoCampoObrigatorio();

    executarReST("PedidoCampoObrigatorio/BuscarPorCodigo", { Codigo: registroSelecionado.Codigo, Duplicar: duplicar }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                PreencherObjetoKnout(_pedidoCampoObrigatorio, retorno);
                _pesquisaPedidoCampoObrigatorio.ExibirFiltros.visibleFade(false);

                ControlarBotoesHabilitados(!duplicar);

                _pedidoCampoObrigatorio.Campo.basicTable.CarregarGrid(_pedidoCampoObrigatorio.Campos.val());
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Deseja realmente excluir este registro?", function () {
        ExcluirPorCodigo(_pedidoCampoObrigatorio, "PedidoCampoObrigatorio/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");

                    RecarregarGridPedidoCampoObrigatorio();
                    LimparCamposPedidoCampoObrigatorio();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", retorno.Msg, 16000);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, null);
    });
}

function ExibirFiltrosClick(e) {
    e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
}

/*
 * Declaração das Funções
 */

function ControlarBotoesHabilitados(isEdicao) {
    _CRUDPedidoCampoObrigatorio.Atualizar.visible(isEdicao);
    _CRUDPedidoCampoObrigatorio.Excluir.visible(isEdicao);
    _CRUDPedidoCampoObrigatorio.Cancelar.visible(isEdicao);
    _CRUDPedidoCampoObrigatorio.Adicionar.visible(!isEdicao);
}

function LimparCamposPedidoCampoObrigatorio() {
    var isEdicao = false;

    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_pedidoCampoObrigatorio);

    _pedidoCampoObrigatorio.Campo.basicTable.CarregarGrid([]);
}

function RecarregarGridPedidoCampoObrigatorio() {
    _gridPedidoCampoObrigatorio.CarregarGrid();
}