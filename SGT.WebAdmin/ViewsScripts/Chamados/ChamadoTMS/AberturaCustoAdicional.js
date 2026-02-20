/// <reference path="Abertura.js" />
/// <reference path="../../Consultas/PedidoTipoPagamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _aberturaCustoAdicional;
var _gridAberturaCustoAdicional;

var AberturaCustoAdicional = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.PedidoTipoPagamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Forma de Cobrança:", idBtnSearch: guid(), required: true, enable: ko.observable(true), visible: ko.observable(true) });

    this.QuantidadeCustoExtra = PropertyEntity({ text: "*Qtd. Custo Extra:", def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.ValorUnitario = PropertyEntity({ text: "*Valor Unitário:", def: "", val: ko.observable(""), getType: typesKnockout.decimal, maxlength: 15, required: true, visible: ko.observable(true), enable: ko.observable(true), configDecimal: { precision: 2, allowZero: false } });
    this.ValorTotal = PropertyEntity({ text: "Valor Total:", def: "0,00", val: ko.observable("0,00"), getType: typesKnockout.decimal, required: false, maxlength: 15, enable: ko.observable(false) });

    this.Adicionar = PropertyEntity({ eventClick: AdicionarCustoAdicionalClick, type: types.event, text: "Adicionar", visible: ko.observable(true), enable: ko.observable(true) });

    this.Grid = PropertyEntity({ type: types.local, id: guid() });

    this.QuantidadeCustoExtra.val.subscribe(function () {
        CalcularTotalCustoAdicional();
    });

    this.ValorUnitario.val.subscribe(function () {
        CalcularTotalCustoAdicional();
    });
};

//*******EVENTOS*******

function loadAberturaCustoAdicional() {
    _aberturaCustoAdicional = new AberturaCustoAdicional();
    KoBindings(_aberturaCustoAdicional, "tabCustoAdicional");

    new BuscarPedidoTipoPagamento(_aberturaCustoAdicional.PedidoTipoPagamento);

    var excluir = { descricao: "Excluir", id: guid(), metodo: ExcluirCustoAdicionalClick, icone: "" };

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [excluir] };

    var header = [
        { data: "Codigo", visible: false },
        { data: "PedidoTipoPagamento", title: "Forma de Cobrança", width: "45%" },
        { data: "QuantidadeCustoExtra", title: "Quantidade Custo Extra", width: "15%" },
        { data: "ValorUnitario", title: "Valor Unitário", width: "15%" },
        { data: "ValorTotal", title: "Total", width: "15%" }
    ];

    _gridAberturaCustoAdicional = new BasicDataTable(_aberturaCustoAdicional.Grid.id, header, menuOpcoes);

    recarregarGridCustosAdicionais();
}

function ExcluirCustoAdicionalClick(data) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o custo adicional?", function () {
        $.each(_abertura.CustosAdicionais.list, function (i, lista) {
            if (data.Codigo == lista.Codigo.val) {
                _abertura.CustosAdicionais.list.splice(i, 1);
                return false;
            }
        });

        recarregarGridCustosAdicionais();
    });
}

function AdicionarCustoAdicionalClick() {
    var valido = ValidarCamposObrigatorios(_aberturaCustoAdicional);
    if (valido) {
        _aberturaCustoAdicional.Codigo.val(guid());
        _abertura.CustosAdicionais.list.push(SalvarListEntity(_aberturaCustoAdicional));

        limparCamposAberturaCustoAdicional();
        $("#" + _aberturaCustoAdicional.PedidoTipoPagamento.id).focus();
    } else
        exibirMensagem(tipoMensagem.atencao, "Campos Obrigatórios", "Informe os campos obrigatórios!");
}

//*******MÉTODOS*******

function recarregarGridCustosAdicionais() {

    var data = new Array();

    $.each(_abertura.CustosAdicionais.list, function (i, lista) {
        var listaGrid = new Object();

        listaGrid.Codigo = lista.Codigo.val;
        listaGrid.PedidoTipoPagamento = lista.PedidoTipoPagamento.val;
        listaGrid.QuantidadeCustoExtra = lista.QuantidadeCustoExtra.val;
        listaGrid.ValorUnitario = lista.ValorUnitario.val;
        listaGrid.ValorTotal = lista.ValorTotal.val;

        data.push(listaGrid);
    });

    _gridAberturaCustoAdicional.CarregarGrid(data);
}

function CalcularTotalCustoAdicional() {
    var quantidade = Globalize.parseFloat(_aberturaCustoAdicional.QuantidadeCustoExtra.val());
    var valorUnitario = Globalize.parseFloat(_aberturaCustoAdicional.ValorUnitario.val());

    if (quantidade > 0 && valorUnitario > 0) {
        var valorTotal = quantidade * valorUnitario;
        _aberturaCustoAdicional.ValorTotal.val(Globalize.format(valorTotal, "n2"));
    } else
        _aberturaCustoAdicional.ValorTotal.val(Globalize.format(0, "n2"));
}

function ControleCamposAberturaCustoAdicional(status) {
    SetarEnableCamposKnockout(_aberturaCustoAdicional, status);
    if (!status)
        _gridAberturaCustoAdicional.DesabilitarOpcoes();
    else
        _gridAberturaCustoAdicional.HabilitarOpcoes();
}

function limparCamposAberturaCustoAdicional() {
    LimparCampos(_aberturaCustoAdicional);
    recarregarGridCustosAdicionais();
}