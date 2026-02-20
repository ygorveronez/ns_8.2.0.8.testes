/// <reference path="../../Enumeradores/EnumFormaPagamento.js" />
/*
 * Declaração de Objetos Globais do Arquivo
 */

var _CRUDPedidoTipoPagamento;
var _pedidoTipoPagamento;
var _pesquisaPedidoTipoPagamento;
var _gridPedidoTipoPagamento;

/*
 * Declaração das Classes
 */

var CRUDPedidoTipoPagamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: AtualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: ExcluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
};

var PedidoTipoPagamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.ObservacaoPedido = PropertyEntity({ text: "Observação automática para o Pedido:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 400 });
    this.FormaPagamento = PropertyEntity({ text: "Forma:", val: ko.observable(EnumFormaPagamento.Nenhum), options: EnumFormaPagamento.obterOpcoes(true), def: EnumTipoComercial.Nenhum, visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ text: "*Situação: ", issue: 557, val: ko.observable(true), options: _status, def: true });
};

var PesquisaPedidoTipoPagamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({ eventClick: ExibirFiltrosClick, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true) });
    this.Pesquisar = PropertyEntity({ eventClick: RecarregarGridPedidoTipoPagamento, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridPedidoTipoPagamento() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: EditarClick, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "PedidoTipoPagamento/ExportarPesquisa", titulo: "Grupos de Despesas" };

    _gridPedidoTipoPagamento = new GridViewExportacao(_pesquisaPedidoTipoPagamento.Pesquisar.idGrid, "PedidoTipoPagamento/Pesquisa", _pesquisaPedidoTipoPagamento, menuOpcoes, configuracoesExportacao);
    _gridPedidoTipoPagamento.CarregarGrid();
}

function LoadPedidoTipoPagamento() {
    _pedidoTipoPagamento = new PedidoTipoPagamento();
    KoBindings(_pedidoTipoPagamento, "knockoutPedidoTipoPagamento");

    HeaderAuditoria("PedidoTipoPagamento", _pedidoTipoPagamento);

    _CRUDPedidoTipoPagamento = new CRUDPedidoTipoPagamento();
    KoBindings(_CRUDPedidoTipoPagamento, "knockoutCRUDPedidoTipoPagamento");

    _pesquisaPedidoTipoPagamento = new PesquisaPedidoTipoPagamento();
    KoBindings(_pesquisaPedidoTipoPagamento, "knockoutPesquisaPedidoTipoPagamento", false, _pesquisaPedidoTipoPagamento.Pesquisar.id);

    LoadGridPedidoTipoPagamento();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AdicionarClick(e, sender) {
    Salvar(_pedidoTipoPagamento, "PedidoTipoPagamento/Adicionar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");

                RecarregarGridPedidoTipoPagamento();
                LimparCamposPedidoTipoPagamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function AtualizarClick(e, sender) {
    Salvar(_pedidoTipoPagamento, "PedidoTipoPagamento/Atualizar", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");

                RecarregarGridPedidoTipoPagamento();
                LimparCamposPedidoTipoPagamento();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, sender);
}

function CancelarClick() {
    LimparCamposPedidoTipoPagamento();
}

function EditarClick(registroSelecionado) {
    LimparCamposPedidoTipoPagamento();

    _pedidoTipoPagamento.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_pedidoTipoPagamento, "PedidoTipoPagamento/BuscarPorCodigo", function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _pesquisaPedidoTipoPagamento.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                ControlarBotoesHabilitados(isEdicao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    }, null);
}

function ExcluirClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_pedidoTipoPagamento, "PedidoTipoPagamento/ExcluirPorCodigo", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");

                    RecarregarGridPedidoTipoPagamento();
                    LimparCamposPedidoTipoPagamento();
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
    _CRUDPedidoTipoPagamento.Atualizar.visible(isEdicao);
    _CRUDPedidoTipoPagamento.Excluir.visible(isEdicao);
    _CRUDPedidoTipoPagamento.Cancelar.visible(isEdicao);
    _CRUDPedidoTipoPagamento.Adicionar.visible(!isEdicao);
}

function LimparCamposPedidoTipoPagamento() {
    var isEdicao = false;
    _pedidoTipoPagamento.FormaPagamento.val(EnumFormaPagamento.Nenhum);
    ControlarBotoesHabilitados(isEdicao);
    LimparCampos(_pedidoTipoPagamento);
}

function RecarregarGridPedidoTipoPagamento() {
    _gridPedidoTipoPagamento.CarregarGrid();
}