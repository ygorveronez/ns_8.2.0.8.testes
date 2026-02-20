/// <reference path="../../Consultas/MovimentacaoContaPagar.js" />
//#region Variaveis Globais
var _modalMovimentacoesContaPagar;
var _modalAdicionarMovimentacoes;
var _gridMovimentacoes;
var _gridAdicionarMovimentacoes;
//#endregion

//#region Construtores
function ModalMovimentacoesContaPagar() {
    this.Movimentacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.AdicionarMovimentacoes = PropertyEntity({ text: 'Adicionar Movimentações', eventClick: adicionarMovimentacoes, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: 'Cancelar', eventClick: cancelarMovimentacao, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ text: 'Salvar', eventClick: atualizarTermo, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.AtualizarTransportador = PropertyEntity({ text: 'Salvar e enviar para Transportador', eventClick: atualizarTermoTransportador, type: types.event, idGrid: guid(), visible: ko.observable(true) });
}
function ModalAdicionarMovimentacoes() {
    this.Movimentacoes = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });
    this.Transportador = PropertyEntity({ type: types.int, val: ko.observable(0) });
    this.DataInicial = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.DataFinal = PropertyEntity({ getType: typesKnockout.date, val: ko.observable(""), def: "" });
    this.CodigosMovimentacoesSelecionados = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()) });

    this.AdicionarMovimentacoes = PropertyEntity({ text: 'Adicionar Movimentações', eventClick: adicionarMovimentacoes, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Cancelar = PropertyEntity({ text: 'Cancelar', eventClick: cancelarMovimentacaoesClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.Adicionar = PropertyEntity({ text: 'Adicionar', eventClick: adicionarMovimentacaoSelecionadasClick, type: types.event, idGrid: guid(), visible: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ type: types.event, val: ko.observable(false), text: "Marcar Todas", visible: ko.observable(true) });

}
//#endregion

//#region Funções Carregadoras
function loadModalMovimentacoes() {
    _modalMovimentacoesContaPagar = new ModalMovimentacoesContaPagar();
    KoBindings(_modalMovimentacoesContaPagar, "knockoutMovimentacoesContaPagar");

    _modalAdicionarMovimentacoes = new ModalAdicionarMovimentacoes();
    KoBindings(_modalAdicionarMovimentacoes, "knockoutAdicionarMovimentacao");

    loadGridMovimentacoes();
    loadGridNovasMovimentacaoes();

    if (_FormularioSomenteLeitura) {
        _modalMovimentacoesContaPagar.AdicionarMovimentacoes.enable(false);
        _modalMovimentacoesContaPagar.Atualizar.enable(false);
        _modalMovimentacoesContaPagar.AtualizarTransportador.enable(false);
    }
}
//#endregion

//#region Funções Auxiliares
function adicionarMovimentacoes() {
    Global.abrirModal("divModalNovasMovimentacaoes");
    _modalAdicionarMovimentacoes.Transportador.val(_termoQuitacao.Transportador.codEntity());
    _modalAdicionarMovimentacoes.DataInicial.val(_termoQuitacao.DataInicial.val());
    _modalAdicionarMovimentacoes.DataFinal.val(_termoQuitacao.DataFinal.val());
    _gridAdicionarMovimentacoes.CarregarGrid();
}

function loadGridMovimentacoes() {
    var header = [
        { data: "Codigo", visible: false },
        { data: "DataDocumento", title: "Data Documento", width: "15%", className: "text-align-center" },
        { data: "TipoDocumento", title: "Tipo Documento", width: "15%", className: "text-align-center" },
        { data: "ValorTotal", title: "Valor Total", width: "15%", className: "text-align-center" },
        { data: "TaxCode", title: "Tax Code", width: "15%", className: "text-align-center" },
        { data: "NumeroDocumento", title: "Numero Documento", width: "15%", className: "text-align-center" },
        { data: "TermoPagamento", title: "Termo Pagamento", width: "15%", className: "text-align-center" },
        { data: "NumeroCte", title: "Nº Cte", width: "15%", className: "text-align-center" }
    ];

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [
            { descricao: "Excluir", id: "clasEditar", evento: "onclick", metodo: _FormularioSomenteLeitura ? null : removerMovimentacaoGrid, tamanho: "15", icone: "" }]
    }
    _gridMovimentacoes = new BasicDataTable(_modalMovimentacoesContaPagar.Movimentacoes.idGrid, header, menuOpcoes);
    _gridMovimentacoes.CarregarGrid([]);
}

function loadGridNovasMovimentacaoes() {
    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _modalAdicionarMovimentacoes.SelecionarTodos,
        callbackNaoSelecionado: function () { },
        callbackSelecionado: callbackretornoMovimentacao,
        callbackSelecionarTodos: function () { },
        somenteLeitura: false
    };

    _gridAdicionarMovimentacoes = new GridView(_modalAdicionarMovimentacoes.Movimentacoes.idGrid, "MovimentacaoContaPagar/PesquisarMovimentacao", _modalAdicionarMovimentacoes, null, null, 15, null, null, null, multiplaescolha);
}

function CarregarDadosGridMovimentacoes(dados = []) {
    let dadosCarregar = [...dados, ..._modalMovimentacoesContaPagar.Movimentacoes.val()]
    _gridMovimentacoes.CarregarGrid(dadosCarregar);
}

function removerMovimentacaoGrid(e) {
    let movimentacoes = _gridMovimentacoes.BuscarRegistros();
    let novasMovimentacoes = movimentacoes.filter(x => x.Codigo != e.Codigo);
    _gridMovimentacoes.CarregarGrid(novasMovimentacoes);
}

function AdicionarMovimentacao(movimentacao) {
    let movimentacoes = _gridMovimentacoes.BuscarRegistros();
    movimentacoes.push(movimentacao);
    _gridMovimentacoes.CarregarGrid(movimentacoes);
}

function atualizarTermo() {
    let movimentacoes = _gridMovimentacoes.BuscarRegistros();
    let codigoMovimentacoes = movimentacoes.map(x => x.Codigo);

    executarReST("TermoQuitacaoFinanceiro/AtualizarMovimentacoes",
        { Movimentacoes: JSON.stringify(codigoMovimentacoes), Codigo: _termoQuitacao.Codigo.val() },
        (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

            cancelarMovimentacao();
            cancelarTermoQuitacao();
            FadeTermoQuitacaoDetalhes();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo atualizado com sucesso");
        });
}

function atualizarTermoTransportador() {
    let movimentacoes = _gridMovimentacoes.BuscarRegistros();
    let codigoMovimentacoes = movimentacoes.map(x => x.Codigo);

    executarReST("TermoQuitacaoFinanceiro/AtualizarMovimentacoes",
        { Movimentacoes: JSON.stringify(codigoMovimentacoes), Codigo: _termoQuitacao.Codigo.val(), AvisarTransportador: true },
        (arg) => {
            if (!arg.Success)
                return exibirMensagem(tipoMensagem.falha, "Erro", arg.Msg);

            cancelarMovimentacao();
            cancelarTermoQuitacao();
            FadeTermoQuitacaoDetalhes();
            exibirMensagem(tipoMensagem.ok, "Sucesso", "Termo atualizado com sucesso");
        });
}
function cancelarMovimentacao() {
    Global.fecharModal("divModalAdicionarMovimentacoes");
}
function callbackretornoMovimentacao(dados) {

}

function adicionarMovimentacaoSelecionadasClick() {
    let novasMovimentacoes = _gridAdicionarMovimentacoes.ObterMultiplosSelecionados();
    let movimentacaoesAtuais = _gridMovimentacoes.BuscarRegistros();
    let movimentacoesAdicionar = [...movimentacaoesAtuais];

    for (var i = 0; i < novasMovimentacoes.length; i++) {
        let movimentacao = novasMovimentacoes[i];

        let [existe] = movimentacaoesAtuais.filter(x => x.Codigo == movimentacao.Codigo);

        if (existe != null)
            continue;
        movimentacoesAdicionar.push(movimentacao);
    }

    let codigosAnteriorSelecioandos = _modalAdicionarMovimentacoes.CodigosMovimentacoesSelecionados.val();
    let codigosSelecioandos = novasMovimentacoes.map(x => x.Codigo);
    let codigosRemover = codigosAnteriorSelecioandos.filter(x => !codigosSelecioandos.includes(x.Codigo));
    movimentacoesAdicionar = movimentacoesAdicionar.filter(x => !codigosRemover.includes(x.Codigo));
    _modalAdicionarMovimentacoes.CodigosMovimentacoesSelecionados.val(codigosSelecioandos);

    _gridMovimentacoes.CarregarGrid(movimentacoesAdicionar);
    cancelarMovimentacaoesClick();
}

function cancelarMovimentacaoesClick() {
    Global.fecharModal("divModalNovasMovimentacaoes");
}
//#endregion