/// <reference path="RetiradaProduto.js" />

/*
 * Declaração de Objetos Globais do Arquivo
 */

var _confirmacao;

/*
 * Declaração das Classes
 */

var Confirmacao = function () {
    this.GridProduto = PropertyEntity({ type: types.local, idGrid: guid() });

    this.Filial = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Unidade.getRequiredFieldDescription(), def: true, val: ko.observable("") });
    this.TipoOperacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.TipoAgendamento.getRequiredFieldDescription(), val: ko.observable(""), visible: ko.observable(false) });
    this.PlacaVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.PlacaDoVeiculo.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.ModeloVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.ModeloDoVeiculo.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.NomeTransportadora = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.NomeTransportadora.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.CpfMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.CPFDoMotorista.getRequiredFieldDescription(), maxlength: 14, getType: typesKnockout.cpf });
    this.NomeMotorista = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.NomeDoMotorista.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Situacao.getRequiredFieldDescription(), val: ko.observable(true), options: _status, def: true });

    this.EmailNotificacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.EnviarConfirmacaoOutroEmail, getType: typesKnockout.string, val: ko.observable(""), enable: ko.observable(true) });
    this.ObservacaoTransportador = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Observacao, val: ko.observable(""), enable: ko.observable(true), visible: ko.observable(false), maxlength: 400 });

    this.CargaNecessitaLonamento = PropertyEntity({ text: "*De acordo que o agendamento da Carga necessita de Lonamento", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });
    this.CargaComplexa = PropertyEntity({ text: "*De acordo que o agendamento é uma Carga Complexa", getType: typesKnockout.bool, val: ko.observable(false), visible: ko.observable(false), required: ko.observable(false) });

    this.NumeroCarregamento = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.NumeroAgendamento, getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });

    this.DataRetiradaVisualizacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.DataRetirada.getFieldDescription(), val: ko.observable(""), visible: false });

    this.Data = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Data.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), visible: false });
    this.Hora = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Hora.getRequiredFieldDescription(), getType: typesKnockout.string, val: ko.observable(""), visible: false });

    this.Pedidos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.Produtos = PropertyEntity({ type: types.map, getType: typesKnockout.dynamic, val: ko.observable(""), def: "", idGrid: guid() });

    this.CapacidadeVeiculo = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.CapacidadeDoVeiculo, getType: typesKnockout.int, val: ko.observable(0) });
    this.PesoTotal = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.PesoTotal, getType: typesKnockout.int, val: ko.observable(0) });
    this.EspacoDisponivel = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Disponivel, getType: typesKnockout.int, val: ko.observable(0) });
    this.Ocupacao = PropertyEntity({ text: Localization.Resources.Pedidos.RetiradaProduto.Ocupacao, getType: typesKnockout.int, val: ko.observable(0) });

    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: Localization.Resources.Pedidos.RetiradaProduto.ExcluirAgendamento, visible: ko.observable(false), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: Localization.Resources.Pedidos.RetiradaProduto.ConfirmarAgendamento, visible: ko.observable(true), enable: ko.observable(true) });
    this.ReenviarIntegracao = PropertyEntity({ eventClick: reenviarIntegracaoClick, type: types.event, text: Localization.Resources.Pedidos.RetiradaProduto.ReenviarIntegracao, visible: ko.observable(false) });

    this.MensagemProblemaCarregamento = PropertyEntity({ getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(false) });
}

/*
 * Declaração das Funções de Inicialização
 */

function loadGridProdutoConfirmacao() {
    var opcaoExcluir = { descricao: Localization.Resources.Pedidos.RetiradaProduto.Excluir, id: guid(), metodo: function (data) { excluirProdutoClick(_pedidoProduto.GridProduto, data) } };
    var editar = { descricao: Localization.Resources.Pedidos.RetiradaProduto.Editar, id: guid(), metodo: function (data) { editarProdutoClick(_pedidoProduto.GridProduto, data) } };

    var menuOpcoes = { tipo: TypeOptionMenu.list, opcoes: [editar, opcaoExcluir], tamanho: 10 };

    var header = [
        { data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Pedidos.RetiradaProduto.Descricao, width: "40%" },
        { data: "Quantidade", title: Localization.Resources.Pedidos.RetiradaProduto.Quantidade, width: "15%" },
        { data: "QuantidadeRetirada", title: Localization.Resources.Pedidos.RetiradaProduto.QuantidadeRetirada, width: "15%" },
    ];
    _gridProduto = new BasicDataTable(_confirmacao.GridProduto.idGrid, header, menuOpcoes, { column: 1, dir: orderDir.asc });
}


/*
 * Declaração das Funções Associadas a Eventos
 */
function loadConfirmacao() {
    _confirmacao = new Confirmacao();
    KoBindings(_confirmacao, "knoutConfirmacao");

    loadGridProdutoConfirmacao();
}

function carregarInformacoesConfirmacao() {
    _confirmacao.PlacaVeiculo.val(_retiradaProduto.PlacaVeiculo.val());
    _confirmacao.ModeloVeiculo.val(obterModeloVeiculo());
    _confirmacao.TipoOperacao.val(obterTipoOperacao());
    _confirmacao.Filial.val(obterFilial());
    _confirmacao.NomeTransportadora.val(_retiradaProduto.NomeTransportadora.val());
    _confirmacao.CpfMotorista.val(_retiradaProduto.Motorista.val());
    _confirmacao.NomeMotorista.val(_retiradaProduto.NomeMotorista.val());
    _confirmacao.DataRetiradaVisualizacao.val(_retiradaProduto.DataRetirada.val());
    _confirmacao.NumeroCarregamento.val(_retiradaProduto.NumeroCarregamento.val());
    _confirmacao.EmailNotificacao.val(_retiradaProduto.EmailNotificacao.val());

    _confirmacao.MensagemProblemaCarregamento.val(_retiradaProduto.MensagemProblemaCarregamento.val());
    _confirmacao.ReenviarIntegracao.visible(_retiradaProduto.Situacao.val() == EnumSituacaoCarregamento.FalhaIntegracao);

    _confirmacao.ObservacaoTransportador.visible(_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe);
    _confirmacao.ObservacaoTransportador.val(_retiradaProduto.ObservacaoTransportador.val());

    if (_retiradaProduto.Codigo.val() > 0) {
        _confirmacao.NumeroCarregamento.visible(true);


        if (_retiradaProduto.Situacao.val() == EnumSituacaoCarregamento.Fechado && _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe)
            _confirmacao.Excluir.visible(false);
        else
            _confirmacao.Excluir.visible(true);
    }

    var produtos = [];
    _confirmacao.Pedidos.val(_pedido.Pedido.basicTable.BuscarRegistros());
    for (var i = 0; i < _confirmacao.Pedidos.val().length; i++) {
        var pedido = _confirmacao.Pedidos.val();
        for (var j = 0; j < pedido[i].Produtos.length; j++) {
            if (pedido)
                var produto = pedido[i].Produtos;
            if (produto[j].QuantidadeRetirada > 0)
                produtos.push(produto[j]);
        }
    }

    const temProdutoComEmbalagemComplexa = produtos.some(produto => produto.TipoEmbalagem == EnumTipoEmbalagem.Complexa);
    const temProdutoComNecessidadeLonamento = produtos.some(produto => produto.TipoEmbalagem == EnumTipoEmbalagem.Lona);

    if (temProdutoComEmbalagemComplexa) {
        _confirmacao.CargaComplexa.visible(temProdutoComEmbalagemComplexa);
        _confirmacao.CargaComplexa.required(temProdutoComEmbalagemComplexa);
    }

    if (temProdutoComNecessidadeLonamento) {
        _confirmacao.CargaNecessitaLonamento.visible(temProdutoComNecessidadeLonamento);
        _confirmacao.CargaNecessitaLonamento.required(temProdutoComNecessidadeLonamento);
    }

    _gridProduto.CarregarGrid(produtos);
    _gridProduto.DesabilitarOpcoes();
    _confirmacao.GridProduto.basicTable = _gridProduto;
}


/*
 * Declaração das Funções
 */

function LimparAbaConfirmaca() {
    if (!_confirmacao) return;

    LimparCampos(_confirmacao);
    _confirmacao.ReenviarIntegracao.visible(false);
}