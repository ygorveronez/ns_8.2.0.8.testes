/// <reference path="../../../wwwroot/js/Global/CRUD.js" />
/// <reference path="../../../wwwroot/js/Global/Grid.js" />
/// <reference path="../../../wwwroot/js/Global/Mensagem.js" />
/// <reference path="../../../wwwroot/js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />
/// <reference path="Carregamento.js" />
/// <reference path="Pedido.js" />

// #region Objetos Globais do Arquivo

var _carregamentoPedido;
var _carregamentoPedidoAlterarPrevisaoEntrega;
var _carregamentoPedidoAlterarRecebedor;
var _carregamentoPedidoDefinirReboque;
var _carregamentoPedidoDefinirTipoCarregamentoPedido;
var _gridPedidosCarregamento;
var _carregamentoPedidoDefinirTipoPaleteClienteMapa;
// #endregion Objetos Globais do Arquivo

var TipoPaleteClienteOptionsMapa = [
    { value: 0, text: "" },
    { value: 1, text: "Chep" },
    { value: 2, text: "Batido" },
    { value: 3, text: "Palete Retorno" }
]

// #region Classes

var CarregamentoPedido = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.BuscarSugestaoPedidos = PropertyEntity({ eventClick: buscarSugestaoPedidosClick, type: types.event, text: Localization.Resources.Cargas.MontagemCargaMapa.BuscarPedidosCompativeis, visible: ko.observable(false), enable: ko.observable(false) });
    this.BipagemPedido = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.CodigoBarrasPedido.getFieldDescription(), idBtnSearch: guid(), eventClick: buscarPedidoBipagem, enable: ko.observable(true), visible: bipagemVisivel() });

    this.OpcoesMenu = PropertyEntity({ text: "Opções", visible: ko.observable(false) });
    this.DefinirRecebedorTodosPedidos = PropertyEntity({ eventClick: DefinirRecebedorPedidosSelecionados, type: types.event, text: "Definir recebedor para pedidos selecionados" });
    this.RemoverRecebedorTodosPedidos = PropertyEntity({ eventClick: RemoverRecebedorPedidosSelecionados, type: types.event, text: "Remover recebedor para pedidos selecionados" });
    this.DefinirTipoPaleteCliente = PropertyEntity({ eventClick: exibirModalDefinirTipoPaleteCliente, type: types.event, text: "Definir Tipo Palete Cliente" });
    this.RemoverTipoPaleteCliente = PropertyEntity({ eventClick: RemoverTipoPaleteClientePedidosSelecionados, type: types.event, text: "Remover Tipo Palete Cliente" });
    this.RemoverPedidos = PropertyEntity({ eventClick: RemoverPedidosSelecionadosClick, type: types.event, text: "Remover Pedidos Selecionados" });

    this.BipagemPedido.val.subscribe(function (val) {
        if (val.length == 14) {
            buscarPedidoBipagem();
        }
    });

}

var CarregamentoPedidoDefinirReboque = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroReboque = PropertyEntity({ val: ko.observable(EnumNumeroReboque.SemReboque), options: EnumNumeroReboque.obterOpcoes(), def: EnumNumeroReboque.SemReboque, text: Localization.Resources.Cargas.MontagemCargaMapa.NumeroDoReboque.getRequiredFieldDescription(), required: true });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoReboquePedidoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Definir) });
}

var CarregamentoPedidoDefinirRecebedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCargaMapa.Recebedor.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false) });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoRecebedorPedidoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Definir) });
}

var CarregamentoPedidoDefinirTipoCarregamentoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: Localization.Resources.Cargas.MontagemCargaMapa.TipoDoCarregamento.getRequiredFieldDescription(), required: true });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoTipoCarregamentoPedidoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Definir) });
}

var CarregamentoPedidoAlterarPrevisaoEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCargaMapa.PrevisaoDeEntrega.getRequiredFieldDescription(), getType: typesKnockout.dateTime, required: true });

    this.Alterar = PropertyEntity({ eventClick: salvarAlteracaoPrevisaoEntregaPedidoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCargaMapa.Alterar) });
}

var CarregamentoPedidoDefinirTipoPaleteClienteMapa = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoPaleteCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Palete Cliente", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false), options: TipoPaleteClienteOptionsMapa });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoTipoPaleteClientePedidosSelecionadosClick, type: types.event, text: ko.observable("Definir") });
}
// #endregion Classes

// #region Funções de Inicialização

function loadCarregamentoPedido() {
    _carregamentoPedido = new CarregamentoPedido();
    KoBindings(_carregamentoPedido, "knoutPedidos");

    _carregamentoPedidoAlterarPrevisaoEntrega = new CarregamentoPedidoAlterarPrevisaoEntrega();
    KoBindings(_carregamentoPedidoAlterarPrevisaoEntrega, "knockoutAlterarPrevisaoEntregaPedido");

    _carregamentoPedidoDefinirReboque = new CarregamentoPedidoDefinirReboque();
    KoBindings(_carregamentoPedidoDefinirReboque, "knockoutDefinirReboquePedido");

    _carregamentoPedidoAlterarRecebedor = new CarregamentoPedidoDefinirRecebedor();
    KoBindings(_carregamentoPedidoAlterarRecebedor, "knockoutDefinirRecebedorPedido");

    _carregamentoPedidoDefinirTipoCarregamentoPedido = new CarregamentoPedidoDefinirTipoCarregamentoPedido();
    KoBindings(_carregamentoPedidoDefinirTipoCarregamentoPedido, "knockoutDefinirTipoCarregamentoPedido");

    _carregamentoPedidoDefinirTipoPaleteClienteMapa = new CarregamentoPedidoDefinirTipoPaleteClienteMapa();
    KoBindings(_carregamentoPedidoDefinirTipoPaleteClienteMapa, "knockoutDefinirTipoPaleteCliente");

    BuscarClientes(_carregamentoPedidoAlterarRecebedor.Recebedor);
    loadGridCarregamentoPedido();
}

function loadGridCarregamentoPedido() {
    const opcaoAlterarPrevisaoEntrega = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.AlterarPrevisaoDeEntrega, id: guid(), metodo: alterarPrevisaoEntregaPedidoClick, icone: "", visibilidade: isPermitirAlterarPrevisaoEntrega };
    const opcaoDefinirReboque = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.DefinirReboque, id: guid(), metodo: definirReboquePedidoClick, icone: "", visibilidade: isPermitirDefinirReboque };
    const opcaoDefinirTipoCarregamentoPedido = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.DefinirTipoDoCarregamento, id: guid(), metodo: definirTipoCarregamentoPedidoClick, icone: "", visibilidade: isPermitirDefinirTipoCarregamentoPedido };
    const opcaoDefinirTipoPaleteCliente = { descricao: "Definir Tipo Palete Cliente", id: guid(), metodo: definirTipoPaleteClienteClick, icone: "", visibilidade: true };
    const opcaoRemoverTipoPaleteCliente = { descricao: "Remover Tipo Palete Cliente", id: guid(), metodo: removerTipoPaleteClientePedidoClick, icone: "", visibilidade: true };
    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesPedidoCarregamentoClick, icone: "" };
    const opcaoProdutos = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.ProdutosDoPedido, id: guid(), metodo: produtosPedidoClick, icone: "", title: Localization.Resources.Cargas.MontagemCargaMapa.ProdutosDoPedidoNoCarregamento };
    const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPedidoClick, icone: "" };
    const opcaoLocalizaMapa = { descricao: Localization.Resources.Cargas.MontagemCargaMapa.LocalizarNoMapa, id: guid(), metodo: localizaMapaClick, icone: "", visibilidade: _CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoAlterarPrevisaoEntrega, opcaoDefinirReboque, opcaoDefinirTipoCarregamentoPedido, opcaoDetalhes, opcaoLocalizaMapa, opcaoProdutos, opcaoRemover, opcaoDefinirTipoPaleteCliente, opcaoRemoverTipoPaleteCliente] };

    const header = [
        { data: "Codigo", visible: false },
        { data: "DataCarregamento", visible: false },
        { data: "DataDescarregamento", visible: false },
        { data: "DataPrevisaoEntrega", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "NumeroReboque", visible: false },
        { data: "TipoCarregamentoPedido", visible: false },
        { data: "NumeroReboqueDescricao", visible: false },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Cargas.MontagemCargaMapa.Numero, className: "text-align-center", width: "12%", widthDefault: "5%", visible: true },
        { data: "Destinatario", title: Localization.Resources.Cargas.MontagemCargaMapa.Destinatario, width: "15%", widthDefault: "5%", callbackToolTip: retornoCallbackRecebedorToolTip, visible: true },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCargaMapa.Destino, width: "15%", widthDefault: "5%", visible: true },
        { data: "Remetente", title: Localization.Resources.Cargas.MontagemCargaMapa.Remetente, width: "15%", widthDefault: "5%", callbackToolTip: retornoCallbackRecebedorToolTip, visible: false },
        { data: "Origem", title: Localization.Resources.Cargas.MontagemCargaMapa.Origem, width: "15%", widthDefault: "5%", visible: false },
        { data: "CEPDestinatario", title: Localization.Resources.Cargas.MontagemCargaMapa.CEP, width: "5%", visible: true },
        { data: "Categoria", title: Localization.Resources.Cargas.MontagemCargaMapa.Categoria, width: "10%", widthDefault: "5%", visible: _CONFIGURACAO_TMS.ExigirCategoriaCadastroPessoa },
        { data: "TipoCarregamentoPedidoDescricao", title: Localization.Resources.Cargas.MontagemCargaMapa.TipoDoCarregamento, className: "text-align-center", width: "12%", widthDefault: "5%", visible: _CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido },
        { data: "VolumesBipagem", title: Localization.Resources.Cargas.MontagemCargaMapa.Bipagem, width: "5%", widthDefault: "5%", visible: bipagemVisivel() },
        { data: "DataPrevisaoEntrega", title: Localization.Resources.Cargas.MontagemCargaMapa.DataPrevisao, width: "15%", widthDefault: "5%", visible: true },
        { data: "Saldo", title: Localization.Resources.Cargas.MontagemCargaMapa.Saldo, width: "15%", widthDefault: "5%", visible: true },
        { data: "Cubagem", title: Localization.Resources.Cargas.MontagemCargaMapa.Cubagem, width: "5%", widthDefault: "5%", visible: true },
        { data: "CodigoAgrupamentoCarregamento", title: Localization.Resources.Cargas.MontagemCargaMapa.CodigoAgrupamentoCarregamento, width: "5%", widthDefault: "5%", visible: false },
    ];

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true }
    _gridPedidosCarregamento = new BasicDataTable(_carregamentoPedido.Grid.id, header, menuOpcoes, { column: 5, dir: "asc" }, configRowsSelect, 100, null, null, null, null, null, null, null, null, null, true, tablePedidosCarregamentoSelecionadoChange, null, "Cargas/MontagemCargaMapa", "grid-montagem-carga-pedido-carregamento");

    _gridPedidosCarregamento.SetPermitirEdicaoColunas(true);
    _gridPedidosCarregamento.SetSalvarPreferenciasGrid(true);

    RenderizarGridMotagemPedidos();
}

// #endregion Funções de Inicialização


function tablePedidosCarregamentoSelecionadoChange(registro, selecionado) {
    const temPedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados().length > 0;

    _carregamentoPedido.OpcoesMenu.visible(temPedidosSelecionados);
}

// #region Funções Associadas a Eventos

function bipagemVisivel() {
    return !_CONFIGURACAO_TMS.MontagemCarga.OcultarBipagem;
}

function alterarPrevisaoEntregaPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoAlterarPrevisaoEntrega.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoAlterarPrevisaoEntrega.DataPrevisaoEntrega.val(pedidoSelecionado.DataPrevisaoEntrega);

    exibirModalAlterarPrevisaoEntregaPedido();
}

function buscarSugestaoPedidosClick() {
    const data = { CodigoPedido: PEDIDOS_SELECIONADOS()[0].Codigo, ModeloVeicular: _carregamento.ModeloVeicularCarga.codEntity(), TipoCarga: _carregamento.TipoDeCarga.codEntity() };
    executarReST("MontagemCargaPedido/BuscarSugestacaoPedidos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                const pedidos = arg.Data;
                let pedidosSelecionados = PEDIDOS_SELECIONADOS();
                pedidosSelecionados = pedidos.concat(pedidosSelecionados);
                PEDIDOS_SELECIONADOS(pedidosSelecionados);

                VerificarVisibilidadeBuscaSugestaoPedido();
                RenderizarGridMotagemPedidos();
                buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function definirReboquePedidoClick(pedidoSelecionado) {
    _carregamentoPedidoDefinirReboque.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoDefinirReboque.NumeroReboque.val(pedidoSelecionado.NumeroReboque);

    exibirModalDefinirReboquePedido();
}

function definirTipoCarregamentoPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoDefinirTipoCarregamentoPedido.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoDefinirTipoCarregamentoPedido.TipoCarregamentoPedido.val(pedidoSelecionado.TipoCarregamentoPedido);

    exibirModalDefinirTipoCarregamentoPedido();
}

function definirTipoPaleteClienteClick(pedidoSelecionado) {
    const tipoPaleteCliente = pedidoSelecionado.TipoPaleteCliente != null ? pedidoSelecionado.TipoPaleteCliente : 0;
    _carregamentoPedidoDefinirTipoPaleteClienteMapa.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoDefinirTipoPaleteClienteMapa.TipoPaleteCliente.val(tipoPaleteCliente);

    exibirModalDefinirTipoPaleteCliente();
}

function detalhesPedidoCarregamentoClick(pedidoSelecionado) {
    if (_filtroDetalhePedido)
        _filtroDetalhePedido.Pedidos.visible(false);
    ObterDetalhesPedido(pedidoSelecionado.Codigo);
}

function localizaMapaClick(pedido) {
    centralizarPedidoMapa(pedido.Codigo);
}

function produtosPedidoClick(pedidoSelecionado) {
    ObterPedidoProdutosCarregamento(pedidoSelecionado.Codigo);
}

function removerPedidoClick(pedidoSelecionado) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCargaMapa.RealmenteDesejaRemoverPedidoDestaMontagemDeCarga.format(pedidoSelecionado.NumeroPedidoEmbarcador), function () {
        removePedidoSelecionado(pedidoSelecionado);        
    });
}

function removePedidoSelecionado(pedidoSelecionado) {
    //AKi...
    var itemPedido = PEDIDOS.where(function (ped) { return ped.Codigo == pedidoSelecionado.Codigo; });

    //#49608-SIMONETTI
    if (itemPedido.CodigoAgrupamentoCarregamento != '' && _AreaPedido.SelecionarTodosMesmoAgrupamento.val()) {

        var pedidos = ko.utils.arrayFilter(PEDIDOS(), function (item) {
            if (item.CodigoAgrupamentoCarregamento == itemPedido.CodigoAgrupamentoCarregamento) {
                return item;
            }
        });

        for (var i = 0; i < pedidos.length; i++) {
            var ped = pedidos[i];
            var pedidoAtualizar = $.extend({}, ped);

            pedidoAtualizar.Selecionado = false;
            pedidoAtualizar.PedidoSelecionadoCompleto = false;

            PEDIDOS.replace(ped, pedidoAtualizar);
            PEDIDOS_SELECIONADOS.remove(function (p) { return p.Codigo == ped.Codigo });
        }

    } else {
        SelecionarPedido(pedidoSelecionado, true);
    }
}

function removerTipoPaleteClientePedidoClick(pedidoSelecionado) {
    const tipoPaleteClientePedido = TipoPaleteClienteOptionsMapa.find(tpc => tpc.value == pedidoSelecionado.TipoPaleteCliente);
    exibirConfirmacao("Confirmação", `Realmente deseja remover o Tipo Palete ${tipoPaleteClientePedido.text}?`, function () {
        _carregamentoPedidoDefinirTipoPaleteClienteMapa.Codigo.val(pedidoSelecionado.Codigo);
        _carregamentoPedidoDefinirTipoPaleteClienteMapa.TipoPaleteCliente.codEntity(0);

        salvarTipoPaleteClientePedidoClick();
    });
}

function RemoverTipoPaleteClientePedidosSelecionados() {
    const pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();
    exibirConfirmacao("Confirmação", "Realmente deseja remover o Tipo Palete?", function () {
        for (let i = 0; i < pedidosSelecionados.length; i++) {
            pedidosSelecionados[i].TipoPaleteCliente = 0;
        }

        RenderizarGridMotagemPedidos();
    });
}

function RemoverPedidosSelecionadosClick() {
    const pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();
    exibirConfirmacao("Confirmação", "Realmente deseja remover os Pedidos Selecionados ?", function () {
        for (let i = 0; i < pedidosSelecionados.length; i++) {
            removePedidoSelecionado(pedidosSelecionados[i]);
        }
        RenderizarGridMotagemPedidos();
    });
}

function salvarAlteracaoPrevisaoEntregaPedidoClick() {
    if (!ValidarCamposObrigatorios(_carregamentoPedidoAlterarPrevisaoEntrega)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PreenchaOsCamposObrigatorios);
        return;
    }

    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Codigo == _carregamentoPedidoAlterarPrevisaoEntrega.Codigo.val()) {
            pedidos[i].DataPrevisaoEntrega = _carregamentoPedidoAlterarPrevisaoEntrega.DataPrevisaoEntrega.val();
            break;
        }
    }

    RenderizarGridMotagemPedidos();
    fecharModalAlterarPrevisaoEntregaPedido();
}

function salvarDefinicaoReboquePedidoClick() {
    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Codigo == _carregamentoPedidoDefinirReboque.Codigo.val()) {
            pedidos[i].NumeroReboque = _carregamentoPedidoDefinirReboque.NumeroReboque.val();
            pedidos[i].NumeroReboqueDescricao = EnumNumeroReboque.obterDescricao(_carregamentoPedidoDefinirReboque.NumeroReboque.val());
            break;
        }
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirReboquePedido();
}

function salvarDefinicaoTipoCarregamentoPedidoClick() {
    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Codigo == _carregamentoPedidoDefinirTipoCarregamentoPedido.Codigo.val()) {
            pedidos[i].TipoCarregamentoPedido = _carregamentoPedidoDefinirTipoCarregamentoPedido.TipoCarregamentoPedido.val();
            pedidos[i].TipoCarregamentoPedidoDescricao = EnumTipoCarregamentoPedido.obterDescricao(_carregamentoPedidoDefinirTipoCarregamentoPedido.TipoCarregamentoPedido.val());
            break;
        }
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirTipoCarregamentoPedido();
}

function salvarTipoPaleteClientePedidoClick() {
    let pedidos = PEDIDOS_SELECIONADOS();

    for (let i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Codigo == _carregamentoPedidoDefinirTipoPaleteClienteMapa.Codigo.val()) {
            pedidos[i].TipoPaleteCliente = _carregamentoPedidoDefinirTipoPaleteClienteMapa.TipoPaleteCliente.val();
            break;
        }
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirTipoPaleteCliente();
    
}

function salvarDefinicaoTipoPaleteClientePedidosSelecionadosClick() {
    let pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();

    if (_carregamentoPedidoDefinirTipoPaleteClienteMapa.Codigo.val() > 0) {
        salvarTipoPaleteClientePedidoClick();
        return;
    }

    for (let i = 0; i < pedidosSelecionados.length; i++) {
        pedidosSelecionados[i].TipoPaleteCliente = _carregamentoPedidoDefinirTipoPaleteClienteMapa.TipoPaleteCliente.val();
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirTipoPaleteCliente();
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas

function carregarDefinicaoReboquePedidosSelecionados() {
    if (isPermitirDefinirReboque()) {
        _gridPedidosCarregamento.ControlarExibicaoColuna("NumeroReboqueDescricao", true);
        return;
    }

    _gridPedidosCarregamento.ControlarExibicaoColuna("NumeroReboqueDescricao", false);

    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i = 0; i < pedidos.length; i++)
        pedidos[i].NumeroReboque = EnumNumeroReboque.SemReboque;
}

function carregarDefinicaoTipoCarregamentoPedidosSelecionados() {
    if (_CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido)
        return;

    var pedidos = PEDIDOS_SELECIONADOS();

    for (var i = 0; i < pedidos.length; i++)
        pedidos[i].TipoCarregamentoPedido = EnumTipoCarregamentoPedido.Normal;
}

function limparCarregamentoPedido() {
    LimparCampos(_carregamentoPedido);
}

// #endregion Funções Públicas

// #region Funções Privadas

function exibirModalAlterarPrevisaoEntregaPedido() {
    Global.abrirModal('modalAlterarPrevisaoEntregaPedido');
    $("#modalAlterarPrevisaoEntregaPedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoAlterarPrevisaoEntrega);
    });
}

function exibirModalDefinirReboquePedido() {
    Global.abrirModal('modalDefinirReboquePedido');
    $("#modalDefinirReboquePedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoDefinirReboque);
    });
}

function exibirModalDefinirTipoCarregamentoPedido() {
    Global.abrirModal('modalDefinirTipoCarregamentoPedido');
    $("#modalDefinirTipoCarregamentoPedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoDefinirTipoCarregamentoPedido);
    });
}

function exibirModalDefinirTipoPaleteCliente() {
    Global.abrirModal("modalDefinirTipoPaleteCliente");
    $("#modalDefinirTipoPaleteCliente").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoDefinirTipoPaleteClienteMapa);
    });
}

function fecharModalAlterarPrevisaoEntregaPedido() {
    Global.fecharModal("modalAlterarPrevisaoEntregaPedido");
}

function fecharModalDefinirReboquePedido() {
    Global.fecharModal("modalDefinirReboquePedido");
}

function fecharModalDefinirTipoCarregamentoPedido() {
    Global.fecharModal("modalDefinirTipoCarregamentoPedido");
}

function fecharModalDefinirTipoPaleteCliente() {
    Global.fecharModal("modalDefinirTipoPaleteCliente");
}

function isPermitirAlterarPrevisaoEntrega() {
    return true;
}

function isPermitirDefinirReboque() {
    return _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido && (_carregamento.ModeloVeicularCarga.numeroReboques > 1);
}

function isPermitirDefinirTipoCarregamentoPedido(pedidoSelecionado) {
    return _CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial;
}

function RenderizarGridMotagemPedidos() {
    _gridPedidosCarregamento.CarregarGrid(PEDIDOS_SELECIONADOS());

    // Vamos ocultar o destinatário para os tipos de montagem por pedido produto.
    let destinatarioVisivel = true;
    if (_sessaoRoteirizador != null && _sessaoRoteirizador != undefined) {
        destinatarioVisivel = (!_sessaoRoteirizador.MontagemCarregamentoPedidoProduto.val());
    }

    let exibirSaldo = false;
    if (PEDIDOS_SELECIONADOS().length > 0)
        exibirSaldo = PEDIDOS_SELECIONADOS().some(p => p.Saldo != null); 

    _gridPedidosCarregamento.ControlarExibicaoColuna("Destinatario", destinatarioVisivel);

    if (_sessaoRoteirizador.TipoRoteirizacaoColetaEntrega.val() == EnumTipoRoteirizacaoColetaEntrega.Coleta) {
        _gridPedidosCarregamento.ControlarExibicaoColuna('Destino', false);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Destinatario', false);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Origem', true);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Remetente', true);
    } else {
        _gridPedidosCarregamento.ControlarExibicaoColuna('Destino', true);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Destinatario', destinatarioVisivel);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Origem', false);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Remetente', false);
        _gridPedidosCarregamento.ControlarExibicaoColuna('Saldo', exibirSaldo);

    }
}

// #endregion Funções Privadas

//# region Definir Recebedor Pedido

function exibirModalDefinirRecebedorPedido() {
    Global.abrirModal('modalDefinirRecebedorPedido');
    $("#modalDefinirRecebedorPedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoAlterarRecebedor);
    });
}

function fecharModalDefinirRecebedorPedido() {
    Global.fecharModal('modalDefinirRecebedorPedido');
}

function DefinirRecebedorPedidosSelecionados() {
    exibirModalDefinirRecebedorPedido()
}

function RemoverRecebedorPedidosSelecionados() {
    const pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();
    exibirConfirmacao("Confirmação", "Realmente deseja remover o recebedor?", function () {
        for (let i = 0; i < pedidosSelecionados.length; i++) {
            pedidosSelecionados[i].Recebedor = "";
            pedidosSelecionados[i].CodRecebedor = 0;
        }

        RenderizarGridMotagemPedidos();
    });
}

function definirRecebedorPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoAlterarRecebedor.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoAlterarRecebedor.Recebedor.codEntity(pedidoSelecionado.CodigoRecebedor);
    _carregamentoPedidoAlterarRecebedor.Recebedor.entityDescription(pedidoSelecionado.Recebedor);
    _carregamentoPedidoAlterarRecebedor.Recebedor.val(pedidoSelecionado.Recebedor);

    exibirModalDefinirRecebedorPedido();
}

function salvarDefinicaoRecebedorPedidoClick() {
    let pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();

    for (let i = 0; i < pedidosSelecionados.length; i++) {
        pedidosSelecionados[i].Recebedor = _carregamentoPedidoAlterarRecebedor.Recebedor.val();
        pedidosSelecionados[i].CodRecebedor = _carregamentoPedidoAlterarRecebedor.Recebedor.codEntity();
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirRecebedorPedido();
}

function retornoCallbackRecebedorToolTip(registro) {
    if (registro.Recebedor != null && registro.Recebedor != "") {
        return 'Recebedor: ' + registro.Recebedor;
    } else {
        return registro.Destinatario;
    }
}

// #endregion Definir Recebedor Pedido