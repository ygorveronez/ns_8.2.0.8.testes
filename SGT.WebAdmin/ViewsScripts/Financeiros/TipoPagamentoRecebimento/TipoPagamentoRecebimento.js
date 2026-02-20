/// <reference path="../../Consultas/PlanoConta.js" />
/// <reference path="../../Enumeradores/EnumAnaliticoSintetico.js" />
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

var _gridTipoPagamentoRecebimento;
var _tipoPagamentoRecebimento;
var _pesquisaTipoPagamentoRecebimento;
var _CRUDTipoPagamentoRecebimento;

var PesquisaTipoPagamentoRecebimento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Conta Gerencial:", idBtnSearch: guid() });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Status: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTipoPagamentoRecebimento.CarregarGrid();
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

var TipoPagamentoRecebimento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, issue: 150 });
    this.PlanoConta = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Conta Gerencial:", idBtnSearch: guid(), required: true, visible: ko.observable(true), issue: 359 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: " });
    this.Observacao = PropertyEntity({ text: "Observação: ", required: false, maxlength: 300 });
    this.LimiteConta = PropertyEntity({ text: "Limite da Conta: ", required: false, getType: typesKnockout.decimal, enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: false, issue: 150 });

    this.Exportar = PropertyEntity({ text: "Exportar?", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });
    this.ObrigaChequeBaixaTitulo = PropertyEntity({ text: "Obriga lançar cheque na Baixa de Títulos?", val: ko.observable(false), getType: typesKnockout.bool, def: false, enable: ko.observable(true), visible: ko.observable(true) });

    this.ListaConfiguracoesExportacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });
    this.ConfiguracoesExportacao = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable([]), def: [], idGrid: guid() });

    this.Exportar.val.subscribe(function (novoValor) {
        if (novoValor)
            $("#liTabExportacao").removeClass("d-none");
        else
            $("#liTabExportacao").addClass("d-none");
    });
};

var CRUDTipoPagamentoRecebimento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******


function loadTipoPagamentoRecebimento() {
    _tipoPagamentoRecebimento = new TipoPagamentoRecebimento();
    KoBindings(_tipoPagamentoRecebimento, "knockoutCadastroTipoPagamentoRecebimento");

    _CRUDTipoPagamentoRecebimento = new CRUDTipoPagamentoRecebimento();
    KoBindings(_CRUDTipoPagamentoRecebimento, "knockoutCRUDTipoPagamentoRecebimento");

    HeaderAuditoria("TipoPagamentoRecebimento", _tipoPagamentoRecebimento);

    _pesquisaTipoPagamentoRecebimento = new PesquisaTipoPagamentoRecebimento();
    KoBindings(_pesquisaTipoPagamentoRecebimento, "knockoutPesquisaTipoPagamentoRecebimento", false, _pesquisaTipoPagamentoRecebimento.Pesquisar.id);

    new BuscarPlanoConta(_tipoPagamentoRecebimento.PlanoConta, "Selecione as Contas Analiticas", "Contas Analiticas", null, EnumAnaliticoSintetico.Analitico);
    new BuscarPlanoConta(_pesquisaTipoPagamentoRecebimento.PlanoConta, "Selecione as Contas Analiticas", "Contas Analiticas", null, EnumAnaliticoSintetico.Analitico);

    buscarTipoPagamentoRecebimentos();

    LoadConfiguracaoExportacao();
}

function adicionarClick(e, sender) {
    _tipoPagamentoRecebimento.ListaConfiguracoesExportacao.val(JSON.stringify(_tipoPagamentoRecebimento.ConfiguracoesExportacao.val()));

    Salvar(_tipoPagamentoRecebimento, "TipoPagamentoRecebimento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTipoPagamentoRecebimento.CarregarGrid();
                limparCamposTipoPagamentoRecebimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    _tipoPagamentoRecebimento.ListaConfiguracoesExportacao.val(JSON.stringify(_tipoPagamentoRecebimento.ConfiguracoesExportacao.val()));

    Salvar(_tipoPagamentoRecebimento, "TipoPagamentoRecebimento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTipoPagamentoRecebimento.CarregarGrid();
                limparCamposTipoPagamentoRecebimento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}


function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de pagamento e recebimento " + _tipoPagamentoRecebimento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tipoPagamentoRecebimento, "TipoPagamentoRecebimento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridTipoPagamentoRecebimento.CarregarGrid();
                limparCamposTipoPagamentoRecebimento();
            } else {
                exibirMensagem(tipoMensagem.falha, "falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposTipoPagamentoRecebimento();
}

//*******MÉTODOS*******

function buscarTipoPagamentoRecebimentos() {
    var editar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarTipoPagamentoRecebimento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTipoPagamentoRecebimento = new GridView(_pesquisaTipoPagamentoRecebimento.Pesquisar.idGrid, "TipoPagamentoRecebimento/Pesquisa", _pesquisaTipoPagamentoRecebimento, menuOpcoes, { column: 1, dir: orderDir.asc });
    _gridTipoPagamentoRecebimento.CarregarGrid();
}

function editarTipoPagamentoRecebimento(tipoPagamentoRecebimentoGrid) {
    limparCamposTipoPagamentoRecebimento();
    _tipoPagamentoRecebimento.Codigo.val(tipoPagamentoRecebimentoGrid.Codigo);
    BuscarPorCodigo(_tipoPagamentoRecebimento, "TipoPagamentoRecebimento/BuscarPorCodigo", function (arg) {
        _pesquisaTipoPagamentoRecebimento.ExibirFiltros.visibleFade(false);
        _CRUDTipoPagamentoRecebimento.Atualizar.visible(true);
        _CRUDTipoPagamentoRecebimento.Cancelar.visible(true);
        _CRUDTipoPagamentoRecebimento.Excluir.visible(true);
        _CRUDTipoPagamentoRecebimento.Adicionar.visible(false);

        RecarregarGridConfiguracaoExportacao();
    }, null);
}

function limparCamposTipoPagamentoRecebimento() {
    _CRUDTipoPagamentoRecebimento.Atualizar.visible(false);
    _CRUDTipoPagamentoRecebimento.Cancelar.visible(false);
    _CRUDTipoPagamentoRecebimento.Excluir.visible(false);
    _CRUDTipoPagamentoRecebimento.Adicionar.visible(true);

    LimparCampos(_tipoPagamentoRecebimento);

    RecarregarGridConfiguracaoExportacao();
    LimparCamposConfiguracaoExportacao();

    Global.ResetarMultiplasAbas();
}
