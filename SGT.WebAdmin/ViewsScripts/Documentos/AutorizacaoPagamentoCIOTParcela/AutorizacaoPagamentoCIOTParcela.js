/*
 * Declaração de Objetos Globais do Arquivo
 */

var _autorizacaoPagamentoCIOTParcela, _gridAutorizacaoPagamentoCIOTParcela;

/*
 * Declaração das Classes
 */

var AutorizacaoPagamentoCIOTParcela = function () {

    this.NumeroCarga = PropertyEntity({ text: "Nº da Carga:", visible: ko.observable(true) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº do Pedido no Embarcador:", visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(Global.DataAtual()), def: "", visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ text: "Situação:", options: EnumSituacaoCIOT.ObterOpcoesPesquisa(), val: ko.observable(EnumSituacaoCIOT.Encerrado), def: EnumSituacaoCIOT.Encerrado, issue: 0, visible: ko.observable(true) });
    this.TipoPagamento = PropertyEntity({ text: "Tipo de Parcela:", options: EnumTipoAutorizacaoPagamentoCIOTParcela.obterOpcoesPesquisa(), val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.TipoParcelaPagamento = PropertyEntity({ text: "Parcela:", val: ko.observable(""), def: "", issue: 0, visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaCodigosCIOT = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Pesquisar = PropertyEntity({ eventClick: function (e) { _gridAutorizacaoPagamentoCIOTParcela.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
    this.AutorizarAbastecimento = PropertyEntity({ eventClick: function (e, sender) { AutorizarPagamentoClick(e, sender, EnumTipoAutorizacaoPagamentoCIOTParcela.Abastecimento); }, type: types.event, text: "Autorizar o Abastecimento", visible: ko.observable(true) });
    this.AutorizarAdiantamento = PropertyEntity({ eventClick: function (e, sender) { AutorizarPagamentoClick(e, sender, EnumTipoAutorizacaoPagamentoCIOTParcela.Adiantamento); }, type: types.event, text: "Autorizar o Adiantamento", visible: ko.observable(true) });
    this.AutorizarSaldo = PropertyEntity({ eventClick: function (e, sender) { AutorizarPagamentoClick(e, sender, EnumTipoAutorizacaoPagamentoCIOTParcela.Saldo); }, type: types.event, text: "Autorizar o Saldo", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

/*
 * Declaração das Funções de Inicialização
 */

function LoadGridAutorizacaoPagamentoCIOTParcela() {
    var menuOpcoes = null;
    var configuracoesExportacao = { url: "AutorizacaoPagamentoCIOTParcela/ExportarPesquisa", titulo: "Autorização de Pagamento do CIOT" };

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () { },
        callbackNaoSelecionado: function () { },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _autorizacaoPagamentoCIOTParcela.SelecionarTodos,
        somenteLeitura: false
    };

    _gridAutorizacaoPagamentoCIOTParcela = new GridViewExportacao(_autorizacaoPagamentoCIOTParcela.ListaCodigosCIOT.idGrid, "AutorizacaoPagamentoCIOTParcela/Pesquisa", _autorizacaoPagamentoCIOTParcela, menuOpcoes, configuracoesExportacao, null, 25, multiplaescolha);
    _gridAutorizacaoPagamentoCIOTParcela.CarregarGrid();
}

function LoadAutorizacaoPagamentoCIOTParcela() {
    _autorizacaoPagamentoCIOTParcela = new AutorizacaoPagamentoCIOTParcela();
    KoBindings(_autorizacaoPagamentoCIOTParcela, "knockoutAutorizacaoPagamentoCIOTParcela");

    BuscarClientes(_autorizacaoPagamentoCIOTParcela.Transportador, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);

    LoadGridAutorizacaoPagamentoCIOTParcela();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AutorizarPagamentoClick(e, sender, tipoPagamento) {
    exibirConfirmacao("Atenção!", "Deseja realmente autorizar o pagamento dos CIOT's selecionados?", function () {
        if (!PreencherListasSelecao())
            return;

        _autorizacaoPagamentoCIOTParcela.TipoParcelaPagamento.val(tipoPagamento);

        Salvar(_autorizacaoPagamentoCIOTParcela, "AutorizacaoPagamentoCIOTParcela/AutorizarPagamento", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Pagamento autorizado com sucesso.");

                    LimparRegistrosSelecionados();
                }
                else
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
            }
            else
                exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
        }, sender);
    });
}

function CancelarClick() {
    LimparRegistrosSelecionados();
}

function LimparRegistrosSelecionados() {
    _autorizacaoPagamentoCIOTParcela.ListaCodigosCIOT.val("");
    _autorizacaoPagamentoCIOTParcela.SelecionarTodos.val(false);
    _gridAutorizacaoPagamentoCIOTParcela.AtualizarRegistrosSelecionados([]);
    _gridAutorizacaoPagamentoCIOTParcela.AtualizarRegistrosNaoSelecionados([]);
    _gridAutorizacaoPagamentoCIOTParcela.CarregarGrid();
}

/*
 * Declaração das Funções
 */

function PreencherListasSelecao() {
    var ciotsSelecionados = null;
    var codigosCIOTs = new Array();

    if (_autorizacaoPagamentoCIOTParcela.SelecionarTodos.val())
        ciotsSelecionados = _gridAutorizacaoPagamentoCIOTParcela.ObterMultiplosNaoSelecionados();
    else
        ciotsSelecionados = _gridAutorizacaoPagamentoCIOTParcela.ObterMultiplosSelecionados();

    for (var i = 0; i < ciotsSelecionados.length; i++)
        codigosCIOTs.push(ciotsSelecionados[i].DT_RowId);

    if (codigosCIOTs && (codigosCIOTs.length > 0 || _autorizacaoPagamentoCIOTParcela.SelecionarTodos.val())) {
        _autorizacaoPagamentoCIOTParcela.ListaCodigosCIOT.val(JSON.stringify(codigosCIOTs));
        return true;
    } else {
        _autorizacaoPagamentoCIOTParcela.ListaCodigosCIOT.val("");
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "É necessário selecionar ao menos um CIOT para autorizar o pagamento.");
        return false;
    }
}