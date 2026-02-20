var _separacaoInformacoes;
var _pesquisaSelecaoNotas;
var _gridSelecaoNotas;
var SeparacaoInformacoes = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.EntregueNaoEntregaClienteFinal = PropertyEntity({ getType: typesKnockout.bool, text: "Entregas pedidos em outro local (ainda não vão para o destino final)?", val: ko.observable(false), def: false, visible: ko.observable(true) });
    this.LocalEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: false, fadeVisible: ko.observable(false), text: "Local da Entrega", idBtnSearch: guid(), issue: 52 });
    this.EnviarSeparacao = PropertyEntity({ eventClick: EnviarSeparacaoClick, type: types.event, text: "Confirmar Informações", visible: ko.observable(false), enable: ko.observable(true) });
    this.DataExpedicao = PropertyEntity({ getType: typesKnockout.dateTime, val: ko.observable(""), visible: ko.observable(false), text: "Data expedição:" });
    this.SelecionarNotas = PropertyEntity({ getType: typesKnockout.bool, text: "Selecionar notas", val: ko.observable(false), def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.NotasIntegradas = PropertyEntity({ getType: typesKnockout.string, text: "Notas integradas: ", val: ko.observable(""), visible: ko.observable(true) });

    this.SelecionarNotas.val.subscribe(function (novoValor) {
        if (_separacaoInformacoes.SelecionarNotas.val() && _separacaoInformacoes.NotasIntegradas.val() == "") {
            loadGridNotas();
            Global.abrirModal('divModalSelecionarNotas');
        }
    });
};

var SelecaoNotas = function () {
    this.Grid = PropertyEntity({ idGrid: guid(), enable: ko.observable(true) });
    this.SelecionarTodos = PropertyEntity({ val: ko.observable(false), def: true, type: types.map, getType: typesKnockout.bool, text: "Marcar/Desmarcar todos", visible: ko.observable(true) });
    this.ListaCodigosPedidosSelecionados = PropertyEntity({ val: ko.observable(""), getType: typesKnockout.string });
    this.Salvar = PropertyEntity({ eventClick: ConfirmarNotasClick, type: types.event, text: "Selecionar Notas", visible: ko.observable(true), enable: ko.observable(true) });
};
function LoadSeparacaoInformacao() {
    _separacaoInformacoes = new SeparacaoInformacoes();
    KoBindings(_separacaoInformacoes, "knoutInformacoesSeparacao");

    _pesquisaSelecaoNotas = new SelecaoNotas();
    KoBindings(_pesquisaSelecaoNotas, "knoutSelecaoNotas");

    new BuscarClientes(_separacaoInformacoes.LocalEntrega);
}

function preencherInformacoesSeparacao(dados) {
    _separacaoInformacoes.Codigo.val(dados.Codigo);
    _separacaoInformacoes.EntregueNaoEntregaClienteFinal.val(dados.EntregueNaoEntregaClienteFinal);
    _separacaoInformacoes.LocalEntrega.val(dados.LocalEntrega.Descricao);
    _separacaoInformacoes.LocalEntrega.codEntity(dados.LocalEntrega.Codigo);
    _separacaoInformacoes.DataExpedicao.val(dados.DataExpedicao);
    _separacaoInformacoes.DataExpedicao.visible(visibilidadeDataExpedicao());
}


function EnviarSeparacaoClick() {
    var itensSelecionados = JSON.stringify(_gridSelecaoNotas.ObterMultiplosSelecionados());

    if (_separacaoInformacoes.SelecionarNotas.val() == true && _gridSelecaoNotas.ObterMultiplosSelecionados().length == 0) {
        exibirMensagem(tipoMensagem.aviso, "Aviso", "Você precisa selecionar ao menos uma nota ao clicar na opção 'Selecionar notas'");
        _separacaoInformacoes.SelecionarNotas.val(false);
        return;
    }
    exibirConfirmacao("Confirmação", "Realmente deseja enviar os pedidos separados?", function () {

        var data = { Codigo: _separacaoInformacoes.Codigo.val(), LocalEntrega: _separacaoInformacoes.LocalEntrega.codEntity(), DataExpedicao: _separacaoInformacoes.DataExpedicao.val(), SelecionarNotas: _separacaoInformacoes.SelecionarNotas.val(), NotasSelecionadas: itensSelecionados };

        executarReST("SeparacaoPedido/SalvarSeparacao", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Salvo e enviado para Integração com sucesso");
                    limparDadosSelecao();
                    buscarSelecoes();
                } else {
                    exibirMensagem(tipoMensagem.atencao, "Atenção", arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        });
    });
}

function limparInformacoesSeparacao() {
    LimparCampos(_separacaoInformacoes);
}

function visibilidadeDataExpedicao() {
    for (var i = 0; i < _separacaoPedido.Pedidos.val().length; i++) {
        var pedidoAtual = _separacaoPedido.Pedidos.val()[i];
        if (pedidoAtual.Reentrega)
            return true;
    }

    return false;
}

function loadGridNotas() {
    var somenteLeitura = false;

    _pesquisaSelecaoNotas.ListaCodigosPedidosSelecionados.val(JSON.stringify(ObterCodigosPedidosSelecionadosParaIntegracao()));

    var multiplaescolha = {
        basicGrid: null,
        eventos: function () { },
        selecionados: new Array(),
        naoSelecionados: new Array(),
        SelecionarTodosKnout: _pesquisaSelecaoNotas.SelecionarTodos,
        somenteLeitura: somenteLeitura
    };

    _gridSelecaoNotas = new GridView(_pesquisaSelecaoNotas.Grid.idGrid, "SeparacaoPedido/ObterNotasPedidosSelecionados", _pesquisaSelecaoNotas, null, null, 25, null, null, null, multiplaescolha, null, null, null);
    _gridSelecaoNotas.CarregarGrid();
}

function ObterCodigosPedidosSelecionadosParaIntegracao() {
    var listaCodigosPedidosSelecionados = new Array();

    for (var i = 0; i < _separacaoPedido.Pedidos.val().length; i++) {
        var pedidosSelecionados = _separacaoPedido.Pedidos.val()[i];

        listaCodigosPedidosSelecionados.push(pedidosSelecionados.Codigo);
    }

    return listaCodigosPedidosSelecionados;
}

function ConfirmarNotasClick() {
    Global.fecharModal("divModalSelecionarNotas");
}