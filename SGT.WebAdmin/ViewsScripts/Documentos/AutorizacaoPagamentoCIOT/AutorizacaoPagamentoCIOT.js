/*
 * Declaração de Objetos Globais do Arquivo
 */

var _autorizacaoPagamentoCIOT, _gridAutorizacaoPagamentoCIOT;

/*
 * Declaração das Classes
 */

var AutorizacaoPagamentoCIOT = function () {

    this.NumeroCarga = PropertyEntity({ text: "Nº da Carga:", visible: ko.observable(true) });
    this.NumeroPedidoEmbarcador = PropertyEntity({ text: "Nº do Pedido no Embarcador:", visible: ko.observable(true) });

    this.DataInicial = PropertyEntity({ text: "Data Inicial:", getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataFinal = PropertyEntity({ text: "Data Final:", getType: typesKnockout.date, val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.Transportador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 0, visible: ko.observable(true) });

    this.Situacao = PropertyEntity({ text: "Situação:", options: EnumSituacaoCIOT.ObterOpcoesPesquisa(), val: ko.observable(EnumSituacaoCIOT.Encerrado), def: EnumSituacaoCIOT.Encerrado, issue: 0, visible: ko.observable(true) });

    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: false, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar Todos", visible: ko.observable(false), enable: ko.observable(true) });
    this.ListaCodigosCIOT = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Pesquisar = PropertyEntity({ eventClick: function (e) { _gridAutorizacaoPagamentoCIOT.CarregarGrid(); }, type: types.event, text: "Pesquisar", visible: ko.observable(true) });
    this.AutorizarPagamentoCIOT = PropertyEntity({ eventClick: AutorizarPagamentoCIOTClick, type: types.event, text: "Autorizar o Pagamento", visible: ko.observable(true) });
    this.LiberarViagem = PropertyEntity({ eventClick: LiberarViagemClick, type: types.event, text: "Liberar a Viagem", visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: CancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};
var ModalPagamentoNaoAutorizado = function () {
    this.ListaMensagem = PropertyEntity({ val: ko.observableArray([]) });
};


/*
 * Declaração das Funções de Inicialização
 */

function LoadGridAutorizacaoPagamentoCIOT() {
    var menuOpcoes = null;
    var configuracoesExportacao = { url: "AutorizacaoPagamentoCIOT/ExportarPesquisa", titulo: "Autorização de Pagamento do CIOT" };

    var multiplaescolha = {
        basicGrid: null,
        callbackSelecionado: function () { },
        callbackNaoSelecionado: function () { },
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _autorizacaoPagamentoCIOT.SelecionarTodos,
        somenteLeitura: false
    };

    _gridAutorizacaoPagamentoCIOT = new GridViewExportacao(_autorizacaoPagamentoCIOT.ListaCodigosCIOT.idGrid, "AutorizacaoPagamentoCIOT/Pesquisa", _autorizacaoPagamentoCIOT, menuOpcoes, configuracoesExportacao, null, 25, multiplaescolha);
    _gridAutorizacaoPagamentoCIOT.CarregarGrid();
}

function LoadAutorizacaoPagamentoCIOT() {
    _autorizacaoPagamentoCIOT = new AutorizacaoPagamentoCIOT();
    KoBindings(_autorizacaoPagamentoCIOT, "knockoutAutorizacaoPagamentoCIOT");

    _modalPagamentoNaoAutorizado = new ModalPagamentoNaoAutorizado()
    KoBindings(_modalPagamentoNaoAutorizado, "knoutModalRetornoNaoAutorizado");

    BuscarClientes(_autorizacaoPagamentoCIOT.Transportador, null, false, [EnumModalidadePessoa.TransportadorTerceiro]);

    LoadGridAutorizacaoPagamentoCIOT();
}

/*
 * Declaração das Funções Associadas a Eventos
 */

function AutorizarPagamentoCIOTClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente autorizar o pagamento dos CIOT's selecionados?", function () {
        if (!PreencherListasSelecao())
            return;

        Salvar(_autorizacaoPagamentoCIOT, "AutorizacaoPagamentoCIOT/AutorizarPagamento", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data.ExibirModal) {
                    exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
                    PreencherModalRetornoNaoAutorizado(retorno.Data.Mensagem);
                    Global.abrirModal("divModalRetornoNaoAutorizado");
                }
                else if (retorno.Data) {
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

function LiberarViagemClick(e, sender) {
    exibirConfirmacao("Atenção!", "Deseja realmente liberar a viagem dos CIOT's selecionados?", function () {
        if (!PreencherListasSelecao())
            return;

        Salvar(_autorizacaoPagamentoCIOT, "AutorizacaoPagamentoCIOT/LiberarViagem", function (retorno) {
            if (retorno.Success) {
                if (retorno.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Liberação de viagem realizada com sucesso.");

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
    _autorizacaoPagamentoCIOT.ListaCodigosCIOT.val("");
    _autorizacaoPagamentoCIOT.SelecionarTodos.val(false);
    _gridAutorizacaoPagamentoCIOT.AtualizarRegistrosSelecionados([]);
    _gridAutorizacaoPagamentoCIOT.AtualizarRegistrosNaoSelecionados([]);
    _gridAutorizacaoPagamentoCIOT.CarregarGrid();
}

/*
 * Declaração das Funções
 */

function PreencherListasSelecao() {
    var ciotsSelecionados = null;
    var codigosCIOTs = new Array();

    if (_autorizacaoPagamentoCIOT.SelecionarTodos.val())
        ciotsSelecionados = _gridAutorizacaoPagamentoCIOT.ObterMultiplosNaoSelecionados();
    else
        ciotsSelecionados = _gridAutorizacaoPagamentoCIOT.ObterMultiplosSelecionados();

    for (var i = 0; i < ciotsSelecionados.length; i++)
        codigosCIOTs.push(ciotsSelecionados[i].DT_RowId);

    if (codigosCIOTs && (codigosCIOTs.length > 0 || _autorizacaoPagamentoCIOT.SelecionarTodos.val())) {
        _autorizacaoPagamentoCIOT.ListaCodigosCIOT.val(JSON.stringify(codigosCIOTs));
        return true;
    } else {
        _autorizacaoPagamentoCIOT.ListaCodigosCIOT.val("");
        exibirMensagem(tipoMensagem.atencao, "Atenção!", "É necessário selecionar ao menos um CIOT para autorizar o pagamento.");
        return false;
    }
}

function PreencherModalRetornoNaoAutorizado(listaMensagem) {
    _modalPagamentoNaoAutorizado.ListaMensagem.val(ko.observableArray([]))
    for (var i in listaMensagem) {
        console.log(listaMensagem[i])
        _modalPagamentoNaoAutorizado.ListaMensagem.val.push(listaMensagem[i])
    }
}