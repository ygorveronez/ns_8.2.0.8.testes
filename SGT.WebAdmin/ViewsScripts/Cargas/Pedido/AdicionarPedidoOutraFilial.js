/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Pedido.js" />
/// <reference path="../../Consultas/PeriodoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />

// #region Objetos Globais do Arquivo

var _detalhePedidoAdicionarPedidoOutraFilial;

// #endregion Objetos Globais do Arquivo

// #region Classes

var DetalhePedidoAdicionarPedidoOutraFilial = function () {
    this.CodigoCarga = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CentroCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Centro de Carregamento:", idBtnSearch: guid(), enable: false });
    this.InicioCarregamento = PropertyEntity({});
    this.NumeroReboque = PropertyEntity({ val: ko.observable(EnumNumeroReboque.SemReboque), options: EnumNumeroReboque.obterOpcoes(), def: EnumNumeroReboque.SemReboque, text: "*Número do Reboque: ", required: false, visible: ko.observable(false) });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: "*Tipo do Carregamento: ", required: false, visible: ko.observable(false) });
    this.Pedido = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), required: true, text: "*Pedido:", idBtnSearch: guid(), enable: ko.observable(true), eventChange: pedidoAdicionarPedidoOutraFilialBlur });

    this.PeriodoCarregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Período de Carregamento:", idBtnSearch: guid(), required: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarPedidoOutraFilialDetalhePedidoClick, type: types.event, text: "Adicionar", enable: ko.observable(true) });
}

// #endregion Classes

// #region Funções de Inicialização

function loadDetalhePedidoAdicionarPedidoOutraFilial() {
    _detalhePedidoAdicionarPedidoOutraFilial = new DetalhePedidoAdicionarPedidoOutraFilial();
    KoBindings(_detalhePedidoAdicionarPedidoOutraFilial, "divModalDetalhesPedidoAdicionarPedidoOutraFilial");

    new BuscarPedidosParaTroca(_detalhePedidoAdicionarPedidoOutraFilial.Pedido, retornoConsultaDetalhePedidoAdicionarPedidoOutraFilial);
    new BuscarPeriodoCarregamento(_detalhePedidoAdicionarPedidoOutraFilial.PeriodoCarregamento, retornoPeriodoCarregamentoPedidoAdicionarPedidoOutraFilial, undefined, _detalhePedidoAdicionarPedidoOutraFilial.CentroCarregamento, undefined, undefined, true);
}

// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos

function adicionarPedidoOutraFilialDetalhePedidoClick() {
    if (!ValidarCamposObrigatorios(_detalhePedidoAdicionarPedidoOutraFilial)) {
        exibirMensagem(tipoMensagem.atencao, "Campo Obrigatório", "Por Favor, informe os campos obrigatórios");
        return;
    }

    var data = {
        CodigoPedido: _detalhePedidoAdicionarPedidoOutraFilial.Pedido.codEntity(),
        CodigoCarga: _detalhePedidoAdicionarPedidoOutraFilial.CodigoCarga.val(),
        NumeroReboque: _detalhePedidoAdicionarPedidoOutraFilial.NumeroReboque.val(),
        TipoCarregamentoPedido: _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.val(),
        InicioCarregamento: _detalhePedidoAdicionarPedidoOutraFilial.InicioCarregamento.val(),
    };

    executarReST("Carga/AdicionarPedidoOutraFilialCarga", data, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                fecharModalAdicionarPedidoOutraFilialDetalhePedido();
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Pedido adicionado com sucesso");
                exibirDetalhesPedidos(retorno.Data.Codigo);
                IniciarBindKnoutCarga(_cargaAtual, retorno.Data);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg, 16000);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function pedidoAdicionarPedidoOutraFilialBlur() {
    if (_detalhePedidoAdicionarPedidoOutraFilial.Pedido.val() == "") {
        limparCentroCarregamentoPedidoAdicionarPedidoOutraFilial();
        limparPeriodoCarregamentoPedidoAdicionarPedidoOutraFilial();

        _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);
        _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.visible(false);
        _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.required = false;
    }
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function adicionarPedidoOutraFilialDetalhePedido(codigoCarga, exigirDefinicaoReboquePedido) {
    _detalhePedidoAdicionarPedidoOutraFilial.CodigoCarga.val(codigoCarga);
    _detalhePedidoAdicionarPedidoOutraFilial.NumeroReboque.visible(exigirDefinicaoReboquePedido);
    _detalhePedidoAdicionarPedidoOutraFilial.NumeroReboque.required = exigirDefinicaoReboquePedido;
    _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.visible(false);
    _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.required = false;

    exibirModalAdicionarPedidoOutraFilialDetalhePedido();
}

// #endregion Funções Públicas

// #region Funções Privadas

function BuscarCentroCarregamentoAdicionarPedidoOutraFilial() {
    limparCentroCarregamentoPedidoAdicionarPedidoOutraFilial();
    limparPeriodoCarregamentoPedidoAdicionarPedidoOutraFilial();

    executarReST("CentroCarregamento/BuscarPorCargaEPedido", { Carga: _detalhePedidoContainer.CodigoCarga.val(), Pedido: _detalhePedidoAdicionarPedidoOutraFilial.Pedido.codEntity() }, function (retorno) {
        if (retorno.Success) {
            if (retorno.Data) {
                _detalhePedidoAdicionarPedidoOutraFilial.CentroCarregamento.codEntity(retorno.Data.CentroCarregamento.Codigo);
                _detalhePedidoAdicionarPedidoOutraFilial.CentroCarregamento.val(retorno.Data.CentroCarregamento.Descricao);
                _detalhePedidoAdicionarPedidoOutraFilial.CentroCarregamento.entityDescription(retorno.Data.CentroCarregamento.Descricao);
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", retorno.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", retorno.Msg);
    });
}

function exibirModalAdicionarPedidoOutraFilialDetalhePedido() {
    Global.abrirModal('divModalDetalhesPedidoAdicionarPedidoOutraFilial');
    $("#divModalDetalhesPedidoAdicionarPedidoOutraFilial").one('hidden.bs.modal', function () {
        LimparCampos(_detalhePedidoAdicionarPedidoOutraFilial);
    });
}

function fecharModalAdicionarPedidoOutraFilialDetalhePedido() {
    Global.fecharModal('divModalDetalhesPedidoAdicionarPedidoOutraFilial');
}

function limparCentroCarregamentoPedidoAdicionarPedidoOutraFilial() {
    LimparCampo(_detalhePedidoAdicionarPedidoOutraFilial.CentroCarregamento);
}

function limparPeriodoCarregamentoPedidoAdicionarPedidoOutraFilial() {
    LimparCampo(_detalhePedidoAdicionarPedidoOutraFilial.InicioCarregamento);
    LimparCampo(_detalhePedidoAdicionarPedidoOutraFilial.PeriodoCarregamento);
}

function retornoConsultaDetalhePedidoAdicionarPedidoOutraFilial(pedidoSelecionado) {
    _detalhePedidoAdicionarPedidoOutraFilial.Pedido.codEntity(pedidoSelecionado.Codigo);
    _detalhePedidoAdicionarPedidoOutraFilial.Pedido.val(pedidoSelecionado.Descricao);
    _detalhePedidoAdicionarPedidoOutraFilial.Pedido.entityDescription(pedidoSelecionado.Descricao);
    _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.visible(_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial);
    _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.required = _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.visible();

    if (!_detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.visible())
        _detalhePedidoAdicionarPedidoOutraFilial.TipoCarregamentoPedido.val(EnumTipoCarregamentoPedido.Normal);

    BuscarCentroCarregamentoAdicionarPedidoOutraFilial();
}

function retornoPeriodoCarregamentoPedidoAdicionarPedidoOutraFilial(registroSelecionado) {
    _detalhePedidoAdicionarPedidoOutraFilial.InicioCarregamento.val(registroSelecionado.DataHoraInicio);
    _detalhePedidoAdicionarPedidoOutraFilial.PeriodoCarregamento.codEntity(registroSelecionado.Codigo);
    _detalhePedidoAdicionarPedidoOutraFilial.PeriodoCarregamento.val(registroSelecionado.Descricao);
    _detalhePedidoAdicionarPedidoOutraFilial.PeriodoCarregamento.entityDescription(registroSelecionado.Descricao);
    _detalhePedidoAdicionarPedidoOutraFilial.PeriodoCarregamento.requiredClass("form-control");
}

// #endregion Funções Privadas
