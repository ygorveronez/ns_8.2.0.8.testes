/// <reference path="../../Enumeradores/EnumTipoPagamentoAutorizacaoPagamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _gridSelecaoContratoFrete;
var _etapaSelecaoContratoFrete;

var EtapaSelecaoContratoFrete = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoPagamento = PropertyEntity({ val: ko.observable(EnumTipoPagamentoAutorizacaoPagamento.PagamentoSaldo), options: EnumTipoPagamentoAutorizacaoPagamento.obterOpcoes(), def: EnumTipoPagamentoAutorizacaoPagamento.PagamentoSaldo, text: "Tipo Pagamento", enable: true });
    this.DataInicial = PropertyEntity({ text: "Data Inicial: ", getType: typesKnockout.date });
    this.DataFinal = PropertyEntity({ text: "Data Final: ", getType: typesKnockout.date });
    this.DataInicial.dateRangeLimit = this.DataFinal;
    this.DataFinal.dateRangeInit = this.DataInicial;

    this.Numero = PropertyEntity({ text: "Número:", getType: typesKnockout.int, val: ko.observable(""), def: "" });
    this.Carga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Carga:", idBtnSearch: guid() });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa/Filial", idBtnSearch: guid() });
    this.Usuario = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Usuário:", idBtnSearch: guid() });
    this.Operadora = PropertyEntity({ val: ko.observable(""), options: EnumOperadoraCIOT.ObterOpcoesPesquisa(), def: "", text: "Operadora:" });
    this.TransportadorTerceiro = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Terceiro:", idBtnSearch: guid(), issue: 56 });


    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), visible: ko.observable(true), text: "Selecionar Todos" });

    this.Filtro = PropertyEntity({ visible: ko.observable(true) });

    this.PesquisarContratoFrete = PropertyEntity({
        eventClick: function (e) {
            GridSelecaoContratoFrete();
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

    this.Adicionar = PropertyEntity({ eventClick: AdicionarClick, type: types.event, text: ko.observable("Gerar Autorização Pagamento Saldo"), visible: ko.observable(true) });

    this.TipoPagamento.val.subscribe(function (novoValor) {
        if (novoValor == EnumTipoPagamentoAutorizacaoPagamento.PagamentoAdiantamento) {
            _etapaSelecaoContratoFrete.Adicionar.text("Gerar Autorização Pagamento Adiantamento");
        }
        else {
            _etapaSelecaoContratoFrete.Adicionar.text("Gerar Autorização Pagamento Saldo");
        }

        GridSelecaoContratoFrete();
    });
}

//*******EVENTOS*******

function LoadEtapaSelecaoContratoFrete() {
    _etapaSelecaoContratoFrete = new EtapaSelecaoContratoFrete();
    KoBindings(_etapaSelecaoContratoFrete, "knockoutEtapaSelecaoContratoFrete");

    // Busca componentes pesquisa
    BuscarFuncionario(_etapaSelecaoContratoFrete.Usuario);
    BuscarTransportadores(_etapaSelecaoContratoFrete.Empresa);
    BuscarCargas(_etapaSelecaoContratoFrete.Carga);
    BuscarClientes(_etapaSelecaoContratoFrete.TransportadorTerceiro);

    // Inicia grid de dados
    GridSelecaoContratoFrete();
}

function AdicionarClick(e, sender) {

    if (ValidaDocumentosSelecionados()) {
        exibirConfirmacao("Gerar Autorização de Pagamento", "Deseja realmente gerar a autorização de pagamento para os contratos de fretes selecionados?", function () {
            let dados = RetornarObjetoPesquisa(_etapaSelecaoContratoFrete);

            dados.SelecionarTodos = _etapaSelecaoContratoFrete.SelecionarTodos.val();

            if (dados.SelecionarTodos === false)
                dados.ListaContratosFrete = JSON.stringify(_gridSelecaoContratoFrete.ObterMultiplosSelecionados().map(o => o.Codigo));
            else
                dados.ListaContratosFrete = JSON.stringify(_gridSelecaoContratoFrete.ObterMultiplosNaoSelecionados().map(o => o.Codigo));

            executarReST("AutorizacaoPagamentoContratoFrete/Adicionar", dados, function (arg) {
                if (arg.Success) {
                    if (arg.Data) {
                        exibirMensagem(tipoMensagem.ok, "Sucesso", "Autorização de pagamento gerada com sucesso!");
                        _gridSelecaoContratoFrete.CarregarGrid();
                        BuscarAutorizacaoPagamentoContratoFrete(arg.Data.Codigo);
                    } else {
                        exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
                }
            });
        });
    }
}

//*******MÉTODOS*******

function GridSelecaoContratoFrete() {
    let menuOpcoes = null;

    let multiplaescolha = {
        basicGrid: null,
        eventos: {},
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _etapaSelecaoContratoFrete.SelecionarTodos,
        callbackNaoSelecionado: function () {
        },
        callbackSelecionado: function () {
        },
        callbackSelecionarTodos: null,
        somenteLeitura: false
    };

    if (_autorizacaoPagamentoContratoFrete.Codigo.val() > 0)
        multiplaescolha = null;

    let configExportacao = {
        url: "AutorizacaoPagamentoContratoFrete/ExportarPesquisaContratoFrete",
        titulo: "Contrato de Frete para Autorização de Pagamento",
        id: "btnExportarContratoFrete"
    };

    _etapaSelecaoContratoFrete.Codigo.val(_autorizacaoPagamentoContratoFrete.Codigo.val());
    _gridSelecaoContratoFrete = new GridView(_etapaSelecaoContratoFrete.PesquisarContratoFrete.idGrid, "AutorizacaoPagamentoContratoFrete/PesquisaContratoFrete", _etapaSelecaoContratoFrete, menuOpcoes, null, 10, null, null, null, multiplaescolha, null, null, configExportacao);
    _gridSelecaoContratoFrete.SetPermitirRedimencionarColunas(true);
    _gridSelecaoContratoFrete.CarregarGrid(function () {
        setTimeout(function () {
            if (_autorizacaoPagamentoContratoFrete.Codigo.val() > 0)
                $("#btnExportarContratoFrete").show();
            else
                $("#btnExportarContratoFrete").hide();
        }, 200);
    });
}

function ValidaDocumentosSelecionados() {
    let valido = true;

    let itens = _gridSelecaoContratoFrete.ObterMultiplosSelecionados();

    if (itens.length == 0 && !_etapaSelecaoContratoFrete.SelecionarTodos.val()) {
        valido = false;
        exibirMensagem(tipoMensagem.atencao, "Contratos de frete Selecionados", "Nenhum contrato de frete selecionado.");
    }

    return valido;
}

function EditarSelecaoContratosFrete(data) {
    _etapaSelecaoContratoFrete.Filtro.visible(false);
    _etapaSelecaoContratoFrete.Adicionar.visible(false);
    _autorizacaoPagamentoContratoFrete.Codigo.val(data.Codigo);

    GridSelecaoContratoFrete();
}