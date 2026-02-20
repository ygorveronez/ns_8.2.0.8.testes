/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Enumeradores/EnumNumeroReboque.js" />
/// <reference path="../../Enumeradores/EnumTipoCarregamentoPedido.js" />
/// <reference path="Carregamento.js" />
/// <reference path="Pedido.js" />

// #region Objetos Globais do Arquivo

var _carregamentoPedido;
var _carregamentoPedidoAlterarPrevisaoEntrega;
var _carregamentoPedidoAlterarQuantidades;
var _carregamentoPedidoAlterarRecebedor;
var _carregamentoPedidoDefinirReboque;
var _carregamentoPedidoDefinirTipoCarregamentoPedido;
var _gridPedidosCarregamento;
var _carregamentoPedidoDefinirTipoPaleteCliente;

// #endregion Objetos Globais do Arquivo

var TipoPaleteClienteOptions = [
    { value: 1, text: "Chep" },
    { value: 2, text: "Batido" },
    { value: 3, text: "Palete Retorno" }
]

// #region Classes

var CarregamentoPedido = function () {
    this.Grid = PropertyEntity({ type: types.local });
    this.BuscarSugestaoPedidos = PropertyEntity({ eventClick: buscarSugestaoPedidosClick, type: types.event, text: Localization.Resources.Cargas.MontagemCarga.BuscarPedidosCompativeis, visible: ko.observable(false), enable: ko.observable(false) });

    this.OpcoesMenu = PropertyEntity({ text: "Opções", visible: ko.observable(false) });
    this.DefinirRecebedorTodosPedidos = PropertyEntity({ eventClick: DefinirRecebedorPedidosSelecionados, type: types.event, text: "Definir recebedor para pedidos selecionados" });
    this.RemoverRecebedorTodosPedidos = PropertyEntity({ eventClick: RemoverRecebedorPedidosSelecionados, type: types.event, text: "Remover recebedor para pedidos selecionados" });
    this.DefinirTipoPaleteCliente = PropertyEntity({ eventClick: exibirModalDefinirTipoPaleteCliente, type: types.event, text: "Definir Tipo Palete Cliente" });
    this.RemoverTipoPaleteCliente = PropertyEntity({ eventClick: RemoverTipoPaleteClientePedidosSelecionados, type: types.event, text: "Remover Tipo Palete Cliente" });
    this.RemoverPedidos = PropertyEntity({ eventClick: RemoverPedidosSelecionadosClick, type: types.event, text: "Remover Pedidos Selecionados" });
}

var CarregamentoPedidoDefinirReboque = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.NumeroReboque = PropertyEntity({ val: ko.observable(EnumNumeroReboque.SemReboque), options: EnumNumeroReboque.obterOpcoes(), def: EnumNumeroReboque.SemReboque, text: Localization.Resources.Cargas.MontagemCarga.NumeroDoReboque.getRequiredFieldDescription(), required: true });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoReboquePedidoClick, type: types.event, text: ko.observable("Definir") });
}

var CarregamentoPedidoDefinirRecebedor = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: Localization.Resources.Cargas.MontagemCarga.Recebedor.getFieldDescription(), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false) });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoRecebedorPedidoClick, type: types.event, text: ko.observable("Definir") });
}

var CarregamentoPedidoDefinirTipoPaleteCliente = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoPaleteCliente = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo Palete Cliente", idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false), options: TipoPaleteClienteOptions });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoTipoPaleteClientePedidosSelecionadosClick, type: types.event, text: ko.observable("Definir") });
}

var CarregamentoPedidoDefinirTipoCarregamentoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.TipoCarregamentoPedido = PropertyEntity({ val: ko.observable(EnumTipoCarregamentoPedido.Normal), options: EnumTipoCarregamentoPedido.obterOpcoes(), def: EnumTipoCarregamentoPedido.Normal, text: Localization.Resources.Cargas.MontagemCarga.TipoCarregamento.getRequiredFieldDescription(), required: true });

    this.Definir = PropertyEntity({ eventClick: salvarDefinicaoTipoCarregamentoPedidoClick, type: types.event, text: ko.observable("Definir") });
}

var CarregamentoPedidoAlterarPrevisaoEntrega = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.DataPrevisaoEntrega = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PrevisaoEntrega.getRequiredFieldDescription(), getType: typesKnockout.dateTime, required: true });

    this.Alterar = PropertyEntity({ eventClick: salvarAlteracaoPrevisaoEntregaPedidoClick, type: types.event, text: ko.observable("Alterar") });
}

var CarregamentoPedidoAlterarQuantidades = function () {

    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.IsPeso = PropertyEntity({ val: ko.observable(true), def: true, getType: typesKnockout.bool });
    this.IsPallet = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Peso = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PesoCarregamento.getFieldDescription(), val: ko.observable(0), visible: ko.observable(true), getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false, allowNegative: false }, required: true });
    this.Pallet = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.PalletCarregamento.getFieldDescription(), val: ko.observable(0), visible: ko.observable(false), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: false }, required: true });

    this.IsPeso.val.subscribe(function (novoValor) {
        if (novoValor === true)
            self.IsPallet.val(false);
        self.Peso.visible(novoValor);
        self.Pallet.visible(!novoValor);
    });

    this.IsPallet.val.subscribe(function (novoValor) {
        if (novoValor === true)
            self.IsPeso.val(false);
        self.Peso.visible(!novoValor);
        self.Pallet.visible(novoValor);
    });

    this.Alterar = PropertyEntity({ eventClick: salvarAlteracaoQuantidadesPedidoCarregamentoClick, type: types.event, text: ko.observable("Alterar") });
}

// #endregion Classes

// #region Funções de Inicialização

function loadCarregamentoPedido() {
    _carregamentoPedido = new CarregamentoPedido();
    KoBindings(_carregamentoPedido, "knoutPedidos");

    _carregamentoPedidoAlterarPrevisaoEntrega = new CarregamentoPedidoAlterarPrevisaoEntrega();
    KoBindings(_carregamentoPedidoAlterarPrevisaoEntrega, "knockoutAlterarPrevisaoEntregaPedido");

    _carregamentoPedidoAlterarQuantidades = new CarregamentoPedidoAlterarQuantidades();
    KoBindings(_carregamentoPedidoAlterarQuantidades, "knockoutAlterarQuantidadesPedidoCarregamento");

    _carregamentoPedidoDefinirReboque = new CarregamentoPedidoDefinirReboque();
    KoBindings(_carregamentoPedidoDefinirReboque, "knockoutDefinirReboquePedido");

    _carregamentoPedidoAlterarRecebedor = new CarregamentoPedidoDefinirRecebedor();
    KoBindings(_carregamentoPedidoAlterarRecebedor, "knockoutDefinirRecebedorPedido");

    _carregamentoPedidoDefinirTipoCarregamentoPedido = new CarregamentoPedidoDefinirTipoCarregamentoPedido();
    KoBindings(_carregamentoPedidoDefinirTipoCarregamentoPedido, "knockoutDefinirTipoCarregamentoPedido");

    _carregamentoPedidoDefinirTipoPaleteCliente = new CarregamentoPedidoDefinirTipoPaleteCliente();
    KoBindings(_carregamentoPedidoDefinirTipoPaleteCliente, "knockoutDefinirTipoPaleteCliente");

    BuscarClientes(_carregamentoPedidoAlterarRecebedor.Recebedor);
    loadGridCarregamentoPedido();
}

function loadGridCarregamentoPedido() {
    const opcaoAlterarPrevisaoEntrega = { descricao: Localization.Resources.Cargas.MontagemCarga.AlterarPrevisaoDeEntrega, id: guid(), metodo: alterarPrevisaoEntregaPedidoClick, icone: "", visibilidade: isPermitirAlterarPrevisaoEntrega };
    const opcaoDefinirReboque = { descricao: Localization.Resources.Cargas.MontagemCarga.DefinirReboque, id: guid(), metodo: definirReboquePedidoClick, icone: "", visibilidade: isPermitirDefinirReboque };
    const opcaoDefinirTipoPaleteCliente = { descricao: "Definir Tipo Palete Cliente", id: guid(), metodo: definirTipoPaleteClienteClick, icone: "", visibilidade: true };
    const opcaoRemoverTipoPaleteCliente = { descricao: "Remover Tipo Palete Cliente", id: guid(), metodo: removerTipoPaleteClientePedidoClick, icone: "", visibilidade: true };
    const opcaoDefinirTipoCarregamentoPedido = { descricao: Localization.Resources.Cargas.MontagemCarga.DefinirTipoCarregamento, id: guid(), metodo: definirTipoCarregamentoPedidoClick, icone: "", visibilidade: isPermitirDefinirTipoCarregamentoPedido };
    const opcaoDetalhes = { descricao: Localization.Resources.Gerais.Geral.Detalhes, id: guid(), metodo: detalhesPedidoClick, icone: "" };
    const opcaoRemover = { descricao: Localization.Resources.Gerais.Geral.Remover, id: guid(), metodo: removerPedidoClick, icone: "" };
    const opcaoAlterarQtdePallet = { descricao: Localization.Resources.Cargas.MontagemCarga.AlterarQtdePallet, id: guid(), metodo: alterarAlterarQtdePalletPedidoClick, icone: "", visibilidade: isPermitirAlterarPalletCarregamentoPedido };
    const opcaoAlterarPeso = { descricao: Localization.Resources.Cargas.MontagemCarga.AlterarPeso, id: guid(), metodo: alterarAlterarPesoPedidoClick, icone: "", visibilidade: isPermitirAlterarPesoCarregamentoPedido };
    const menuOpcoes = { tipo: TypeOptionMenu.list, descricao: Localization.Resources.Gerais.Geral.Opcoes, tamanho: 15, opcoes: [opcaoAlterarPrevisaoEntrega, opcaoAlterarPeso, opcaoAlterarQtdePallet, opcaoDefinirReboque, opcaoDefinirTipoCarregamentoPedido, opcaoDetalhes, opcaoRemover, opcaoDefinirTipoPaleteCliente, opcaoRemoverTipoPaleteCliente] };

    const editarColuna = {
        permite: true,
        atualizarRow: true,
        callback: AtualizarOrdemPedidoCarregamento
    };

    const editableOrdem = {
        editable: true,
        type: EnumTipoColunaEditavelGrid.int,
        mask: "",
        maxlength: 0,
        numberMask: ConfigInt()
    };

    const header = [
        { data: "Codigo", visible: false },
        { data: "DataCarregamento", visible: false },
        { data: "DataDescarregamento", visible: false },
        { data: "DataPrevisaoEntrega", visible: false },
        { data: "DT_RowColor", visible: false },
        { data: "NumeroReboque", visible: false },
        { data: "TipoCarregamentoPedido", visible: false },
        { data: "Ordem", title: Localization.Resources.Cargas.MontagemCarga.Ordem, width: "5%", widthDefault: "5%", className: "text-align-center", editableCell: editableOrdem, visible: true, permiteEsconderColuna: true },
        { data: "NumeroPedidoSequencial", title: Localization.Resources.Cargas.MontagemCarga.Numero, className: "text-align-center", width: "10%", widthDefault: "10%", callbackToolTip: retornoCallbackToolTip, visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador), permiteEsconderColuna: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) },
        { data: "NumeroPedido", title: Localization.Resources.Cargas.MontagemCarga.Numero, className: "text-align-center", width: "10%", widthDefault: "10%", callbackToolTip: retornoCallbackToolTip, visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS) },
        { data: "Destinatario", title: Localization.Resources.Cargas.MontagemCarga.Destinatario, width: "15%", visible: true },
        { data: "Destino", title: Localization.Resources.Cargas.MontagemCarga.Destino, width: "15%", widthDefault: "15%", visible: true, permiteEsconderColuna: true },
        { data: "Recebedor", title: Localization.Resources.Cargas.MontagemCarga.Recebedor, width: "15%", widthDefault: "15%", visible: true, permiteEsconderColuna: true },
        { data: "TipoCarga", title: Localization.Resources.Cargas.MontagemCarga.TipoDeCarga, width: "15%", widthDefault: "15%", visible: true, permiteEsconderColuna: true },
        { data: "NumeroReboqueDescricao", title: Localization.Resources.Cargas.MontagemCarga.Reboque, className: "text-align-center", width: "5%", widthDefault: "5%", visible: false },
        { data: "TipoCarregamentoPedidoDescricao", title: Localization.Resources.Cargas.MontagemCarga.TipoCarregamento, className: "text-align-center", width: "5%", widthDefault: "5%", visible: _CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido, permiteEsconderColuna: _CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido },
        { data: "CategoriaCliente", title: Localization.Resources.Cargas.MontagemCarga.CategoriaCliente, className: "text-align-center", width: "10%", widthDefault: "10%", visible: true, permiteEsconderColuna: true },
        { data: "NumeroPedidoEmbarcador", title: Localization.Resources.Cargas.MontagemCarga.NumeroPedidoEmbarcador, className: "text-align-center", width: "10%", widthDefault: "10%", callbackToolTip: retornoCallbackToolTip, visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador), permiteEsconderColuna: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) },
        { data: "Peso", title: Localization.Resources.Cargas.MontagemCarga.Peso, className: "text-align-center", width: "5%", widthDefault: "5%", callbackToolTip: retornoCallbackToolTip, visible: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador), permiteEsconderColuna: (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador) },
    ];

    const configRowsSelect = { permiteSelecao: true, marcarTodos: false, permiteSelecionarTodos: true };

    _gridPedidosCarregamento = new BasicDataTable(_carregamentoPedido.Grid.id, header, menuOpcoes, { column: 1, dir: "asc" }, configRowsSelect, null, null, null, editarColuna, retornoOrdenacaoPedidosCarregamento, null, null, null, null, null, true, tablePedidosCarregamentoSelecionadoChange);

    _gridPedidosCarregamento.SetPermitirEdicaoColunas((_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador));

    RenderizarGridMotagemPedidos();

}

// #endregion Funções de Inicialização

function tablePedidosCarregamentoSelecionadoChange(registro, selecionado) {
    const temPedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados().length > 0;

    _carregamentoPedido.OpcoesMenu.visible(temPedidosSelecionados);
}

// #region Funções Associadas a Eventos

function alterarPrevisaoEntregaPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoAlterarPrevisaoEntrega.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoAlterarPrevisaoEntrega.DataPrevisaoEntrega.val(pedidoSelecionado.DataPrevisaoEntrega);

    exibirModalAlterarPrevisaoEntregaPedido();
}

function alterarAlterarQtdePalletPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoAlterarQuantidades.IsPallet.val(true);
    alterarQuantidadesPedidoCarregamento(pedidoSelecionado);
}

function alterarAlterarPesoPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoAlterarQuantidades.IsPeso.val(true);
    alterarQuantidadesPedidoCarregamento(pedidoSelecionado);
}

function alterarQuantidadesPedidoCarregamento(pedidoSelecionado) {
    _carregamentoPedidoAlterarQuantidades.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoAlterarQuantidades.Pallet.val(Globalize.format(pedidoSelecionado.PalletPedidoCarregamento, "n2"));
    _carregamentoPedidoAlterarQuantidades.Peso.val(Globalize.format(pedidoSelecionado.PesoPedidoCarregamento, "n4"));
    exibirModalAlterarQuantidadesPedidoCarregamento();
}

function buscarSugestaoPedidosClick() {
    var data = { CodigoPedido: PEDIDOS_SELECIONADOS()[0].Codigo, ModeloVeicular: _carregamento.ModeloVeicularCarga.codEntity(), TipoCarga: _carregamento.TipoDeCarga.codEntity() };
    executarReST("MontagemCargaPedido/BuscarSugestacaoPedidos", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var pedidos = arg.Data;
                var pedidosSelecionados = PEDIDOS_SELECIONADOS();
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

function DefinirRecebedorPedidosSelecionados() {
    exibirModalDefinirRecebedorPedido()
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
    _carregamentoPedidoDefinirTipoPaleteCliente.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoDefinirTipoPaleteCliente.TipoPaleteCliente.val(pedidoSelecionado.TipoPaleteCliente);

    exibirModalDefinirTipoPaleteCliente();
}

function definirRecebedorPedidoClick(pedidoSelecionado) {
    _carregamentoPedidoAlterarRecebedor.Codigo.val(pedidoSelecionado.Codigo);
    _carregamentoPedidoAlterarRecebedor.Recebedor.codEntity(pedidoSelecionado.CodigoRecebedor);
    _carregamentoPedidoAlterarRecebedor.Recebedor.entityDescription(pedidoSelecionado.Recebedor);
    _carregamentoPedidoAlterarRecebedor.Recebedor.val(pedidoSelecionado.Recebedor);

    exibirModalDefinirRecebedorPedido();
}

function removerRecebedorPedidoClick(pedidoSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover o recebedor?", function () {
        _carregamentoPedidoAlterarRecebedor.Codigo.val(pedidoSelecionado.Codigo);
        _carregamentoPedidoAlterarRecebedor.Recebedor.codEntity(0);
        _carregamentoPedidoAlterarRecebedor.Recebedor.entityDescription("");
        _carregamentoPedidoAlterarRecebedor.Recebedor.val("");

        salvarDefinicaoRecebedorPedidoClick();
    });
}

function removerTipoPaleteClientePedidoClick(pedidoSelecionado) {
    exibirConfirmacao("Confirmação", "Realmente deseja remover o Tipo Palete?", function () {
        _carregamentoPedidoDefinirTipoPaleteCliente.Codigo.val(pedidoSelecionado.Codigo);
        _carregamentoPedidoDefinirTipoPaleteCliente.TipoPaleteCliente.codEntity(0);

        salvarTipoPaleteClientePedidoClick();
    });
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

function detalhesPedidoClick(pedidoSelecionado) {
    ObterDetalhesPedido(pedidoSelecionado.Codigo);
}

function removerPedidoClick(pedidoSelecionado) {
    exibirConfirmacao(Localization.Resources.Gerais.Geral.Confirmacao, Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaRemoverPedidoDessaMontagemDeCarga.format(pedidoSelecionado.NumeroPedidoEmbarcador), function () {
        removePedidoSelecionado(pedidoSelecionado);
    });
}

function removePedidoSelecionado(pedidoSelecionado) {
    //AKi...
    var itemPedido = PEDIDOS.where(function (ped) { return ped.Codigo == pedidoSelecionado.Codigo; });

    //#49608-SIMONETTI
    if (itemPedido.CodigoAgrupamentoCarregamento != '') {

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

            if (i == (pedidos.length - 1))
                roteirizarAutomaticamenteAoAdicionarRemoverPedido();
        }

    } else {
        SelecionarPedido(pedidoSelecionado, true);
    }
}

function salvarAlteracaoPrevisaoEntregaPedidoClick() {
    if (!ValidarCamposObrigatorios(_carregamentoPedidoAlterarPrevisaoEntrega)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
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

function salvarAlteracaoQuantidadesPedidoCarregamentoClick() {
    if (!ValidarCamposObrigatorios(_carregamentoPedidoAlterarQuantidades)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.PorFavorInformeOsCamposObrigatorios);
        return;
    }

    var pedidos = PEDIDOS_SELECIONADOS();
    var pesoAtual = Globalize.parseFloat(_carregamento.Peso.val());
    var pesoAtualPedido = 0;
    var palletAtual = Globalize.parseFloat(_carregamento.Pallets.val());
    var palletAtualPedido = 0;
    var pesoAtualizado = 0;
    var palletAtualizado = 0;

    for (var i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Codigo == _carregamentoPedidoAlterarQuantidades.Codigo.val()) {
            if (_carregamentoPedidoAlterarQuantidades.IsPeso.val()) {
                pesoAtualizado = Globalize.parseFloat(_carregamentoPedidoAlterarQuantidades.Peso.val());
                pesoAtualPedido = pedidos[i].PesoPedidoCarregamento;
                pedidos[i].PesoPedidoCarregamento = pesoAtualizado;
                break;
            } else if (_carregamentoPedidoAlterarQuantidades.IsPallet.val()) {
                palletAtualizado = Globalize.parseFloat(_carregamentoPedidoAlterarQuantidades.Pallet.val());
                palletAtualPedido = pedidos[i].PalletPedidoCarregamento;
                pedidos[i].PalletPedidoCarregamento = palletAtualizado;
                break;
            }
        }
    }
    
    if (pesoAtualizado > 0)
        _carregamento.Peso.val(Globalize.format(pesoAtual - (pesoAtualPedido - pesoAtualizado), "n4"));

    if (palletAtualizado > 0)
        _carregamento.Pallets.val(Globalize.format(palletAtual - (palletAtualPedido - palletAtualizado), "n2"));

    RenderizarGridMotagemPedidos();
    
    fecharModalAlterarQuantidadesPedidoCarregamento();
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

function salvarDefinicaoRecebedorPedidoClick() {
    let pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();

    for (let i = 0; i < pedidosSelecionados.length; i++) {
        pedidosSelecionados[i].Recebedor = _carregamentoPedidoAlterarRecebedor.Recebedor.val();
        pedidosSelecionados[i].CodRecebedor = _carregamentoPedidoAlterarRecebedor.Recebedor.codEntity();
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirRecebedorPedido();
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
        if (pedidos[i].Codigo == _carregamentoPedidoDefinirTipoPaleteCliente.Codigo.val()) {
            pedidos[i].TipoPaleteCliente = _carregamentoPedidoDefinirTipoPaleteCliente.TipoPaleteCliente.val();
            break;
        }
    }

    RenderizarGridMotagemPedidos();
    fecharModalDefinirTipoPaleteCliente();
}

function salvarDefinicaoTipoPaleteClientePedidosSelecionadosClick() {
    let pedidosSelecionados = _gridPedidosCarregamento.ListaSelecionados();

    if (_carregamentoPedidoDefinirTipoPaleteCliente.Codigo.val() > 0) {
        salvarTipoPaleteClientePedidoClick();
        return;
    }

    for (let i = 0; i < pedidosSelecionados.length; i++) {
        pedidosSelecionados[i].TipoPaleteCliente = _carregamentoPedidoDefinirTipoPaleteCliente.TipoPaleteCliente.val();
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

function AtualizarOrdemPedidoCarregamento(dataRow, row, head) {
    // AtualizarPedidoProduto(dataRow, row, head, null, false);
}

function exibirModalAlterarPrevisaoEntregaPedido() {
    Global.abrirModal("modalAlterarPrevisaoEntregaPedido");
    $("#modalAlterarPrevisaoEntregaPedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoAlterarPrevisaoEntrega);
    });
}

function exibirModalAlterarQuantidadesPedidoCarregamento() {
    Global.abrirModal("modalAlterarQuantidadesPedidoCarregamento");
    $("#modalAlterarQuantidadesPedidoCarregamento").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoAlterarQuantidades);
    });
}

function exibirModalDefinirReboquePedido() {
    Global.abrirModal("modalDefinirReboquePedido");
    $("#modalDefinirReboquePedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoDefinirReboque);
    });
}

function exibirModalDefinirRecebedorPedido() {
    Global.abrirModal("modalDefinirRecebedorPedido");
    $("#modalDefinirRecebedorPedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoAlterarRecebedor);
    });
}

function exibirModalDefinirTipoPaleteCliente() {
    Global.abrirModal("modalDefinirTipoPaleteCliente");
    $("#modalDefinirTipoPaleteCliente").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoDefinirTipoPaleteCliente);
    });
}

function exibirModalDefinirTipoCarregamentoPedido() {
    Global.abrirModal("modalDefinirTipoCarregamentoPedido");
    $("#modalDefinirTipoCarregamentoPedido").one('hidden.bs.modal', function () {
        LimparCampos(_carregamentoPedidoDefinirTipoCarregamentoPedido);
    });
}

function fecharModalAlterarPrevisaoEntregaPedido() {
    Global.fecharModal("modalAlterarPrevisaoEntregaPedido");
}

function fecharModalAlterarQuantidadesPedidoCarregamento() {
    Global.fecharModal("modalAlterarQuantidadesPedidoCarregamento");
}

function fecharModalDefinirReboquePedido() {
    Global.fecharModal("modalDefinirReboquePedido");
}

function fecharModalDefinirRecebedorPedido() {
    Global.fecharModal("modalDefinirRecebedorPedido");
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

function isPermitirAlterarPalletCarregamentoPedido() {
    return isPermitirAlterarQuantidadesCarregamentoPedido(true, false);
}

function isPermitirAlterarPesoCarregamentoPedido() {
    return isPermitirAlterarQuantidadesCarregamentoPedido(false, true);
}

function isPermitirDefinirReboque() {
    return _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido && (_carregamento.ModeloVeicularCarga.numeroReboques > 1);
}

function isPermitirDefinirTipoCarregamentoPedido(pedidoSelecionado) {
    return _CONFIGURACAO_TMS.MontagemCarga.ExigirDefinicaoTipoCarregamentoPedido && pedidoSelecionado.PedidoDestinadoAFilial;
}

function isPermitirAlterarQuantidadesCarregamentoPedido(pallet, peso) {
    // #63988 - Comentado pois a superpao alterou o serviço
    //if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
    //    return false;

    if (peso && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Peso)
        return true;

    if (pallet && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Pallet)
        return true;

    return false;
}

function RenderizarGridMotagemPedidos() {
    var pedidos = PEDIDOS_SELECIONADOS();
    var max = 0;

    PEDIDOS_SELECIONADOS().map(function (pedido) {
        var tmp = parseInt(pedido.Ordem);
        if (!isNaN(tmp) && tmp > max)
            max = tmp;
    });

    for (var i = 0; i < pedidos.length; i++) {
        if (pedidos[i].Ordem == "0" || pedidos[i].Ordem == "") {
            pedidos[i].Ordem = (max + 1);
            max++;
        }
    }

    pedidos.sort(function (a, b) { return a.Ordem - b.Ordem });

    for (var i = 0; i < pedidos.length; i++) {
        pedidos[i].Ordem = (i + 1);
        pedidos[i].DT_Enable = true;
        pedidos[i].DT_RowId = pedidos[i].Codigo;
    }

    _gridPedidosCarregamento.CarregarGrid(PEDIDOS_SELECIONADOS());
}

function retornoCallbackToolTip(registro) {
    var cnpj = '';
    if (registro.Destinatario != null) {
        var index = registro.Destinatario.lastIndexOf('(');
        if (index >= 0) {
            var tmp = registro.Destinatario.substring(index);
            index = tmp.indexOf(')');
            if (index >= 0)
                tmp = tmp.substring(0, index + 1);
            cnpj = tmp;
        }
    }
    return Localization.Resources.Cargas.MontagemCarga.PedidoPesoCubagemPallet.format(cnpj, registro.NumeroPedidoEmbarcador, registro.Peso, registro.Cubagem, registro.TotalPallets)
}

function retornoOrdenacaoPedidosCarregamento(retornoOrdenacao) {

    var listaRegistros = _gridPedidosCarregamento.BuscarRegistros();
    var listaRegistrosReordenada = [];
    var descontoPosicao = (listaRegistros[0].Ordem == 0) ? 1 : 0;

    for (var i = 0; i < retornoOrdenacao.listaRegistrosReordenada.length; i++) {
        var registroReordenado = retornoOrdenacao.listaRegistrosReordenada[i];

        for (var j = 0; j < listaRegistros.length; j++) {
            var registro = listaRegistros[j];

            if (registro.DT_RowId == registroReordenado.idLinha) {
                registro.Ordem = registroReordenado.posicao - descontoPosicao;
                listaRegistrosReordenada.push(registro);
                break;
            }
        }
    }
    _gridPedidosCarregamento.CarregarGrid(listaRegistrosReordenada);
}

// #endregion Funções Privadas
